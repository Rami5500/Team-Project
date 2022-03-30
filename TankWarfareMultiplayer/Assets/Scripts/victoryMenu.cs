using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using Mirror;

public class victoryMenu : NetworkBehaviour
{
    public PlayfabManager myPlayFabManger;
    public GameObject win;
    public GameObject lose;


    bool hasUpdatedScore = false;

    Text text;

    //Player[] players;

    // Start is called before the first frame update
    void Start()
    {

        text = win.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

        changeVictory();



    }

    //[ClientRpc]
    void changeVictory()
    {
        PlayerLobby[] players = GameObject.FindObjectsOfType<PlayerLobby>(); //Finds all the scripts called PlayerLOBBY
                                                                             //PlayFabClient[] playersfabs;// = GameObject.FindObjectsOfType<PlayFabClient>();

        foreach (PlayerLobby player in players)  //Loops through them
        {

                 if (player.hasAuthority)  //if you have authority i.e you own it
            {

                      if (player.score >= 3) //checks to see your score - 3 being if you won
                {

                        if (hasUpdatedScore == false) //checks if it has already updated your score
                    {
                            if (myPlayFabManger.isLoggedIn()) //only add to the leader if you are logged in
                        {
                                myPlayFabManger.SendLeaderBoard(100); //adds 100 to player score
                            hasUpdatedScore = true;
                            }
                        }
                    // win.SetActive(true);
                    // lose.SetActive(false);
                          text.text = "You won"; //change text to say you win
                    return;
                      }
                      else
                      {
                    if (hasUpdatedScore == false)
                    {
                        
                         if (myPlayFabManger.isLoggedIn())
                        {
                            myPlayFabManger.SendLeaderBoard(-50); //if the player loses lose 50 points a risk to playing
                            hasUpdatedScore = true;
                        }
                    }
                    
                    //win.SetActive(false);
                    //lose.SetActive(true);
                    text.text = "You lost";
                      }

                  }                         
            }
        
        
    }


    public void goToMainMenu() //go back to mainmenu
    {
        // SceneManager.LoadScene("Scene_MainMenu");
        NetworkManager.singleton.StopHost();  //leave the server
        NetworkManager.singleton.StopClient();
    }

}
