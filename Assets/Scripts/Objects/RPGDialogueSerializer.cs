#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEngine;

// Flat DTO layer that matches the on-disk JSON format.
// Exists only to bridge [SerializeReference] (which JsonUtility cannot handle)
// with JsonUtility's flat serialization. The DTO format is backwards-compatible
// with JSON files saved by the original flat RPGDialogueCommand class.
//
// Limitation: Unity object references (variableToSet, signalToRaise,
// notificationToRaise, onCommandEvent) cannot be round-tripped via JSON.
// They are omitted from the DTO. After loading, reassign them in the Inspector.

namespace RPGDialogueSystem
{
    [Serializable]
    internal class RPGDialogueCommandDTO
    {
        public int    type;
        public string speakerName     = "";
        public string speakerAnimParam = "";
        public string boxColorHex     = "#FFFFFF";
        public string text            = "";   // maps to ShowText.text OR ShowChoices.promptText
        public List<RPGDialogueChoiceDTO> choices = new List<RPGDialogueChoiceDTO>();

        // SetVariable value fields
        public bool   setBoolValue;
        public float  setFloatValue;
        public int    setIntValue;
        public string setStringValue  = "";
    }

    [Serializable]
    internal class RPGDialogueChoiceDTO
    {
        public string choiceText = "";
        public List<RPGDialogueCommandDTO> nestedCommands = new List<RPGDialogueCommandDTO>();
    }

    [Serializable]
    internal class RPGDialogueDataDTO
    {
        public List<RPGDialogueCommandDTO> commands = new List<RPGDialogueCommandDTO>();
    }

    internal static class RPGDialogueSerializer
    {
        // ── Public API ─────────────────────────────────────────────────────────

        public static string Serialize(IReadOnlyList<RPGDialogueCommand> commands)
        {
            var data = new RPGDialogueDataDTO();
            foreach (var cmd in commands)
                data.commands.Add(ToDTO(cmd));
            return JsonUtility.ToJson(data, prettyPrint: true);
        }

        public static List<RPGDialogueCommand> Deserialize(string json)
        {
            var data = JsonUtility.FromJson<RPGDialogueDataDTO>(json);
            var result = new List<RPGDialogueCommand>();
            if (data?.commands == null) return result;
            foreach (var dto in data.commands)
                result.Add(FromDTO(dto));
            return result;
        }

        // ── Command → DTO ──────────────────────────────────────────────────────

        private static RPGDialogueCommandDTO ToDTO(RPGDialogueCommand cmd)
        {
            var dto = new RPGDialogueCommandDTO { type = (int)cmd.Type };

            if (cmd is ShowTextCommand t)
            {
                dto.speakerName      = t.speakerName      ?? "";
                dto.speakerAnimParam = t.speakerAnimParam ?? "";
                dto.boxColorHex      = t.boxColorHex      ?? "#FFFFFF";
                dto.text             = t.text             ?? "";
            }
            else if (cmd is ShowChoicesCommand c)
            {
                dto.speakerName      = c.speakerName      ?? "";
                dto.speakerAnimParam = c.speakerAnimParam ?? "";
                dto.boxColorHex      = c.boxColorHex      ?? "#FFFFFF";
                dto.text             = c.promptText       ?? ""; // stored in "text" for compat
                foreach (var choice in c.choices)
                    dto.choices.Add(ToChoiceDTO(choice));
            }
            else if (cmd is SetVariableCommand sv)
            {
                dto.setBoolValue   = sv.setBoolValue;
                dto.setFloatValue  = sv.setFloatValue;
                dto.setIntValue    = sv.setIntValue;
                dto.setStringValue = sv.setStringValue ?? "";
                // variableToSet (Unity object ref) cannot be serialized to JSON — reassign in Inspector after load.
            }
            // RaiseSignal / RaiseNotification / InvokeEvent: all hold Unity object refs only — nothing to serialize.

            return dto;
        }

        private static RPGDialogueChoiceDTO ToChoiceDTO(RPGDialogueChoice choice)
        {
            var dto = new RPGDialogueChoiceDTO { choiceText = choice.choiceText ?? "" };
            foreach (var cmd in choice.nestedCommands)
                dto.nestedCommands.Add(ToDTO(cmd));
            return dto;
        }

        // ── DTO → Command ──────────────────────────────────────────────────────

        private static RPGDialogueCommand FromDTO(RPGDialogueCommandDTO dto)
        {
            switch ((CommandType)dto.type)
            {
                case CommandType.ShowText:
                    return new ShowTextCommand
                    {
                        speakerName      = dto.speakerName,
                        speakerAnimParam = dto.speakerAnimParam,
                        boxColorHex      = dto.boxColorHex,
                        text             = dto.text
                    };

                case CommandType.ShowChoices:
                {
                    var cmd = new ShowChoicesCommand
                    {
                        speakerName      = dto.speakerName,
                        speakerAnimParam = dto.speakerAnimParam,
                        boxColorHex      = dto.boxColorHex,
                        promptText       = dto.text   // "text" field is reused for prompt
                    };
                    foreach (var c in dto.choices)
                        cmd.choices.Add(FromChoiceDTO(c));
                    return cmd;
                }

                case CommandType.SetVariable:
                    return new SetVariableCommand
                    {
                        setBoolValue   = dto.setBoolValue,
                        setFloatValue  = dto.setFloatValue,
                        setIntValue    = dto.setIntValue,
                        setStringValue = dto.setStringValue
                        // variableToSet: must be re-linked in the Inspector after load
                    };

                case CommandType.RaiseSignal:
                    return new RaiseSignalCommand();   // signalToRaise: re-link in Inspector

                case CommandType.RaiseNotification:
                    return new RaiseNotificationCommand(); // re-link in Inspector

                case CommandType.InvokeEvent:
                    return new InvokeEventCommand();   // re-link in Inspector

                default:
                    Debug.LogWarning($"[RPGDialogueSerializer] Unknown command type {dto.type} — skipped.");
                    return new ShowTextCommand { text = $"[Unknown type {dto.type}]" };
            }
        }

        private static RPGDialogueChoice FromChoiceDTO(RPGDialogueChoiceDTO dto)
        {
            var choice = new RPGDialogueChoice { choiceText = dto.choiceText };
            foreach (var cmd in dto.nestedCommands)
                choice.nestedCommands.Add(FromDTO(cmd));
            return choice;
        }
    }
}

#endif
