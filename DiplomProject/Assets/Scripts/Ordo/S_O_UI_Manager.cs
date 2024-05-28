using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_O_UI_Manager : MonoBehaviour
{
    public Text FirstPlayerScore;
    public Text SecondPlayerScore;
    public Text FirstPlayerTimer;
    public Text SecondPlayerTimer;

    //*******************************************************ROUNDS IMAGES**********************************************************************************************
    public List<Image> RoundsUI;

    //***************************************************NEEED SCRIPTS***********************************************************************************************
    public S_O_GameManager GameManager;
    public S_O_PLayerController FirstPlayer;
    public S_O_PLayerController SecondPlayer;
    //***************************************************NEEED SCRIPTS***********************************************************************************************
    public Sprite FirstPlayerWin;
    public Sprite SecondPlayerWin;
    public Sprite Draw;

    void Start()
    {
        GameManager.OnRoundWinnerDetermined += SetImagesForRounds;

        FirstPlayer.OurScore.OnScoreChanged += SetScore;
        SecondPlayer.OurScore.OnScoreChanged += SetScore;
    }

    void Update()
    {
        FirstPlayerTimer.text = FirstPlayer.TimerString;
        SecondPlayerTimer.text = SecondPlayer.TimerString;

    }

    void SetScore()
    {
        FirstPlayerScore.text = FirstPlayer.OurScore.score.ToString();
        SecondPlayerScore.text = SecondPlayer.OurScore.score.ToString();
    }

    void SetImagesForRounds()
    {
        if (GameManager.gameEnded == true)
        {
            int CurrentRound = GameManager.CurrentRound;
            if (GameManager.RoundsWinner[CurrentRound] == 1)
            {
                RoundsUI[CurrentRound].sprite = FirstPlayerWin;
            }
            if (GameManager.RoundsWinner[CurrentRound] == 2)
            {
                RoundsUI[CurrentRound].sprite = SecondPlayerWin;
            }
            if (GameManager.RoundsWinner[CurrentRound] == 0)
            {
                RoundsUI[CurrentRound].sprite = Draw;
            }
        }
    }
}
