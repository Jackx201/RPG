using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Enemmy[] enemies;
    public pot[] pots;
    public GameObject virtualCamera;

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            int enemiesquantity = enemies.Length;
            int potsquantity = pots.Length;

            for(int i=0; i< enemiesquantity; i++)
            {
                ChangeActive(enemies[i], true);
            }

            for (int i=0; i < potsquantity; i++)
            {
                ChangeActive(pots[i], true);
            }
            virtualCamera.SetActive(true);
        }
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            int enemiesquantity = enemies.Length;
            int potsquantity = pots.Length;

            for(int i=0; i< enemiesquantity; i++)
            {
                ChangeActive(enemies[i], false);
            }

            for (int i=0; i < potsquantity; i++)
            {
                ChangeActive(pots[i], false);
            }
            virtualCamera.SetActive(false);
        }
    }

    public void OnDisable()
    {
        virtualCamera.SetActive(false);
    }

    public void ChangeActive(Component component, bool activation)
    {
        component.gameObject.SetActive(activation);
    }
}
