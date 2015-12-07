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



    private Seeker _seeker;
    private CharacterController _controller;
    public Path Path;
    public float Speed = 500;
    private UnitController _unit;

    // the max distance from the AI to a waypoint for it to continue to the next waypoint
    public float NextWayPointDistance = 1;

    // Current Waypoint
    private int _currentWaypoint = 0;

    public BacteriaController BacteriaToAttack { get; set; }
    public MacrophageController MacrophageToHeal { get; set; }
    private bool _checkPathAgain = true;
    private AIPath _aiPath;

    void Start()
    {
        _seeker = GetComponent<Seeker>();
        _controller = GetComponent<CharacterController>();
        _unit = GetComponent<UnitController>();
        _aiPath = GetComponent<AIPath>();
    }



    public void LateUpdate()
    {

        // if our unit is currently attacking some bacteria
        if (BacteriaToAttack != null && _seeker.IsDone())
        {
            _aiPath.target = BacteriaToAttack.transform;
            _aiPath.SearchPath();
            //_seeker.StartPath(transform.position, BacteriaToAttack.transform.position, OnPathComplete);
            //StartCoroutine(SeekPath());
        }


        else if (MacrophageToHeal != null && _seeker.IsDone())
        {
            _aiPath.target = MacrophageToHeal.transform;
            _aiPath.SearchPath();
            //_seeker.StartPath(transform.position, MacrophageToHeal.transform.position, OnPathComplete);
            //StartCoroutine(SeekPath());

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
        if (Path == null || Path.vectorPath == null || _currentWaypoint >= Path.vectorPath.Count || !_unit.IsWalkable || MacrophageToHeal != null)
            return;

        var dir = (Path.vectorPath[_currentWaypoint] - transform.position).normalized;
        dir *= Speed * Time.deltaTime;
        _controller.SimpleMove(dir);
        var distance = Vector3.Distance(transform.position, Path.vectorPath[_currentWaypoint]);
        if (distance < NextWayPointDistance)
        {
            _currentWaypoint++;
        }
    }


    IEnumerator SeekPath()
    {
        _checkPathAgain = false;
        yield return new WaitForSeconds(0.1f);
        _checkPathAgain = true;
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