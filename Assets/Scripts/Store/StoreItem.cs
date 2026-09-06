using UnityEngine;

public class StoreItem : Interactable
{
    [SerializeField] private IntValue playerMoney;
    [SerializeField] private int price;
    [SerializeField] private Notification updateCoinsSignal;
    [SerializeField] private GameObject actionButton;
    [SerializeField] private StringValue itemDescription;
    [SerializeField] private string newItemDescription;
    [SerializeField] private Notification itemDescriptionNotification;
    [SerializeField] private BoolValue itemBought;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject itemInstance;
    [SerializeField] private Transform itemContainer;

    private Transform itemParent;
    private Vector3 itemLocalPosition;
    private Quaternion itemLocalRotation;
    private Vector3 itemLocalScale;
    private bool repeatableItemBought;
    private bool itemDescriptionActive;

    // Cached once in Awake — avoids repeated string-based child lookups at runtime.
    private Transform _itemBlocker;
    private Transform _priceDialog;

    private void Awake()
    {
        itemParent = itemContainer;

        if (itemInstance != null)
        {
            Transform t = itemInstance.transform;
            if (itemParent == null)
                itemParent = t.parent;

            itemLocalPosition = t.localPosition;
            itemLocalRotation = t.localRotation;
            itemLocalScale    = t.localScale;
        }

        _itemBlocker = transform.Find("StoreItemBlocker");
        _priceDialog = transform.Find("PriceDialog");
    }

    private void Update()
    {
        if (playerInRange && !IsBought() && CanBuy())
        {
            if (Input.GetButtonDown("Check"))
                Buy();
        }
    }

    public override void OnTriggerEnter2D(UnityEngine.Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (!IsBought() && other.gameObject.CompareTag(otherTag) && !other.isTrigger)
        {
            if (actionButton != null)
                actionButton.SetActive(true);

            itemDescription.value  = newItemDescription;
            itemDescriptionActive  = true;

            if (itemDescriptionNotification != null)
                itemDescriptionNotification.Raise();
        }
    }

    public override void OnTriggerExit2D(UnityEngine.Collider2D other)
    {
        base.OnTriggerExit2D(other);

        if (other.gameObject.CompareTag(otherTag) && !other.isTrigger)
        {
            if (actionButton != null)
                actionButton.SetActive(false);

            if (!IsPersistentItem() && repeatableItemBought)
                RespawnRepeatableItem();

            if (itemDescriptionActive && !IsBought())
                DeactivateItemDescription();
        }
    }

    /// <summary>Returns true if the player has enough money to buy this item.</summary>
    public bool CanBuy()
    {
        return playerMoney.RuntimeValue >= price;
    }

    /// <summary>Returns true if this item tracks a persistent bought state (i.e. is non-repeatable).</summary>
    private bool IsPersistentItem()
    {
        return itemBought != null;
    }

    /// <summary>Returns true if this is a persistent item that has already been purchased.</summary>
    private bool IsBought()
    {
        return IsPersistentItem() && itemBought.value;
    }

    private void DeactivateItemDescription()
    {
        itemDescriptionActive = false;

        if (itemDescriptionNotification != null)
            itemDescriptionNotification.Raise();
    }

    private void Buy()
    {
        // Guard: re-validate affordability before mutating any state.
        if (!CanBuy())
            return;

        playerMoney.RuntimeValue -= price;
        updateCoinsSignal.Raise();
        SetStoreItemVisuals(false);

        if (IsPersistentItem())
            itemBought.value = true;
        else
            repeatableItemBought = true;

        DeactivateItemDescription();

        if (actionButton != null)
            actionButton.SetActive(false);

#if UNITY_EDITOR
        Debug.Log($"StoreItem: '{name}' purchased for {price} coins.");
#endif
    }

    private void RespawnRepeatableItem()
    {
        if (itemInstance != null)
        {
            itemInstance.SetActive(true);
            SetStoreItemVisuals(true);
            repeatableItemBought = false;
            return;
        }

        if (itemPrefab != null && itemParent != null)
        {
            itemInstance = Instantiate(itemPrefab, itemParent, false);
            Transform t  = itemInstance.transform;
            t.localPosition = itemLocalPosition;
            t.localRotation = itemLocalRotation;
            t.localScale    = itemLocalScale;
            itemInstance.SetActive(true);
            SetStoreItemVisuals(true);
            repeatableItemBought = false;
            return;
        }

        // Respawn failed: no instance or prefab available.
        // repeatableItemBought is intentionally NOT cleared here so the
        // next exit will retry rather than silently swallowing the failure.
        SetStoreItemVisuals(true);
    }

    private void SetStoreItemVisuals(bool isActive)
    {
        if (_itemBlocker != null)
            _itemBlocker.gameObject.SetActive(isActive);

        if (_priceDialog != null)
            _priceDialog.gameObject.SetActive(isActive);
    }
}