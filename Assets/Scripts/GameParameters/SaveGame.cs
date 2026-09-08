using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SaveGame : MonoBehaviour
{
    public static SaveGame gameSave;
    public List<ScriptableObject> objects = new List<ScriptableObject>();

    public void ResetScriptable()
    {
        ResetAllScriptables();
    }

    public static void ResetAllScriptables()
    {
        // 1. Delete all save files on disk
        for (int i = 0; i < 200; i++)
        {
            string path = Application.persistentDataPath + string.Format("/{0}.dat", i);
            if (File.Exists(path))
            {
                try { File.Delete(path); } catch { }
            }
            string invPath = Application.persistentDataPath + string.Format("/{0}.inv", i);
            if (File.Exists(invPath))
            {
                try { File.Delete(invPath); } catch { }
            }
        }

        string scenePath = Application.persistentDataPath + "/savedScene.dat";
        if (File.Exists(scenePath))
        {
            try { File.Delete(scenePath); } catch { }
        }
        PlayerPrefs.DeleteKey("SavedScene");
        PlayerPrefs.Save();

        // 2. Reset all in-memory BoolValue objects
        BoolValue[] bools = Resources.FindObjectsOfTypeAll<BoolValue>();
        foreach (var b in bools)
        {
            if (b != null)
            {
                b.ResetValue();
            }
        }

        // 3. Reset all in-memory FloatValue objects
        FloatValue[] floats = Resources.FindObjectsOfTypeAll<FloatValue>();
        foreach (var f in floats)
        {
            if (f != null)
            {
                f.RuntimeValue = f.initialValue;
            }
        }

        // 4. Reset all in-memory IntValue objects
        IntValue[] ints = Resources.FindObjectsOfTypeAll<IntValue>();
        foreach (var it in ints)
        {
            if (it != null)
            {
                it.RuntimeValue = it.initialValue;
            }
        }

        // 5. Reset all in-memory VectorValue objects
        VectorValue[] vectors = Resources.FindObjectsOfTypeAll<VectorValue>();
        foreach (var v in vectors)
        {
            if (v != null)
            {
                v.initialValue = v.defaultValue;
            }
        }

        // 6. Reset all in-memory Inventory objects
        Inventory[] inventories = Resources.FindObjectsOfTypeAll<Inventory>();
        foreach (var inv in inventories)
        {
            if (inv != null && inv.myInventory != null)
            {
                inv.myInventory.Clear();
            }
        }

        // 7. Reset active instance objects list if any
        if (gameSave != null && gameSave.objects != null)
        {
            for (int i = 0; i < gameSave.objects.Count; i++)
            {
                ResetObject(gameSave.objects[i]);
            }
        }

        Debug.Log("All ScriptableObjects and save files reset successfully.");
    }

    public static void ResetObject(ScriptableObject obj)
    {
        if (obj == null) return;

        if (obj is BoolValue boolVal)
        {
            boolVal.ResetValue();
        }
        else if (obj is FloatValue floatVal)
        {
            floatVal.RuntimeValue = floatVal.initialValue;
        }
        else if (obj is IntValue intVal)
        {
            intVal.RuntimeValue = intVal.initialValue;
        }
        else if (obj is VectorValue vectorVal)
        {
            vectorVal.initialValue = vectorVal.defaultValue;
        }
    }

    private void OnEnable()
    {
        if (HasSave())
        {
            LoadScriptables();
        }
        else
        {
            for (int i = 0; i < objects.Count; i++)
            {
                ResetObject(objects[i]);
            }
        }
    }

    private void OnDisable()
    {
        SaveScriptables();
    }

    public void SaveScriptables()
    {
        SaveScene();
        for (int i = 0; i < objects.Count; i++)
        {
            FileStream file = File.Create(Application.persistentDataPath + string.Format("/{0}.dat", i));
            BinaryFormatter binary = new BinaryFormatter();
            var json = JsonUtility.ToJson(objects[i]);
            Debug.Log("Saving " + objects[i].name + " to " + Application.persistentDataPath + string.Format("/{0}.dat", i));
            binary.Serialize(file, json);
            file.Close();
        }
    }

    public void SaveScene()
    {
        SaveCurrentScene();
    }

    public static void SaveCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (!string.IsNullOrEmpty(currentScene) && currentScene != "StartMenu" && currentScene != "MainMenu")
        {
            try
            {
                string scenePath = Application.persistentDataPath + "/savedScene.dat";
                FileStream file = File.Create(scenePath);
                BinaryFormatter binary = new BinaryFormatter();
                binary.Serialize(file, currentScene);
                file.Close();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error saving scene to file: " + e.Message);
            }
            PlayerPrefs.SetString("SavedScene", currentScene);
            PlayerPrefs.Save();
            Debug.Log("Saved scene: " + currentScene);
        }
    }

    public string GetSavedScene(string defaultScene = "SampleScene")
    {
        return GetSavedSceneName(defaultScene);
    }

    public static string GetSavedSceneName(string defaultScene = "SampleScene")
    {
        string scenePath = Application.persistentDataPath + "/savedScene.dat";
        if (File.Exists(scenePath))
        {
            try
            {
                FileStream file = File.Open(scenePath, FileMode.Open);
                BinaryFormatter binary = new BinaryFormatter();
                string sceneName = (string)binary.Deserialize(file);
                file.Close();
                if (!string.IsNullOrEmpty(sceneName) && sceneName != "StartMenu" && sceneName != "MainMenu")
                {
                    return sceneName;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error reading saved scene file: " + e.Message);
            }
        }

        string prefScene = PlayerPrefs.GetString("SavedScene", "");
        if (!string.IsNullOrEmpty(prefScene) && prefScene != "StartMenu" && prefScene != "MainMenu")
        {
            return prefScene;
        }

        return defaultScene;
    }

    public bool HasSaveData()
    {
        return HasSave();
    }

    public static bool HasSave()
    {
        return File.Exists(Application.persistentDataPath + "/savedScene.dat") ||
               File.Exists(Application.persistentDataPath + "/0.dat") ||
               PlayerPrefs.HasKey("SavedScene");
    }

    public void LoadScriptables()
    {
        for (int i = 0; i < objects.Count; i++)
        {
            string path = Application.persistentDataPath + string.Format("/{0}.dat", i);
            if (File.Exists(path))
            {
                FileStream file = File.Open(path, FileMode.Open);
                BinaryFormatter binary = new BinaryFormatter();
                JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), objects[i]);
                file.Close();
            }
            else
            {
                ResetObject(objects[i]);
            }
        }
    }

    private void Awake()
    {
        if (gameSave == null)
        {
            gameSave = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }
}
