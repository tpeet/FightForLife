using UnityEngine;
using System.Collections;

public class DestroyParentGameObject : MonoBehaviour {
	void DestroyParentObject()
	{
		Destroy (this.gameObject.transform.parent.gameObject);
	}
}
