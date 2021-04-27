﻿using System.Collections;
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
    void Start()
    {
        health = charInfo.maxHealth;
        damage = charInfo.damage;
        mana = charInfo.mana;
        color = charInfo.color.ToString();

        healthSystem.SetCurrentHealth(health);
        manaSystem.SetMana(mana);
    }
}
