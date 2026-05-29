using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextClue : MonoBehaviour
{
    [SerializeField] private SpriteRenderer mySprite;
    [SerializeField] private bool clueActive = false;
    private Animator myAnimator;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }

    public void ChangeClue()
    {
        clueActive = !clueActive;
        mySprite.enabled = clueActive;
        
        if (myAnimator != null)
        {
            myAnimator.enabled = clueActive;
        }
    }



}
