using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Knockback : MonoBehaviour
{

    [SerializeField] string otherTag;
    [SerializeField] float knockTime;
    [SerializeField] float knockStrength;

    public void OnTriggerEnter2D(Collider2D
     other)
    {
        if (other.gameObject.CompareTag(otherTag) && other.isTrigger)
        {
            Rigidbody2D temp = other.GetComponentInParent<Rigidbody2D>();
            if (temp)
            {
                Vector2 direction = other.transform.position - transform.position;
                //temp.transform.DOMove((Vector2)other.transform.position + (direction.normalized * knockStrength), knockTime);  
                // temp.DOMove((Vector2) other.transform.position + 
                // (direction.normalized * knockStrength), knockTime); 
                Vector3 tempdirection = temp.transform.position 
                + (Vector3) direction.normalized * knockStrength;
                //temp.transform.DOMove(tempdirection, knockTime);
                temp.DOMove(tempdirection, knockTime).SetUpdate(UpdateType.Fixed);
            }
        }
    }
}