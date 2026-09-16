using UnityEngine;
using UnityEngine.Events;

public class BoxColliderTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private UnityEvent onPlayerEnter = new UnityEvent();

    private void Start()
    {
        // Ensure the BoxCollider2D is set as a trigger
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object entering has the Player tag
        if (collision.CompareTag(playerTag))
        {
            // Raise the signal
            onPlayerEnter?.Invoke();
            Debug.Log("Player entered trigger!");
        }
    }
}
