using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class InventorySaver : MonoBehaviour
{
    private void OnEnable()
    {
        myInventory.myInventory.Clear();
        LoadScriptables();
    }

    private void OnDisable()
    {
        SaveScriptables();
    }

    [SerializeField] private PlayerInventory myInventory;

    public void ResetScriptable()
    {
        string slotDir = SaveGame.GetSlotDirectory();
        int i = 0;
        while (File.Exists(Path.Combine(slotDir, string.Format("{0}.inv", i))))
        {
            File.Delete(Path.Combine(slotDir, string.Format("{0}.inv", i)));
            i++;
        }
    }

    public void SaveScriptables()
    {
        ResetScriptable();
        string slotDir = SaveGame.GetSlotDirectory();
        for (int i = 0; i < myInventory.myInventory.Count; i++)
        {
            FileStream file = File.Create(Path.Combine(slotDir, string.Format("{0}.inv", i)));
            BinaryFormatter binary = new BinaryFormatter();
            var json = JsonUtility.ToJson(myInventory.myInventory[i]);
            binary.Serialize(file, json);
            file.Close();
        }
    }

    public void LoadScriptables()
    {
        string slotDir = SaveGame.GetSlotDirectory();
        int i = 0;
        while (File.Exists(Path.Combine(slotDir, string.Format("{0}.inv", i))))
        {
            var temp = ScriptableObject.CreateInstance<ItemInventory>();
            FileStream file = File.Open(Path.Combine(slotDir, string.Format("{0}.inv", i)), FileMode.Open);
            BinaryFormatter binary = new BinaryFormatter();
            JsonUtility.FromJsonOverwrite((string)binary.Deserialize(file), temp);
            file.Close();
            myInventory.myInventory.Add(temp);
            i++;
        }
    }
}
