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
        var currentX = MapHealthToPosition(CurrentOverallHealth, 0, maxHealth, MinX, MaxX);
        HealthTransform.position = new Vector3(currentX, CachedY);

        // more than 50% health
        if (CurrentOverallHealth > maxHealth/2)
        {
            var redChannelValue = MapHealthToPosition(CurrentOverallHealth, maxHealth/2, maxHealth, 255, 0);
            VisualHealth.color = new Color32((byte) redChannelValue, 255, 0, 255);
        }

        //less than 50% health
        else
        {
            var greenChannelValue = MapHealthToPosition(CurrentOverallHealth, 0, maxHealth/2, 0, 255);
            VisualHealth.color = new Color32(255, (byte) greenChannelValue, 0, 255);
        }
    }

    private float MapHealthToPosition(float x, float inMin, float inMax, float outMin, float outMax)
    {
        return (x - inMin)*(outMax - outMin)/(inMax - inMin) + outMin;
    }
}
