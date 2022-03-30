using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;


public class LobbyScript : NetworkBehaviour
{

    //public GameObject BalancePrefab;

    //public GameObject SpeedPrefab;

   // public GameObject HeavyPrefab;

    public GameObject Map1;
    public GameObject Map2;
    public GameObject Map3;

    public GameObject myTank;
    public GameObject myPlayer;


    public GameObject ButtonTank1;
    public GameObject ButtonTank2;
    public GameObject ButtonTank3;

    GameObject selectedTank;

    GameObject selectedMap;


    public GameObject ButtonMap1;
    public GameObject ButtonMap2;
    public GameObject ButtonMap3;




    // Start is called before the first frame update
    void Start()
    {
       // myPlayer = Instantiate(PlayerPrefab);

       // if (SceneManager.GetActiveScene().name == "Scene_Lobby")
       // {
        //    NetworkServer.Spawn(myPlayer, connectionToClient);
       // }
    }

    void SetCurrentTank(GameObject currentTank)
    {
        selectedTank = currentTank;
    }

    void SetCurrentMap(GameObject currentMap)
    {
        selectedMap = currentMap;
    }

    void HighlightSelectedTank(GameObject currentTank)
    {
        if (selectedTank != null)
        {
            selectedTank.GetComponent<Button>().GetComponent<Image>().color = Color.white;
        }
        SetCurrentTank(currentTank);

        selectedTank.GetComponent<Button>().GetComponent<Image>().color = Color.green;
    }

    void HighlightSelectedMap(GameObject currentMap)
    {
        if (selectedMap != null)
        {

            selectedMap.GetComponent<Button>().GetComponent<Image>().color = Color.white;
        }
        SetCurrentMap(currentMap);
        selectedMap.GetComponent<Button>().GetComponent<Image>().color = Color.green;
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
        HighlightSelectedTank(ButtonTank2);
        changeTank(1);
    }

    public void ChangeSpeedTank()
    {
        HighlightSelectedTank(ButtonTank1);
        changeTank(2);
    }

    public void ChangeHeavyTank()
    {
        HighlightSelectedTank(ButtonTank3);
        changeTank(3);
    }

    public void ChangeFloatMap()
    {
        HighlightSelectedMap(ButtonMap2);
        changeMap(1);
    }

    public void ChangeDesertMap()
    {
        HighlightSelectedMap(ButtonMap1);
        changeMap(2);
    }

    public void ChangeIceMap()
    {
        HighlightSelectedMap(ButtonMap3);
        changeMap(3);
    }



    //[Command(requiresAuthority = false)]
    //[Server]
    //  [Command(requiresAuthority = false)]
    //[Server]
    //[Command]
    void changeTank(int i)
    {
       PlayerLobby[] Players = GameObject.FindObjectsOfType<PlayerLobby>();
        //myTank = Instantiate(TankPrefab);
        //myTank = TankPrefab;
      // Debug.Log(myTank);
        foreach (PlayerLobby player in Players)
        {
            if (player.hasAuthority)
            {
                player.ChangeTank(i);
            }

        }
    }

     void changeMap(int i)
    {
        PlayerLobby[] Players = GameObject.FindObjectsOfType<PlayerLobby>();
        //myTank = Instantiate(TankPrefab);
        //myTank = TankPrefab;
        // Debug.Log(myTank);
        foreach (PlayerLobby player in Players)
        {
            if (player.hasAuthority)
            {
                player.ChangeMap(i);
            }

        }
    }



}
