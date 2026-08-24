using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Objects/Abilities/Projectile Ability", fileName = "New Projectile Ability")]
public class ArrowAbility : GenericAbility
{
    [SerializeField] 
    private GameObject thisprojectile;
    public string animParameter;
    public float shootDelay = 0.2f;

    public override void Ability(Vector2 playerPosition, Vector2 playerFacingDirection, 
    Animator playerAnimator = null, Rigidbody2D playerRigidbody = null) 
    {
        if(playerMagic.RuntimeValue >= costMagic)
        {
            DecreaseMagic();
            if (playerRigidbody != null)
            {
                MonoBehaviour mono = playerRigidbody.GetComponent<MonoBehaviour>();
                if (mono != null)
                {
                    mono.StartCoroutine(ShootDelayCo(playerRigidbody, playerFacingDirection, shootDelay));
                    return;
                }
            }
            Shoot(playerPosition, playerFacingDirection);
        }
    }

    private IEnumerator ShootDelayCo(Rigidbody2D playerRigidbody, Vector2 playerFacingDirection, float delay)
    {
        yield return new WaitForSeconds(delay);
        Shoot(playerRigidbody.transform.position, playerFacingDirection);
    }

    private void Shoot(Vector2 playerPosition, Vector2 playerFacingDirection)
    {
        float facingRotation = Mathf.Atan2(playerFacingDirection.y, playerFacingDirection.x) * Mathf.Rad2Deg;
        GameObject newProjectile = Instantiate(thisprojectile, playerPosition, Quaternion.Euler(0f, 0f, facingRotation));
        GenericProjectile temp = newProjectile.GetComponent<GenericProjectile>();
        if(temp){
            temp.Setup(playerFacingDirection);
        }
    }
}
