using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

public class MouseController : MonoBehaviour
{

    #region Variables
    RaycastHit hit;

    // variables for selecting units
    public static List<GameObject> CurrentlySelectedUnits = new List<GameObject>();
	private Vector3 mouseDownPoint;
    public float ClickDragZone = 5f;

    // variables for target assignment
	public GameObject target;


    // variables for dragging
    public float TimeLimitBeforeDeclareDrag = 0.5f;
    public static bool UserIsDragging;
    private static float TimeLeftBeforeDeclareDrag;
    private static Vector2 MouseDragStart;
    private static Vector3 MouseUpPoint;
    private static Vector3 CurrentMousePoint;

    public GUIStyle MouseDragSkin;

    #endregion
    void Awake() {
		mouseDownPoint = Vector3.zero;
	}

	void Update () {
		var ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
            var hitGameObject = hit.collider.gameObject;
		    CurrentMousePoint = hit.point;


            // Mouse dragging
		    if (Input.GetMouseButtonDown(0))
		    {
                mouseDownPoint = hit.point;
		        TimeLeftBeforeDeclareDrag = TimeLimitBeforeDeclareDrag;
		        MouseDragStart = Input.mousePosition;

		    }
		    else if (Input.GetMouseButton(0))
		    {
		        if (!UserIsDragging)
		        {
		            TimeLeftBeforeDeclareDrag -= Time.deltaTime;
                    if (TimeLeftBeforeDeclareDrag <= 0f || IsUserDraggingByPosition(MouseDragStart, Input.mousePosition))
		                UserIsDragging = true;
		        }
                else
                    Debug.Log("User is dragging");
		    }
            else if (Input.GetMouseButtonUp(0))
            {
                TimeLeftBeforeDeclareDrag = 0f;
                UserIsDragging = false;
            }
			    

            // mouse click
		    if (UserIsDragging) return; 
			if (hit.collider.name == "Terrain") 
			{
				target.transform.position = hit.point;

				//right mousebutton
				if(Input.GetMouseButtonDown(1))
				{
				    var targetObj = Instantiate(target, hit.point, Quaternion.identity) as GameObject;
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

                        //if selecting an object which is already selected
                        else
                        {
                            if (ShiftKeyDown())
                                RemoveUnitFromCurrentlySelectedUnits(hitGameObject);
                            else
                            {
                                DeselectGameobjectsIfSelected();
                                hit.collider.transform.FindChild("Selected").gameObject.SetActive(true);
                                CurrentlySelectedUnits.Add(hitGameObject);
                            }
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

	} // end of Update()


    void OnGUI()
    {
        if (UserIsDragging)
        {
            var dragAreaWidth = Camera.main.WorldToScreenPoint(mouseDownPoint).x -
                           Camera.main.WorldToScreenPoint(CurrentMousePoint).x;
            var dragAreaHeight = Camera.main.WorldToScreenPoint(mouseDownPoint).y -
                           Camera.main.WorldToScreenPoint(CurrentMousePoint).y;
            var dragAreaLeft = Input.mousePosition.x;
            var dragAreaRight = (Screen.height - Input.mousePosition.y) - dragAreaHeight;
            GUI.Box(new Rect(dragAreaLeft, dragAreaRight, dragAreaWidth, dragAreaHeight), "", MouseDragSkin);
        }
        
    }







	#region Helper functions

    public bool IsUserDraggingByPosition(Vector2 dragStartPoint, Vector2 newPoint)
    {
        return ((newPoint.x > dragStartPoint.x + ClickDragZone || newPoint.x < dragStartPoint.x - ClickDragZone) ||
            (newPoint.y > dragStartPoint.y + ClickDragZone || newPoint.y < dragStartPoint.y - ClickDragZone));
    }



	public bool DidUserClickLeftMouse(Vector3 hitPoint)
	{
	    Debug.Log(String.Format("{0} - {1}", mouseDownPoint.x, hitPoint.x));
		return (
			(mouseDownPoint.x < hitPoint.x + ClickDragZone && mouseDownPoint.x > hitPoint.x - ClickDragZone) &&
			(mouseDownPoint.y < hitPoint.y + ClickDragZone && mouseDownPoint.y > hitPoint.y - ClickDragZone) &&
			(mouseDownPoint.z < hitPoint.z + ClickDragZone && mouseDownPoint.z > hitPoint.z - ClickDragZone));
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
