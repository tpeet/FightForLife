using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

public class MouseController : MonoBehaviour {
	
	RaycastHit hit;

    //public static GameObject CurrentlySelectedUnit;

    public static List<GameObject> CurrentlySelectedUnits = new List<GameObject>();

	private Vector3 mouseDownPoint;

	public GameObject target;



	void Awake() {
		mouseDownPoint = Vector3.zero;
	}

	void Update () {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {

			if (Input.GetMouseButtonDown(0))
			    mouseDownPoint = hit.point;

		    var hitGameObject = hit.collider.gameObject;

            // hitting terrain
			if (hit.collider.name == "Terrain") 
			{
				target.transform.position = hit.point;

				//right mousebutton
				if(Input.GetMouseButtonDown(1))
				{
				    var targetObj = Instantiate(target, mouseDownPoint, Quaternion.identity) as GameObject;
				    if (targetObj != null) targetObj.name = "Target (Instantiated)";
				}

                // left mousebutton
				else if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint) && !ShiftKeyDown())
					DeselectGameobjectsIfSelected();

			}
			else
			{
                // hitting other objects than terrain
			    if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
			    {
                    // is the user hitting a unit?
			        if (hit.collider.transform.FindChild("Selected"))
			        {
                        Debug.Log(("Found a unit!"));

                        // are we selecting a different object?
                        if (!UnitAlreadyInCurrentlySelectedUnits(hitGameObject))
                        {
                            //if shift not down, remove the rest of the units
                            if (!ShiftKeyDown())
                                DeselectGameobjectsIfSelected();

                            hit.collider.transform.FindChild("Selected").gameObject.SetActive(true);

                            // add unit to currently selected list
                            CurrentlySelectedUnits.Add(hitGameObject);                            

                        }
                        else
                        {
                            RemoveUnitFromCurrentlySelectedUnits(hitGameObject);
                        }
			        }
			        else
                        if (!ShiftKeyDown())
                            DeselectGameobjectsIfSelected();
			    }
					
			}
		}
		else
		{
		    if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint) && !ShiftKeyDown())
                DeselectGameobjectsIfSelected();
		}

		Debug.DrawRay(ray.origin, ray.direction * 500, Color.yellow);
	}









	#region Helper functions

	public bool DidUserClickLeftMouse(Vector3 hitPoint)
	{
		var clickZone = 0.8f;
		return (
			(mouseDownPoint.x < hitPoint.x + clickZone && mouseDownPoint.x > hitPoint.x - clickZone) &&
			(mouseDownPoint.y < hitPoint.y + clickZone && mouseDownPoint.y > hitPoint.y - clickZone) &&
			(mouseDownPoint.z < hitPoint.z + clickZone && mouseDownPoint.z > hitPoint.z - clickZone));
	}



	public static void DeselectGameobjectsIfSelected()
	{
		if (CurrentlySelectedUnits.Any())
		{
		    foreach (var arrayListUnit in CurrentlySelectedUnits)
		        arrayListUnit.transform.FindChild("Selected").gameObject.SetActive(false);
		    
		    CurrentlySelectedUnits.Clear();
		}
	}


    public static bool UnitAlreadyInCurrentlySelectedUnits(GameObject unit)
    {
       return CurrentlySelectedUnits.Any(currentlySelectedUnit => currentlySelectedUnit == unit);
    }


    public void RemoveUnitFromCurrentlySelectedUnits(GameObject unit)
    {
        if (CurrentlySelectedUnits.Any())
        {
            for (var i = 0; i < CurrentlySelectedUnits.Count; i++)
            {
                var currentlySelectedUnit = CurrentlySelectedUnits[i];
                if (currentlySelectedUnit == unit)
                {
                    CurrentlySelectedUnits.RemoveAt(i); 
                    currentlySelectedUnit.transform.FindChild("Selected").gameObject.SetActive(false);
                }
                    
            }
        }

    }


    public static bool ShiftKeyDown()
    {
        return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }
    

	#endregion
}
