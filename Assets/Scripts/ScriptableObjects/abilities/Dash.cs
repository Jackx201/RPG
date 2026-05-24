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
            playerMagic.RuntimeValue -= costMagic;
            Vector3 dashVector = playerRigidbody.transform.position + 
            (Vector3)playerFacingDirection.normalized * dashForce;
            playerRigidbody.DOMove(dashVector, duration);
            usePlayerMagic.Raise();

            // Ignorar colisión con la capa de enemigos durante el dash
            int playerLayer = playerRigidbody.gameObject.layer;
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

            // Restaurar colisión al terminar el dash usando la MonoBehaviour del jugador
            MonoBehaviour playerMono = playerRigidbody.GetComponent<MonoBehaviour>();
            if (playerMono != null)
            {
                playerMono.StartCoroutine(RestoreCollisionCo(playerLayer, enemyLayer, duration));
            }
        }
    }

    private IEnumerator RestoreCollisionCo(int playerLayer, int enemLayer, float delay)
    {
        yield return new WaitForSeconds(delay);
        Physics2D.IgnoreLayerCollision(playerLayer, enemLayer, false);
    }
}
