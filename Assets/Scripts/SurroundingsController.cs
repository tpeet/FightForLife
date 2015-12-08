using UnityEngine;
using System.Collections;

public class SurroundingsController : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        Common.DestroyCharacter(gameObject);
    }
}
