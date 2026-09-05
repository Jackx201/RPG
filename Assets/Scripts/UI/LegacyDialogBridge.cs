using UnityEngine;
using TMPro;

/// <summary>
/// Legacy adapter kept for Sign and other objects that use the StringValue/Notification system.
/// Drives the shared dialog UI directly without touching DialogController's API.
/// </summary>
public class LegacyDialogBridge : MonoBehaviour
{
    // -------------------------------------------------------
    // Legacy fields — Sign / StringValue system
    // -------------------------------------------------------
    [Header("Legacy (Sign / StringValue system)")]
    [SerializeField] private StringValue stringText;
    [SerializeField] private Notification dialogNotification;

    // -------------------------------------------------------
    // UI references (assign the same objects as DialogController)
    // -------------------------------------------------------
    [Header("Dialog UI")]
    [SerializeField] private GameObject dialogObject;
    [SerializeField] private TextMeshProUGUI dialogText;

    private bool dialogActive = false;

    // -------------------------------------------------------
    // LEGACY: called by Sign and other existing objects
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
            dialogObject.SetActive(false);
        }
    }
}
