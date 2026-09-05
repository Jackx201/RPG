using UnityEngine;

public class SpawnerTriggerArea : MonoBehaviour
{
    private EnemySpawner spawner;

    private void Awake()
    {
        // Finds the EnemySpawner script on the parent object
        spawner = GetComponentInParent<EnemySpawner>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (spawner != null && other.CompareTag("Player") && !other.isTrigger)
        {
            spawner.PlayerEnteredZone(other.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (spawner != null && other.CompareTag("Player") && !other.isTrigger)
        {
            spawner.PlayerLeftZone();
        }
    }
}
