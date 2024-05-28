using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_O_Score : MonoBehaviour
{

    public int WinRounds;
    public int score;
    public S_O_PLayerController OwnerPlayer;

    public delegate void ScoreChangedDelegate();
    public event ScoreChangedDelegate OnScoreChanged;

    public void AddScore()
    {
        score++;
        OnScoreChanged?.Invoke();
    }

    public void AddWinRounds()
    {
        WinRounds++;
    }
    public void ResetScore()
    {
        score = 0;
        OnScoreChanged?.Invoke();
    }
}
