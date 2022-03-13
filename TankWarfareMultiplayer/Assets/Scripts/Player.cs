using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class Player : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (isServer == true)
        {
            if (SceneManager.GetActiveScene().name == "Game")
            // SpawnTank();
            {

            }
        }
    }

    [SyncVar]
    public GameObject TankPrefab;


    [SyncVar]
    public GameObject BalancePrefab;

    [SyncVar]
    public GameObject SpeedPrefab;

    [SyncVar]
    public GameObject HeavyPrefab;

    [SyncVar]
    public int tankNum;

    public GameObject myTank;


    public GameObject currentTank;


    [Tooltip("Diagnostic flag indicating whether this player is ready for the game to begin")]
    [SyncVar]
    public bool readyToBegin;


    [SyncVar]
    public int score = 0;

    [SyncVar]
    public int playerNum;

    [Command(requiresAuthority = false)]
    public void ChangeTank(GameObject newTankPrefab)
    {
        TankPrefab = newTankPrefab;
    }

    [Command]
    public void setPlayer(int num)
    {
        playerNum = num;
    }

    public void addScore()
    {
        score += 1;
    }



    public void DestroyTank()
    {
        NetworkServer.Destroy(myTank);
    }



    public void SpawnTank()
    {
        if( isServer == false)
        {
            return;
        }


        //GameObject go = Instantiate(TankPrefab);

        // NetworkServer.Spawn(go, connectionToClient);
        //NetworkServer.Spawn(go);

        //myTank = Instantiate(TankPrefab);
        /* if (tankNum == 1)
         {
             if (playerNum == 1)
             {
                 myTank = Instantiate(BalancePrefab, new Vector2(-16, 11), Quaternion.Euler(0, 0, 0));
             }
             if (playerNum == 2)
             {
                 myTank = Instantiate(BalancePrefab, new Vector2(29, 5), Quaternion.Euler(0, 0, 0));
             }
         }



         if (tankNum == 2)
         {
             if (playerNum == 1)
             {
                 myTank = Instantiate(SpeedPrefab, new Vector2(-16, 11), Quaternion.Euler(0, 0, 0));
             }
             if (playerNum == 2)
             {
                 myTank = Instantiate(SpeedPrefab, new Vector2(29, 5), Quaternion.Euler(0, 0, 0));
             }
         }



         if (tankNum == 3)
         {
             if (playerNum == 1)
             {
                 myTank = Instantiate(HeavyPrefab, new Vector2(-16, 11), Quaternion.Euler(0, 0, 0));
             }
             if (playerNum == 2)
             {
                 myTank = Instantiate(HeavyPrefab, new Vector2(29, 5), Quaternion.Euler(0, 0, 0));
             }
         }
        */
        SpawnMyTank();
        myTank = Instantiate(currentTank, new Vector2(29, 5), Quaternion.Euler(0, 0, 0));



        // myTank.GetComponent<Tank>().ChangePosition(new Vector3(-8, -3, 0));

        NetworkServer.Spawn(myTank, connectionToClient);
        
       /* if (isServer)
        {
            myTank.GetComponent<Tank>().ChangePosition(new Vector3(-16, 11, 0));
        }
        else
        {
            myTank.GetComponent<Tank>().ChangePosition(new Vector3(29, 5, 0));
      
        }
       */
     
    }


    public void SpawnMyTank()
    {
        if(tankNum == 1)
        {
            currentTank = BalancePrefab;
        }
        if (tankNum == 2)
        {
            currentTank = SpeedPrefab;
        }
        if (tankNum == 3)
        {
            currentTank = HeavyPrefab;
        }
    }



    [Command(requiresAuthority = false)]
    public void ChangeMyTank( int i)
    {
      
        tankNum = i;
    }

        // Update is called once per frame

        void Update()
    {
        
        if (Input.GetKey(KeyCode.Escape))
        {

            if (isServer)
            {
                NetworkManager.singleton.StopHost();

            }
            NetworkManager.singleton.StopClient();


        }
        if (Input.GetKey(KeyCode.Return))
        {
          
        }
    }
   

    }
