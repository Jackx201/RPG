using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGDialogueSystem
{
    [Serializable]
    public sealed class ShowChoicesCommand : RPGDialogueCommand
    {
        public override CommandType Type => CommandType.ShowChoices;

        public string speakerName;
        public string speakerAnimParam;
        public string boxColorHex = "#FFFFFF";
        [TextArea(2, 5)]
        public string promptText;
        public List<RPGDialogueChoice> choices = new List<RPGDialogueChoice>();
    }
}
