using System;
using UnityEngine;

namespace RPGDialogueSystem
{
    // variableType string removed — runtime should use `is` type checks on variableToSet directly.
    [Serializable]
    public sealed class SetVariableCommand : RPGDialogueCommand
    {
        public override CommandType Type => CommandType.SetVariable;

        public ScriptableObject variableToSet;

        // Only the field matching the concrete type of variableToSet is meaningful.
        // The editor only draws the relevant one.
        public bool setBoolValue;
        public float setFloatValue;
        public int setIntValue;
        public string setStringValue;
    }
}
