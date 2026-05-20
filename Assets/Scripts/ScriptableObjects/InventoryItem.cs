using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Scriptable Objects/Inventory Item", fileName = "New Inventory Item")]
public class InventoryItem : ScriptableObject
{
    public Sprite mySprite;
    public string myName;
    public string myDescription;
    public bool isUsable;
    public bool isUnique;
    public int numberHeld;

    public UnityEvent thisEvent;

    public void Use()
    {
        thisEvent.Invoke();
        if(!isUnique)
        {
            DecreaseAmount(1);
        }
    }

    public void DecreaseAmount(int amountToDecrease)
    {
        numberHeld -= amountToDecrease;
        if(numberHeld < 0)
        {
            numberHeld = 0;
        }
    }

    public void swapAbility()
    {
        thisEvent.Invoke();
    }
}

