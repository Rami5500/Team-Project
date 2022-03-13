using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;


public class LobbyScript : NetworkBehaviour
{

    public GameObject BalancePrefab;

    public GameObject SpeedPrefab;

    public GameObject HeavyPrefab;

    public GameObject Map1;
    public GameObject Map2;
    public GameObject Map3;


    public GameObject myPlayer;

    // Start is called before the first frame update
    void Start()
    {
       // myPlayer = Instantiate(PlayerPrefab);

       // if (SceneManager.GetActiveScene().name == "Scene_Lobby")
       // {
        //    NetworkServer.Spawn(myPlayer, connectionToClient);
       // }
    }

    // Update is called once per frame
    void Update()
    {
        if(isServer)
        {
            Map1.SetActive(true);
            Map2.SetActive(true);
            Map3.SetActive(true);
        }
    }


    public void ChangeBalanceTank()
    {
        ChangeTank(BalancePrefab,1);
    }

    public void ChangeSpeedTank()
    {
        ChangeTank(SpeedPrefab,2);
    }

    public void ChangeHeavyTank()
    {
        ChangeTank(HeavyPrefab,3);
    }

    void ChangeTank(GameObject TankPrefab, int i)
    {
       PlayerLobby[] Players = GameObject.FindObjectsOfType<PlayerLobby>();
      foreach (PlayerLobby player in Players)
        {
            if (player.hasAuthority)
            {
                player.ChangeTank(TankPrefab, i);
            }

        }
    }


}
