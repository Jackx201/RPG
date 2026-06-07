using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogController : MonoBehaviour
{
    [SerializeField] private StringValue stringText;
    [SerializeField] private Notification dialogNotification;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private GameObject dialogObject;
    [SerializeField] private bool dialogActive = false;

    // -------------------------------------------------------
    // Metodo original: usado por Sign y otros objetos existentes
    // -------------------------------------------------------
    public void ActivateDialog()
    {
        dialogActive = !dialogActive;
        if (dialogActive)
        {
            SetDialog();
        }
        else
        {
            DeactivateDialog();
        }
    }

    void SetDialog()
    {
        dialogObject.SetActive(true);
        dialogText.text = stringText.value;
    }

    // -------------------------------------------------------
    // Metodo nuevo: usado por Dialogues.cs (lista de dialogos)
    // -------------------------------------------------------

    private string[] currentLines;
    private int currentIndex = 0;

    /// <summary>
    /// Inicia o avanza una secuencia de dialogos.
    /// - Primer llamado: abre el panel y muestra la linea 0.
    /// - Llamados siguientes: avanza linea a linea y se cierra al terminar.
    /// </summary>
    public void StartDialog(string[] lines)
    {
        if (!dialogActive)
        {
            if (lines == null || lines.Length == 0) return;
            currentLines = lines;
            currentIndex = 0;
            dialogActive = true;
            dialogObject.SetActive(true);
            dialogText.text = currentLines[currentIndex];
        }
        else
        {
            currentIndex++;
            if (currentIndex < currentLines.Length)
            {
                dialogText.text = currentLines[currentIndex];
            }
            else
            {
                ForceClose();
            }
        }
    }

    /// <summary>
    /// Cierra el dialogo inmediatamente (cuando el jugador sale del rango).
    /// </summary>
    public void ForceClose()
    {
        if (dialogActive)
        {
            dialogActive = false;
            currentIndex = 0;
            currentLines = null;
            DeactivateDialog();
        }
    }

    void DeactivateDialog()
    {
        dialogObject.SetActive(false);
    }
}
