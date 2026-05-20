using UnityEngine;

public class Magic : MonoBehaviour
{
    [SerializeField] public int currentMagic;
    [SerializeField] public int maxMagic;
    
    public FloatValue playerMagic;

    public bool CanUseMagic(float amount)
    {
        if(playerMagic.RuntimeValue >= amount)
        {
            return true;
        }
        return false;
    }

    public void UseMagic(float amount)
    {
        playerMagic.RuntimeValue -= amount;
        if(playerMagic.RuntimeValue <= 0)
        {
            playerMagic.RuntimeValue = 0;
        }
    }


    // public bool CanUseMagic(int amountToUse)
    // {
    //     if(currentMagic >= amountToUse)
    //     {
    //         return true;
    //     }
    //     return false;
    // }

    // public void UseMagic(int amountToUse)
    // {
    //     currentMagic -= amountToUse;
    //     if(currentMagic<= 0)
    //     {
    //         currentMagic = 0;
    //     }
    // }

    public void UseAllMagic()
    {
        playerMagic.RuntimeValue  = 0;
    }

    public void FillMagic()
    {
        playerMagic.RuntimeValue  = maxMagic;
    }

    public void AddMagic(float amountToAdd)
    {
        playerMagic.RuntimeValue  += amountToAdd;
        if(playerMagic.RuntimeValue  > maxMagic)
        {
            playerMagic.RuntimeValue  = maxMagic;
        }
    }
}
