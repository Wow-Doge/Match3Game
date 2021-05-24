using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Health : MonoBehaviour
{
    public HealthSystem healthSystem;
    Image image;

    private void Start()
    {
        image = gameObject.transform.GetChild(1).transform.GetComponent<Image>();
    }

    private void OnEnable()
    {
        healthSystem.OnHealthChange += UIHealthUpdate;
    }

    private void UIHealthUpdate(object sender, System.EventArgs e)
    {
        image.fillAmount = (float)healthSystem.currentHealth / healthSystem.maxHealth;
    }

    private void OnDisable()
    {
        healthSystem.OnHealthChange -= UIHealthUpdate;
    }
}
