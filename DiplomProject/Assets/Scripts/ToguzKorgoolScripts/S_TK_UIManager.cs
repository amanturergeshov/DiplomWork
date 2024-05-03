using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_TK_UIManager : MonoBehaviour
{
    public Text FirstPlayerScore;
    public Text SecondPlayerScore;
    public Text FirstPlayerTimer;
    public Text SecondPlayerTimer;
    public S_TK_PlayerController FirstPlayer;
    public S_TK_PlayerController SecondPlayer;
    // Start is called before the first frame update

    private int Score1; 
    private int Score2;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        FirstPlayerScore.text = FirstPlayer.OurScoreLunka.score.ToString();
        SecondPlayerScore.text = SecondPlayer.OurScoreLunka.score.ToString();

        
        FirstPlayerTimer.text = FirstPlayer.TimerString;
        SecondPlayerTimer.text = SecondPlayer.TimerString;
        
    }
}
