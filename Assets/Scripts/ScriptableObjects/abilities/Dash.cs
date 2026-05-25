using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Scriptable Objects/Abilities/Dash Ability", fileName = "Dash Ability")]

public class Dash : GenericAbility
{
    public float dashForce;
    [SerializeField] private float cooldown;
    [SerializeField] private int enemyLayer = 8; // Número de layer de enemigos (verificar en Unity: Edit > Project Settings > Tags & Layers)
    private Magic myMagic;
    

    public override void Ability(Vector2 playerPosition, Vector2 playerFacingDirection, 
    Animator playerAnimator = null, Rigidbody2D playerRigidbody = null) 
    {
        if (playerRigidbody && playerMagic.RuntimeValue >= costMagic)
        {
            // Activar animación de dash con la dirección correcta
            if (playerAnimator != null)
            {
                playerAnimator.SetFloat("moveX", Mathf.Round(playerFacingDirection.x));
                playerAnimator.SetFloat("moveY", Mathf.Round(playerFacingDirection.y));
                playerAnimator.SetBool("Dashing", true);
            }

            playerMagic.RuntimeValue -= costMagic;
            Vector3 dashVector = playerRigidbody.transform.position + 
            (Vector3)playerFacingDirection.normalized * dashForce;
            playerRigidbody.DOMove(dashVector, duration);
            usePlayerMagic.Raise();



            // Ignorar colisión con la capa de enemigos durante el dash
            int playerLayer = playerRigidbody.gameObject.layer;
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

            // Restaurar colisión y animación al terminar el dash
            MonoBehaviour playerMono = playerRigidbody.GetComponent<MonoBehaviour>();
            if (playerMono != null)
            {
                playerMono.StartCoroutine(RestoreCollisionCo(playerLayer, enemyLayer, duration, playerAnimator));
            }
        }
    }

    private IEnumerator RestoreCollisionCo(int playerLayer, int enemLayer, float delay, Animator playerAnimator)
    {
        yield return new WaitForSeconds(delay);
        Physics2D.IgnoreLayerCollision(playerLayer, enemLayer, false);
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("Dashing", false);
        }
    }
}
