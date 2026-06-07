using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Componente de dialogos en secuencia.
/// Añade este script a cualquier objeto interactuable que necesite
/// mostrar varios mensajes uno tras otro al pulsar "Check".
/// </summary>
public class Dialogues : Interactable
{
    [SerializeField] private DialogController dialogController;

    [TextArea(2, 6)]
    [SerializeField] private string[] dialogLines;

    public virtual void Update()
    {
        if (playerInRange)
        {
            if (Input.GetButtonDown("Check"))
            {
                dialogController.StartDialog(dialogLines);
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
