using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicManager : MonoBehaviour
{
    public Slider magicSlider;
    public Magic playerInventory;

    void Start()
    {
        magicSlider.maxValue = playerInventory.maxMagic;
        magicSlider.value = playerInventory.maxMagic;
        playerInventory.currentMagic = playerInventory.maxMagic;
        UpdateMagic();
    }

    public void UpdateMagic()
    {
        magicSlider.value = playerInventory.playerMagic.RuntimeValue;
    }
}
