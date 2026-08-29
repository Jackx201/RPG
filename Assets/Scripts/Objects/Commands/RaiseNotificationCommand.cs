using System;
using UnityEngine;

namespace RPGDialogueSystem
{
    [Serializable]
    public sealed class RaiseNotificationCommand : RPGDialogueCommand
    {
        public override CommandType Type => CommandType.RaiseNotification;

        public Notification notificationToRaise;
    }
}
