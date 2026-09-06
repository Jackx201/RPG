using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    private bool Paused;
    public GameObject pausePanel;
    public GameObject inventoryPanel;
    public string MainMenu;
    public bool usingPausePanel;
    [SerializeField] StateMachine playerState;
    [SerializeField] GameObject selectedButton;
    [SerializeField] GameObject pauseSelectedButton;

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
            SelectPanelButton(pausePanel, pauseSelectedButton);
        }
        else
        {
            inventoryPanel.SetActive(false);
            pausePanel.SetActive(false);
            usingPausePanel = false;
            EventSystem.current?.SetSelectedGameObject(null);
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
        usingPausePanel = !usingPausePanel;
        if (usingPausePanel)
        {
            pausePanel.SetActive(true);
            inventoryPanel.SetActive(false);
            SelectPanelButton(pausePanel, pauseSelectedButton);
        }
        else
        {
            inventoryPanel.SetActive(true);
            pausePanel.SetActive(false);
            SelectPanelButton(inventoryPanel, selectedButton);
        }
    }

    private void SelectPanelButton(GameObject panel, GameObject preferredButton)
    {
        var eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            return;
        }

        var button = preferredButton != null
            ? preferredButton
            : panel.GetComponentInChildren<Selectable>(true)?.gameObject;

        if (button != null && button.activeInHierarchy)
        {
            eventSystem.SetSelectedGameObject(button, new BaseEventData(eventSystem));
        }
    }
}
