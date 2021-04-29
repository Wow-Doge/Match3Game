using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "Ability")]
public class AbilityInformation : ScriptableObject
{
    public string abilityName;
    public int damage;
     
    public enum TargetType
    {
        Single,
        All
    }

    public TargetType targetType;
}
