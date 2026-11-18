using System;

namespace RPGDialogueSystem
{
    [Serializable]
    public enum CommandType
    {
        ShowText,
        ShowChoices,
        SetVariable,
        RaiseSignal,
        RaiseNotification,
        InvokeEvent
    }

    [Serializable]
    public abstract class RPGDialogueCommand
    {
        public abstract CommandType Type { get; }
    }
}
