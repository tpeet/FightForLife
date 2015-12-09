using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour {


    void Start()
    {
        if (PlayerPrefs.GetInt("DidWin") == 1)
        {
            DisplayPanel("WinPanel");
            PlayerPrefs.SetInt("KilledBacterias", PlayerPrefs.GetInt("KilledBacterias") + PlayerPrefs.GetInt("KilledBacteriasThisLevel"));

        }
        else
        {
            DisplayPanel("LosePanel");
        }
            
    }

    public void QuitToMainMenu()
    {
        Application.LoadLevel("menu");
    }

    public void LoadNextLevel()
    {
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
        Application.LoadLevel("level");
    }

    public void TryAgain()
    {
        Application.LoadLevel("level");
    }





    public static void DisplayPanel(string panel)
    {
        var gameOverCanvas = GameObject.Find("GameOverCanvas").GetComponent<CanvasGroup>();
        var panelCanvas = gameOverCanvas.transform.FindChild(panel).GetComponent<CanvasGroup>();
        panelCanvas.transform.FindChild("Level").GetComponent<Text>().text = "Level: " + PlayerPrefs.GetInt("Level");
        panelCanvas.transform.FindChild("BacteriasKilledLevel").GetComponent<Text>().text = "Bacterias killed this level: " + PlayerPrefs.GetInt("KilledBacteriasThisLevel");
        //if (PlayerPrefs.GetInt("DidWin") == 1)
        //    panelCanvas.transform.FindChild("AllBacteriasKilled").GetComponent<Text>().text = "All bacterias killed: " + (PlayerPrefs.GetInt("KilledBacterias") + PlayerPrefs.GetInt("KilledBacteriasThisLevel"));
        //else
            panelCanvas.transform.FindChild("AllBacteriasKilled").GetComponent<Text>().text = "All bacterias killed: " + (PlayerPrefs.GetInt("KilledBacterias") + PlayerPrefs.GetInt("KilledBacteriasThisLevel"));
        
        Common.EnableCanvasGroup(gameOverCanvas, 1);
        Common.EnableCanvasGroup(panelCanvas, 1);
    }
}
