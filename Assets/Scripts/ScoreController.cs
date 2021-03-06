﻿//using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public int maxHealth = 1000;
    public int maxResources = 100;

    private int _currentOverallHealth;

    public int CurrentOverallHealth
    {
        get { return _currentOverallHealth; }
        set
        {
            _currentOverallHealth = value;
            HandleHealth();
        }
    }

    private int _currentResources;
    
    public int CurrentResources
    {
        get { return _currentResources; }
        set
        {
            _currentResources = value;
            HandleResources();
        }
    }


    public RectTransform HealthTransform;
    private float HealthCachedY;
    private float HealthMinX;
    private float HealthMaxX;
    public Text HealthText;
    public Image VisualHealth;



    public RectTransform ResourcesTransform;
    private float ResourcesCachedY;
    private float ResourcesMinX;
    private float ResourcesMaxX;
    public Text ResourcesText;
    public Image VisualResources;

    // Sweating
    public int SweatHealthBonus = 20;
    public int SweatCost = 30;
    private Button _sweatButton;

    // Repair variables
    private Button _repairButton;
    public int RepairBonus = 20;
    public int RepairCost = 30;

    // Neutrophil variables
    private Button _neutrophilButton;
    public int NeutrophilCost = 30;

    // Helper T variables
    private Button _healerButton;
    public int HealerCost = 10;

    // Macrophage variables
    private Button _macrophageButton;
    public int MacrophageCost = 10;


    public GameObject Macrophage;
    public GameObject Neutrophil;
    public GameObject Healer;

    public static  int BacteriasKilledThisLevel;
    private bool _hasWon = false;

    void Start ()
    {
        BacteriasKilledThisLevel = 0;
        // Buttons init
        _sweatButton = GameObject.Find("SweatButton").GetComponent<Button>();
        _repairButton = GameObject.Find("RepairButton").GetComponent<Button>();
        _healerButton = GameObject.Find("HelperTButton").GetComponent<Button>();
        _neutrophilButton = GameObject.Find("NeutrophilButton").GetComponent<Button>();
        _macrophageButton = GameObject.Find("MacrophageButton").GetComponent<Button>();

        //Healthbar init
        HealthCachedY = HealthTransform.position.y;
        HealthMaxX = HealthTransform.position.x;
        HealthMinX = HealthTransform.position.x - HealthTransform.rect.width;
        CurrentOverallHealth = maxHealth;

        //ResourceBar init
        ResourcesCachedY = ResourcesTransform.position.y;
        ResourcesMaxX = ResourcesTransform.position.x;
        ResourcesMinX = ResourcesTransform.position.x -ResourcesTransform.rect.width;
        CurrentResources = maxResources;

        OnCoolDown = false;
    }

    public void Repair()
    {
        CurrentResources -= RepairCost;
        GameObject.Find("WallContainer").GetComponent<WallController>().CurrentHealth += 30;
    }

    public void CreateHealer()
    {
        CreateCharacter(Healer, "HealerContainer");
        CurrentResources -= HealerCost;
    }

    public void CreateNeutrophil()
    {
        CreateCharacter(Neutrophil, "NeutrophilContainer");
        CurrentResources -= NeutrophilCost;
    }

    public void CreateMacrophage()
    {
        CreateCharacter(Macrophage, "MacrophageContainer");
        CurrentResources -= MacrophageCost;
    }

    public void CreateCharacter(GameObject character, string name)
    {
        var worldTerrain = FindObjectsOfType<Terrain>().FirstOrDefault(x => x.name == "Terrain"); ;
        var terrainWidth = worldTerrain.terrainData.size.x;
        var terrainLength = worldTerrain.terrainData.size.z;
        var allowedAreaBeggingX = worldTerrain.GetPosition().x + terrainWidth / 6;
        var allowedAreaEndingX = worldTerrain.GetPosition().x + 5 * terrainWidth / 6;
        var allowedAreaBegginingY = worldTerrain.GetPosition().z + (terrainLength / 12);
        var allowedAreaEndingY = worldTerrain.GetPosition().z + (terrainLength / 4);
        var positionX = Random.Range(allowedAreaBeggingX, allowedAreaEndingX);
        var positionZ = Random.Range(allowedAreaBegginingY, allowedAreaEndingY);
        var positionVector = new Vector3(positionX, 0.5f, positionZ); ;
        var otherCharacters = Common.GetAllPlayerCharacters();

        // if Macrophage position is too close to other player objects, then generate new positions
        while (otherCharacters.Any(x => Vector3.Distance(x.transform.position, positionVector) < 5))
        {
            positionX = Random.Range(allowedAreaBeggingX, allowedAreaEndingX);
            positionZ = Random.Range(allowedAreaBegginingY, allowedAreaEndingY);
            positionVector = new Vector3(positionX, 0.5f, positionZ);
        }

        var newCharacter = Instantiate(character, positionVector, Quaternion.identity) as GameObject;
        if (newCharacter != null)
            newCharacter.name = name;
    }
	
	// Update is called once per frame
	void Update () {
	    if (!OnCoolDown && CurrentOverallHealth > 0)
	    {
	        StartCoroutine(CoolDownDamage());
	        CurrentOverallHealth--;
            if (CurrentResources < maxResources)
	            CurrentResources++;
	    }
	}



    public float CoolDown;
    private bool OnCoolDown;
    IEnumerator CoolDownDamage()
    {
        OnCoolDown = true;
        yield return new WaitForSeconds(CoolDown);
        OnCoolDown = false;
    }


    public static void GameWon()
    {
        PlayerPrefs.SetInt("KilledBacteriasThisLevel", BacteriasKilledThisLevel);
        PlayerPrefs.SetInt("DidWin", 1);
        Application.LoadLevel("gameOver");
    }

    public static void GameLost()
    {
        PlayerPrefs.SetInt("KilledBacteriasThisLevel", BacteriasKilledThisLevel);
        PlayerPrefs.SetInt("DidWin", 0);
        Application.LoadLevel("gameOver");
    }

    #region HealthBar movement
    private void HandleHealth()
    {
        HealthText.text = "Health: " + CurrentOverallHealth;
        var currentX = Common.MapValues(CurrentOverallHealth, 0, maxHealth, HealthMinX, HealthMaxX);
        HealthTransform.position = new Vector3(currentX, HealthCachedY);

        // more than 50% health
        if (CurrentOverallHealth > maxHealth/2)
        {
            var redChannelValue = Common.MapValues(CurrentOverallHealth, maxHealth/2.0f, maxHealth, 255, 0);
            VisualHealth.color = new Color32((byte) redChannelValue, 255, 0, 255);
        }

        //less than 50% health
        else
        {
            var greenChannelValue = Common.MapValues(CurrentOverallHealth, 0, maxHealth/2.0f, 0, 255);
            VisualHealth.color = new Color32(255, (byte) greenChannelValue, 0, 255);
        }

        if (CurrentOverallHealth <= 0 && !_hasWon)
        {
            GameLost();
            _hasWon = true;
        }
    }



    #endregion






    #region ResourceBar movement
    private void HandleResources()
    {
        ResourcesText.text = "Resources: " + CurrentResources;
        var currentX = Common.MapValues(CurrentResources, 0, maxResources, ResourcesMaxX, ResourcesMinX) + ResourcesTransform.rect.width;
        ResourcesTransform.position = new Vector3(currentX, ResourcesCachedY);

        // more than 50% health
        if (CurrentResources > maxResources / 2)
        {
            var redChannelValue = Common.MapValues(CurrentResources, maxResources / 2.0f, maxResources, 255, 0);
            VisualResources.color = new Color32((byte)redChannelValue, 255, 0, 255);
        }

        //less than 50% health
        else
        {
            var greenChannelValue = Common.MapValues(CurrentResources, 0, maxResources / 2.0f, 0, 255);
            VisualResources.color = new Color32(255, (byte)greenChannelValue, 0, 255);
        }

        // enable or disable the buttons depending if body has enough resources
        _healerButton.interactable = CurrentResources > HealerCost;
        _neutrophilButton.interactable = CurrentResources > NeutrophilCost;
        _repairButton.interactable = CurrentResources > RepairCost;
        _macrophageButton.interactable = CurrentResources > MacrophageCost;
        _sweatButton.interactable = CurrentResources > SweatCost;

    }

    #endregion



    public void Sweat()
    {
        CurrentOverallHealth += SweatHealthBonus;
        CurrentOverallHealth = Mathf.Clamp(CurrentOverallHealth, 0, 100);
        CurrentResources -= SweatCost;
    }

}
