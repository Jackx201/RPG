using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;
    public FloatValue heartContainers;
    public FloatValue playerCurrentHealth;

    void Start()
    {
       InitHearts(); 
       UpdateHearts();
    }

    public void InitHearts()
    {
        for(int i = 0; i< heartContainers.RuntimeValue; i++)
        {
            if(i< hearts.Length)
            {
            hearts[i].gameObject.SetActive(true);
            hearts[i].sprite = fullHeart;
            }
        }
    }

    public void UpdateHearts()
    {
        InitHearts();
        float tempHealth = playerCurrentHealth.RuntimeValue / 2;
        //Debug.Log("Current Health is: " + tempHealth);
        for(int i = 0; i< heartContainers.RuntimeValue; i++)
        {
            if( i<= tempHealth-1)
            //Full♥
            {
                hearts[i].sprite = fullHeart;
            } else if (i >= tempHealth) 
            { 
            //Empty♥
                hearts[i].sprite = emptyHeart;
            } else 
            { 
            //Half♥
                hearts[i].sprite = halfHeart;
            }
        }
    }
}
