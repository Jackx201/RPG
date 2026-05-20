using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Scriptable Objects/Abilities/Dash Ability", fileName = "Dash Ability")]

public class Dash : GenericAbility
{
    public float dashForce;
    [SerializeField] private float cooldown;
    private Magic myMagic;
    

    public override void Ability(Vector2 playerPosition, Vector2 playerFacingDirection, 
    Animator playerAnimator = null, Rigidbody2D playerRigidbody = null) 
    {
        if (playerRigidbody && playerMagic.RuntimeValue >= costMagic)
        {
            playerMagic.RuntimeValue -= costMagic;
            Vector3 dashVector = playerRigidbody.transform.position + 
            (Vector3)playerFacingDirection.normalized * dashForce;
            playerRigidbody.DOMove(dashVector, duration);
            usePlayerMagic.Raise();
        }
    }
}
