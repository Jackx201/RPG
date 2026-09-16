using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GlobalAbilities : ScriptableObject
{
    // Runtime references (not serialized by JsonUtility)
    [System.NonSerialized] public GenericAbility mainAbility;
    [System.NonSerialized] public GenericAbility secondaryAbility;

    // These ARE serialized by JsonUtility - used for save/load
    public string mainAbilityName;
    public string secondaryAbilityName;

    public Notification changedAbility;

    // Call after loading to rebuild references from saved names
    public void LoadFromNames()
    {
        mainAbility = string.IsNullOrEmpty(mainAbilityName)
            ? null
            : Resources.Load<GenericAbility>(mainAbilityName);
        secondaryAbility = string.IsNullOrEmpty(secondaryAbilityName)
            ? null
            : Resources.Load<GenericAbility>(secondaryAbilityName);
    }

    // Call when assigning an ability so the name is always in sync
    public void SetMainAbility(GenericAbility ability)
    {
        mainAbility = ability;
        mainAbilityName = ability != null ? ability.name : "";
    }

    public void SetSecondaryAbility(GenericAbility ability)
    {
        secondaryAbility = ability;
        secondaryAbilityName = ability != null ? ability.name : "";
    }

    public void Reset()
    {
        mainAbility = null;
        secondaryAbility = null;
        mainAbilityName = "";
        secondaryAbilityName = "";
    }
}
