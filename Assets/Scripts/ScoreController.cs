using System;
using UnityEngine;
using System.Collections;
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
    public int SweatTimeout = 30;
    private Button _sweatButton;

    // Repair variables
    private Button _repairButton;
    public int RepairBonus = 20;
    public int RepairResources = 30;

    // Neutrophil variables
    private Button _neutrophilButton;
    public int NeutrophilCost = 30;

    // Helper T variables
    private Button _healerButton;
    public int HealerCost = 10;



    void Start ()
    {
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

        // Buttons init
        _sweatButton = GameObject.Find("SweatButton").GetComponent<Button>();
        _repairButton = GameObject.Find("RepairButton").GetComponent<Button>();
        _healerButton = GameObject.Find("HelperTButton").GetComponent<Button>();
        _neutrophilButton = GameObject.Find("NeutrophilButton").GetComponent<Button>();

        OnCoolDown = false;
    }

    public void Repair()
    {
        CurrentResources -= 5;
    }

    public void CreateHealer()
    {
        
    }

    public void CreateNeutrophil()
    {
        
    }
	
	// Update is called once per frame
	void Update () {
	    if (!OnCoolDown && CurrentOverallHealth > 0)
	    {
	        StartCoroutine(CoolDownDamage());
	        CurrentOverallHealth -= 1;
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



    #region HealthBar movement
    private void HandleHealth()
    {
        HealthText.text = "Health: " + CurrentOverallHealth;
        var currentX = MapHealthBarValues(CurrentOverallHealth, 0, maxHealth, HealthMinX, HealthMaxX);
        HealthTransform.position = new Vector3(currentX, HealthCachedY);

        // more than 50% health
        if (CurrentOverallHealth > maxHealth/2)
        {
            var redChannelValue = MapHealthBarValues(CurrentOverallHealth, maxHealth/2, maxHealth, 255, 0);
            VisualHealth.color = new Color32((byte) redChannelValue, 255, 0, 255);
        }

        //less than 50% health
        else
        {
            var greenChannelValue = MapHealthBarValues(CurrentOverallHealth, 0, maxHealth/2, 0, 255);
            VisualHealth.color = new Color32(255, (byte) greenChannelValue, 0, 255);
        }
    }

    private static float MapHealthBarValues(float x, float inMin, float inMax, float outMin, float outMax)
    {
        return (x - inMin)*(outMax - outMin)/(inMax - inMin) + outMin;
    }

    #endregion






    #region ResourceBar movement
    private void HandleResources()
    {
        ResourcesText.text = "Resources: " + CurrentResources;
        var currentX = MapHealthBarValues(CurrentResources, 0, maxResources, ResourcesMaxX, ResourcesMinX) + ResourcesTransform.rect.width;
        ResourcesTransform.position = new Vector3(currentX, ResourcesCachedY);

        // more than 50% health
        if (CurrentResources > maxResources / 2)
        {
            var redChannelValue = MapHealthBarValues(CurrentResources, maxResources / 2, maxResources, 255, 0);
            VisualResources.color = new Color32((byte)redChannelValue, 255, 0, 255);
        }

        //less than 50% health
        else
        {
            var greenChannelValue = MapHealthBarValues(CurrentResources, 0, maxResources / 2, 0, 255);
            VisualResources.color = new Color32(255, (byte)greenChannelValue, 0, 255);
        }
    }

    #endregion







    #region Sweating


    public void Sweat()
    {
        CurrentOverallHealth += SweatHealthBonus;
        CurrentOverallHealth = Mathf.Clamp(CurrentOverallHealth, 0, 100);
        StartCoroutine(SweatWait());

    }

    IEnumerator SweatWait()
    {
        _sweatButton.interactable = false;
        yield return new WaitForSeconds(SweatTimeout);
        _sweatButton.interactable = true;
    }


    #endregion

}
