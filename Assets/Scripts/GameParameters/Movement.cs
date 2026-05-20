using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] public Rigidbody2D myRigidbody;

    public void Motion(Vector2 direction)
    {
        direction = direction.normalized;
        myRigidbody.linearVelocity = direction * speed;
    }
}
