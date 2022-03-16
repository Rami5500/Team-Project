using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class victoryMenu : MonoBehaviour
{

    public Text win;
    public Text lose;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void goToMainMenu()
    {
        SceneManager.LoadScene("Scene_MainMenu");
    }

}
