using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Scriptable Objects/Abilities/Dash Ability", fileName = "Dash Ability")]

public class Dash : GenericAbility
{
    public float dashForce;
    [SerializeField] private float postDashImmunity = 2f;
    [SerializeField] private int enemyLayer = 8; // Número de layer de enemigos (verificar en Unity: Edit > Project Settings > Tags & Layers)
    
    [Header("Echo Effect")]
    [SerializeField] private bool useEchoEffect = true;
    [SerializeField] private float echoInterval = 0.05f;
    [SerializeField] private float echoDestroyTime = 0.3f;
    [SerializeField] private float echoStartAlpha = 0.5f;
    
    private Magic myMagic;
    

    public override void Ability(Vector2 playerPosition, Vector2 playerFacingDirection, 
    Animator playerAnimator = null, Rigidbody2D playerRigidbody = null) 
    {
        if (playerRigidbody && playerMagic.RuntimeValue >= costMagic && canUse)
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
            PlayerHealth playerHealth = playerRigidbody.GetComponent<PlayerHealth>();
            TrailRenderer trailRenderer = playerRigidbody.GetComponentInChildren<TrailRenderer>();

            if (playerHealth != null)
            {
                playerHealth.isInvincible = true;
            }

            if (trailRenderer != null)
            {
                trailRenderer.emitting = true;
            }

            if (playerMono != null)
            {
                playerMono.StartCoroutine(RestoreCollisionCo(playerLayer, enemyLayer, duration, playerAnimator, playerHealth, trailRenderer));
                
                if (useEchoEffect)
                {
                    playerMono.StartCoroutine(EchoEffectCo(playerRigidbody, duration));
                }

                StartCooldown(playerMono);
            }
        }
    }

    private IEnumerator RestoreCollisionCo(int playerLayer, int enemLayer, float delay, Animator playerAnimator, PlayerHealth playerHealth, TrailRenderer trailRenderer)
    {
        yield return new WaitForSeconds(delay);
        
        Physics2D.IgnoreLayerCollision(playerLayer, enemLayer, false);

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("Dashing", false);
        }

        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
        }

        // Frames de invulnerabilidad post Dash
        yield return new WaitForSeconds(postDashImmunity);
        
        if (playerHealth != null)
        {
            playerHealth.isInvincible = false;
        }
    }

    private IEnumerator EchoEffectCo(Rigidbody2D playerRigidbody, float dashDuration)
    {
        SpriteRenderer playerSprite = playerRigidbody.GetComponent<SpriteRenderer>();
        if (playerSprite == null) yield break;

        float timePassed = 0f;
        while (timePassed < dashDuration)
        {
            // Crear un GameObject para el eco
            GameObject echo = new GameObject("DashEcho");
            echo.transform.position = playerRigidbody.transform.position;
            echo.transform.rotation = playerRigidbody.transform.rotation;
            echo.transform.localScale = playerRigidbody.transform.localScale;

            // Agregar SpriteRenderer y copiar configuraciones
            SpriteRenderer echoSprite = echo.AddComponent<SpriteRenderer>();
            echoSprite.sprite = playerSprite.sprite;
            echoSprite.flipX = playerSprite.flipX;
            echoSprite.flipY = playerSprite.flipY;
            echoSprite.sortingLayerID = playerSprite.sortingLayerID;
            echoSprite.sortingOrder = playerSprite.sortingOrder - 1; // Ponerlo detrás del jugador

            // Establecer el alpha
            Color echoColor = playerSprite.color;
            echoColor.a = echoStartAlpha;
            echoSprite.color = echoColor;

            echoSprite.DOFade(0f, echoDestroyTime).OnComplete(() => Destroy(echo));

            yield return new WaitForSeconds(echoInterval);
            timePassed += echoInterval;
        }
    }
}
