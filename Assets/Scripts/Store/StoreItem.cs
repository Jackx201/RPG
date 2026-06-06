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
    private bool itemBought = false;
    

    private void Update()
    {
        if(playerInRange && !itemBought && canBuy())
        {
            if(Input.GetButtonDown("Check"))
            {
                Debug.Log("Store buying");
                buy();
            }
        }
    }

    public override void OnTriggerEnter2D(UnityEngine.Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (!itemBought && other.gameObject.CompareTag(otherTag) && !other.isTrigger)
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
            if (itemDescriptionActive)
            {
                itemDescriptionActive = false;
                if (itemDescriptionNotification != null && !itemBought)
                    itemDescriptionNotification.Raise();
            }
        }
    }


    public bool canBuy(){
        return playerMoney.RuntimeValue >= price;
    }


    void buy(){
        playerMoney.RuntimeValue -= price;
        updateCoinsSignal.Raise();
        transform.Find("StoreItemBlocker").gameObject.SetActive(false);
        transform.Find("PriceDialog").gameObject.SetActive(false);
        itemBought = true;
        itemDescriptionNotification.Raise();
        actionButton.SetActive(false);
    }

    

    
}