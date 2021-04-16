using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public CharacterInformation charInfo;

    public int maxHealth;
    public int currentHealth;
    public int damage;
    public int maxMana;
    public int currentMana;
    public string color;

    void Start()
    {
        maxHealth = charInfo.maxHealth;
        damage = charInfo.damage;
        maxMana = charInfo.mana;
        currentHealth = maxHealth;
        currentMana = 0;
        color = charInfo.color.ToString();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            DestroyCharacter();
        }
    }

    public void DestroyCharacter()
    {
        BattleSystem.Instance.charBattle.Remove(this.gameObject);
        Destroy(this.gameObject);
    }

    public void GainMana(int amount)
    {
        currentMana += amount;
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }
    }
}
