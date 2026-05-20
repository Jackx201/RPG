using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : PowerUp
{
    public IntValue playercoins;
    [SerializeField] private Notification obtainedcoin;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            playercoins.RuntimeValue += 1;
            obtainedcoin.Raise();
            Destroy(this.gameObject);
        }
    }
}
