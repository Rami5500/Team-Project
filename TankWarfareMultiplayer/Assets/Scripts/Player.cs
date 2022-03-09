using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (isServer == true)
        {
            SpawnTank();
        }
    }

    public GameObject TankPrefab;
      public GameObject myTank;


    [SyncVar]
    public int score = 0;

    [SyncVar]
    public int playerNum;



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


        myTank = Instantiate(TankPrefab);
       
        
     
           // myTank.GetComponent<Tank>().ChangePosition(new Vector3(-8, -3, 0));
        
        NetworkServer.Spawn(myTank, connectionToClient);
        if (isServer)
        {
         //   myTank.GetComponent<Tank>().transform.position = (new Vector3(8, -3, 0));
        }
     
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
