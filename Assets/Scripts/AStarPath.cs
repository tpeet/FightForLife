using UnityEngine;
using System.Collections;
using Pathfinding;

public class AStarPath : MonoBehaviour
{

    public Vector3 TargetPosition;
    private Seeker Seeker;
    private CharacterController Controller;
    public Path Path;
    public float Speed;

    // the max distance from the AI to a waypoint for it to continue to the next waypoint
    public float NextWayPointDistance = 10;

    // Current Waypoint
    private int CurrentWaypoint = 0;

	// Use this for initialization
	void Start ()
	{
	    TargetPosition = GameObject.Find("Target").transform.position;
	    Seeker = GetComponent<Seeker>();
	    Controller = GetComponent<CharacterController>();

	    Seeker.StartPath(transform.position, TargetPosition, OnPathComplete);
	}

    // Update is called once per frame
    public void FixedUpdate()
    {
        if (Path == null || CurrentWaypoint >= Path.vectorPath.Count)
            return;


        var dir = (Path.vectorPath[CurrentWaypoint] - transform.position).normalized;

        dir *= Speed*Time.fixedDeltaTime;

        Debug.Log(dir);
        Controller.SimpleMove(dir);

        if (Vector3.Distance(transform.position, Path.vectorPath[CurrentWaypoint]) > NextWayPointDistance)
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
