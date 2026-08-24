using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    [SerializeField] public AnimatorController anim;
    [SerializeField] private StateMachine myState;
    [SerializeField] private float WeaponAttackDuration;
    [SerializeField] private ReceiveItem myItem;
    [SerializeField] private GlobalAbilities abilities;
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private InventoryItem swordItem;
    public GenericAbility currentAbility;
    public GenericAbility secondaryAbility;


    public Vector2 tempMovement = Vector2.left;
    private Vector2 facingDirection = Vector2.down;
    public SignalSender screenKick;

    void Start()
    {
        currentAbility.canUse = true;
        UpdateAbilities();
        myState.ChangeState(GenericState.idle);
    }

    void Update()
    {
        if(myState.myState == GenericState.receiveItem)
        {
            if(Input.GetButtonDown("Check"))
            {
                myState.ChangeState(GenericState.idle);
                anim.SetAnimParameter("receiveItem", false);
                myItem.ChangeSpriteState();
            }
            return;
        }
        if(!IsRestrictedState(myState.myState) && !myState.blockPlayerMovement)
        {
            GetInput();
            SetAnimation();
        } else {
            tempMovement = Vector2.zero;
        }
    }


    void SetState(GenericState newState)
    {
        myState.ChangeState(newState);
    }


    void GetInput()
    {
        if(Input.GetButtonDown("Weapon Attack"))
        {
            if (playerInventory != null && swordItem != null && playerInventory.IsItemInInventory(swordItem))
            {
                StartCoroutine(WeaponCo());
                //tempMovement = Vector2.zero;
                Motion(tempMovement);
            }
        }

        if(Input.GetButtonDown("skill") && currentAbility.canUse)
        {
            if(currentAbility){
                StartCoroutine(AbilityCo(currentAbility.duration));
                StartCoroutine(CoolDownCo());
            }
        }

        if(Input.GetButtonDown("skill2") && secondaryAbility.canUse)
        {
            if(secondaryAbility){
                StartCoroutine(SecondaryAbiltyCo(secondaryAbility.duration));
                StartCoroutine(CoolDownCo());
            }
        }

        else if (myState.myState != GenericState.attack || myState.myState != GenericState.dead)
        {
            tempMovement.x = Input.GetAxisRaw("Horizontal");
            tempMovement.y = Input.GetAxisRaw("Vertical");
            tempMovement.Normalize();
            Motion(tempMovement);
        }
    }

    void SetAnimation()
    {
        if (tempMovement.magnitude > 0 && !IsRestrictedState(myState.myState))
        {
            anim.SetAnimParameter("moveX", Mathf.Round(tempMovement.x));
            anim.SetAnimParameter("moveY", Mathf.Round(tempMovement.y));
            anim.SetAnimParameter("Moving", true);
            SetState(GenericState.walk);
            facingDirection = tempMovement;
        }
        else
        {
            anim.SetAnimParameter("Moving", false);
            myRigidbody.linearVelocity = Vector3.zero;
            if(myState.myState != GenericState.attack)
            {
                SetState(GenericState.idle);
            }
        }
    }

    bool IsRestrictedState(GenericState currentState)
    {
        if(currentState == GenericState.attack || currentState == GenericState.ability || currentState == GenericState.dead)
        {
            return true;
        } 
            return false;
    }

    public IEnumerator WeaponCo()
    {
        myState.ChangeState(GenericState.attack);
        anim.SetAnimParameter("Attacking", true);
        yield return new WaitForSeconds(WeaponAttackDuration);
        myState.ChangeState(GenericState.idle);
        anim.SetAnimParameter("Attacking", false);
    }

     public IEnumerator AbilityCo(float abilityDuration)
     {
         myState.ChangeState(GenericState.ability);
         ArrowAbility currentArrow = currentAbility as ArrowAbility;
         if (currentArrow != null && !string.IsNullOrEmpty(currentArrow.animParameter))
         {
             anim.SetAnimParameter(currentArrow.animParameter, true);
         }
         currentAbility.Ability(transform.position, facingDirection, anim.anim, myRigidbody);
         yield return new WaitForSeconds(0.3f);
         if (currentArrow != null && !string.IsNullOrEmpty(currentArrow.animParameter))
         {
             anim.SetAnimParameter(currentArrow.animParameter, false);
         }
         myState.ChangeState(GenericState.idle);
     }

     public IEnumerator SecondaryAbiltyCo(float abilityDuration)
     {
         myState.ChangeState(GenericState.ability);
         ArrowAbility secondaryArrow = secondaryAbility as ArrowAbility;
         if (secondaryArrow != null && !string.IsNullOrEmpty(secondaryArrow.animParameter))
         {
             anim.SetAnimParameter(secondaryArrow.animParameter, true);
         }
         secondaryAbility.Ability(transform.position, facingDirection, anim.anim, myRigidbody);
         yield return new WaitForSeconds(abilityDuration);
         if (secondaryArrow != null && !string.IsNullOrEmpty(secondaryArrow.animParameter))
         {
             anim.SetAnimParameter(secondaryArrow.animParameter, false);
         }
         myState.ChangeState(GenericState.idle);
     }

     public IEnumerator CoolDownCo()
     {
        currentAbility.canUse = false;
        yield return new WaitForSeconds(currentAbility.coolDown);
        currentAbility.canUse = true;
     }

     public void UpdateAbilities()
     {
         currentAbility = abilities.mainAbility;
         secondaryAbility = abilities.secondaryAbility;
     }
}