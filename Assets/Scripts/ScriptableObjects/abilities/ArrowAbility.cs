using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Objects/Abilities/Projectile Ability", fileName = "New Projectile Ability")]
public class ArrowAbility : GenericAbility
{
    [SerializeField] 
    private GameObject thisprojectile;

    public override void Ability(Vector2 playerPosition, Vector2 playerFacingDirection, 
    Animator playerAnimator = null, Rigidbody2D playerRigidbody = null) 
    {
        if(playerMagic.RuntimeValue >= costMagic)
        {
            DecreaseMagic();
            float facingRotation = Mathf.Atan2(playerFacingDirection.y, playerFacingDirection.x) * Mathf.Rad2Deg;
            GameObject newProjectile = Instantiate(thisprojectile, playerPosition, Quaternion.Euler(0f, 0f, facingRotation));
            GenericProjectile temp = newProjectile.GetComponent<GenericProjectile>();
            if(temp){
                temp.Setup(playerFacingDirection);
            }
        }
    }
}
