using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAbility : MonoBehaviour
{
    [SerializeField] public GenericAbility myAbility;
    [SerializeField] public GlobalAbilities myAbilities;
    [SerializeField] private IntValue button;
 

    public void Use()
    {
        if(button.RuntimeValue == 1)
        {
                if(myAbility == myAbilities.secondaryAbility)
                {
                    //Swap ability from Y to B if the ability is already selected in Y.
                    myAbilities.secondaryAbility = myAbilities.mainAbility;
                    myAbilities.mainAbility = myAbility;
                }
                myAbilities.mainAbility = myAbility; 
        }
        
        if(button.RuntimeValue == 2)
        {
                if(myAbility == myAbilities.mainAbility)
                {
                    //Swap ability from Y to B if the ability is already selected in Y.
                    myAbilities.mainAbility = myAbilities.secondaryAbility;
                    myAbilities.secondaryAbility = myAbility;
                }
                myAbilities.secondaryAbility = myAbility; 
        }          
               
        myAbilities.changedAbility.Raise();
    }
}
