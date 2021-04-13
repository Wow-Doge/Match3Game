using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public CharacterInformation characterInformation;

    public int maxHealth;
    public int currentHealth;
    public int damage;
    public int mana;
    public string color;
    void Start()
    {
        maxHealth = characterInformation.maxHealth;
        damage = characterInformation.damage;
        mana = characterInformation.mana;
        currentHealth = maxHealth;
        color = characterInformation.color.ToString();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
    }
}
