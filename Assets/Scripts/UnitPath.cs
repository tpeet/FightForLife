using UnityEngine;
using System.Collections;
using Pathfinding;



public class UnitPath : MonoBehaviour {



    private Seeker Seeker;
    private CharacterController Controller;
    public Path Path;
    public float Speed = 500;
    private UnitController Unit;

    // the max distance from the AI to a waypoint for it to continue to the next waypoint
    public float NextWayPointDistance = 1;

    // Current Waypoint
    private int CurrentWaypoint = 0;

    // Use this for initialization
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
    }










    // Update is called once per frame
    public void Update()
    {

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

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            Path = p;
            CurrentWaypoint = 0;
        }
    }
}
