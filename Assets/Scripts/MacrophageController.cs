using UnityEngine;
using System.Collections;
using System.Linq;

public class MacrophageController : MonoBehaviour
{

    public int MaxHealth = 100;

    public float AttackDistance = 10;

    // how much time between two killings
    public float KillTimeOut = 0.9f;

    private int _currentHealth = 100;

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

    private bool _canKill = true;
    public Material Selector;

    void Start()
    {
        CurrentHealth = MaxHealth;
    }


    // Update is called once per frame
    void Update()
    {
        var bacteriaToAttack = GetComponent<UnitController>().BacteriaToAttack;
        // if Macrophage is close enough, start killing bacteria
        if (bacteriaToAttack != null && Vector3.Distance(transform.position, bacteriaToAttack.transform.position) <
            AttackDistance && _canKill)
        {
            // find all enemies who are in the bacteria group being attacked and who are not parents. Then choose the first one
            var enemies = GameObject.FindGameObjectsWithTag("Enemy")
                .Where(x =>
                    x.GetComponent<BacteriaController>() != null && !x.GetComponent<BacteriaController>().IsParent && x.GetComponent<BacteriaController>().ParentBacteria == bacteriaToAttack.gameObject);
            //enemies =
            //    enemies.Where(x => x.GetComponent<BacteriaController>().ParentBacteria == bacteriaToAttack.gameObject);
            var enemy = enemies.FirstOrDefault();

            // if there are children bacterias
            if (enemy != null)
            {
                Destroy(enemy);
                ScoreController.BacteriasKilledThisLevel++;
                CurrentHealth--;
            }
            // if we are fighting with the parent bacteria
            else
                bacteriaToAttack.CurrentHealth--;

            CurrentHealth--;
            StartCoroutine(KillingTimeout());
        }
    }


    private void HandleHealth()
    {
        if (_currentHealth < 0)
        {
            Common.DestroyCharacter(gameObject);            
        }

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
                    color = new Color32(255, (byte) greenChannelValue, 0, 255)
                };
                projector.material = newMaterial;
            }
        }

        Debug.Log("Macrophage health: " + CurrentHealth);
            
    }


    IEnumerator KillingTimeout()
    {
        _canKill = false;
        yield return new WaitForSeconds(KillTimeOut);
        _canKill = true;
    }
}
