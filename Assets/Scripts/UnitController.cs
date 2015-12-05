using UnityEngine;
using System.Collections;
using Pathfinding;


// Used for showing if unit is selected
public class UnitController : MonoBehaviour
{
    public Vector2 ScreenPos;
    private bool OnScreen;
    public bool Selected = false;
    public bool IsWalkable = true;



    private Seeker Seeker;
    private CharacterController Controller;
    public Path Path;
    public float Speed = 500;
    private UnitController Unit;

    // the max distance from the AI to a waypoint for it to continue to the next waypoint
    public float NextWayPointDistance = 1;

    // Current Waypoint
    private int CurrentWaypoint = 0;

    private BacteriaController _bacteriaController;
    public BacteriaController BacteriaToAttack
    {
        get { return _bacteriaController; }
        set
        {
            _bacteriaController = value;
            if (value != null)
                StartCoroutine(SeekPath());
            else
                StopCoroutine(SeekPath());
        }
    }

    


    void Start()
    {
        Seeker = GetComponent<Seeker>();
        Controller = GetComponent<CharacterController>();
        Unit = GetComponent<UnitController>();
    }



    public void LateUpdate()
    {

        if (Unit.Selected && Unit.IsWalkable)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Seeker.StartPath(transform.position, MouseController.RightClickPoint, OnPathComplete);
            }
        }

        

        // if our unit is currently attacking some bacteria
        if (BacteriaToAttack != null && Seeker.IsDone())
        {
            Seeker.StartPath(transform.position, BacteriaToAttack.transform.position, OnPathComplete);
        }
    }

    void Update () {
	    // indicates wheter unit is on the screen or not
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


        // moves unit towards the target
        if (Path == null || CurrentWaypoint >= Path.vectorPath.Count || !Unit.IsWalkable)
            return;

        var dir = (Path.vectorPath[CurrentWaypoint] - transform.position).normalized;
        dir *= Speed * Time.deltaTime;
        Controller.SimpleMove(dir);
        var distance = Vector3.Distance(transform.position, Path.vectorPath[CurrentWaypoint]);
        if (distance < NextWayPointDistance)
        {
            CurrentWaypoint++;
        }
    }

    IEnumerator SeekPath()
    {
        if (Seeker.IsDone())
            Seeker.StartPath(transform.position, BacteriaToAttack.transform.position, OnPathComplete);
        yield return new WaitForSeconds(1f);
    }









    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            Path = p;
            CurrentWaypoint = 0;
        }

        
    }
}