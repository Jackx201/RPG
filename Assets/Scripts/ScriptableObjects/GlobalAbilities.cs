using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GlobalAbilities : ScriptableObject
{
    public GenericAbility mainAbility;
    public GenericAbility secondaryAbility;
    public Notification changedAbility;
}
