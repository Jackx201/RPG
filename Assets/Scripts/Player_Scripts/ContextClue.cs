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

    private void Start()
    {
        clueActive = false;
        mySprite.enabled = false;
        if (myAnimator != null)
        {
            myAnimator.enabled = false;
        }
    }

    public void ChangeClue()
    {
        clueActive = !clueActive;
        Debug.Log($"[ContextClue] ChangeClue called on {gameObject.name}. clueActive is now: {clueActive}", this);
        mySprite.enabled = clueActive;
        
        if (myAnimator != null)
        {
            myAnimator.enabled = clueActive;
        }
    }



}
