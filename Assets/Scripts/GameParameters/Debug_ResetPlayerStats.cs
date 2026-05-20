using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_ResetPlayerStats : MonoBehaviour
{
    [SerializeField] IntValue coins;
    [SerializeField] FloatValue life;
    [SerializeField] FloatValue magic;
    [SerializeField] FloatValue hearthcontainers;
    
    public void ResetStats()
    {
        coins.RuntimeValue = coins.initialValue;
        life.RuntimeValue = life.initialValue;
        magic.RuntimeValue = magic.initialValue;
        hearthcontainers.RuntimeValue = hearthcontainers.initialValue;
    }

}
