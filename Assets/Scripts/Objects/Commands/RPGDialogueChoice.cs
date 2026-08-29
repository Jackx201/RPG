using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGDialogueSystem
{
    [Serializable]
    public class RPGDialogueChoice
    {
        public string choiceText;

        [SerializeReference]
        public List<RPGDialogueCommand> nestedCommands = new List<RPGDialogueCommand>();
    }
}
