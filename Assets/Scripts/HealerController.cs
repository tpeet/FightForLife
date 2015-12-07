using UnityEngine;
using System.Collections;
using System.Linq;

public class HealerController : MonoBehaviour
{
    public int MaxHealth = 200;

    private int _currentHealth;
    public int CurrentHealth
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            _currentHealth = value;
            HandleHealth();
        }
    }

    public int HealingDistance = 10;
    public float HealingTimeout = 1;
    public float BacteriaDamageDistance = 2;


    private bool _canHeal = true;
    private bool _canTakeDamage = true;


	void Start ()
	{
	    CurrentHealth = MaxHealth;
	}
	
	// Update is called once per frame
	void Update ()
	{
        // if there are bacterias near healer reduce health
	    if (_canTakeDamage && GameObject.FindGameObjectsWithTag("Enemy")
	            .Any(x => Vector3.Distance(transform.position, x.transform.position) < BacteriaDamageDistance))
	    {
            CurrentHealth--;
	        StartCoroutine(TakeDamage());
	    }
	        

	    if (_canHeal)
	    {
            var unitController = GetComponent<UnitController>();
            MacrophageController macrophageController = null;
            if (unitController != null)
                macrophageController = unitController.MacrophageToHeal;
	        if (macrophageController != null &&
	            Vector3.Distance(transform.position, macrophageController.transform.position) < HealingDistance)
	        {
                StartCoroutine(Heal());
	            macrophageController.CurrentHealth++;
	            CurrentHealth--;
	        }
                

        }
	    
	}

    IEnumerator TakeDamage()
    {
        _canTakeDamage = false;
        yield return new WaitForSeconds(HealingTimeout);
        _canHeal = true;
    }

    IEnumerator Heal()
    {
        _canHeal = false;
        yield return new WaitForSeconds(HealingTimeout);
        _canHeal = true;
    }


    public void HandleHealth()
    {
        if (CurrentHealth < 0)
            Destroy(gameObject);
    }
}
