using UnityEngine;
using System.Collections;

/// <summary>
/// A persistent, always-active singleton MonoBehaviour that can run coroutines
/// on behalf of inactive objects. Survives scene loads.
/// Usage: CoroutineRunner.Instance.Run(myEnumerator);
/// </summary>
public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("[CoroutineRunner]");
                _instance = go.AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>Runs a coroutine on this always-active object.</summary>
    public void Run(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
