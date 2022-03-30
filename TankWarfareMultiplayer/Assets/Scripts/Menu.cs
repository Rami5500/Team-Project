using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using NobleConnect.Mirror;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public NobleNetworkManager networkManager;
    public PlayfabManager myPlayfabManager;
    public GameObject menuPanel;

    public GameObject leaderBoardButton;
    public GameObject logoutButton;
    public GameObject loginButton;


    // The relay ip and port from the GUI text box
    string hostIP = "";
    string hostPort = "";

    // Used to determine which GUI to display
    bool isHost, isClient;

    public void Host()
    {
     

        isHost = true;
        isClient = false;
     
        networkManager.StartHost();
        //Debug.Log(networkManager.HostEndPoint.Address.ToString());
       // Debug.Log(networkManager.HostEndPoint.Port.ToString());
        menuPanel.SetActive(false);
       
        //GUIHost();
    }

    public void SetIP(string ip)
    {
        hostIP = ip.ToString();
        //networkManager.networkAddress = ip;
    }

    public void SetPort(string hostPort1)
    {
        //networkManager.networkPort = ushort.Parse(hostPort1);
        hostPort = hostPort1.ToString();
    }

    public void Join()
    {
        Debug.Log(hostPort);
        Debug.Log(hostIP);
        networkManager.InitClient();
        isHost = false;
        isClient = true;

       networkManager.networkAddress = hostIP;
        networkManager.networkPort = ushort.Parse(hostPort);
        //Debug.Log(networkManager.networkAddress);
       // Debug.Log(networkManager.networkPort);
        networkManager.StartClient();
        menuPanel.SetActive(false);
    }


    void GUIHost()
    {
        // Display host addresss
        if (networkManager.HostEndPoint != null)
        {
            GUI.Label(new Rect(10, 10, 150, 22), "Host IP:");
            GUI.TextField(new Rect(170, 10, 420, 22), networkManager.HostEndPoint.Address.ToString(), "Label");
            GUI.Label(new Rect(10, 37, 150, 22), "Host Port:");
            GUI.TextField(new Rect(170, 37, 160, 22), networkManager.HostEndPoint.Port.ToString(), "Label");
        }

        // Disconnect Button
        if (GUI.Button(new Rect(10, 81, 110, 30), "Disconnect"))
        {
            networkManager.StopHost();
            isHost = false;
        }

        if (!NobleServer.active) isHost = false;
    }


    void GUIClient()
    {
        if (!networkManager.isNetworkActive)
        { 
      
            
        }
        else if (networkManager.client != null)
        {
            // Disconnect button
            GUI.Label(new Rect(10, 10, 150, 22), "Connection type: " + networkManager.client.latestConnectionType);
            if (GUI.Button(new Rect(10, 50, 110, 30), "Disconnect"))
            {
                if (networkManager.client.isConnected)
                {
                    // If we are already connected it is best to quit gracefully by sending
                    // a disconnect message to the host.
                    networkManager.client.Disconnect();
                }
                else
                {
                    // If the connection is still in progress StopClient will cancel it
                    networkManager.StopClient();
                }
                isClient = false;
            }
        }
    }


    private void OnGUI()
    {
        if (!isHost && !isClient)
        {
         
        }
        else

        {
            // Host or client GUI
            if (isHost) GUIHost();
            else if (isClient) GUIClient();
        }
          
    }
    
    public void buttonLogin()
    {
        SceneManager.LoadScene("Scene_Login_&_Register");
        menuPanel.SetActive(false);

    }

   public void buttonLeaderboard()
    {
        SceneManager.LoadScene("Leader&Sent");
        menuPanel.SetActive(false);
    }






    // Start is called before the first frame update
    void Start()
    {
       networkManager = (NobleNetworkManager)NetworkManager.singleton;
        menuPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Scene_MainMenu")
        {
            menuPanel.SetActive(true);
            if (myPlayfabManager.isLoggedIn())
            {
                loginButton.SetActive(false);
                leaderBoardButton.SetActive(true);
                logoutButton.SetActive(true);
            }
            else
            {
                leaderBoardButton.SetActive(false);
                logoutButton.SetActive(false);
                loginButton.SetActive(true);
            }
        }
        else
        {
            menuPanel.SetActive(false);
        }


    }
}
