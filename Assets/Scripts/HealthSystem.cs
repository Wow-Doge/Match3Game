using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;

    public void SetCurrentHealth(int maxHealth)
    {
        currentHealth = maxHealth;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
            UnitDie();
        }
    }

    public void UnitDie()
    {
        Destroy(gameObject);
        BattleSystem.Instance.enemyBattle.Remove(gameObject);
        BattleSystem.Instance.charBattle.Remove(gameObject);
        Debug.Log(gameObject.name + "die");
        if (BattleSystem.Instance.enemyBattle.Count > 0)
        {
            BattleSystem.Instance.AutoTarget();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / (float)maxHealth;
    }
}
