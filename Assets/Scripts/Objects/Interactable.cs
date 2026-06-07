using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    [SerializeField] public bool playerInRange;
    [SerializeField] public string otherTag;
	[SerializeField] public Notification myNotification;
    [SerializeField] public AnimatorController animm;

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (string.IsNullOrEmpty(otherTag) || other == null)
        {
            return;
        }

        if (other.gameObject.CompareTag(otherTag) && !other.isTrigger)
        {
            if (animm != null)
            {
                
                animm.SetAnimParameter("contextActive", true);
            }

            playerInRange = true;

            if (myNotification != null)
            {
                Debug.Log("Entered zone, Raising Notification");
                myNotification.Raise();
            }
        }
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        if (string.IsNullOrEmpty(otherTag) || other == null)
        {
            return;
        }

        if (other.gameObject.CompareTag(otherTag) && !other.isTrigger)
        {
            if (animm != null)
            {
                animm.SetAnimParameter("contextActive", false);
            }

            playerInRange = false;

            if (myNotification != null)
            {
                Debug.Log("Exited zone, Raising Notification");
                myNotification.Raise();
            }
        }
    }
}
