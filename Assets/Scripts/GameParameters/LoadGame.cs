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

    private void Start()
    {
        CheckSaveFile();
    }

    private void OnEnable()
    {
        CheckSaveFile();
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
        return SaveGame.HasSave();
    }

    public void LoadGameData()
    {
        // 1. Load persisted ScriptableObjects if manager or list exists
        if (saveGameManager != null)
        {
            saveGameManager.LoadScriptables();
        }
        else if (SaveGame.gameSave != null)
        {
            SaveGame.gameSave.LoadScriptables();
        }
        else if (objectsToLoad != null && objectsToLoad.Count > 0)
        {
            for (int i = 0; i < objectsToLoad.Count; i++)
            {
                string path = Application.persistentDataPath + string.Format("/{0}.dat", i);
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
        string sceneToLoad = SaveGame.GetSavedSceneName(defaultScene);

        // 3. Load the scene
        Debug.Log("Loading saved scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}

