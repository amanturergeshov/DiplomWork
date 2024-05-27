using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_O_PlayZone : MonoBehaviour
{
    public List<GameObject> InsideAlchikObjects = new List<GameObject>();
    public List<S_O_PLayerController> Players;

    public S_O_PLayerController ActivePlayer;
    void Start()
    {
        if (Players.Count > 0)
        {
            // Подписываем метод SetActivePlayer на событие OnTurnChange каждого игрока
            foreach (var player in Players)
            {
                player.OnTurnChange += SetActivePlayer;

                if (player.isMyTurn == true)
                {
                    SetActivePlayer(player);
                }
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (InsideAlchikObjects.Contains(other.gameObject))
        {
            StartCoroutine(RemoveChuko(other.gameObject));
            if (ActivePlayer != null)
            {
                ActivePlayer.alchikKnockedOut = true; // Установить флаг
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        print("Entering" + other.gameObject.name);
    }

    IEnumerator RemoveChuko(GameObject Chuko)
    {
        if (ActivePlayer)
        {
            ActivePlayer.OurScore.AddScore();
        }
        InsideAlchikObjects.Remove(Chuko);
        if (Players.Count > 0)
        {
            foreach (var player in Players)
            {
                player.alchikObjects = InsideAlchikObjects;
            }
        }

        yield return new WaitForSeconds(2f);
        Destroy(Chuko);
    }

    public void SetActivePlayer(S_O_PLayerController newActivePlayer)
    {
        ActivePlayer = newActivePlayer;
    }
}
