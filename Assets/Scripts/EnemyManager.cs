using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyInformation enemyInformation;

    public int maxHealth;
    public int currentHealth;
    public int damage;
    public int charge;
    void Start()
    {
        maxHealth = enemyInformation.maxHealth;
        damage = enemyInformation.damage;
        charge = enemyInformation.charge;
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("enemy health: " + currentHealth);
    }
}
