using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (isServer)
        {
            GameManager.Instance().RegisterResolutionObject(gameObject);
        }
    }




    public float armTime = 3;
    Rigidbody2D rb;

    public float Radius = 2f;
    public float Damage = 10f;
    public bool DamageFallsOff = true;
    public Tank SourceTank; //that tank that spawned the bullet

    public float LifeSpan = 5;

    public bool RotatesWithVelocity = true;

    public AudioSource expolsionSound;


    public GameObject ExplosionPrefab;


    // Update is called once per frame
    void Update()
    {
        if (RotatesWithVelocity == true)
        { 
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
        }

     

        if ( isServer == true)
        {
            armTime -= 1;
            LifeSpan -= Time.deltaTime;
            if(LifeSpan <= 0)
            {
                Die(); //destroy the bullet
            }
        }

    }



    //[ClientRpc]
    void RpcDoExplosion(Vector2 position)
    {
       GameObject go = Instantiate(ExplosionPrefab, position, Quaternion.identity); //instantiates an explosion and set it to a gameobject
       //  go.GetComponent<BulletExplosion>().Radius = Radius;
        NetworkServer.Spawn(go); //spawned the expolsion

    }



    void OnCollisionEnter2D(Collision2D collision)
    {
        OnTriggerEnter2D(collision.collider);

    }



    void OnTriggerEnter2D(Collider2D collider)
    {
        if(isServer == false || armTime > 0)  //won't do anything if it is not the host
        {
            return;
        }

        if (GetComponent<Rigidbody2D>() == collider.attachedRigidbody) //If it colliders with itself return
        {
            return;
        }

        Collider2D[] cols = Physics2D.OverlapCircleAll(this.transform.position, Radius); //store all the collisions it made around the radius (explosion size)
        foreach (Collider2D col in cols)  //loops for all the colliders
        {
            if( col.attachedRigidbody == null) //goes to next collider if there is no rigidbody
            {
                continue;
            }


            Health h = col.attachedRigidbody.GetComponent<Health>(); //gets the health component

            if (h != null)
            {
                Debug.Log(col.attachedRigidbody.gameObject.name);
               
                h.CmdChangeHealth(-Damage);  //reduces the health on the object

            }
        }
       
        RpcDoExplosion(this.transform.position);
        //terrainDestroyer.instance.DestroyTerrain(this.transform.position, 2);
       
            DestroyTerrain(this.transform.position, 2); //destroys terrain


        RPCDestroyTerrain(this.transform.position, 2);
        
        // Destroy(gameObject);
        Die();
    }

    void DestroyTerrain(Vector3 location, int radius)
    {
        TerrainDestroyer.instance.DestroyTerrain(location, radius);
    }

    [ClientRpc]
    void RPCDestroyTerrain(Vector3 location, int radius)
    {
        TerrainDestroyer.instance.DestroyTerrain(location, radius);
    }

      void Die()
    {
      
        // Should Die() be the thing that triggers the damage explosion?
        // i.e. if we time-out in midair (or during bouncing)

        if (isServer)
        {
            GameManager.Instance().UnregisterResolutionObject(gameObject);
        }

       
        // Remove ourselves from the game
        Destroy(gameObject);

    }


}
