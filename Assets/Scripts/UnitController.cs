using UnityEngine;
using System.Collections;

public class UnitController : MonoBehaviour
{
    public Vector2 ScreenPos;
    private bool OnScreen;
    public bool Selected = false;

	
	void Update () {
	    // if unit not selected, get screen space
	    if (!Selected)
	    {
	        // track screen position
	        ScreenPos = Camera.main.WorldToScreenPoint(this.transform.position);

	        if (MouseController.UnitWithinScreenSpace(ScreenPos))
	        {
	            if (!OnScreen)
	            {
	                MouseController.UnitsOnScreen.Add(this.gameObject);
	                OnScreen = true;
	            }
	            else
	            {
	                if (OnScreen)
	                {
	                    MouseController.UnitsOnScreen.Remove(this.gameObject);
	                    OnScreen = false;
	                }
	            }
	        }
	    }
	}
}
