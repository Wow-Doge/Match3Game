using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ManaSystem : MonoBehaviour
{
    public int maxMana;
    public int currentMana;

    public void SetMana(int maxMana)
    {
        currentMana = 0;
        this.maxMana = maxMana;
    }

    public void IncreaseMana(int amount)
    {
        currentMana += amount;
        if (currentMana >= maxMana)
        {
            currentMana = maxMana;
        }
    }

    public void ResetMana()
    {
        currentMana = 0;
    }
}
