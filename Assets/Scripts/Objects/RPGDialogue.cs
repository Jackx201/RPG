using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace RPGDialogueSystem
{
    [DisallowMultipleComponent]
    public class RPGDialogue : MonoBehaviour
    {
        [Header("File Storage")]
        [Tooltip("Path relative to the Assets folder. Dev utility only — JsonUtility does not support [SerializeReference].")]
        [SerializeField] private string filePath = "";

        [Header("Dialogue Content")]
        [SerializeReference] private List<RPGDialogueCommand> commands = new List<RPGDialogueCommand>();

        public IReadOnlyList<RPGDialogueCommand> Commands => commands;

        private void Reset()
        {
#if UNITY_EDITOR
            AutoGenerateFilePath();
#endif
        }

#if UNITY_EDITOR
        private void AutoGenerateFilePath()
        {
            if (!string.IsNullOrEmpty(filePath)) return;

            const string folder = "Assets/Dialogues";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            filePath = $"{folder}/{gameObject.name}_Dialogue.json";
        }

        // NOTE: JsonUtility cannot serialize [SerializeReference] polymorphic types directly.
        // A flat DTO layer (RPGDialogueSerializer) is used to bridge the two.
        // Unity object references (signals, events, variable assets) are NOT preserved in JSON
        // and must be re-linked in the Inspector after loading.
        public void SaveToFile()
        {
            if (string.IsNullOrEmpty(filePath))
                AutoGenerateFilePath();

            try
            {
                string json = RPGDialogueSerializer.Serialize(commands);
                File.WriteAllText(filePath, json);
                AssetDatabase.ImportAsset(filePath);
                Debug.Log($"[RPGDialogue] Saved {commands.Count} command(s) to: {filePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[RPGDialogue] Save failed: {e.Message}");
            }
        }

        public void LoadFromFile()
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                Debug.LogWarning($"[RPGDialogue] File not found: '{filePath}'");
                return;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                commands = RPGDialogueSerializer.Deserialize(json);
                EditorUtility.SetDirty(this);
                Debug.Log($"[RPGDialogue] Loaded {commands.Count} command(s) from: {filePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[RPGDialogue] Load failed: {e.Message}");
            }
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RPGDialogue))]
    public class RPGDialogueEditor : Editor
    {
        private const int MaxNestingDepth = 4;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            RPGDialogue rpgDialogue = (RPGDialogue)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("filePath"));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save to File")) rpgDialogue.SaveToFile();
            if (GUILayout.Button("Load from File"))
            {
                rpgDialogue.LoadFromFile();
                serializedObject.Update();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("RPG Maker-Style Dialogues", EditorStyles.boldLabel);

            DrawCommandList(serializedObject.FindProperty("commands"), 0);

            serializedObject.ApplyModifiedProperties();
        }

        // ── Command List ─────────────────────────────────────────────────────────

        private void DrawCommandList(SerializedProperty listProp, int indentLevel)
        {
            if (listProp == null) return;

            if (indentLevel > MaxNestingDepth)
            {
                EditorGUILayout.HelpBox("Maximum nesting depth reached.", MessageType.Warning);
                return;
            }

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(listProp.displayName, EditorStyles.boldLabel);
            if (GUILayout.Button("+ Add Command", GUILayout.Width(120)))
                ShowAddCommandMenu(listProp);
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < listProp.arraySize; i++)
            {
                SerializedProperty cmdProp = listProp.GetArrayElementAtIndex(i);
                RPGDialogueCommand cmd = cmdProp.managedReferenceValue as RPGDialogueCommand;

                EditorGUI.indentLevel = indentLevel + 1;
                EditorGUILayout.BeginVertical("helpbox");

                // Header row: type label + delete button
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(cmd != null ? cmd.Type.ToString() : "(null)", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    listProp.DeleteArrayElementAtIndex(i);
                    i--;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    continue;
                }
                EditorGUILayout.EndHorizontal();

                if (cmd != null)
                    DrawCommandFields(cmdProp, cmd, indentLevel);

                EditorGUILayout.EndVertical();
            }

            EditorGUI.indentLevel = indentLevel;
            EditorGUILayout.EndVertical();
        }

        // ── Per-type Field Drawing ────────────────────────────────────────────────

        private void DrawCommandFields(SerializedProperty cmdProp, RPGDialogueCommand cmd, int indentLevel)
        {
            if (cmd is ShowTextCommand)
            {
                EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("speakerName"),    new GUIContent("Speaker Name"));
                EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("speakerAnimParam"), new GUIContent("Portrait Param"));
                EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("boxColorHex"),    new GUIContent("Box Color Hex"));
                EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("text"),           new GUIContent("Text"));
            }
            else if (cmd is ShowChoicesCommand)
            {
                EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("speakerName"),    new GUIContent("Speaker Name"));
                EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("speakerAnimParam"), new GUIContent("Portrait Param"));
                EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("boxColorHex"),    new GUIContent("Box Color Hex"));
                EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("promptText"),     new GUIContent("Prompt Text"));
                DrawChoicesList(cmdProp.FindPropertyRelative("choices"), indentLevel);
            }
            else if (cmd is SetVariableCommand)
            {
                DrawSetVariableFields(cmdProp);
            }
            else if (cmd is RaiseSignalCommand)
            {
                EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("signalToRaise"), new GUIContent("Signal To Raise"));
            }
            else if (cmd is RaiseNotificationCommand)
            {
                EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("notificationToRaise"), new GUIContent("Notification To Raise"));
            }
            else if (cmd is InvokeEventCommand)
            {
                EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("onCommandEvent"), new GUIContent("Event To Invoke"));
            }
        }

        private void DrawChoicesList(SerializedProperty choicesProp, int indentLevel)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Choices", EditorStyles.boldLabel);
            if (GUILayout.Button("+ Add Choice", GUILayout.Width(100)))
            {
                choicesProp.arraySize++;
                SerializedProperty newChoice = choicesProp.GetArrayElementAtIndex(choicesProp.arraySize - 1);
                newChoice.FindPropertyRelative("choiceText").stringValue = $"Option {choicesProp.arraySize}";
                newChoice.FindPropertyRelative("nestedCommands").ClearArray();
            }
            EditorGUILayout.EndHorizontal();

            for (int j = 0; j < choicesProp.arraySize; j++)
            {
                SerializedProperty choiceProp = choicesProp.GetArrayElementAtIndex(j);
                EditorGUILayout.BeginVertical("helpbox");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(choiceProp.FindPropertyRelative("choiceText"), GUIContent.none, GUILayout.Width(150));
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Delete Choice", GUILayout.Width(100)))
                {
                    choicesProp.DeleteArrayElementAtIndex(j);
                    j--;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    continue;
                }
                EditorGUILayout.EndHorizontal();

                DrawCommandList(choiceProp.FindPropertyRelative("nestedCommands"), indentLevel + 1);

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawSetVariableFields(SerializedProperty cmdProp)
        {
            SerializedProperty varProp = cmdProp.FindPropertyRelative("variableToSet");
            EditorGUILayout.PropertyField(varProp, new GUIContent("Variable Object"));

            ScriptableObject varObj = varProp.objectReferenceValue as ScriptableObject;
            if (varObj == null) return;

            if (varObj is BoolValue)
                EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("setBoolValue"),   new GUIContent("Bool Value"));
            else if (varObj is FloatValue)
                EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("setFloatValue"),  new GUIContent("Float Value"));
            else if (varObj is IntValue)
                EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("setIntValue"),    new GUIContent("Int Value"));
            else if (varObj is StringValue)
                EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("setStringValue"), new GUIContent("String Value"));
            else
                EditorGUILayout.HelpBox("Unsupported variable type. Supported: BoolValue, FloatValue, IntValue, StringValue.", MessageType.Warning);
        }

        // ── Add Command Menu ──────────────────────────────────────────────────────

        private void ShowAddCommandMenu(SerializedProperty listProp)
        {
            GenericMenu menu = new GenericMenu();
            AddMenuEntry<ShowTextCommand>(menu,          listProp, "Show Text");
            AddMenuEntry<ShowChoicesCommand>(menu,       listProp, "Show Choices");
            AddMenuEntry<SetVariableCommand>(menu,       listProp, "Set Variable");
            AddMenuEntry<RaiseSignalCommand>(menu,       listProp, "Raise Signal");
            AddMenuEntry<RaiseNotificationCommand>(menu, listProp, "Raise Notification");
            AddMenuEntry<InvokeEventCommand>(menu,       listProp, "Invoke Event");
            menu.ShowAsContext();
        }

        private void AddMenuEntry<T>(GenericMenu menu, SerializedProperty listProp, string label)
            where T : RPGDialogueCommand, new()
        {
            menu.AddItem(new GUIContent(label), false, () =>
            {
                listProp.arraySize++;
                listProp.GetArrayElementAtIndex(listProp.arraySize - 1).managedReferenceValue = new T();
                serializedObject.ApplyModifiedProperties();
            });
        }
    }
#endif
}
