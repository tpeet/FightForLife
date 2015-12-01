using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public int maxHealth = 100;

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


    public RectTransform HealthTransform;
    private float CachedY;
    private float MinX;
    private float MaxX;
    public Text HealthText;
    public Image VisualHealth;

    //
    public float CoolDown;
    private bool OnCoolDown;

    void Start ()
    {
        CachedY = HealthTransform.position.y;
        MaxX = HealthTransform.position.x;
        MinX = HealthTransform.position.x - HealthTransform.rect.width;
        CurrentOverallHealth = maxHealth;
        OnCoolDown = false;
        _sweatButton = GameObject.Find("SweatButton").GetComponent<Button>();
    }
	
	// Update is called once per frame
	void Update () {
	    if (!OnCoolDown && CurrentOverallHealth > 0)
	    {
	        StartCoroutine(CoolDownDamage());
	        CurrentOverallHealth -= 1;
	    }
	}

    IEnumerator CoolDownDamage()
    {
        OnCoolDown = true;
        yield return new WaitForSeconds(CoolDown);
        OnCoolDown = false;
    }

    private void HandleHealth()
    {
        HealthText.text = "Health: " + CurrentOverallHealth;
        var currentX = MapHealthBarValues(CurrentOverallHealth, 0, maxHealth, MinX, MaxX);
        HealthTransform.position = new Vector3(currentX, CachedY);

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


    // Sweating
    public int SweatHealthBonus = 20;
    public int SweatTimeout = 30;
    private Button _sweatButton;

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




}
