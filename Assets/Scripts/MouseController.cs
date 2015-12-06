using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


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
    private static Vector3 CurrentMousePoint;

    public GUIStyle MouseDragSkin;

    private float BoxWidth;
    private float BoxHeight;
    private float BoxTop;
    private float BoxLeft;
    private Vector2 BoxStart;
    private Vector2 BoxFinish;

    public static List<GameObject> UnitsOnScreen = new List<GameObject>();
    public static List<GameObject> UnitsInDrag = new List<GameObject>();
    private bool FinishedDragOnThisFrame;
    private bool StartedDrag;


    // pathfinding
    public static Vector3 RightClickPoint;

    #endregion


    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            var hitGameObject = hit.collider.gameObject;
            CurrentMousePoint = hit.point;


            // Mouse dragging
            if (Input.GetMouseButtonDown(0))
            {
                mouseDownPoint = hit.point;
                TimeLeftBeforeDeclareDrag = TimeLimitBeforeDeclareDrag;
                MouseDragStart = Input.mousePosition;
                StartedDrag = true;
            }
            else if (Input.GetMouseButton(0))
            {
                if (!UserIsDragging)
                {
                    TimeLeftBeforeDeclareDrag -= Time.deltaTime;
                    if (TimeLeftBeforeDeclareDrag <= 0f || IsUserDraggingByPosition(MouseDragStart, Input.mousePosition))
                    {
                        UserIsDragging = true;
                    }

                }

            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (UserIsDragging)
                {
                    FinishedDragOnThisFrame = true;
                }

                UserIsDragging = false;

            }


            var isOnlyHealerSelected = CurrentlySelectedUnits.Count == 1 && CurrentlySelectedUnits.First().CompareTag("Healer");
            if (!UserIsDragging)
            {



                if (hit.collider.CompareTag("Ground"))
                {
                    target.transform.position = hit.point;

                    //right mousebutton
                    if (Input.GetMouseButtonDown(1))
                    {

                        // enemy highlight
                        var selectedBacteriaController = GameObject.Find("GameController")
                            .GetComponent<InfectionController>()
                            .ParentBacterias.FirstOrDefault(x => IsPositionInsideArea(CurrentMousePoint, x.GetBacteriaGroupBoundary()));

                      

                        // if right clicked on a group of bacteria to attack them
                        if (selectedBacteriaController != null && !isOnlyHealerSelected)
                        {
                            foreach (var unit in CurrentlySelectedUnits)
                            {
                                if (!unit.CompareTag("Healer"))
                                {
                                    var controller = unit.GetComponent<UnitController>();
                                    controller.BacteriaToAttack = selectedBacteriaController;
                                }
                            }
                            Debug.Log("Highlight: " + selectedBacteriaController.name);
                        }

                        // if click was somewhere else on the terrain
                        else
                        {
                            // reset attack mode
                            var allCharacters = Common.GetAllPlayerCharacters();
                            foreach (var character in allCharacters.Where(x => CurrentlySelectedUnits.Contains(x)))
                            {
                                character.GetComponent<UnitController>().BacteriaToAttack = null;
                                character.GetComponent<UnitController>().MacrophageToHeal = null;
                            }
                                

                            var targetObj = Instantiate(target, hit.point, Quaternion.identity) as GameObject;
                            if (targetObj != null)
                                targetObj.name = "Target (Instantiated)";
                        }

                        // used by characters to seek for a target
                        RightClickPoint = hit.point;

                    }

                    // left mousebutton
                    else if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint) && !Common.ShiftKeyDown())
                        DeselectGameobjectsIfSelected();

                }
                else
                {
                    // hitting other objects than terrain
                    if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint))
                    {
                        // is the user hitting a unit?
                        if (hit.collider.gameObject.GetComponent<UnitController>())
                        {
                            Debug.Log(("Found a unit!"));

                            // are we selecting a different object?
                            if (!CurrentlySelectedUnits.Contains(hitGameObject))
                            {
                                //if shift not down, remove the rest of the units
                                if (!Common.ShiftKeyDown())
                                    DeselectGameobjectsIfSelected();

                                // add unit to currently selected list
                                hit.collider.transform.FindChild("Selected").gameObject.SetActive(true);
                                CurrentlySelectedUnits.Add(hitGameObject);

                                hit.collider.gameObject.GetComponent<UnitController>().Selected = true;

                            }

                            //if selecting an object which is already selected
                            else
                            {
                                if (Common.ShiftKeyDown())
                                    RemoveUnitFromCurrentlySelectedUnits(hitGameObject);
                                else
                                {
                                    DeselectGameobjectsIfSelected();
                                    hit.collider.transform.FindChild("Selected").gameObject.SetActive(true);
                                    CurrentlySelectedUnits.Add(hitGameObject);
                                    hit.collider.gameObject.GetComponent<UnitController>().Selected = true;
                                }
                            }
                        }
                        else if (!Common.ShiftKeyDown())
                            DeselectGameobjectsIfSelected();
                    }


                    if (Input.GetMouseButtonUp(0) && isOnlyHealerSelected && hitGameObject.CompareTag("Macrophage"))
                    {
                        CurrentlySelectedUnits.First().GetComponent<UnitController>().MacrophageToHeal =
                            hitGameObject.GetComponent<MacrophageController>();
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0) && DidUserClickLeftMouse(mouseDownPoint) && !Common.ShiftKeyDown())
                    DeselectGameobjectsIfSelected();
            }

        }

        if (!Common.ShiftKeyDown() && StartedDrag && UserIsDragging)
        {
            DeselectGameobjectsIfSelected();
            StartedDrag = false;
        }




        if (UserIsDragging)
        {
            var mousePosition = Input.mousePosition;
            BoxWidth = Camera.main.WorldToScreenPoint(mouseDownPoint).x - Camera.main.WorldToScreenPoint(CurrentMousePoint).x;
            BoxHeight = Camera.main.WorldToScreenPoint(mouseDownPoint).y - Camera.main.WorldToScreenPoint(CurrentMousePoint).y;
            BoxLeft = mousePosition.x;
            BoxTop = (Screen.height - mousePosition.y) - BoxHeight;
            //Debug.Log(String.Format("BoxWidth: {0}; BoxHeight: {1}", BoxWidth, BoxHeight));
            // mouse is at the top left corner
            if (BoxWidth > 0 && BoxHeight < 0)
                BoxStart = new Vector2(mousePosition.x, mousePosition.y);

            // mouse is at the bottom left corner
            else if (BoxWidth > 0 && BoxHeight > 0)
                BoxStart = new Vector2(mousePosition.x, mousePosition.y + BoxHeight);

            // mouse is at the top right corner
            else if (BoxWidth < 0 && BoxHeight < 0)
                BoxStart = new Vector2(mousePosition.x + BoxWidth, mousePosition.y);

            // mouse is at the bottom right corner
            else if (BoxWidth < 0 && BoxHeight > 0)
                BoxStart = new Vector2(mousePosition.x + BoxWidth, mousePosition.y + BoxHeight);

            BoxFinish = new Vector2(BoxStart.x + Math.Abs(BoxWidth), BoxStart.y - Math.Abs(BoxHeight));
        }
    } // end of Update()



    void LateUpdate()
    {
        UnitsInDrag.Clear();

        if ((UserIsDragging || FinishedDragOnThisFrame) && UnitsOnScreen.Any())
        {
            foreach (var unitObj in UnitsOnScreen)
            {
                var unitScript = unitObj.GetComponent<UnitController>();
                var selectedObj = unitObj.transform.FindChild("Selected").gameObject;

                if (!UnitsInDrag.Contains(unitObj))
                {
                    if (IsUnitInsideDrag(unitScript.ScreenPos))
                    {
                        selectedObj.SetActive(true);
                        UnitsInDrag.Add(unitObj);
                        unitScript.Selected = true;
                        //Debug.Log("Unit is inside Drag area - " + selectedObj.transform.parent.name);
                    }
                    else
                    {
                        selectedObj.SetActive(false);
                        unitScript.Selected = false;

                    }
                }
            }
        }

        if (FinishedDragOnThisFrame)
        {
            FinishedDragOnThisFrame = false;
            PutDraggedUnitsInCurrentlySelectedUnits();
        }
    }






    void OnGUI()
    {
        if (UserIsDragging)
            GUI.Box(new Rect(BoxLeft, BoxTop, BoxWidth, BoxHeight), "", MouseDragSkin);
    }







    #region Helper functions

    public bool IsUserDraggingByPosition(Vector2 dragStartPoint, Vector2 newPoint)
    {
        return ((newPoint.x > dragStartPoint.x + ClickDragZone || newPoint.x < dragStartPoint.x - ClickDragZone) ||
            (newPoint.y > dragStartPoint.y + ClickDragZone || newPoint.y < dragStartPoint.y - ClickDragZone));
    }



    public bool DidUserClickLeftMouse(Vector3 hitPoint)
    {

        var value =
            (mouseDownPoint.x < hitPoint.x + ClickDragZone && mouseDownPoint.x > hitPoint.x - ClickDragZone) &&
            (mouseDownPoint.y < hitPoint.y + ClickDragZone && mouseDownPoint.y > hitPoint.y - ClickDragZone) &&
            (mouseDownPoint.z < hitPoint.z + ClickDragZone && mouseDownPoint.z > hitPoint.z - ClickDragZone);
        //Debug.Log(String.Format("DidUserClickedLeftMouse - {0}", value));
        return value;
    }



    public static void DeselectGameobjectsIfSelected()
    {
        if (CurrentlySelectedUnits.Any())
        {
            foreach (var listUnit in CurrentlySelectedUnits)
            {
                listUnit.transform.FindChild("Selected").gameObject.SetActive(false);
                listUnit.GetComponent<UnitController>().Selected = false;
            }


            CurrentlySelectedUnits.Clear();
        }
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
                    currentlySelectedUnit.GetComponent<UnitController>().Selected = false;
                }

            }
        }

    }


    public static bool IsPositionInsideArea(Vector3 pos, Common.Boundary boundary)
    {
        var value = (pos.x > boundary.xMin && pos.x < boundary.xMax && pos.z < boundary.yMax && pos.z >boundary.yMin);
        return value;
    }


    public static bool UnitWithinScreenSpace(Vector2 unitScreenPos)
    {
        return unitScreenPos.x < Screen.width && unitScreenPos.y < Screen.height && unitScreenPos.x > 0 &&
               unitScreenPos.y > 0;
    }



    public bool IsUnitInsideDrag(Vector2 unitScreenPos)
    {
        var value = unitScreenPos.x > BoxStart.x && unitScreenPos.y < BoxStart.y && unitScreenPos.x < BoxFinish.x && unitScreenPos.y > BoxFinish.y;
        //Debug.Log(String.Format("{6} -> BoxStart ({0},{1}); BoxFinish ({2},{3}) unitScreenPos ({4},{5})", BoxStart.x, BoxStart.y, BoxFinish.x, BoxFinish.y, unitScreenPos.x, unitScreenPos.y, value));

        return value;
    }


    //take all units from UnitsInDrag, into currentlySelectedUnits
    public static void PutDraggedUnitsInCurrentlySelectedUnits()
    {
        var unitsInDragNotCurrentlySelected = UnitsInDrag.Where(x => !CurrentlySelectedUnits.Contains(x)).ToList();
        foreach (var o in unitsInDragNotCurrentlySelected)
        {
            o.GetComponent<UnitController>().Selected = true;
            o.transform.FindChild("Selected").gameObject.SetActive(true);
        }
            
        CurrentlySelectedUnits.AddRange(unitsInDragNotCurrentlySelected);
    }

    #endregion
}
