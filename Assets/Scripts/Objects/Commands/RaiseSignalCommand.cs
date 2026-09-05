using System;
using UnityEngine;

namespace RPGDialogueSystem
{
    [Serializable]
    public sealed class RaiseSignalCommand : RPGDialogueCommand
    {
        public override CommandType Type => CommandType.RaiseSignal;

        public SignalSender signalToRaise;
    }
}
