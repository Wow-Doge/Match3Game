using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public int maxHealth = 3000;
    public int currentHealth;

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

    void Update()
    {
        
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        ReduceAlpha();
    }

    void ReduceAlpha()
    {
        Color color = image.color;
        Debug.Log(currentHealth);
        float newAlpha = ((3000 - (3000 - currentHealth)) / 3000);
        color.a = newAlpha;
        Debug.Log("newAlpha: = " + newAlpha);
        Debug.Log("alpha: " + color.a);
        image.color = new Color(color.r, color.g, color.b, color.a);
    }
}
