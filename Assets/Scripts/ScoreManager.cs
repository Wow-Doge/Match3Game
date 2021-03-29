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
}
