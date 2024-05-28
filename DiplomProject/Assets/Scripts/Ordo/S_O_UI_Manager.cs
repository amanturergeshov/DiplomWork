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
    public S_O_PLayerController FirstPlayer;
    public S_O_PLayerController SecondPlayer;
    void Start()
    {

    }

    void Update()
    {
        FirstPlayerScore.text = FirstPlayer.OurScore.score.ToString();
        SecondPlayerScore.text = SecondPlayer.OurScore.score.ToString();


        FirstPlayerTimer.text = FirstPlayer.TimerString;
        SecondPlayerTimer.text = SecondPlayer.TimerString;

    }
}
