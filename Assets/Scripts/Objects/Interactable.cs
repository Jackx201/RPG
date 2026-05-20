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
        if (other.gameObject.CompareTag(otherTag) && !other.isTrigger)
        {
            animm.SetAnimParameter("contextActive", true);
            playerInRange = true;
			myNotification.Raise();
        }
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(otherTag) && !other.isTrigger)
        {
            animm.SetAnimParameter("contextActive", false);
            playerInRange = false;
			myNotification.Raise();
        }
    }
}
