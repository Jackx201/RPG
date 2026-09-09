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

    public static int activeSlot = 0;

    public static void SetActiveSlot(int slot)
    {
        activeSlot = Mathf.Max(0, slot);
        PlayerPrefs.SetInt("ActiveSlot", activeSlot);
        PlayerPrefs.Save();
    }

    public static int GetActiveSlot()
    {
        activeSlot = PlayerPrefs.GetInt("ActiveSlot", 0);
        return activeSlot;
    }

    public static string GetSlotDirectory(int slot = -1)
    {
        if (slot < 0)
        {
            slot = GetActiveSlot();
        }
        string dir = Path.Combine(Application.persistentDataPath, "slot_" + slot);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        return dir;
    }

    public void ResetScriptable()
    {
        ResetAllScriptables(GetActiveSlot());
    }

    public static void ResetSlot(int slot)
    {
        string slotDir = GetSlotDirectory(slot);
        if (Directory.Exists(slotDir))
        {
            try
            {
                Directory.Delete(slotDir, true);
                Directory.CreateDirectory(slotDir);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error resetting slot " + slot + ": " + e.Message);
            }
        }
        PlayerPrefs.DeleteKey("SavedScene_Slot_" + slot);
        PlayerPrefs.Save();
    }

    public static void ResetAllScriptables(int slot = -1)
    {
        if (slot < 0)
        {
            slot = GetActiveSlot();
        }

        // 1. Delete slot save files
        ResetSlot(slot);

        // Also clean legacy root files if slot 0
        if (slot == 0)
        {
            for (int i = 0; i < 200; i++)
            {
                string path = Application.persistentDataPath + string.Format("/{0}.dat", i);
                if (File.Exists(path)) { try { File.Delete(path); } catch { } }
                string invPath = Application.persistentDataPath + string.Format("/{0}.inv", i);
                if (File.Exists(invPath)) { try { File.Delete(invPath); } catch { } }
            }
            string scenePath = Application.persistentDataPath + "/savedScene.dat";
            if (File.Exists(scenePath)) { try { File.Delete(scenePath); } catch { } }
            PlayerPrefs.DeleteKey("SavedScene");
            PlayerPrefs.Save();
        }

        // 2. Reset in-memory BoolValue objects
        BoolValue[] bools = Resources.FindObjectsOfTypeAll<BoolValue>();
        foreach (var b in bools)
        {
            if (b != null)
            {
                b.ResetValue();
            }
        }

        // 3. Reset in-memory FloatValue objects
        FloatValue[] floats = Resources.FindObjectsOfTypeAll<FloatValue>();
        foreach (var f in floats)
        {
            if (f != null)
            {
                f.RuntimeValue = f.initialValue;
            }
        }

        // 4. Reset in-memory IntValue objects
        IntValue[] ints = Resources.FindObjectsOfTypeAll<IntValue>();
        foreach (var it in ints)
        {
            if (it != null)
            {
                it.RuntimeValue = it.initialValue;
            }
        }

        // 5. Reset in-memory VectorValue objects
        VectorValue[] vectors = Resources.FindObjectsOfTypeAll<VectorValue>();
        foreach (var v in vectors)
        {
            if (v != null)
            {
                v.initialValue = v.defaultValue;
            }
        }

        // 6. Reset in-memory Inventory objects
        Inventory[] inventories = Resources.FindObjectsOfTypeAll<Inventory>();
        foreach (var inv in inventories)
        {
            if (inv != null && inv.myInventory != null)
            {
                inv.myInventory.Clear();
            }
        }

        // 7. Reset numberHeld on every InventoryItem asset
        InventoryItem[] items = Resources.FindObjectsOfTypeAll<InventoryItem>();
        foreach (var item in items)
        {
            if (item != null) item.ResetValue();
        }

        // 7b. Reset numberHeld on every ItemInventory asset
        ItemInventory[] itemInvs = Resources.FindObjectsOfTypeAll<ItemInventory>();
        foreach (var item in itemInvs)
        {
            if (item != null) item.ResetValue();
        }

        // 8. Reset active instance objects list
        if (gameSave != null && gameSave.objects != null)
        {
            for (int i = 0; i < gameSave.objects.Count; i++)
            {
                ResetObject(gameSave.objects[i]);
            }
        }

        Debug.Log("Reset all ScriptableObjects for slot " + slot);
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
        else if (obj is InventoryItem itemVal)
        {
            itemVal.ResetValue();
        }
        else if (obj is ItemInventory itemInvVal)
        {
            itemInvVal.ResetValue();
        }
    }

    private void OnEnable()
    {
        if (HasSave(GetActiveSlot()))
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
        int slot = GetActiveSlot();
        SaveCurrentScene(slot);
        string slotDir = GetSlotDirectory(slot);

        for (int i = 0; i < objects.Count; i++)
        {
            FileStream file = File.Create(Path.Combine(slotDir, string.Format("{0}.dat", i)));
            BinaryFormatter binary = new BinaryFormatter();
            var json = JsonUtility.ToJson(objects[i]);
            Debug.Log("Saving slot " + slot + " " + objects[i].name + " to " + slotDir);
            binary.Serialize(file, json);
            file.Close();
        }
    }

    public void SaveScene()
    {
        SaveCurrentScene(GetActiveSlot());
    }

    public void SaveScene(int slot)
    {
        SaveCurrentScene(slot);
    }

    public static void SaveCurrentScene(int slot = -1)
    {
        if (slot < 0) slot = GetActiveSlot();
        string currentScene = SceneManager.GetActiveScene().name;
        if (!string.IsNullOrEmpty(currentScene) && currentScene != "StartMenu" && currentScene != "MainMenu")
        {
            string slotDir = GetSlotDirectory(slot);
            try
            {
                string scenePath = Path.Combine(slotDir, "savedScene.dat");
                FileStream file = File.Create(scenePath);
                BinaryFormatter binary = new BinaryFormatter();
                binary.Serialize(file, currentScene);
                file.Close();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error saving slot scene to file: " + e.Message);
            }
            PlayerPrefs.SetString("SavedScene_Slot_" + slot, currentScene);
            PlayerPrefs.Save();
            Debug.Log("Saved slot " + slot + " scene: " + currentScene);
        }
    }

    public string GetSavedScene(string defaultScene = "SampleScene")
    {
        return GetSavedSceneName(GetActiveSlot(), defaultScene);
    }

    public static string GetSavedSceneName(int slot = -1, string defaultScene = "SampleScene")
    {
        if (slot < 0) slot = GetActiveSlot();
        string slotDir = GetSlotDirectory(slot);
        string scenePath = Path.Combine(slotDir, "savedScene.dat");

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
                Debug.LogWarning("Error reading saved scene for slot " + slot + ": " + e.Message);
            }
        }

        string prefScene = PlayerPrefs.GetString("SavedScene_Slot_" + slot, "");
        if (!string.IsNullOrEmpty(prefScene) && prefScene != "StartMenu" && prefScene != "MainMenu")
        {
            return prefScene;
        }

        // Backward compatibility for slot 0
        if (slot == 0)
        {
            string legacyPath = Application.persistentDataPath + "/savedScene.dat";
            if (File.Exists(legacyPath))
            {
                try
                {
                    FileStream file = File.Open(legacyPath, FileMode.Open);
                    BinaryFormatter binary = new BinaryFormatter();
                    string sceneName = (string)binary.Deserialize(file);
                    file.Close();
                    if (!string.IsNullOrEmpty(sceneName)) return sceneName;
                }
                catch { }
            }
        }

        return defaultScene;
    }

    public bool HasSaveData()
    {
        return HasSave(GetActiveSlot());
    }

    public static bool HasSave(int slot = -1)
    {
        if (slot < 0) slot = GetActiveSlot();
        string slotDir = GetSlotDirectory(slot);
        bool hasSlotFiles = File.Exists(Path.Combine(slotDir, "savedScene.dat")) ||
                            File.Exists(Path.Combine(slotDir, "0.dat")) ||
                            PlayerPrefs.HasKey("SavedScene_Slot_" + slot);

        if (hasSlotFiles) return true;

        if (slot == 0)
        {
            return File.Exists(Application.persistentDataPath + "/savedScene.dat") ||
                   File.Exists(Application.persistentDataPath + "/0.dat") ||
                   PlayerPrefs.HasKey("SavedScene");
        }

        return false;
    }

    public void LoadScriptables()
    {
        LoadScriptables(GetActiveSlot());
    }

    public void LoadScriptables(int slot)
    {
        string slotDir = GetSlotDirectory(slot);
        for (int i = 0; i < objects.Count; i++)
        {
            string path = Path.Combine(slotDir, string.Format("{0}.dat", i));
            // Backward compatibility fallback for slot 0
            if (!File.Exists(path) && slot == 0)
            {
                path = Application.persistentDataPath + string.Format("/{0}.dat", i);
            }

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
