using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayerPosition : MonoBehaviour
{
    [SerializeField] private VectorValue playerPosition;

    // Start is called before the first frame update
    void Start()
    {
        if (playerPosition)
        {
            Debug.Log("Position reset to: " + playerPosition.initialValue);
            transform.position = playerPosition.initialValue;
            Debug.Log("Player position is now: " + transform.position);
        }
    }
}
