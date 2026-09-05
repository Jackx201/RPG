using System;
using UnityEngine;

namespace RPGDialogueSystem
{
    [Serializable]
    public sealed class ShowTextCommand : RPGDialogueCommand
    {
        public override CommandType Type => CommandType.ShowText;

        public string speakerName;
        public string speakerAnimParam;
        public string boxColorHex = "#FFFFFF";
        [TextArea(2, 5)]
        public string text;
    }
}
