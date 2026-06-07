using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this to any interactable object to give it a dialog sequence.
///
/// SEQUENTIAL MODE:
///   Leave dialogNodes empty and fill dialogLines with plain strings.
///   Player presses Check to advance line by line.
///
/// BRANCHING MODE:
///   Fill dialogNodes with DialogNode entries. Each node can be either:
///     - Linear:   leave choices[] empty, set nextNodeIndex.
///     - Branching: add up to 2 choices, each pointing to another node index.
///   Use -1 as nextNodeIndex (or choice.nextNodeIndex) to end the conversation.
///   Player presses Check on linear nodes to advance.
///   On choice nodes, player clicks a button (Check shortcuts to choice A).
///
/// If dialogNodes has any entries, it takes priority over dialogLines.
/// </summary>
public class Dialogues : Interactable
{
    [SerializeField] private DialogController dialogController;

    [Header("Sequential Mode")]
    [SerializeField] private string sequentialSpeakerName;
    [SerializeField] private string sequentialSpeakerAnimParam;

    [TextArea(2, 6)]
    [SerializeField] private string[] dialogLines;

    [Header("Branching Mode (overrides Sequential)")]
    [SerializeField] private DialogNode[] dialogNodes;

    public virtual void Update()
    {
        if (playerInRange)
        {
            if (Input.GetButtonDown("Check"))
            {
                if (dialogNodes != null && dialogNodes.Length > 0)
                    dialogController.StartDialog(dialogNodes);
                else
                    dialogController.StartDialog(dialogLines, sequentialSpeakerName, sequentialSpeakerAnimParam);
            }
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
         animm.SetAnimParameter("contextActive", true);

        base.OnTriggerEnter2D(other);
        
        // Debug check to help find why the context animation isn't activating for Dialogues
        if (other.gameObject.CompareTag(otherTag) && !other.isTrigger)
        {
            if (animm != null)
            {
                animm.SetAnimParameter("contextActive", true);
            }
        }
    }

    public override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        if (other.gameObject.CompareTag(otherTag) && !other.isTrigger)
        {
            dialogController.ForceClose();
        }
    }
}
