using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : Interactable
{
    [SerializeField] private Notification individualDialogueNotification;
    [SerializeField] private StringValue individualDialogueStringText;
    [SerializeField] private string newIndividualDialogueStringText;
    [SerializeField] private bool dialogActive = false;
    

    public virtual void Update()
    {
        if (playerInRange)
        {
            if(Input.GetButtonDown("Check"))
            {
                dialogActive = !dialogActive;
                individualDialogueStringText.value = newIndividualDialogueStringText;
                individualDialogueNotification.Raise();
            }
        }
    }

    public override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        if(other.gameObject.CompareTag(otherTag) && !other.isTrigger)
        {
            if (dialogActive)
            {
                dialogActive = !dialogActive;
                individualDialogueNotification.Raise();
            }
        }
    }

}
