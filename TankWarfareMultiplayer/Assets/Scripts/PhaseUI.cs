 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PhaseUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
    }

    GameManager gameManager;
    Text text;


    public int Playernum;


    // Update is called once per frame
    void Update()
    {
        if (gameManager == null)
        {
            gameManager = GameObject.FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                return;
            }
        }


        if (gameManager.matchHasFinished == true)
        {
            {
                Player[] players = gameManager.GetAllPlayer();
                foreach (Player player in players)
                {
                    if (player.myTank != null)
                    {
                        Playernum = player.playerNum;
                    }
                }
             if (Playernum == 1)
                {
                    text.text = "Player 2 Won, Press enter for another round";
                }
             if (Playernum == 2)
                {
                    text.text = "Player 1 Won, Press enter for another round";
                }
             return;
            }
        }



            text.text = gameManager.PlayerPhase;

    }
}
