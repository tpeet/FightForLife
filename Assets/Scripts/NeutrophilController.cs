using UnityEngine;
using System.Collections;
using System.Linq;

public class NeutrophilController : MonoBehaviour
{

    public float ExplosionDistance = 5;
    public int DamageAmount = 5;

    public bool DontExplode = false;
	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        
        if (DontExplode || other.gameObject.CompareTag("CharacterChildren") || other.gameObject == gameObject) return;
       
        // if other object is player character or NPC
        if (other.gameObject.layer == 8 || other.gameObject.layer == 12)
        {
            Explode();
        }
            
    }

    private void Explode()
    {
        var allCharacters = Common.GetAllPlayerCharacters();
        allCharacters.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        allCharacters.Add(GameObject.Find("WallContainer"));
        var charactersInRange =
            allCharacters.Where(x => Vector3.Distance(transform.position, x.transform.position) < ExplosionDistance).ToList();

        // remove itself from selected objects
        charactersInRange.Remove(gameObject);

        foreach (var character in charactersInRange)
            ReduceHealth(character);
        Common.DestroyCharacter(gameObject);
        GameObject.Find("GameController").GetComponent<ScoreController>().CurrentOverallHealth -= DamageAmount;
    }


    private void ReduceHealth(GameObject character)
    {
        // reduce Macrophage health
        var macrophageController = character.GetComponent<MacrophageController>();
        if (macrophageController != null)
            macrophageController.CurrentHealth -= DamageAmount;

        // reduce Healer health
        var healerController = character.GetComponent<HealerController>();
        if (healerController != null)
            healerController.CurrentHealth -= DamageAmount;

        // reduce Bacteria health or destroy them
        var bacteriaController = character.GetComponent<BacteriaController>();
        if (bacteriaController != null)
        {
            if (bacteriaController.IsParent)
                bacteriaController.CurrentHealth -= DamageAmount;
            else
            {
                Destroy(bacteriaController.gameObject);
                ScoreController.BacteriasKilledThisLevel++;
            }
        }

        // reduce The Wall health, if too close to it
        var wallController = character.GetComponent<WallController>();
        if (wallController != null)
            wallController.CurrentHealth -= DamageAmount;

    }
}
