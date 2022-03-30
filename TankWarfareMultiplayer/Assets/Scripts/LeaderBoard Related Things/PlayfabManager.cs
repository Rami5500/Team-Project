using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayfabManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Text messageText;
    public InputField userNameInput;
    public InputField emailInput;
    public InputField passwordInput;

    public GameObject userNameText;

    public GameObject loginButton;
    public GameObject registerButton;
    public GameObject resetPasswordButton;

    public GameObject switchLoginButton;
    public GameObject switchRegisterButton;


    public GameObject logNregWindow;
    public GameObject leaderboardWindow;

    public Button test;

    public GameObject rowPrefab;
    public Transform rowsParent;
    void Start()
    {
        //Login();
    }


    //Register an Account
    public void RegisterButton()
    {
        if (passwordInput.text.Length < 6)
        {
            messageText.text = "Password too short";
            return;
        }
        var request = new RegisterPlayFabUserRequest
        {
            DisplayName = userNameInput.text,
            Email = emailInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    //Move to the leaderboard scene once registered
    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        //SendLeaderBoard(5);
        messageText.text = "Registered and logged in!";
  
        SceneManager.LoadScene("Leader&Sent");
    }

    //Login into an account if already has one
    public void LoginButton()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text, // or instead for only user name -- Username = userNameInput.text 
            Password = passwordInput.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnError);
    }


    //Move to leaderboard scene once login in
    void OnLoginSuccess(LoginResult result)
    {
        messageText.text = "Logged in!";
        Debug.Log("Successful login");
        string name = null;
        if (result.InfoResultPayload.PlayerProfile != null)
        {
            name = result.InfoResultPayload.PlayerProfile.DisplayName;
        }

        if(name == null)
        {
            logNregWindow.SetActive(true);
        }
        else
        {
            //leaderboardWindow.SetActive(true);
            SceneManager.LoadScene("Leader&Sent");
        }
    }


    //reset user password when button is pressed
    public void ResetPasswordButton()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = emailInput.text,
            TitleId = "19EE9"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }

    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        messageText.text = "Password reset mail sent!";
    }


    //displays the leaderboard
    void OnLeaderBoardGet(GetLeaderboardResult result)
    {
        foreach (Transform item in rowsParent)
        {
            Destroy(item.gameObject);
        }

        foreach(var item in result.Leaderboard)
        {
            GameObject newGO = Instantiate(rowPrefab, rowsParent);
            Text[] texts = newGO.GetComponentsInChildren<Text>();
            texts[0].text = item.Position.ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();
            Debug.Log(string.Format(("PLACE: {0} | ID: {1} | VALUE: {2}"), item.Position, item.PlayFabId, item.StatValue));
        }
    }

    public void UsernameUpdate()
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = userNameInput.text
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Updated display name!");
    }



    //use to test sending data to the leaderboard
    public void LeaderBoardTest()
    {
        SendLeaderBoard(50);
        Debug.Log("MAYBE");
    }
    

    void OnSuccess(LoginResult result)
    {
        Debug.Log("Successful login/account create!");
    }
   

    public void SendLeaderBoard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "TankWarfare LeaderBoard",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderBoardUpdate, OnError);
    }

    void OnLeaderBoardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successful LeaderBoard Sent");
    }


    //Gets the leaderboard data from the database
    public void GetLeaderBoard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "TankWarfare LeaderBoard",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderBoardGet, OnError);
    }

  
    

    void OnError(PlayFabError error)
    {
        messageText.text = error.ErrorMessage;
        Debug.Log(error.GenerateErrorReport());
    }

    //go back to main menu
    public void goToMainMenu()
    {
        SceneManager.LoadScene("Scene_MainMenu");
    }

    //checks if user is logged in.
    public bool isLoggedIn()
    {
        return PlayFabClientAPI.IsClientLoggedIn();
    }


    public void Logout()
    {
        PlayFabClientAPI.ForgetAllCredentials();
    }

    public string getName()
    {
     
        return "test";
    }



    //change to register menu
    public void showRegister()
    {
        userNameText.SetActive(true);
        loginButton.SetActive(false);
        registerButton.SetActive(true);
        resetPasswordButton.SetActive(false);
        switchLoginButton.SetActive(true);
        switchRegisterButton.SetActive(false);
  

    }
    //change to login menu
    public void showLogin()
    {
        userNameText.SetActive(false);
        loginButton.SetActive(true);
        registerButton.SetActive(false);
        resetPasswordButton.SetActive(true);
        switchLoginButton.SetActive(false);
        switchRegisterButton.SetActive(true);

    }

    
}





