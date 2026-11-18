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
///
/// RPG MAKER STYLE:
///   Use the RPGDialogue component to handle advanced sequences and events.
///   This takes priority over Sequential Mode.
/// </summary>
public class Dialogues : Interactable
{
    [SerializeField] private DialogController dialogController;

    [Header("Sequential Mode")]
    [SerializeField] private string sequentialSpeakerName;
    [SerializeField] private string sequentialSpeakerAnimParam;

    [TextArea(2, 6)]
    [SerializeField] private string[] dialogLines;

    [Header("RPG Maker-Style Dialogue (overrides Sequential)")]
    [SerializeField] private RPGDialogueSystem.RPGDialogue rpgDialogue;

    public virtual void Update()
    {
        if (playerInRange)
        {
            if (Input.GetButtonDown("Check"))
            {
                if (rpgDialogue != null)
                    dialogController.StartDialog(rpgDialogue.Commands);
                else
                    dialogController.StartDialog(dialogLines, sequentialSpeakerName, sequentialSpeakerAnimParam);
            }
        }
    }

    protected override void OnAnimationEnter()
    {
        
        if (animm != null)
        {
            animm.SetAnimParameter("dialogueClue", true);
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    protected override void OnAnimationExit()
    {
        if (animm != null)
        {
            animm.SetAnimParameter("dialogueClue", false);
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
