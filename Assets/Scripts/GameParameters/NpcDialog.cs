using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcDialog : Interactable
{
    [SerializeField] private TextAssetValue dialogValue;
    [SerializeField] private TextAsset myDialog;
    [SerializeField] private Notification BracnhingNotification;
    [SerializeField] private Notification LeavingNotification;

    void Update()
    {
        if(playerInRange)
        {
            if(Input.GetButtonDown("Check"))
            {
                dialogValue.value = myDialog;
                BracnhingNotification.Raise();
            }
        } else {
            LeavingNotification.Raise();
        }
    }
}
