using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] protected bool playerInRange;
    [SerializeField] protected string otherTag;
    [SerializeField] protected Notification myNotification;
    [SerializeField] protected AnimatorController animm;

    protected virtual void OnAnimationEnter()
    {
        if (animm != null)
            animm.SetAnimParameter("contextActive", true);
    }

    protected virtual void OnAnimationExit()
    {
        if (animm != null)
            animm.SetAnimParameter("contextActive", false);
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (string.IsNullOrEmpty(otherTag) || other == null)
            return;

        if (other.gameObject.CompareTag(otherTag) && !other.isTrigger)
        {
            OnAnimationEnter();
            playerInRange = true;

            if (myNotification != null)
                myNotification.Raise();
        }
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        if (string.IsNullOrEmpty(otherTag) || other == null)
            return;

        if (other.gameObject.CompareTag(otherTag) && !other.isTrigger)
        {
            OnAnimationExit();
            playerInRange = false;

            if (myNotification != null)
                myNotification.Raise();
        }
    }
}
