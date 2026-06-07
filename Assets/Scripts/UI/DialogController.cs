using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogController : MonoBehaviour
{
    // -------------------------------------------------------
    // Shared UI references
    // -------------------------------------------------------
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private GameObject dialogObject;

    // -------------------------------------------------------
    // Name Box UI (shows the speaker's name)
    // -------------------------------------------------------
    [Header("Name Box")]
    [SerializeField] private GameObject nameBoxObject;
    [SerializeField] private TextMeshProUGUI nameText;

    // -------------------------------------------------------
    // Portrait animator (NPC picture with talking animation)
    // -------------------------------------------------------
    [Header("Portrait")]
    [SerializeField] private GameObject portraitContainer;
    [SerializeField] private AnimatorController portraitAnimator;

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
    // Legacy fields — kept for Sign and other existing objects
    // -------------------------------------------------------
    [Header("Legacy (Sign / StringValue system)")]
    [SerializeField] private StringValue stringText;
    [SerializeField] private Notification dialogNotification;

    // -------------------------------------------------------
    // Internal state
    // -------------------------------------------------------
    private bool dialogActive = false;
    private bool waitingForChoice = false;

    // Sequential mode (string[])
    private string[] currentLines;
    private int currentLineIndex = 0;

    // Branching mode (DialogNode[])
    private DialogNode[] currentNodes;
    private int currentNodeIndex = 0;
    private bool branchingMode = false;

    // -------------------------------------------------------
    // LEGACY: ActivateDialog — used by Sign and existing objects
    // -------------------------------------------------------
    public void ActivateDialog()
    {
        dialogActive = !dialogActive;
        if (dialogActive)
        {
            dialogObject.SetActive(true);
            dialogText.text = stringText.value;
        }
        else
        {
            ClosePanel();
        }
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
        if (waitingForChoice) return;

        if (!dialogActive)
        {
            if (lines == null || lines.Length == 0) return;
            currentLines = lines;
            currentLineIndex = 0;
            branchingMode = false;
            dialogActive = true;
            dialogObject.SetActive(true);
            dialogText.text = currentLines[currentLineIndex];

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
        else if (!branchingMode)
        {
            currentLineIndex++;
            if (currentLineIndex < currentLines.Length)
                dialogText.text = currentLines[currentLineIndex];
            else
                ForceClose();
        }
    }

    // -------------------------------------------------------
    // BRANCHING: StartDialog(DialogNode[]) — used by Dialogues.cs
    // when branching is needed
    // -------------------------------------------------------

    /// <summary>
    /// Opens a dialog session driven by a DialogNode graph.
    /// On Check press: advances linear nodes or selects choice A on choice nodes.
    /// </summary>
    public void StartDialog(DialogNode[] nodes)
    {
        if (waitingForChoice)
        {
            // Check press shortcuts to choice A
            SelectChoiceA();
            return;
        }

        if (!dialogActive)
        {
            if (nodes == null || nodes.Length == 0) return;
            currentNodes = nodes;
            currentNodeIndex = 0;
            branchingMode = true;
            dialogActive = true;
            dialogObject.SetActive(true);
            ShowNode(0);
        }
        else if (branchingMode)
        {
            // Advance linear node on Check press
            DialogNode current = currentNodes[currentNodeIndex];
            if (current.choices == null || current.choices.Length == 0)
            {
                GoToNode(current.nextNodeIndex);
            }
            // If it's a choice node, Check shortcuts to choice A
            else
            {
                SelectChoiceA();
            }
        }
    }

    // -------------------------------------------------------
    // Internal branching logic
    // -------------------------------------------------------

    void ShowNode(int index)
    {
        if (index < 0 || index >= currentNodes.Length)
        {
            ForceClose();
            return;
        }

        currentNodeIndex = index;
        DialogNode node = currentNodes[index];
        dialogText.text = node.text;

        // Display speaker name
        if (!string.IsNullOrEmpty(node.speakerName))
        {
            if (nameBoxObject != null) nameBoxObject.SetActive(true);
            if (nameText != null) nameText.text = node.speakerName;
        }
        else
        {
            if (nameBoxObject != null) nameBoxObject.SetActive(false);
        }

        // Switch portrait animation to the new speaker
        ApplySpeakerAnim(node.speakerAnimParam);

        bool hasChoices = node.choices != null && node.choices.Length > 0;
        if (hasChoices)
            ShowChoices(node.choices);
        else
            HideChoices();
    }

    void ShowChoices(DialogChoice[] choices)
    {
        waitingForChoice = true;
        choiceContainer.SetActive(true);

        // Button A — always shown if at least 1 choice
        SetupButton(choiceButtonA, choiceLabelA,
            choices.Length > 0 ? choices[0] : null);

        // Button B — shown only if there's a second choice
        SetupButton(choiceButtonB, choiceLabelB,
            choices.Length > 1 ? choices[1] : null);
    }

    void SetupButton(Button btn, TextMeshProUGUI label, DialogChoice choice)
    {
        if (choice == null)
        {
            btn.gameObject.SetActive(false);
            return;
        }

        btn.gameObject.SetActive(true);
        label.text = choice.text;
        btn.onClick.RemoveAllListeners();
        int next = choice.nextNodeIndex;                          // capture for lambda
        UnityEngine.Events.UnityEvent evt = choice.onSelect;     // capture for lambda
        btn.onClick.AddListener(() => OnChoiceSelected(next, evt));
    }

    void OnChoiceSelected(int nextNodeIndex, UnityEngine.Events.UnityEvent onSelect)
    {
        HideChoices();
        onSelect?.Invoke();
        GoToNode(nextNodeIndex);
    }

    void SelectChoiceA()
    {
        if (!waitingForChoice) return;
        DialogNode current = currentNodes[currentNodeIndex];
        if (current.choices != null && current.choices.Length > 0)
        {
            DialogChoice choiceA = current.choices[0];
            OnChoiceSelected(choiceA.nextNodeIndex, choiceA.onSelect);
        }
    }

    void GoToNode(int index)
    {
        if (index < 0)
            ForceClose();
        else
            ShowNode(index);
    }

    void HideChoices()
    {
        waitingForChoice = false;
        choiceContainer.SetActive(false);
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
        branchingMode = false;
        currentLineIndex = 0;
        currentNodeIndex = 0;
        currentLines = null;
        currentNodes = null;
        
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
