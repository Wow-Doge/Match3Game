using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
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

    void Update()
    {
        
    }
}
