using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using RPGDialogueSystem;

public class DialogController : MonoBehaviour
{
    // Existing fields...
    // Added fields for movement blocking
    [Header("Dialogue Options")]
    [SerializeField] private bool blockPlayerMovement = false;
    // Reference to the StateMachine handling player state
    [SerializeField] private StateMachine stateMachine;

    // -------------------------------------------------------
    // Shared UI references
    // -------------------------------------------------------
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private GameObject dialogObject;
    [SerializeField] private Image dialogBoxImage;

    // -------------------------------------------------------
    // Name Box UI (shows the speaker's name)
    // -------------------------------------------------------
    [Header("Name Box")]
    [SerializeField] private GameObject nameBoxObject;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image nameBoxImage;

    // -------------------------------------------------------
    // Portrait animator (NPC picture with talking animation)
    // -------------------------------------------------------
    [Header("Portrait")]
    [SerializeField] private GameObject portraitContainer;
    [SerializeField] private AnimatorController portraitAnimator;
    [SerializeField] private Image portraitBoxImage;

    /// Tracks the last active speaker param so we can reset it when the speaker changes.
    private string lastSpeakerParam = "";

    // -------------------------------------------------------
    // Choice UI (your existing two-button container)
    // -------------------------------------------------------
    [Header("Choice Buttons")]
    [SerializeField] private GameObject choiceContainer;
    [SerializeField] private Button choiceButtonA;
    [SerializeField] private Button choiceButtonB;
    [SerializeField] private TextMeshProUGUI choiceLabelA;
    [SerializeField] private TextMeshProUGUI choiceLabelB;


    // -------------------------------------------------------
    // Internal state
    // -------------------------------------------------------
    private bool dialogActive = false;
    private bool waitingForChoice = false;
    private float lastChoiceTime = -1f;

    // Sequential mode (string[])
    private string[] currentLines;
    private int currentLineIndex = 0;

    // RPG dialogue mode
    private Stack<RPGDialogueState> rpgStateStack = new Stack<RPGDialogueState>();
    private bool rpgMode = false;

    private class RPGDialogueState
    {
        public IReadOnlyList<RPGDialogueCommand> commands;
        public int index;
    }


    // -------------------------------------------------------
    // SEQUENTIAL: StartDialog(string[]) — used by Dialogues.cs
    // when no branching is needed
    // -------------------------------------------------------

    /// <summary>
    /// Opens the dialog panel and shows each string one by one on every Check press.
    /// Auto-closes when the last line is passed.
    /// </summary>
    public void StartDialog(string[] lines, string speakerName = "", string speakerAnimParam = "")
    {
        if (Time.time - lastChoiceTime < 0.1f) return;
        if (waitingForChoice) return;

        if (!dialogActive)
        {
            if (lines == null || lines.Length == 0) return;
            currentLines = lines;
            currentLineIndex = 0;
            dialogActive = true;
            dialogObject.SetActive(true);
            dialogText.text = currentLines[currentLineIndex];

            // Set movement block based on inspector setting
            if (stateMachine != null)
                stateMachine.SetMovementBlock(blockPlayerMovement);

            // Display speaker name
            if (!string.IsNullOrEmpty(speakerName))
            {
                if (nameBoxObject != null) nameBoxObject.SetActive(true);
                if (nameText != null) nameText.text = speakerName;
            }
            else
            {
                if (nameBoxObject != null) nameBoxObject.SetActive(false);
            }

            // Switch portrait animation
            ApplySpeakerAnim(speakerAnimParam);
        }
        else if (currentLines != null)
        {
            currentLineIndex++;
            if (currentLineIndex < currentLines.Length)
                dialogText.text = currentLines[currentLineIndex];
            else
                ForceClose();
        }
    }

    void SelectChoiceA()
    {
        if (!waitingForChoice) return;
        
        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected == choiceButtonB.gameObject)
        {
            choiceButtonB.onClick.Invoke();
        }
        else
        {
            choiceButtonA.onClick.Invoke();
        }
    }

    public void StartDialog(IReadOnlyList<RPGDialogueCommand> commands)
    {
        if (Time.time - lastChoiceTime < 0.1f) return;
        if (waitingForChoice)
        {
            SelectChoiceA();
            return;
        }

        if (!dialogActive)
        {
            if (commands == null || commands.Count == 0) return;
            rpgStateStack.Clear();
            rpgStateStack.Push(new RPGDialogueState { commands = commands, index = 0 });
            rpgMode = true;
            dialogActive = true;
            dialogObject.SetActive(true);
            AdvanceRPGDialogue();

            // Set movement block based on inspector setting
            if (stateMachine != null)
                stateMachine.SetMovementBlock(blockPlayerMovement);
        }
        else if (rpgMode)
        {
            AdvanceRPGDialogue();
        }
    }

    private void AdvanceRPGDialogue()
    {
        if (rpgStateStack.Count == 0)
        {
            ForceClose();
            return;
        }

        var currentState = rpgStateStack.Peek();
        if (currentState.index >= currentState.commands.Count)
        {
            rpgStateStack.Pop();
            AdvanceRPGDialogue();
            return;
        }

        var cmd = currentState.commands[currentState.index];
        currentState.index++;

        if (cmd is ShowTextCommand showText)
        {
            dialogText.text = showText.text;
            if (!string.IsNullOrEmpty(showText.speakerName))
            {
                if (nameBoxObject != null) nameBoxObject.SetActive(true);
                if (nameText != null) nameText.text = showText.speakerName;
            }
            else
            {
                if (nameBoxObject != null) nameBoxObject.SetActive(false);
            }
            ApplySpeakerAnim(showText.speakerAnimParam);
            ApplyColorToImage(dialogBoxImage, showText.boxColorHex);
            ApplyColorToImage(nameBoxImage, showText.boxColorHex);
            ApplyColorToImage(portraitBoxImage, showText.boxColorHex);
            HideChoices();
        }
        else if (cmd is ShowChoicesCommand showChoices)
        {
            if (!string.IsNullOrEmpty(showChoices.promptText))
            {
                dialogText.text = showChoices.promptText;
                if (!string.IsNullOrEmpty(showChoices.speakerName))
                {
                    if (nameBoxObject != null) nameBoxObject.SetActive(true);
                    if (nameText != null) nameText.text = showChoices.speakerName;
                }
                else
                {
                    if (nameBoxObject != null) nameBoxObject.SetActive(false);
                }
                ApplySpeakerAnim(showChoices.speakerAnimParam);
                ApplyColorToImage(dialogBoxImage, showChoices.boxColorHex);
                ApplyColorToImage(nameBoxImage, showChoices.boxColorHex);
                ApplyColorToImage(portraitBoxImage, showChoices.boxColorHex);
            }
            ShowRPGChoices(showChoices.choices);
        }
        else if (cmd is SetVariableCommand setVar)
        {
            if (setVar.variableToSet != null)
            {
                if (setVar.variableToSet is BoolValue boolVal)
                    boolVal.value = setVar.setBoolValue;
                else if (setVar.variableToSet is FloatValue floatVal)
                    floatVal.RuntimeValue = setVar.setFloatValue;
                else if (setVar.variableToSet is IntValue intVal)
                    intVal.RuntimeValue = setVar.setIntValue;
                else if (setVar.variableToSet is StringValue stringVal)
                    stringVal.value = setVar.setStringValue;
            }
            AdvanceRPGDialogue();
        }
        else if (cmd is RaiseSignalCommand raiseSignal)
        {
            raiseSignal.signalToRaise?.Raise();
            AdvanceRPGDialogue();
        }
        else if (cmd is RaiseNotificationCommand raiseNotif)
        {
            raiseNotif.notificationToRaise?.Raise();
            AdvanceRPGDialogue();
        }
        else if (cmd is InvokeEventCommand invokeEvent)
        {
            invokeEvent.onCommandEvent?.Invoke();
            AdvanceRPGDialogue();
        }
    }

    private void ApplyColorToImage(Image img, string hexColor)
    {
        if (img == null || string.IsNullOrEmpty(hexColor)) return;
        
        if (ColorUtility.TryParseHtmlString(hexColor, out Color color))
        {
            img.color = color;
        }
    }

    private void ShowRPGChoices(List<RPGDialogueChoice> choices)
    {
        waitingForChoice = true;
        choiceContainer.SetActive(true);

        if (choices != null && choices.Count > 0)
        {
            choiceButtonA.gameObject.SetActive(true);
            choiceLabelA.text = choices[0].choiceText;
            choiceButtonA.onClick.RemoveAllListeners();
            var choice = choices[0];
            choiceButtonA.onClick.AddListener(() => OnRPGChoiceSelected(choice));
        }
        else
        {
            choiceButtonA.gameObject.SetActive(false);
        }

        if (choices != null && choices.Count > 1)
        {
            choiceButtonB.gameObject.SetActive(true);
            choiceLabelB.text = choices[1].choiceText;
            choiceButtonB.onClick.RemoveAllListeners();
            var choice = choices[1];
            choiceButtonB.onClick.AddListener(() => OnRPGChoiceSelected(choice));
        }
        else
        {
            choiceButtonB.gameObject.SetActive(false);
        }

        if (choices != null && choices.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(choiceButtonA.gameObject);
        }
    }

    private void OnRPGChoiceSelected(RPGDialogueChoice choice)
    {
        HideChoices();
        if (choice.nestedCommands != null && choice.nestedCommands.Count > 0)
        {
            rpgStateStack.Push(new RPGDialogueState { commands = choice.nestedCommands, index = 0 });
        }
        AdvanceRPGDialogue();
    }


    void HideChoices()
    {
        waitingForChoice = false;
        choiceContainer.SetActive(false);
        lastChoiceTime = Time.time;
    }

    /// <summary>
    /// Disables the previous speaker's anim bool and enables the new one.
    /// Safely ignored if portraitAnimator is not assigned or param is empty.
    /// </summary>
    void ApplySpeakerAnim(string newParam)
    {
        if (portraitAnimator == null) return;

        // Turn off the previous speaker
        if (!string.IsNullOrEmpty(lastSpeakerParam))
            portraitAnimator.SetAnimParameter(lastSpeakerParam, false);

        // Turn on the new speaker and show the portrait container
        if (!string.IsNullOrEmpty(newParam))
        {
            if (portraitContainer != null) portraitContainer.SetActive(true);
            else portraitAnimator.gameObject.SetActive(true); // fallback

            portraitAnimator.SetAnimParameter(newParam, true);
        }
        else
        {
            // Hide the portrait container if there is no speaker
            if (portraitContainer != null) portraitContainer.SetActive(false);
            else portraitAnimator.gameObject.SetActive(false); // fallback
        }

        lastSpeakerParam = newParam;
    }

    // -------------------------------------------------------
    // Close
    // -------------------------------------------------------

    /// <summary>
    /// Force-closes any active dialog (e.g. player left trigger range).
    /// </summary>
    public void ForceClose()
    {
        if (!dialogActive) return;

        dialogActive = false;
        waitingForChoice = false;
        rpgMode = false;
        rpgStateStack.Clear();
        currentLineIndex = 0;
        currentLines = null;

        // Reset movement block when dialog ends
        if (stateMachine != null)
            stateMachine.SetMovementBlock(false);
        
        if (nameBoxObject != null) nameBoxObject.SetActive(false);
        ApplySpeakerAnim("");   // reset portrait animator
        lastSpeakerParam = "";

        HideChoices();
        ClosePanel();
    }

    void ClosePanel()
    {
        dialogObject.SetActive(false);
    }
}
