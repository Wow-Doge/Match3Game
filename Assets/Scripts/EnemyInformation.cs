using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemyInformation : ScriptableObject
{
    public int maxHealth;
    public int damage;
    public int charge;
}
