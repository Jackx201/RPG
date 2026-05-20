using UnityEngine;

public class DestroyOverTime : MonoBehaviour
{
    [Header("Lifetime")]
    [SerializeField] private float lifetime;

    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime<=0)
        {
            Destroy(this.gameObject);
        }
    }
}
