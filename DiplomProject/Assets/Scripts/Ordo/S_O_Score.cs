using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_O_Score : MonoBehaviour
{

    public int WinRounds;
    public int score;
    // Start is called before the first frame update
    public S_O_PLayerController OwnerPlayer;

    // Update is called once per frame
    public void AddScore()
    {
        score++;
    }

    public void AddWinRounds()
    {
        WinRounds++;
    }
}
