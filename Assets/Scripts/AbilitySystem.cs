using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySystem : MonoBehaviour
{
    public AbilityInformation ability;

    public string abilityName;
    public int damage;
    public string targetType;
    void Start()
    {
        abilityName = ability.abilityName;
        damage = ability.damage;
        targetType = ability.targetType.ToString();
    }


}
