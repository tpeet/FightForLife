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
    public Material Selector;

    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    // Update is called once per frame
    void Update()
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
            // selects macrophage which is: 
            //  * attacking the same bacteria as the healer,
            //  * with the distance close enough,
            //  * with the lowest health
            var macrophage = GameObject.FindGameObjectsWithTag("Macrophage").
                Where(x => /*x.GetComponent<UnitController>().BacteriaToAttack == unitController.BacteriaToAttack &&*/ Vector3.Distance(x.transform.position, transform.position) < HealingDistance).
                OrderBy(x => x.GetComponent<MacrophageController>().CurrentHealth).FirstOrDefault();
            //BacteriaController macrophageController = null;
            //if (unitController != null)
            //    macrophageController = unitController.MacrophageToHeal;
            if (macrophage != null)
            {
                StartCoroutine(Heal());
                macrophage.GetComponent<MacrophageController>().CurrentHealth++;
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
            Common.DestroyCharacter(gameObject);


        var child = transform.GetChild(0);
        if (child.childCount > 0 && child.GetChild(0).gameObject.activeSelf)
        {
            // more than 50% health
            if (CurrentHealth > MaxHealth / 2)
            {
                var redChannelValue = Common.MapValues(CurrentHealth, MaxHealth / 2.0f, MaxHealth, 255, 0);

                var projector = child.GetComponentInChildren<Projector>();
                var newMaterial = new Material(Selector)
                {
                    color = new Color32((byte)redChannelValue, 255, 0, 255)
                };
                projector.material = newMaterial;
            }

            //less than 50% health
            else
            {
                var greenChannelValue = Common.MapValues(CurrentHealth, 0, MaxHealth / 2.0f, 0, 255);
                var projector = child.GetComponentInChildren<Projector>();
                var newMaterial = new Material(Selector)
                {
                    color = new Color32(255, (byte)greenChannelValue, 0, 255)
                };
                projector.material = newMaterial;
            }
        }
    }
}
