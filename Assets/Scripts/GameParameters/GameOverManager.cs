using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] StateMachine playerState;
    [SerializeField] GameObject gameOverUI;
    [SerializeField] float GameOverTransition;
    [SerializeField] AnimatorController anim;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] GameObject exitButton;

    public void GameOver()
    {
        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(exitButton, new BaseEventData(eventSystem));
        playerState.ChangeState(GenericState.dead);
        playerMovement.anim.SetAnimParameter("moveX", 0);
        playerMovement.anim.SetAnimParameter("moveY", 0);
        playerMovement.anim.SetAnimParameter("Moving", false);
        playerMovement.anim.SetAnimParameter("game_over", true);
        playerMovement.myRigidbody.linearVelocity = Vector3.zero;
        StartCoroutine(GameOverCo());
    }

    public IEnumerator GameOverCo()
    {
        gameOverUI.SetActive(true);
        anim.SetAnimParameter("dying", true);
        yield return new WaitForSeconds(GameOverTransition);
        anim.SetAnimParameter("dead", true);
    }
}
