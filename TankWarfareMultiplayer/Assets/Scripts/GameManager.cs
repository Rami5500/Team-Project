using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Mirror;


public class GameManager : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
  
        //StartNewMatch();
        //Note: Start() runs before anyone connects to a server
    }

    [SyncVar]
    float _TimeLeft = 3;
    public float TimeLeft
    {
        get { return _TimeLeft; }
        set { _TimeLeft = value; }
    }


    [SyncVar]
    int _Round = 1;
    public int Round
    {
        get { return _Round; }
        set { _Round = value; }
    }


    [SyncVar]
    string _PlayerPhase = null;
    public string PlayerPhase
    {
        get { return _PlayerPhase; }
        set { _PlayerPhase = value; }
    }

    //public static GameManager Instance;

    public enum TURNSTATE { MOVE, SHOOT, RESOLVE } //Defines the different turnstates 
    //move when a tank can move
    //Shoot when a tank can aim
    //resolve when the bullet is spawned


    [SyncVar]
    TURNSTATE _TurnState;
    public TURNSTATE TurnState
    {
        get { return _TurnState; }
        protected set { _TurnState = value; }
    }

    public int TurnNumber { get; protected set; }

    [SyncVar, System.NonSerialized]
    bool matchHasStarted = false;

    


    [SyncVar, System.NonSerialized]
    public bool matchHasFinished = false;

    bool haveFireBullets = false;
    bool bulletsHaveSpawned = false;
    List<GameObject> activeResolutionsObjects;

    
    Queue<GameObject> eventQueue;

    
    GameObject currentEvent;
    public GameObject NextGame;

    public GameObject NewTurnAnimationPrefab;
    
    public GameObject terrain;


    [SyncVar, System.NonSerialized]
    public bool mapSet = false;


    //[SyncVar]
    public GameObject currentMap;


    
    public GameObject myMap;

    [SyncVar]
    public GameObject iceMap;

    [SyncVar]
    public GameObject desertMap;

    [SyncVar]
    public GameObject floatMap;



    [SyncVar]
    public int MapNum;

    bool scoreAdded = false;



    // Update is called once per frame
    void Update()
    {
       
        if (isServer == false)
        {
            return;
        }
        if(myMap != null)
        {
            mapSet = true;
        }

        if(GetAllPlayer().Length == 2 && mapSet == false)
        {
                  
                    Debug.Log("IF sTATEMENT");
                    //currentMap = floatMap;
                    loadMap();
                
        }

        if (Input.GetKey(KeyCode.Escape))
        {

            matchHasStarted = false;
            matchHasFinished = false;
            TimeLeft = 3;
        }

            Tank[] tanks = GetAllTanks();
        if (tanks.Length != 2)
        {
            if (matchHasStarted == false)
            {
                
                return;
            }
            matchHasFinished = true;
            if(scoreAdded == false)
            {
                AddScore();
                scoreAdded = true;
            }
            Player[] players = GetAllPlayer();
            foreach (Player player in players)
            {
            
                    if (player.score >= 3) //when a player has won
                {
                    Debug.Log("GAME OVER");
                 
            
                        NetworkManager.singleton.ServerChangeScene("Scene_Victory");
       
                   
                }
                              
            }
            ResetGame(); //reset the game -- start a new round
            if (Input.GetKey(KeyCode.Return))
            {
            
                Debug.Log("New Game");
                ResetGame();
            }
       // }
            return;
            
            
        }
       
      


        if(ProcessEvent())
        {
          
            return;
        }


        TimeLeft -= Time.deltaTime;

        if (matchHasStarted == false )
        {
            if (TimeLeft > 0)
            {
                return;
            }
            else
            {
                StartNewMatch();
            }
        }

        if( TurnState == TURNSTATE.RESOLVE)
        {
            if( ProcessResolvePhase() == false)
            {
                SwapTurn();
                AdvanceTurnPhase();
            }
        }
        else
        {
            if(TimeLeft <=0 || IsPhaseLocked())
            {
                AdvanceTurnPhase();
            }
        }
        

        if (tanks[0].tankTurn == true)
        {
            UIPlayerTurn(1);
        }
        if (tanks[1].tankTurn == true)
        {
            UIPlayerTurn(0);
        }


    }

    static public GameManager Instance()
    {
        return GameObject.FindObjectOfType<GameManager>();
    }

    public void EnqueueEvent( GameObject go)
    {
        if(eventQueue == null)
        {
            eventQueue = new Queue<GameObject>();
        }
        go.SetActive(false);
        eventQueue.Enqueue(go);
    }


    public bool IsProcessingEvent()  //checks if an event is being processed
    {
        if (currentEvent == null)
        {
            return false; 
        }
        return true;
    }


    bool ProcessEvent()
    {
        if(currentEvent!= null)
        {
            return true; ;
        }
        if(eventQueue == null || eventQueue.Count ==0 )
        {
            return false; 
        }
        currentEvent = eventQueue.Dequeue();
        currentEvent.SetActive(true);
        return true;
    }
   




    public void RegisterResolutionObject( GameObject o)
    {
       
        activeResolutionsObjects.Add(o);
    }

    public void UnregisterResolutionObject( GameObject o)
    {
        activeResolutionsObjects.Remove(o);
    }


    //resolves and complete all the animation/ bullet firing
    bool ProcessResolvePhase()
    {

        Tank[] tanks = GetAllTanks();
        int num = 0;

        if(haveFireBullets == false)
        {
            activeResolutionsObjects = new List<GameObject>();
            bulletsHaveSpawned = false; 
            foreach (Tank tank in tanks)
            {
                num++;
                Debug.Log("tanks:"+num);
                tank.Fire();
            }
            haveFireBullets = true;

           
        }
        if (activeResolutionsObjects.Count > 0 )
        {
            bulletsHaveSpawned = true;
        }

      //Debug.Log(activeResolutionsObjects.Count);

        if (bulletsHaveSpawned && activeResolutionsObjects.Count ==0)
        {
            Debug.Log(activeResolutionsObjects.Count);
            return false;
        }
        return true;

    }


    public void ResetGame()
    {

        Round += 1;

        Player[] players = GetAllPlayer();

        Destroy(myMap); //destroy the current map and spawns a new one so the terrain is reset
        loadMap();
      
        Tank[] tanks = GetAllTanks();
         foreach (Tank tank in tanks)
         {
             tank.DestroyTank();
         }
         matchHasStarted = false;
         matchHasFinished = false;
         TimeLeft = 3;
        
         foreach (Player player in players) //destroy all remaining tanks
        {
             player.DestroyTank();
       
         }

        players[1].DestroyTank();
        players[0].DestroyTank();
        players[1].SpawnTank();  //spawn the tanks back in
        new WaitForSeconds(4f);
        players[0].SpawnTank();
        scoreAdded = false; ;






    }


    public void AddScore() //increase the player score
    {
        Player[] players = GetAllPlayer();
        foreach (Player player in players)
        {
            if (player.myTank != null)
            {
                player.addScore();
            }
        }
    }




    public bool TankCanMove(Tank tank)
    {
        return matchHasStarted == true && TurnState == TURNSTATE.MOVE;
    }

    public bool TankCanShoot(Tank tank)
    {
        return matchHasStarted == true && TurnState == TURNSTATE.SHOOT;
    }



    void StartNewMatch()
    {
       
        matchHasStarted = true;
        TurnNumber = 0;
     

     
        Player[] players = GetAllPlayer();
      
        Debug.Log("IF sTATEMENT");
    

        Tank[] tanks = GetAllTanks();

     

        tanks[0].tankTurn = false; //sets the tanks turn

        tanks[1].tankTurn = true;
        



        StartNewTurn();
    }

    void StartNewTurn()
    {
        TurnNumber++;
        TurnState = TURNSTATE.MOVE;
        TimeLeft = 10;
        haveFireBullets = false;
        Debug.Log("Starting Turn: " + TurnNumber);

        

        GameObject ntgo = Instantiate(NewTurnAnimationPrefab);
        EnqueueEvent(ntgo);
    }

    void SwapTurn()
    {
        Tank[] tanks = GetAllTanks();
        tanks[0].CmdChangeTurn();
        tanks[1].CmdChangeTurn();

    }



    void AdvanceTurnPhase()
    {
        switch (TurnState) //switchs based on the turnstate
        {
            case TURNSTATE.MOVE: 
                TurnState = TURNSTATE.SHOOT;
                TimeLeft = 10;
                break;
            case TURNSTATE.SHOOT:
                TurnState = TURNSTATE.RESOLVE;
                TimeLeft = 0;
                break;
            case TURNSTATE.RESOLVE:
                StartNewTurn(); //fire bullets         
                break;
            default:
                Debug.LogError("UNKNOWN TURNSTATE!!");
                break;
        }
     
        Debug.Log("New Phase Started: " + TurnState.ToString());


        Tank[] tanks = GetAllTanks();
        foreach ( Tank tank in tanks)
        {
            tank.RpcNewPhase();
        }
    }


    void UIPlayerTurn(int num)
    {
        num = num + 1;
      //GameObject pt_go = GameObject.Find("Phase");
        PlayerPhase= "Player " + num + ":" + TurnState.ToString();
      //pt_go.GetComponent<UnityEngine.UI.Text>().text = "Player " + num+1 + ":" + TurnState.ToString();

    }

    Tank[] GetAllTanks()
    {
        return GameObject.FindObjectsOfType<Tank>();
    }

    public Player[] GetAllPlayer()
    {
        return GameObject.FindObjectsOfType<Player>();
    }

    bool IsPhaseLocked()
    {
        Tank[] tanks = GetAllTanks();

        if (tanks == null || tanks.Length == 0)
        {
            Debug.Log("No tanks");
            return false;
        }

        foreach (Tank tank in tanks)
        {
            if (tank.tankTurn == true)
            {
                if (tank.TankIsLockedIn == false)
                {
                    return false;
                }
            }
        }


        return true;
    }



 
    [Server]
    void loadMap()
    {
        Debug.Log("Load Map");
        settNumMap();
        //currentMap = floatMap;
        swapMap();
        if (isServer)
        {
            Debug.Log("isSERVER");
            
            myMap = Instantiate(currentMap);
            NetworkServer.Spawn(myMap, connectionToClient);
        }
      
    }

    [Server]
    void settNumMap()
    {
        Player[] players = GetAllPlayer();
    

        foreach (Player player in players)
         {

            if (player.isServer) ;
            {
                MapNum = player.mapNum;
            }
         }
    }

    [Server]
    void swapMap()
    {
        if(MapNum == 1)
        {
            currentMap = floatMap;
        }

        if (MapNum == 2)
        {
            currentMap = desertMap;
        }


        if (MapNum == 3)
        {
            currentMap = iceMap;
        }
    }




    [ClientRpc]
    public void RpcSetMap()
    {
        swapMap();
    }




    IEnumerator WaitForReady()
    {
        while (!connectionToClient.isReady)
        {
            yield return new WaitForSeconds(0.25f);
        }
        loadMap();
    }


}
