using System;
using UnityEngine.Events;

namespace RPGDialogueSystem
{
    [Serializable]
    public sealed class InvokeEventCommand : RPGDialogueCommand
    {
        public override CommandType Type => CommandType.InvokeEvent;

        // UnityEvent serializes correctly in scenes/prefabs via Unity's serialization.
        // It cannot be round-tripped via JsonUtility — this is a known limitation.
        public UnityEvent onCommandEvent;
    }
}
