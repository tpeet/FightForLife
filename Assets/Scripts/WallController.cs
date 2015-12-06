using UnityEngine;
using System.Collections;

public class WallController : MonoBehaviour
{
    public int MaxHealth = 100;

    private int _currentHealth = 100;

    public int CurrentHealth
    {
        get
        {
            return _currentHealth;
        }

        set
        {
            if (_currentHealth <= 0)
                Application.LoadLevel("menu");
            Debug.Log(_currentHealth);
            _currentHealth = value;
            if (_currentHealth > MaxHealth)
                _currentHealth = MaxHealth;
        }
    }



    void Start()
    {
        CurrentHealth = MaxHealth;
    }


    void OnTriggerEnter(Collider other)
    {
        // if other object is enemy
        if (other.gameObject.layer == 12)
        {
            //GameObject.Find("GameController").gameObject.GetComponent<ScoreController>().CurrentOverallHealth--;
            var bacteriaController = other.gameObject.GetComponent<BacteriaController>();
            if (bacteriaController != null && bacteriaController.IsParent)
            {
                CurrentHealth -= bacteriaController.CurrentHealth;
                GameObject.Find("GameController")
                    .GetComponent<InfectionController>()
                    .ParentBacterias.Remove(bacteriaController);
            }
                
            else { }             
                CurrentHealth--;
            Destroy(other.gameObject);
        }

    }

}
