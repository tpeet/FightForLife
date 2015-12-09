using UnityEngine;
using System.Collections;
using Pathfinding;


// Used for showing if unit is selected
public class UnitController : MonoBehaviour
{
    public Vector2 ScreenPos;
    private bool _onScreen;
    public bool Selected = false;
    public bool IsWalkable = true;

    private GameObject _target;

    public GameObject Target
    {
        get { return _target; }
        set
        {
            Destroy(_target);
            _target = value;
            if (value != null)
            {
                _aiPath.target = value.transform;
                _aiPath.SearchPath();
            }
        }
    }


    private Seeker _seeker;
    public Path Path;
    public float Speed = 500;
    private UnitController _unit;

    // the max distance from the AI to a waypoint for it to continue to the next waypoint
    public float NextWayPointDistance = 1;

    // Current Waypoint
    private int _currentWaypoint = 0;

    private BacteriaController _bacteriaController;
    public BacteriaController BacteriaToAttack
    {
        get { return _bacteriaController; }
        set
        {
            _bacteriaController = value;
            if (value != null)
            {
                _aiPath.target = value.transform;
                _aiPath.SearchPath();
            }
            
        }
    }

    private AIPath _aiPath;

    void Start()
    {
        _seeker = GetComponent<Seeker>();
        _unit = GetComponent<UnitController>();
        _aiPath = GetComponent<AIPath>();
    }



    public void LateUpdate()
    {

        // if our unit is currently attacking some bacteria
        if (BacteriaToAttack != null && _seeker.IsDone())
        {
            //_aiPath.target = BacteriaToAttack.transform;
            //_aiPath.SearchPath();
            ;
        }


        else if (_unit.Selected && _unit.IsWalkable)
        {
            if (Input.GetMouseButtonDown(1))
            {
                _seeker.StartPath(transform.position, MouseController.RightClickPoint, OnPathComplete);
            }
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
	            if (!_onScreen)
	            {
	                MouseController.UnitsOnScreen.Add(this.gameObject);
	                _onScreen = true;
	            }
	            else
	            {
	                if (_onScreen)
	                {
	                    MouseController.UnitsOnScreen.Remove(this.gameObject);
	                    _onScreen = false;
	                }
	            }
	        }
	    }


        // moves unit towards the target
        //if (Path == null || Path.vectorPath == null || _currentWaypoint >= Path.vectorPath.Count || !_unit.IsWalkable)
        //    return;

        //var dir = (Path.vectorPath[_currentWaypoint] - transform.position).normalized;
        //dir *= Speed * Time.deltaTime;
        //_controller.SimpleMove(dir);
        //var distance = Vector3.Distance(transform.position, Path.vectorPath[_currentWaypoint]);
        //if (distance < NextWayPointDistance)
        //{
        //    _currentWaypoint++;
        //}
    }


    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            Path = p;
            _currentWaypoint = 0;
        }

        
    }
}