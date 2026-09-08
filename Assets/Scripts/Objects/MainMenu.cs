using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string defaultScene = "SampleScene";

    public void NewGame()
    {
        NewGameSlot(SaveGame.GetActiveSlot());
    }

    public void NewGameSlot(int slot)
    {
        SaveGame.SetActiveSlot(slot);
        SaveGame.ResetAllScriptables(slot);
        SceneManager.LoadScene(defaultScene);
    }

    public void NewGameSlot1() { NewGameSlot(0); }
    public void NewGameSlot2() { NewGameSlot(1); }
    public void NewGameSlot3() { NewGameSlot(2); }

    public void LoadSavedGame()
    {
        LoadSlot(SaveGame.GetActiveSlot());
    }

    public void LoadSlot(int slot)
    {
        SaveGame.SetActiveSlot(slot);
        if (SaveGame.gameSave != null)
        {
            SaveGame.gameSave.LoadScriptables(slot);
        }

        string sceneToLoad = SaveGame.GetSavedSceneName(slot, defaultScene);
        Debug.Log("MainMenu loading slot " + slot + " scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }

    public void LoadSlot1() { LoadSlot(0); }
    public void LoadSlot2() { LoadSlot(1); }
    public void LoadSlot3() { LoadSlot(2); }

    public void DeleteSlot(int slot)
    {
        SaveGame.ResetSlot(slot);
    }

    public void DeleteSlot1() { DeleteSlot(0); }
    public void DeleteSlot2() { DeleteSlot(1); }
    public void DeleteSlot3() { DeleteSlot(2); }

    public bool HasSlotSave(int slot)
    {
        return SaveGame.HasSave(slot);
    }

    public void QuitToDesktop()
    {
        Application.Quit();
    }
}
