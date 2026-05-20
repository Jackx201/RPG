using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class GenericProjectile : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D myRigidbody;
    [SerializeField]
    private float mySpeed;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 moveDirection)
    {
        myRigidbody.linearVelocity = moveDirection.normalized * mySpeed;
    }

        public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("enemy"))
        {
        Destroy(this.gameObject);
        }
    }
}
