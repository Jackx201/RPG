using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsAbility : MonoBehaviour
{
    [SerializeField] private GenericAbility currentability;
    [SerializeField] private GenericAbility secondaryAbility;
    [SerializeField] private PlayerMovement players;
    [SerializeField] private Image currentAbilityContainer;
    [SerializeField] private Image secondaryAbilityContainer;

    public void Start()
    {
        UpdateAbilities();
    }

    public void UpdateAbilities()
    {
        if(players.currentAbility)
        {
        currentability = players.currentAbility;
        currentAbilityContainer.sprite = currentability.uiImage;
        }
        if(players.secondaryAbility)
        {
            secondaryAbility = players.secondaryAbility;
            secondaryAbilityContainer.sprite = secondaryAbility.uiImage;
        }
    }
}

/*
Habilidades Globales -> Current & Secundary
Change Ability -> Prefab 
Buttons Ability -> UI 
Player Movement -> Habilidades en el jugador
*/