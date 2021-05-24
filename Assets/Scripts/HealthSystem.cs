using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth;
    public int currentHealth;
    public event EventHandler OnHealthChange;
    public void SetCurrentHealth(int maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = this.maxHealth;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log(gameObject + " take damage");
        OnHealthChange?.Invoke(this, EventArgs.Empty);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            UnitDie();
        }
    }

    public void UnitDie()
    {
        gameObject.SetActive(false);

        BattleSystem.Instance.enemyBattle.Remove(gameObject);
        BattleSystem.Instance.charBattle.Remove(gameObject);
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
