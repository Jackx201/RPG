using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory Info")]
    public Inventory playerInventory;
    [SerializeField] private GameObject blankInventorySlot;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Text description;
    [SerializeField] private GameObject useButton;
    public InventoryItem currentItem;
    [SerializeField] private GameObject yButton;
    [SerializeField] private GameObject bButton;
    [SerializeField] private ButtonsAbility abilities;
    [SerializeField] private IntValue button;
 
    

    public void SetTextAndButton(string descriptionText, bool btnactive)
    {
        description.text = descriptionText;
        if(btnactive){
            useButton.SetActive(true);
        } else {
            useButton.SetActive(false);
        }
    }

    void MakeInventorySlots(){
        if(playerInventory){
            for(int i=0; i<playerInventory.myInventory.Count;i++)
            {
                if(playerInventory.myInventory[i].numberHeld > 0)
                {
                    GameObject temp = 
                    Instantiate(blankInventorySlot, 
                    inventoryPanel.transform.position, Quaternion.identity);
                    temp.transform.SetParent(inventoryPanel.transform);
                    InventorySlot newSlot = temp.GetComponent<InventorySlot>();
                    
                    if(newSlot){
                        newSlot.Setup(playerInventory.myInventory[i], this);
                    }
                }
            }
        }
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        ClearInventorySlots();
        MakeInventorySlots();
        SetTextAndButton("", false);   

    }

    public void SetupDescriptionAndButton(string newDescription, bool isUsable, InventoryItem newItem, bool isUnique)
    {
        description.text = newDescription;
        yButton.SetActive(isUnique);
        bButton.SetActive(isUnique);
        useButton.SetActive(isUsable);
        currentItem = newItem;
    }

    void ClearInventorySlots()
    {
        for(int i = 0; i < inventoryPanel.transform.childCount; i++)
        {
            Destroy(inventoryPanel.transform.GetChild(i).gameObject);
        }
    }

    public void UseButtonPRessed()
    {
        if(currentItem)
        {
            currentItem.Use();
            ClearInventorySlots();
            MakeInventorySlots();
            if(currentItem.numberHeld == 0)
            {
            SetTextAndButton("", false);
            }
        }
    }

    public void ChangeMainAbility()
    {
        button.RuntimeValue = 2;
        currentItem.swapAbility();
        abilities.UpdateAbilities();
    }

    public void ChangeSecondary()
    {
        Debug.Log("Button B");
        button.RuntimeValue = 1;
        currentItem.swapAbility();
        abilities.UpdateAbilities();
    }
}

