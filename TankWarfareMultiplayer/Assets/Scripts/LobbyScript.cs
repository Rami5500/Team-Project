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

    public GameObject Map1Image;
    public GameObject Map2Image;
    public GameObject Map3Image;

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
        { //Shows the different maps if you are host
            Map1.SetActive(true);
            Map2.SetActive(true);
            Map3.SetActive(true);
            Map1Image.SetActive(true);
            Map2Image.SetActive(true);
            Map3Image.SetActive(true);
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




    void changeTank(int i) //changes user tank
    {
       PlayerLobby[] Players = GameObject.FindObjectsOfType<PlayerLobby>();
      
        foreach (PlayerLobby player in Players)
        {
            if (player.hasAuthority)
            {
                player.ChangeTank(i);
            }

        }
    }

     void changeMap(int i) //changes the map
    {
        PlayerLobby[] Players = GameObject.FindObjectsOfType<PlayerLobby>();

        foreach (PlayerLobby player in Players)
        {
            if (player.hasAuthority)
            {
                player.ChangeMap(i);
            }

        }
    }



}
