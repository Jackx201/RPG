using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Objects/Abilities/Multi Projectile Ability", fileName = "Multi Projectile Ability")]

public class MultiProjectile : GenericAbility
{
    [SerializeField] 
    private GameObject thisprojectile;
    [SerializeField]
    private int numberOfProjectiles;
    [SerializeField]
    private float spread;

    public override void Ability(Vector2 playerPosition, Vector2 playerFacingDirection, 
    Animator playerAnimator = null, Rigidbody2D playerRigidbody = null) 
    {
        float facingRotation = Mathf.Atan2(playerFacingDirection.y, playerFacingDirection.x) * Mathf.Rad2Deg;
        float startRotation = facingRotation + spread / 2f;
        float angleIncrease = spread/((float)numberOfProjectiles - 1f );

        for (int i = 0; i<numberOfProjectiles; i++)
        {
            float tempRot = startRotation - angleIncrease * i;
        GameObject newProjectile = Instantiate(thisprojectile, playerPosition, Quaternion.Euler(0f, 0f, tempRot));
        GenericProjectile temp = newProjectile.GetComponent<GenericProjectile>();
            if(temp){
                temp.Setup(new Vector2(Mathf.Cos(tempRot * Mathf.Deg2Rad), 
                    Mathf.Sin(tempRot * Mathf.Deg2Rad)));
            }

        }
    }
}
