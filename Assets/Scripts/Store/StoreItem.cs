using UnityEngine;

public class StoreItem : Interactable
{
    
    [SerializeField] IntValue playerMoney;
    [SerializeField] int price;
    [SerializeField] Notification updateCoinsSignal;
    [SerializeField] GameObject actionButton;
    [SerializeField] private StringValue itemDescription;
    [SerializeField] private string newItemDescription;
    [SerializeField] private bool itemDescriptionActive = false;
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
    
    

    private void Update()
    {
        if(playerInRange && !IsBought() && canBuy())
        {
            if(Input.GetButtonDown("Check"))
            {
                Debug.Log("Store buying");
                buy();
            }
        }
    }

    private void Awake()
    {
        itemParent = itemContainer;
        if (itemInstance != null)
        {
            Transform itemTransform = itemInstance.transform;
            if (itemParent == null)
                itemParent = itemTransform.parent;
            itemLocalPosition = itemTransform.localPosition;
            itemLocalRotation = itemTransform.localRotation;
            itemLocalScale = itemTransform.localScale;
        }
    }

    public override void OnTriggerEnter2D(UnityEngine.Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (!IsBought() && other.gameObject.CompareTag(otherTag) && !other.isTrigger)
        {
            actionButton.SetActive(true);
            itemDescription.value = newItemDescription;
            itemDescriptionActive = true;
            if (itemDescriptionNotification != null)
                itemDescriptionNotification.Raise();
        }
    }

    public override void OnTriggerExit2D(UnityEngine.Collider2D other)
    {
        base.OnTriggerExit2D(other);
        if (other.gameObject.CompareTag(otherTag) && !other.isTrigger)
        {
            actionButton.SetActive(false);
            if (!IsUnique() && repeatableItemBought)
            {
                RespawnRepeatableItem();
                repeatableItemBought = false;
            }

            if (itemDescriptionActive)
            {
                itemDescriptionActive = false;
                if (itemDescriptionNotification != null && !IsBought())
                {
                    Debug.Log("Exit 2D area, toggling description and button");
                    itemDescriptionNotification.Raise();
                }
            }
        }
    }


    public bool canBuy(){
        return playerMoney.RuntimeValue >= price;
    }

    private bool IsUnique()
    {
        return itemBought != null;
    }

    private bool IsBought()
    {
        return IsUnique() && itemBought.value;
    }

    private void RespawnRepeatableItem()
    {
        if (itemInstance != null)
        {
            itemInstance.SetActive(true);
            SetStoreItemVisuals(true);
            return;
        }

        if (itemPrefab != null && itemParent != null)
        {
            itemInstance = Instantiate(itemPrefab, itemParent, false);
            Transform itemTransform = itemInstance.transform;
            itemTransform.localPosition = itemLocalPosition;
            itemTransform.localRotation = itemLocalRotation;
            itemTransform.localScale = itemLocalScale;
            itemInstance.SetActive(true);
            SetStoreItemVisuals(true);
            return;
        }

        SetStoreItemVisuals(true);
    }

    private void SetStoreItemVisuals(bool isActive)
    {
        Transform itemBlocker = transform.Find("StoreItemBlocker");
        Transform priceDialog = transform.Find("PriceDialog");

        if (itemBlocker != null)
            itemBlocker.gameObject.SetActive(isActive);
        if (priceDialog != null)
            priceDialog.gameObject.SetActive(isActive);
    }


    void buy(){
        playerMoney.RuntimeValue -= price;
        updateCoinsSignal.Raise();
        SetStoreItemVisuals(false);

        if (IsUnique())
        {
            itemBought.value = true;
        }
        else
        {
            repeatableItemBought = true;
        }
        itemDescriptionActive = false;
        if (itemDescriptionNotification != null)
            itemDescriptionNotification.Raise();
        Debug.Log("Bought, toggling description and button");
        actionButton.SetActive(false);
    }

    

    
}