using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float spawnRadius;

    [Header("Kill Tracking")]
    [SerializeField] private SignalSender enemyDefeatedSignal;
    [SerializeField] private int killTarget; // How many kills to stop spawning
    [SerializeField] private GameObject chestToActivate;

    private int enemiesDefeated;
    private float spawnTimer;
    private bool isPlayerInZone;
    private Transform playerTransform;

    private void OnEnable()
    {
        if (enemyDefeatedSignal != null)
            enemyDefeatedSignal.RegisterListener(GetComponent<SignalListener>());
    }

    private void OnDisable()
    {
        if (enemyDefeatedSignal != null)
            enemyDefeatedSignal.DeRegisterListener(GetComponent<SignalListener>());
    }

    // Called by SignalListener's UnityEvent when an enemy dies
    public void OnEnemyDefeated()
    {
        enemiesDefeated++;
        Debug.Log($"Enemies defeated: {enemiesDefeated} / {killTarget}");

        if (killTarget > 0 && enemiesDefeated >= killTarget)
        {
            isPlayerInZone = false; // Stop spawning
            if (chestToActivate != null)
                chestToActivate.SetActive(true);
        }
    }
    private void Update()
    {
        if (isPlayerInZone)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                SpawnEnemy();
                spawnTimer = 0f;
            }
        }
    }

    void SpawnEnemy()
    {
        // Use insideUnitCircle for 2D spawning
        Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomPos.x, randomPos.y, 0f);
        
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        // Automatically assign player variables to the instantiated enemy
        log enemyLog = enemy.GetComponent<log>();
        if (enemyLog != null && playerTransform != null)
        {
            enemyLog.target = playerTransform;
            
            // Also assign the Player's StateMachine if needed
            StateMachine playerSM = playerTransform.GetComponentInChildren<StateMachine>();
            if (playerSM != null)
            {
                enemyLog.playerState = playerSM;
            }
        }
    }

    public void PlayerEnteredZone(Transform player)
    {
        isPlayerInZone = true;
        playerTransform = player;
        spawnTimer = spawnInterval; // Trigger first spawn immediately
    }

    public void PlayerLeftZone()
    {
        isPlayerInZone = false;
    }
}
