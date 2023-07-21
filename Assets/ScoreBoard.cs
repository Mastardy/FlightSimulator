using System;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    private static ScoreBoard instance;
    public static ScoreBoard Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ScoreBoard>();
            }

            return instance;
        }
    }

    [SerializeField] private Text scoreText;
    
    private int score;
    public int Score
    {
        get => score;
        set
        {
            scoreText.text = value.ToString();
            score = value;
        }
    }

    public void Increment()
    {
        Score += 1;
    }
}
