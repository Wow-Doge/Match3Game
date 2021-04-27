using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Skill : MonoBehaviour
{
    public Image image;
    public CharacterManager characterManager;
    public ManaSystem manaSystem;
    public GameObject button;

    private void OnEnable()
    {
        BattleSystem.Instance.OnPlayerEndTurn += UpdateUI;
    }
    private void OnDisable()
    {
        BattleSystem.Instance.OnPlayerEndTurn -= UpdateUI;
    }

    private void UpdateUI(object sender, System.EventArgs e)
    {
        float minValue = manaSystem.currentMana;
        float maxValue = manaSystem.maxMana;

        float fillPercentage = minValue / maxValue;
        image.fillAmount = fillPercentage;
        if (fillPercentage == 1)
        {
            SkillReady();
        }
    }

    public void UseSkill()
    {
        if (BattleSystem.Instance.battleState == BattleState.PLAYERTURN)
        {
            Debug.Log("use skill");
            manaSystem.ResetMana();
            ResetSkill();
        }
    }

    public void SkillReady()
    {
        image.color = Color.yellow;
        button.SetActive(true);
    }

    public void ResetSkill()
    {
        image.color = Color.white;
        button.SetActive(false);

        float minValue = manaSystem.currentMana;
        float maxValue = manaSystem.maxMana;
        float fillPercentage = minValue / maxValue;
        image.fillAmount = fillPercentage;
    }
}
