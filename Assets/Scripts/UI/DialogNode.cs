using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A single option shown to the player during a branching node.
/// </summary>
[System.Serializable]
public class DialogChoice
{
    /// <summary>Label shown on the choice button.</summary>
    public string text;

    /// <summary>
    /// Index of the DialogNode to jump to when this choice is picked.
    /// Use -1 to end the conversation.
    /// </summary>
    public int nextNodeIndex;

    /// <summary>
    /// Optional events fired when this choice is selected.
    /// Wire up anything in the Inspector: scene loads, animations,
    /// notifications, inventory changes, etc.
    /// </summary>
    public UnityEvent onSelect;
}

/// <summary>
/// One step in a dialog sequence.
/// - If choices[] is empty  → linear node: press Check to go to nextNodeIndex.
/// - If choices[] has items → branching node: player must pick a choice button.
/// </summary>
[System.Serializable]
public class DialogNode
{
    [TextArea(2, 5)]
    public string text;

    /// <summary>
    /// Name of the speaker, displayed in the name box.
    /// Leave empty to hide the name box.
    /// </summary>
    public string speakerName;

    /// <summary>
    /// Name of the Animator bool parameter to set TRUE when this node is shown.
    /// The controller will set the previous parameter back to FALSE automatically.
    /// Leave empty to keep the portrait animation unchanged.
    /// Example: "Guard", "Merchant", "Player"
    /// </summary>
    public string speakerAnimParam;

    /// <summary>
    /// Leave empty for a linear node.
    /// Add up to 2 entries for a branching node.
    /// </summary>
    public DialogChoice[] choices;

    /// <summary>
    /// Used only when choices[] is empty.
    /// Index of the next DialogNode, or -1 to end the conversation.
    /// </summary>
    public int nextNodeIndex;
}
