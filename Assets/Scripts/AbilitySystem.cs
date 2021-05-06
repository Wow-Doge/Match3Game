using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AbilitySystem : MonoBehaviour
{
    public AbilityInformation ability;

    public string abilityName;
    public int damage;
    public string targetType;

    public event EventHandler OnUsingSkill;
    void Start()
    {
        abilityName = ability.abilityName;
        damage = ability.damage;
        targetType = ability.targetType.ToString();
    }
    private void OnEnable()
    {
        OnUsingSkill += UseSkill;
    }

    private void OnDisable()
    {
        OnUsingSkill -= UseSkill;
    }

    private void UseSkill(object sender, EventArgs e)
    {
        switch (targetType)
        {
            case "Single":
                GameObject target = BattleSystem.Instance.enemyBattle.Find(enemy => enemy.GetComponent<EnemyManager>().isSelected == true);
                target.GetComponent<EnemyManager>().healthSystem.TakeDamage(damage);
                break;
            case "All":
                for (int i = 0; i < BattleSystem.Instance.enemyBattle.Count; i++)
                {
                    GameObject enemy = BattleSystem.Instance.enemyBattle[i];
                    enemy.GetComponent<EnemyManager>().healthSystem.TakeDamage(damage);
                }
                break;
            default:
                break;
        }
        Debug.Log(gameObject + " use skill");
    }

    public void UsingSkill()
    {
        OnUsingSkill?.Invoke(this, EventArgs.Empty);
    }
}
