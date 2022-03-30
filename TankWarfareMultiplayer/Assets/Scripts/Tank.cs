using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Tank : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        facingRight = true;
        gameManager = GameObject.FindObjectOfType<GameManager>();
    }

    GameManager gameManager;

    float MovemenetPerTurn = 5;
    float MovementLeft;
    private bool facingRight = false;

    public float Speed = 10;
    float TurretSpeed = 180;
    float TurretPowerSpeed = 10;

    public float MaxPower = 20;

    public GameObject CurrentBulletPrefab;
    public Transform TurretPivot;
    public Transform BulletSpawnPoint;
    public GameObject aimArrow;
    public AudioSource tankMovementSound;
    public AudioSource firingSound;



    public GameObject smallBullet;
    public GameObject mediumBullet;
    public GameObject heavyBullet;


    public GameObject activeArrow;

    public GameObject myArrow;
    public GameObject enemyArrow;

    public SpriteRenderer turretBody;
    public SpriteRenderer tankBody;





    static public Tank LocalTank{ get; protected set; }


    [SyncVar]
    public bool tankTurn = false;

    [SyncVar (hook = "OnTurretAngleChange")]
    float turretAngle = 0f;


    float turretPower = 10f;

    [SyncVar]
    Vector3 serverPosition;

    [SyncVar]
    Quaternion serverRotation;




   Vector3 serverPositionSmoothVelocity;


   // [SyncVar]
    public bool TankIsLockedIn { get; protected set; }
    

    void NewTurn() //For limiting the amount of speed but found the timelimit was good enough
    {
        MovementLeft = MovemenetPerTurn;
    }



    // Update is called once per frame
    void Update()
    {
        if( isServer)
        {

        }
        if (hasAuthority)
        {
          
        }

        tankMovementSound.Pause();

        if ( hasAuthority && tankTurn == true)
        {
            
            LocalTank = this; //If this is your tank make it your localtank

            AuthorityUpdate();
            
        }
        if(tankTurn == true)
        {

       
            CmdActiveArrow(true); //set the arrow above the user to show if its there turn
        }
        if(tankTurn == false)
        {
            CmdActiveArrow(false);  //does the oppsite
        }
        if (hasAuthority == false)
        {
            CmdEnemyArrow(true);
            CmdActiveArrow(false);
        }

        //when its not your turn or if it isn't your tank
        if ( hasAuthority == false || tankTurn == false)
        {

        
            aimArrow.SetActive(false);
  
            CmdUpdatePosition(Vector3.SmoothDamp(transform.position, serverPosition, ref serverPositionSmoothVelocity, 0.25f));
            CmdUpdateRotation(serverRotation);
         
        }

        Vector3 euler = transform.eulerAngles;
        if (euler.z > 180) euler.z = euler.z - 360;
        euler.z = Mathf.Clamp(euler.z, -45, 45);
        transform.eulerAngles = euler;
        TurretPivot.localRotation = Quaternion.Euler(0, 0, turretAngle);

    }

    void AuthorityUpdate()
    {
        if(GameManager.Instance().IsProcessingEvent())
        {
            return;
        }


        AuthorityUpdateMovement();
        AuthorityUpdateShooting();

        GameObject pn_go = GameObject.Find("PowerNumber");
        pn_go.GetComponent<UnityEngine.UI.Text>().text = turretPower.ToString("#.00");

    }



    void AuthorityUpdateMovement()
    {

        if(TankIsLockedIn == true || gameManager.TankCanMove( this) == false)
        {
            return;
        }

        // tankMovementSound.Pause();
        tankMovementSound.UnPause();


        CmdMovementSound();
        RpcMovementSound();
        float movement = Input.GetAxis("Horizontal") * Speed * Time.deltaTime; //gets the movement
        float jumpVelocity = 1f;

        //float Jump = Input.GetAxis("Horizontal") * jumpVelocity * Time.deltaTime;

   
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))  //reduce the movement to allow for more precise movements
        {
           
            movement *= 0.1f;
        }
        float jump = Input.GetAxis("Vertical") * Speed * Time.deltaTime; ;


        if(Input.GetKey(KeyCode.UpArrow))
        {

          jump *= jumpVelocity; //jumps
        }


        transform.Translate(movement, jump, 0); //sets it location
        //Flip(movement);
        if (Input.GetAxis("Horizontal") < 0 && !facingRight)
        {
            Flip(movement);
        }
        if (Input.GetAxis("Horizontal") > 0 && facingRight)
        {
            Flip(movement);
        }
        CmdUpdatePosition(transform.position);  //update the location for the other user


        if (Input.GetKeyUp(KeyCode.Space)) //locks in movement
        {
            TankIsLockedIn = true;
            CmdLockIn();
        }
    }



    void AuthorityUpdateShooting()
    {
        //aimArrow.SetActive(true);

        if (TankIsLockedIn == true || gameManager.TankCanShoot(this) == false)
        {
            return;
        }

        float turretMovement = Input.GetAxis("TurretHorizontal") * TurretSpeed * Time.deltaTime;
        if(Input.GetKey(KeyCode.LeftShift)|| Input.GetKey(KeyCode.RightShift))
        {
            turretMovement *= 0.1f;
        }

        turretAngle = Mathf.Clamp(turretAngle + turretMovement, 0 , 180); //changes the angle of the turret
        CmdChangeTurretAngle(turretAngle);

        float powerChange = Input.GetAxis("Vertical") *TurretPowerSpeed* Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            powerChange *= 0.1f;
        }

        turretPower = Mathf.Clamp(turretPower + powerChange, 0, MaxPower);  //updates the power limiting the max to the maxpower
        CmdSetTurretPower(turretPower);

       // UpdateAimArrow();

        if (Input.GetKeyUp(KeyCode.Space))  //locks in the shooting
        {
           
            TankIsLockedIn = true;
            CmdLockIn();


            //Vector2 velocity = new Vector2(turretPower * Mathf.Cos(turretAngle * Mathf.Deg2Rad), turretPower * Mathf.Sin(turretAngle * Mathf.Deg2Rad));
            //CmdFireBullet(BulletSpawnPoint.position, velocity);
        }

    }


    public void Fire() //used by gameManager to fire the bullet
    {
        if(tankTurn == false) //Won't fire the your bullet if isn't your turn
        { 
            return;
        }
       // firingSound.Play();
        Vector2 velocity = new Vector2(turretPower * Mathf.Cos(turretAngle * Mathf.Deg2Rad), turretPower * Mathf.Sin(turretAngle * Mathf.Deg2Rad));
        aimArrow.SetActive(false);
        firingSound.Play();
        RpcBulletSound();
        CmdFireBullet(BulletSpawnPoint.position, velocity);
    }

    private void Flip(float horizontal) //Flips the tank depends on its facing direction
    {
       
        if(Input.GetAxis("Horizontal") < 0)
          {
            // transform.rotation = Quaternion.Euler(0, 180f, 0);
             tankBody.flipX = facingRight;
            CmdFlipTank(facingRight);
            RPCFlipTank(facingRight);
            turretAngle = Mathf.Clamp(180, 0, 180);
            CmdChangeTurretAngle(turretAngle);
            // turretBody.flipX = true;
        }
          if (Input.GetAxis("Horizontal") > 0)
          {
            //tankBody.flipX = true;
            turretAngle = Mathf.Clamp(0, 0, 180);
            tankBody.flipX = facingRight;
            CmdFlipTank(facingRight);
            RPCFlipTank(facingRight);
            CmdChangeTurretAngle(turretAngle);
            //turretBody.flipX = false;
            // transform.rotation = Quaternion.Euler(0, 0, 0);
        }

   
        facingRight = !facingRight;


    }


    [Command(requiresAuthority = false)]
    public void ChangePosition(Vector3 newPosition)
    {
        CmdUpdatePosition(newPosition);
        RpcFixPosition(newPosition);
    }

    [Command(requiresAuthority = false)]
    public void ChangeRotation(Quaternion newPosition)
    {
        CmdUpdateRotation(newPosition);
        RpcFixRotation(newPosition);
    }




    [Command(requiresAuthority = false)]
    public void CmdChangeTurn()
    {
        tankTurn = !tankTurn;
    }


    [Command(requiresAuthority = false)]
    public void DestroyTank()
    {
       // NetworkServer.Destroy(this);
    }


    void UpdateAimArrow()
    {
        aimArrow.transform.rotation = Quaternion.AngleAxis(turretAngle, Vector3.forward);
        aimArrow.transform.localScale = new Vector2(turretPower / 12, turretPower / 12);
    }

    [Command(requiresAuthority = false)]
    void CmdFlipTank(bool flipDirection)
    {
        tankBody.flipX = flipDirection;
    }

    [ClientRpc]
    void RPCFlipTank(bool flipDirection)
    {
        tankBody.flipX = flipDirection;
    }



    [Command]
    void CmdLockIn()
    {
        TankIsLockedIn = true;
    }


   // [Command(requiresAuthority = false)]
    void CmdActiveArrow(bool activeState)
    {
        activeArrow.SetActive(activeState);
    }

    void CmdEnemyArrow(bool activeState)
    {
        enemyArrow.SetActive(activeState);
    }



    [Command]
    void CmdChangeTurretAngle(float angle)
    {
        turretAngle = angle;
    }

    [Command]
    void CmdSetTurretPower(float power)
    {
        turretPower = power;
    }


    [Command(requiresAuthority = false)]
    void CmdFireBullet(Vector2 bulletPosition, Vector2 velocity)
    {
        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;

        GameObject go = Instantiate(CurrentBulletPrefab,bulletPosition, Quaternion.Euler(0, 0, angle));
        go.GetComponent<Bullet>().SourceTank = this;

        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();        
        rb.velocity = velocity;
        firingSound.Play();

        NetworkServer.Spawn(go); //Spawns the bullet
    }


    [Command(requiresAuthority = false)] // THIS IS BAD WE NEED TO BE CHANGE ALLOWES FOR CHEATING (TELEPORTING)
    public void CmdUpdatePosition(Vector3 newPosition)
    {
        if(gameManager.TankCanMove(this) == false)
        {
            //SHOULD NOT MOVE
        }
     
        serverPosition = newPosition;
    }


    [Command(requiresAuthority = false)] // THIS IS BAD WE NEED TO BE CHANGE ALLOWES FOR CHEATING (TELEPORTING)
    public void CmdUpdateRotation(Quaternion newPosition)
    {
        if (gameManager.TankCanMove(this) == false)
        {
            //SHOULD NOT MOVE
        }
        serverRotation = newPosition;
    }

    [Command(requiresAuthority = false)] // THIS IS BAD WE NEED TO BE CHANGE ALLOWES FOR CHEATING (TELEPORTING)
    public void CmdUpdateFacing(Vector3 newFacing)
    {

       // serverFacing = newFacing;



    }

    [Command(requiresAuthority = false)]
    void CmdMovementSound()
    {
        tankMovementSound.UnPause();
    }

    [ClientRpc]
    void RpcBulletSound()
    {
        firingSound.Play();
    }

    [ClientRpc]
    void RpcMovementSound()
    {
        tankMovementSound.UnPause();
    }




    [ClientRpc]
    void RpcFixPosition( Vector3 newPosition)
    {
        transform.position = newPosition;
    }


    [ClientRpc]
    void RpcFixRotation(Quaternion newPosition)
    {
        transform.rotation = newPosition;
    }



    [ClientRpc]
    void RpcNewTurn()
    {
       
    }


    [ClientRpc]
    public void RpcNewPhase()
    {
        TankIsLockedIn = false;
    }



    //SyncVar Hook

    void OnTurretAngleChange(float oldAngle, float newAngle)
    {
        if (hasAuthority)
        {
            return;
        }


        turretAngle = newAngle;
    }




}
