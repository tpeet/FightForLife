using UnityEngine;
using System;
using System.Collections;
using UnityEditor.AnimatedValues;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    private CanvasGroup _confirmQuitCanvasGroup;
    private CanvasGroup _uiCanvasGroup;

    void Awake()
    {
        PlayerPrefs.SetFloat("Difficulty", FindObjectOfType<Slider>().value);
        PlayerPrefs.SetInt("KilledBacterias", 0);
        _confirmQuitCanvasGroup = GameObject.Find("QuitPanel").GetComponent<CanvasGroup>();
        _uiCanvasGroup = GameObject.Find("MainMenu").GetComponent<CanvasGroup>();
    }

    public void PlayGame()
    {
        PlayerPrefs.SetInt("Level", 1);
        Application.LoadLevel("level");
    }

    public void ChangeDifficulty(float difficulty)
    {
        PlayerPrefs.SetFloat("Difficulty", difficulty);
        Debug.Log("Difficulty value: "+difficulty);
    }

    public void ExitGame()
    {
        _confirmQuitCanvasGroup.alpha = 1;
        _confirmQuitCanvasGroup.interactable = true;
        _confirmQuitCanvasGroup.blocksRaycasts = true;

        _uiCanvasGroup.alpha = 0.5f;
        _uiCanvasGroup.interactable = false;
        _uiCanvasGroup.blocksRaycasts = false;
    }

    public void QuitYes()
    {
        Application.Quit();
    }

    public void QuitNo()
    {
        _confirmQuitCanvasGroup.alpha = 0;
        _confirmQuitCanvasGroup.interactable = false;
        _confirmQuitCanvasGroup.blocksRaycasts = false;

        _uiCanvasGroup.alpha = 1;
        _uiCanvasGroup.interactable = true;
        _uiCanvasGroup.blocksRaycasts = true;
    }
}
