using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using NobleConnect.Mirror;
using UnityEngine.SceneManagement;

public class LoginButtonScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void buttonLogin()
    {
        SceneManager.LoadScene("Scene_Login_&_Register");

    }

    public void buttonLeaderboard()
    {
        SceneManager.LoadScene("Leader&Sent");
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
