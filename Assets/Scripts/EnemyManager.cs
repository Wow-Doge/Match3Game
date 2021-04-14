using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyManager : MonoBehaviour, IPointerClickHandler
{
    public EnemyInformation enemyInfo;

    public int maxHealth;
    public int currentHealth;
    public int damage;
    public int charge;
    public int currentCharge;
    public bool isSelected = false;
    void Start()
    {
        maxHealth = enemyInfo.maxHealth;
        damage = enemyInfo.damage;
        charge = enemyInfo.charge;
        currentHealth = maxHealth;
        currentCharge = charge;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
    }

    public void DecreaseCharge()
    {
        currentCharge--;
    }

    public void ResetCharge()
    {
        currentCharge = charge;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ShowSelectCircle();
        List<GameObject> enemyList = BattleSystem.Instance.enemyBattle;
        foreach (var enemy in enemyList)
        {
            if (enemy != this.gameObject)
            {
                enemy.GetComponent<EnemyManager>().HideSelectCircle();
            }
        }
    }

    public void HideSelectCircle()
    {
        isSelected = false;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ShowSelectCircle()
    {
        isSelected = true;
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }
}
