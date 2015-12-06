using UnityEngine;
using System.Collections;

public class HealerController : MonoBehaviour
{
    public int MaxHealth = 200;
    public int CurrentHealth;
	// Use this for initialization
	void Start ()
	{
	    CurrentHealth = MaxHealth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
