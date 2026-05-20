using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{

    [Header("UI Stuff to change")]
    [SerializeField] private TextMeshProUGUI itemNumberText;
    [SerializeField] private Image itemImage;

    [Header("Variables from the Item")]
    public InventoryItem thisItem;
    public InventoryManager thisManager;

    public void Setup(InventoryItem newItem, InventoryManager newManager)
    {
        thisItem = newItem;
        thisManager = newManager;
        if(thisItem)
        {
            itemImage.sprite = thisItem.mySprite;
            itemNumberText.text = thisItem.numberHeld.ToString();

        }
    }

    public void ClickedOn()
    {
        if(thisItem){
            thisManager.SetupDescriptionAndButton(thisItem.myDescription, thisItem.isUsable, thisItem, thisItem.isUnique);
        }
    }
}

//IsUnique