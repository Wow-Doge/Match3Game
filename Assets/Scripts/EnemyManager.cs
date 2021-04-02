using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public int maxHealth = 3000;
    private int currentHealth;
    public int charge;

    private Image image;

    public static EnemyManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        currentHealth = maxHealth;
        image = GetComponent<Image>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0)
        {
            EnemyDead();
        }
    }

    void EnemyDead()
    {
        Debug.Log("enemy dead");
        //Destroy(gameObject);
    }
}
