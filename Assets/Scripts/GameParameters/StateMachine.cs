using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GenericState
{
    idle,
    walk,
    attack,
    stun,
    dead,
    receiveItem,
    ability,
    dying
}

public class StateMachine : MonoBehaviour
{
    public GenericState myState;
    // Determines if player movement should be blocked (e.g., during certain dialogues)
    public bool blockPlayerMovement = false;

    public void ChangeState(GenericState newState)
    {
        if(myState != newState)
        {
            myState = newState;
        }
    }

    // Allows other scripts (e.g., DialogController) to enable/disable movement blocking
    public void SetMovementBlock(bool shouldBlock)
    {
        blockPlayerMovement = shouldBlock;
    }
}
