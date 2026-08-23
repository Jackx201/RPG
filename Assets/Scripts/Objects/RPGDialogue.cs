using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RPGDialogueSystem
{
    [Serializable]
    public enum CommandType
    {
        ShowText,
        ShowChoices,
        SetVariable,
        RaiseSignal,
        RaiseNotification
    }

    [Serializable]
    public class RPGDialogueCommand
    {
        public CommandType type = CommandType.ShowText;

        [Header("Show Text Settings")]
        public string speakerName;
        public string speakerAnimParam;
        public string boxColorHex = "#FFFFFF";
        [TextArea(2, 5)]
        public string text;

        [Header("Show Choices Settings")]
        public List<RPGDialogueChoice> choices = new List<RPGDialogueChoice>();

        [Header("Set Variable Settings")]
        public ScriptableObject variableToSet;
        public string variableType; // "Bool", "Float", "Int", "String"
        public bool setBoolValue;
        public float setFloatValue;
        public int setIntValue;
        public string setStringValue;

        [Header("Raise Signal Settings")]
        public SignalSender signalToRaise;

        [Header("Raise Notification Settings")]
        public Notification notificationToRaise;
    }

    [Serializable]
    public class RPGDialogueChoice
    {
        public string choiceText;
        public List<RPGDialogueCommand> nestedCommands = new List<RPGDialogueCommand>();
    }

    [Serializable]
    public class RPGDialogueData
    {
        public List<RPGDialogueCommand> commands = new List<RPGDialogueCommand>();
    }

    [DisallowMultipleComponent]
    public class RPGDialogue : MonoBehaviour
    {
        [Header("File Storage")]
        [Tooltip("The text file path relative to the Assets folder where the dialogue will be saved/loaded.")]
        [SerializeField] private string filePath = "";

        [Header("Dialogue Content")]
        [SerializeField] private List<RPGDialogueCommand> commands = new List<RPGDialogueCommand>();

        public List<RPGDialogueCommand> Commands => commands;

        private void Reset()
        {
            AutoGenerateFilePath();
        }

        private void AutoGenerateFilePath()
        {
            if (string.IsNullOrEmpty(filePath))
            {
                string folder = Path.Combine("Assets", "Dialogues");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                filePath = Path.Combine(folder, $"{gameObject.name}_Dialogue.json").Replace("\\", "/");
            }
        }

        public void SaveToFile()
        {
            if (string.IsNullOrEmpty(filePath))
            {
                AutoGenerateFilePath();
            }

            try
            {
                RPGDialogueData data = new RPGDialogueData { commands = this.commands };
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(filePath, json);
#if UNITY_EDITOR
                AssetDatabase.ImportAsset(filePath);
#endif
                Debug.Log($"Dialogue saved to: {filePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving dialogue to {filePath}: {e.Message}");
            }
        }

        public void LoadFromFile()
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                RPGDialogueData data = JsonUtility.FromJson<RPGDialogueData>(json);
                if (data != null)
                {
                    this.commands = data.commands;
                    Debug.Log($"Dialogue loaded from: {filePath}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading dialogue from {filePath}: {e.Message}");
            }
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(filePath))
            {
                AutoGenerateFilePath();
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RPGDialogue))]
    public class RPGDialogueEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            RPGDialogue rpgDialogue = (RPGDialogue)target;
            
            // File Storage field
            SerializedProperty filePathProp = serializedObject.FindProperty("filePath");
            EditorGUILayout.PropertyField(filePathProp);
            
            // Buttons to manual Save/Load
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save to Text File"))
            {
                rpgDialogue.SaveToFile();
            }
            if (GUILayout.Button("Load from Text File"))
            {
                rpgDialogue.LoadFromFile();
                serializedObject.Update();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("RPG Maker-Style Dialogues", EditorStyles.boldLabel);
            
            SerializedProperty commandsProp = serializedObject.FindProperty("commands");
            DrawCommandList(commandsProp, 0);
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCommandList(SerializedProperty listProp, int indentLevel)
        {
            if (listProp == null) return;

            EditorGUILayout.BeginVertical("box");
            
            // Header with add command button
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(listProp.displayName), EditorStyles.boldLabel);
            if (GUILayout.Button("+ Add Command", GUILayout.Width(120)))
            {
                listProp.arraySize++;
                SerializedProperty newCmd = listProp.GetArrayElementAtIndex(listProp.arraySize - 1);
                // Reset fields of new command
                newCmd.FindPropertyRelative("type").enumValueIndex = 0;
                newCmd.FindPropertyRelative("speakerName").stringValue = "";
                newCmd.FindPropertyRelative("speakerAnimParam").stringValue = "";
                newCmd.FindPropertyRelative("boxColorHex").stringValue = "#FFFFFF";
                newCmd.FindPropertyRelative("text").stringValue = "";
                newCmd.FindPropertyRelative("choices").ClearArray();
                newCmd.FindPropertyRelative("variableToSet").objectReferenceValue = null;
                newCmd.FindPropertyRelative("variableType").stringValue = "";
                newCmd.FindPropertyRelative("signalToRaise").objectReferenceValue = null;
                newCmd.FindPropertyRelative("notificationToRaise").objectReferenceValue = null;
            }
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < listProp.arraySize; i++)
            {
                SerializedProperty cmdProp = listProp.GetArrayElementAtIndex(i);
                
                // Indent level representation
                EditorGUI.indentLevel = indentLevel + 1;
                
                EditorGUILayout.BeginVertical("helpbox");
                
                EditorGUILayout.BeginHorizontal();
                SerializedProperty typeProp = cmdProp.FindPropertyRelative("type");
                EditorGUILayout.PropertyField(typeProp, GUIContent.none, GUILayout.Width(100));
                
                // Delete button
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
                
                CommandType type = (CommandType)typeProp.enumValueIndex;
                if (type == CommandType.ShowText)
                {
                    EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("speakerName"), new GUIContent("Speaker Name"));
                    EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("speakerAnimParam"), new GUIContent("Portrait Param"));
                    EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("boxColorHex"), new GUIContent("Box Color Hex"));
                    EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("text"), new GUIContent("Text"));
                }
                else if (type == CommandType.ShowChoices)
                {
                    // Optionally show a prompt text for the choice
                    EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("speakerName"), new GUIContent("Speaker Name"));
                    EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("speakerAnimParam"), new GUIContent("Portrait Param"));
                    EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("boxColorHex"), new GUIContent("Box Color Hex"));
                    EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("text"), new GUIContent("Prompt Text"));

                    SerializedProperty choicesProp = cmdProp.FindPropertyRelative("choices");
                    
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Choices Options", EditorStyles.boldLabel);
                    if (GUILayout.Button("+ Add Choice", GUILayout.Width(100)))
                    {
                        choicesProp.arraySize++;
                        SerializedProperty newChoice = choicesProp.GetArrayElementAtIndex(choicesProp.arraySize - 1);
                        newChoice.FindPropertyRelative("choiceText").stringValue = "Option " + choicesProp.arraySize;
                        newChoice.FindPropertyRelative("nestedCommands").ClearArray();
                    }
                    EditorGUILayout.EndHorizontal();

                    for (int j = 0; j < choicesProp.arraySize; j++)
                    {
                        SerializedProperty choiceProp = choicesProp.GetArrayElementAtIndex(j);
                        EditorGUILayout.BeginVertical("helpbox");
                        
                        EditorGUILayout.BeginHorizontal();
                        SerializedProperty choiceTextProp = choiceProp.FindPropertyRelative("choiceText");
                        EditorGUILayout.PropertyField(choiceTextProp, GUIContent.none, GUILayout.Width(150));
                        
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

                        // Nested commands
                        SerializedProperty nestedCmdsProp = choiceProp.FindPropertyRelative("nestedCommands");
                        DrawCommandList(nestedCmdsProp, indentLevel + 1);

                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();
                }
                else if (type == CommandType.SetVariable)
                {
                    SerializedProperty varProp = cmdProp.FindPropertyRelative("variableToSet");
                    EditorGUILayout.PropertyField(varProp, new GUIContent("Variable Object"));
                    
                    ScriptableObject varObj = (ScriptableObject)varProp.objectReferenceValue;
                    if (varObj != null)
                    {
                        string varType = "";
                        if (varObj.GetType().Name == "BoolValue")
                        {
                            varType = "Bool";
                            EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("setBoolValue"), new GUIContent("Set Bool Value"));
                        }
                        else if (varObj.GetType().Name == "FloatValue")
                        {
                            varType = "Float";
                            EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("setFloatValue"), new GUIContent("Set Float Value"));
                        }
                        else if (varObj.GetType().Name == "IntValue")
                        {
                            varType = "Int";
                            EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("setIntValue"), new GUIContent("Set Int Value"));
                        }
                        else if (varObj.GetType().Name == "StringValue")
                        {
                            varType = "String";
                            EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("setStringValue"), new GUIContent("Set String Value"));
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Unsupported Variable Type. Only BoolValue, FloatValue, IntValue, and StringValue are supported.", MessageType.Warning);
                        }
                        cmdProp.FindPropertyRelative("variableType").stringValue = varType;
                    }
                }
                else if (type == CommandType.RaiseSignal)
                {
                    EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("signalToRaise"), new GUIContent("Signal To Raise"));
                }
                else if (type == CommandType.RaiseNotification)
                {
                    EditorGUILayout.PropertyField(cmdProp.FindPropertyRelative("notificationToRaise"), new GUIContent("Notification To Raise"));
                }
                
                EditorGUILayout.EndVertical();
            }
            
            EditorGUI.indentLevel = indentLevel;
            EditorGUILayout.EndVertical();
        }
    }
#endif
}
