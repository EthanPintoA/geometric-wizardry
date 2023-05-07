using System;
using UnityEngine;
using TMPro;

public class GameOverTimer : MonoBehaviour
{
    public float timeSeconds;

    private TextMeshProUGUI timerText;

    void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();

        timerText.text = FormatSeconds((int)timeSeconds);
    }

    void Update()
    {
        timeSeconds -= Time.deltaTime;
        timerText.text = FormatSeconds((int)timeSeconds);
    }

    private string FormatSeconds(int seconds)
    {
        return string.Format("{0:00}:{1:00}", seconds / 60, seconds % 60);
        // return $"{seconds / 60}:{seconds % 60}";
    }
}
