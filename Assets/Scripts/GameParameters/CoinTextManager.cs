using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinTextManager : MonoBehaviour
{
    public IntValue coins;
    public TextMeshProUGUI coinDisplay;

    void Start()
    {
        UpdateCoinCount();
    } 

    public void UpdateCoinCount()
    {
        coinDisplay.text = "" + coins.RuntimeValue;
    }
}
