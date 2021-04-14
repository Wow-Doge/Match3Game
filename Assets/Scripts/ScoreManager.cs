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
                case "Red":
                    redBar.GetComponent<TextMeshProUGUI>().text = "red\n " + kvp.Value.Count.ToString();
                    break;
                case "Blue":
                    blueBar.GetComponent<TextMeshProUGUI>().text = "blue\n " + kvp.Value.Count.ToString();
                    break;
                case "Yellow":
                    yellowBar.GetComponent<TextMeshProUGUI>().text = "yellow\n " + kvp.Value.Count.ToString();
                    break;
                case "Purple":
                    purpleBar.GetComponent<TextMeshProUGUI>().text = "purple\n " + kvp.Value.Count.ToString();
                    break;
                case "Green":
                    greenBar.GetComponent<TextMeshProUGUI>().text = "green\n " + kvp.Value.Count.ToString();
                    break;
                default:
                    break;
            }
        }
        
    }
}
