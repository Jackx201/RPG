using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitions : MonoBehaviour
{
    [Header("NewSceneVariables")]
    public string sceneToLoad;
    public Vector2 playerPosition;
    public VectorValue playerStorage;
    public Vector2 cameraNewMax;
    public Vector2 cameraNewMin;
    public VectorValue cameraMin;
    public VectorValue cameraMax;


    [Header("Transition Variables")]
    public GameObject fadeInPanel;
    public GameObject fadeOutPanel;
    public float fadeWait;

    private void Awake(){
        if(fadeInPanel != null){
            GameObject panel = Instantiate(fadeInPanel, Vector3.zero, Quaternion.identity) as GameObject;
            Destroy(panel, 1);
        }
    }

    public void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Player")&& !other.isTrigger){
            playerStorage.initialValue = playerPosition;
            StartCoroutine(fadeCo());
            //SceneManager.LoadScene(sceneToLoad);
        }
    }

    /// <summary>
    /// Call this from a UnityEvent (e.g. dialog choice onSelect) to trigger
    /// the fade + scene load. Sets the player spawn position first.
    /// </summary>
    public void StartTransition()
    {
        playerStorage.initialValue = playerPosition;
        // Route through CoroutineRunner: works even if this object or its
        // parents are inactive (inactive objects can't run coroutines).
        CoroutineRunner.Instance.Run(fadeCo());
    }

    public IEnumerator fadeCo()
    {
        if(fadeOutPanel != null)
        {
            Instantiate(fadeOutPanel, Vector3.zero, Quaternion.identity);
        }
        yield return new WaitForSeconds (fadeWait);
        ResetCameraBounds();
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        while(!asyncOperation.isDone)
        {
            yield return null;
        }
    }

    public void ResetCameraBounds()
    {
        cameraMax.initialValue = cameraNewMax;
        cameraMin.initialValue = cameraNewMin;
    }
    
}
