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
        currentability = players.currentAbility;
        secondaryAbility = players.secondaryAbility;

        currentAbilityContainer.sprite = currentability != null ? currentability.uiImage : null;
        secondaryAbilityContainer.sprite = secondaryAbility != null ? secondaryAbility.uiImage : null;
    }
}

/*
Habilidades Globales -> Current & Secundary
Change Ability -> Prefab 
Buttons Ability -> UI 
Player Movement -> Habilidades en el jugador
*/