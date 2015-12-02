using UnityEngine;
using System.Collections;

public class EnemyTargetController : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        // if other object is enemy
        if (other.gameObject.layer == 12)
        {
            
            GameObject.Find("GameController").gameObject.GetComponent<ScoreController>().CurrentOverallHealth--;
            Destroy(other.gameObject);
        }
        
    }
}
