using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class CharacterInformation : ScriptableObject
{
    public int maxHealth;
    public int damage;
    public int mana;
    public enum CharacterColor
    {
        Red,
        Blue,
        Green,
        Yellow,
        Purple,
    }
    public CharacterColor color;
}
