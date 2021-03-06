﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;
    private SpriteRenderer sprite;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        MakeLighter();
    }

    void MakeLighter()
    {
        Color color = sprite.color;
        float newAlpha = color.a * 0.5f;
        sprite.color = new Color(color.r, color.g, color.b, newAlpha);
    }
}
