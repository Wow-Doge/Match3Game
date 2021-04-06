using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersController : MonoBehaviour
{
    public CharacterInformation characterInformation;

    public int maxHealth;
    public int currentHealth;
    public int damage;
    public int mana;
    void Start()
    {
        maxHealth = characterInformation.maxHealth;
        damage = characterInformation.damage;
        mana = characterInformation.mana;
        currentHealth = maxHealth;
    }
}
