using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Scriptable Objects/Abilities/Generic Ability", fileName = "New Generic Ability")]
public class GenericAbility : ScriptableObject
{
    public float costMagic;
    public float duration;

    public FloatValue playerMagic;
    public Notification usePlayerMagic;
    public bool canUse = true;
    public float coolDown;
    public Sprite uiImage;

    public virtual void Ability(Vector2 playerPosition, Vector2 playerFacingDirection, 
    Animator playerAnimator = null, Rigidbody2D playerRigidbody = null) 
    {
        
    }

    public void DecreaseMagic()
    {
        playerMagic.RuntimeValue -= costMagic;
        usePlayerMagic.Raise();
    }

    public void StartCooldown(MonoBehaviour playerMono)
    {
        if (playerMono != null)
        {
            playerMono.StartCoroutine(CooldownCo());
        }
    }

    protected System.Collections.IEnumerator CooldownCo()
    {
        canUse = false;
        yield return new WaitForSeconds(coolDown);
        canUse = true;
    }
}
