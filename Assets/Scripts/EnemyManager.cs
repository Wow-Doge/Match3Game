using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyInformation enemyInfo;

    public int maxHealth;
    public int currentHealth;
    public int damage;
    public int charge;
    public int currentCharge;
    void Start()
    {
        maxHealth = enemyInfo.maxHealth;
        damage = enemyInfo.damage;
        charge = enemyInfo.charge;
        currentHealth = maxHealth;
        currentCharge = charge;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
    }

    public void DecreaseCharge()
    {
        currentCharge--;
    }

    public void ResetCharge()
    {
        currentCharge = charge;
    }
}
