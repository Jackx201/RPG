using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGame : MonoBehaviour
{
    [Header("Save / Load Configuration")]
    [SerializeField] private SaveGame saveGameManager;
    [SerializeField] private string defaultScene = "SampleScene";
    [SerializeField] private Button loadButton;
    [SerializeField] private List<ScriptableObject> objectsToLoad = new List<ScriptableObject>();

    [Header("Multi-Slot UI (Optional)")]
    [SerializeField] private Button[] slotButtons;
    [SerializeField] private Text[] slotTexts;

    private void Start()
    {
        CheckSaveFile();
        UpdateSlotUI();
    }

    private void OnEnable()
    {
        CheckSaveFile();
        UpdateSlotUI();
    }

    public void CheckSaveFile()
    {
        if (loadButton != null)
        {
            loadButton.interactable = HasSaveGame();
        }
    }

    public bool HasSaveGame()
    {
        return SaveGame.HasSave(SaveGame.GetActiveSlot());
    }

    public bool HasSlotSave(int slot)
    {
        return SaveGame.HasSave(slot);
    }

    public string GetSlotScene(int slot)
    {
        if (!SaveGame.HasSave(slot)) return "Empty";
        return SaveGame.GetSavedSceneName(slot, defaultScene);
    }

    public void UpdateSlotUI()
    {
        if (slotButtons != null && slotButtons.Length > 0)
        {
            for (int i = 0; i < slotButtons.Length; i++)
            {
                if (slotButtons[i] != null)
                {
                    slotButtons[i].interactable = HasSlotSave(i);
                }
            }
        }

        if (slotTexts != null && slotTexts.Length > 0)
        {
            for (int i = 0; i < slotTexts.Length; i++)
            {
                if (slotTexts[i] != null)
                {
                    slotTexts[i].text = string.Format("Slot {0}: {1}", i + 1, GetSlotScene(i));
                }
            }
        }
    }

    public void SelectSlot(int slot)
    {
        SaveGame.SetActiveSlot(slot);
    }

    public void LoadSlot(int slot)
    {
        SaveGame.SetActiveSlot(slot);
        LoadGameData();
    }

    public void LoadSlot1() { LoadSlot(0); }
    public void LoadSlot2() { LoadSlot(1); }
    public void LoadSlot3() { LoadSlot(2); }

    public void NewGameSlot(int slot)
    {
        SaveGame.SetActiveSlot(slot);
        SaveGame.ResetAllScriptables(slot);
        SceneManager.LoadScene(defaultScene);
    }

    public void NewGameSlot1() { NewGameSlot(0); }
    public void NewGameSlot2() { NewGameSlot(1); }
    public void NewGameSlot3() { NewGameSlot(2); }

    public void DeleteSlot(int slot)
    {
        SaveGame.ResetSlot(slot);
        UpdateSlotUI();
        CheckSaveFile();
    }

    public void DeleteSlot1() { DeleteSlot(0); }
    public void DeleteSlot2() { DeleteSlot(1); }
    public void DeleteSlot3() { DeleteSlot(2); }

    public void LoadGameData()
    {
        int slot = SaveGame.GetActiveSlot();

        // 1. Load persisted ScriptableObjects
        if (saveGameManager != null)
        {
            saveGameManager.LoadScriptables(slot);
        }
        else if (SaveGame.gameSave != null)
        {
            SaveGame.gameSave.LoadScriptables(slot);
        }
        else if (objectsToLoad != null && objectsToLoad.Count > 0)
        {
            string slotDir = SaveGame.GetSlotDirectory(slot);
            for (int i = 0; i < objectsToLoad.Count; i++)
            {
                string path = Path.Combine(slotDir, string.Format("{0}.dat", i));
                if (!File.Exists(path) && slot == 0)
                {
                    path = Application.persistentDataPath + string.Format("/{0}.dat", i);
                }

                if (File.Exists(path))
                {
                    FileStream file = File.Open(path, FileMode.Open);
                    BinaryFormatter binary = new BinaryFormatter();
                    JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), objectsToLoad[i]);
                    file.Close();
                }
                else
                {
                    SaveGame.ResetObject(objectsToLoad[i]);
                }
            }
        }

        // 2. Get saved scene name
        string sceneToLoad = SaveGame.GetSavedSceneName(slot, defaultScene);

        // 3. Load the scene
        Debug.Log("Loading saved slot " + slot + " scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}

