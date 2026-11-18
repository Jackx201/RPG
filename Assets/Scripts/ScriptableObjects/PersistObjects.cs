using UnityEngine;

public class PersistObjects : MonoBehaviour
{
    [Tooltip("Add your scene-specific booleans here to prevent them from resetting")]
    public ScriptableObject[] objectsToPersist;
    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
    }
}
