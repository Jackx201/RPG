using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    private bool Paused;
    public GameObject pausePanel;
    public GameObject inventoryPanel;
    public string MainMenu;
    public bool usingPausePanel;
    [SerializeField] StateMachine playerState;
    [SerializeField] GameObject selectedButton;

    void Start()
    {
        Paused = false;
        pausePanel.SetActive(false);
        inventoryPanel.SetActive(false);
        usingPausePanel = false;
    }


    void Update()
    {
        if (Input.GetButtonDown("Pause") && playerState.myState != GenericState.dead)
        {
            ChangePause();
        }
    }

    public void ChangePause()
    {
        Paused = !Paused;
        if (Paused)
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0f;
            usingPausePanel = true;
        }
        else
        {
            inventoryPanel.SetActive(false);
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void Exit()
    {
        SceneManager.LoadScene(MainMenu);
        Time.timeScale = 1f;
    }

    public void SwitchPanels()
    {
        var eventSystem = EventSystem.current;
        usingPausePanel = !usingPausePanel;
        if (usingPausePanel)
        {
            pausePanel.SetActive(true);
            inventoryPanel.SetActive(false);
        }
        else
        {
            inventoryPanel.SetActive(true);
            pausePanel.SetActive(false);
             eventSystem.SetSelectedGameObject(selectedButton, new BaseEventData(eventSystem));
        }
    }
}
