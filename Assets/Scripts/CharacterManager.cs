using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public CharacterInformation charInfo;

    public int health;
    public int damage;
    public int mana;
    public string color;

    public HealthSystem healthSystem;
    public ManaSystem manaSystem;
    public AbilitySystem abilitySystem;
    void Start()
    {
        health = charInfo.maxHealth;
        damage = charInfo.damage;
        mana = charInfo.mana;
        color = charInfo.color.ToString();

        healthSystem.SetCurrentHealth(health);
        manaSystem.SetMana(mana);
    }

    public void UsingSkill()
    {
        switch (abilitySystem.targetType)
        {
            case "Single":
                GameObject target = BattleSystem.Instance.enemyBattle.Find(enemy => enemy.GetComponent<EnemyManager>().isSelected == true);
                target.GetComponent<EnemyManager>().healthSystem.TakeDamage(abilitySystem.damage);
                Debug.Log(abilitySystem.damage);
                break;
            case "All":
                foreach (var enemy in BattleSystem.Instance.enemyBattle)
                {
                    enemy.GetComponent<EnemyManager>().healthSystem.TakeDamage(abilitySystem.damage);
                    Debug.Log(abilitySystem.damage);
                }
                break;
            default:
                break;
        }
        Debug.Log(gameObject + " use skill");
    }
}
