using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public int score;

    public static ScoreManager Instance;

    public GameObject redBar;
    public GameObject blueBar;
    public GameObject yellowBar;
    public GameObject purpleBar;
    public GameObject greenBar;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        scoreText.text = score.ToString();
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
    }

    public void ToText()
    {
        foreach (var kvp in BoardManager.Instance.dict)
        {
            switch (kvp.Key)
            {
                case "red":
                    redBar.GetComponent<TextMeshProUGUI>().text = "red\n " + kvp.Value.Count.ToString();
                    break;
                case "blue":
                    blueBar.GetComponent<TextMeshProUGUI>().text = "blue\n " + kvp.Value.Count.ToString();
                    break;
                case "yellow":
                    yellowBar.GetComponent<TextMeshProUGUI>().text = "yellow\n " + kvp.Value.Count.ToString();
                    break;
                case "purple":
                    purpleBar.GetComponent<TextMeshProUGUI>().text = "purple\n " + kvp.Value.Count.ToString();
                    break;
                case "green":
                    greenBar.GetComponent<TextMeshProUGUI>().text = "green\n " + kvp.Value.Count.ToString();
                    break;
                default:
                    break;
            }
        }
        
    }
}
