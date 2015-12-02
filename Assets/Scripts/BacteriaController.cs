using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;
using Pathfinding;

public class BacteriaController : MonoBehaviour
{


    public bool IsParent = false;
    public int BacteriaGenerationTime;
    public int MaxNumberOfBacterias;
    
    public GameObject ParentBacteria;

    public Vector3 Target = Vector3.zero;

    
    private GameObject _bacteria;

    // pathfinding variables
    public float Speed = 100;
    public float NextWayPointDistance = 1;
    public Path Path;
    private Seeker _seeker;
    private int _currentWaypoint;

    void Start()
    {
        _bacteria = gameObject;
        _seeker = GetComponent<Seeker>();

        if (IsParent)
        {
            StartCoroutine(GenerateBacteria());

            var targetX = Random.Range(transform.position.x - 5, transform.position.x + 5);
            var targetZ = GameObject.FindGameObjectWithTag("Finish").transform.position.z - 10;
            Target = new Vector3(targetX, 0.5f, targetZ);
            
        }
        else
        {
            var parentBacteriaController = ParentBacteria.GetComponent<BacteriaController>();
            //var parentLocation = parentBacteriaController.transform.position;
            var locationDifference = parentBacteriaController.transform.position - transform.position;
            Target = parentBacteriaController.Target - locationDifference;

            

        }
        _seeker.StartPath(transform.position, Target, OnPathComplete);
    }

    void Update()
    {
        if (Path == null || _currentWaypoint >= Path.vectorPath.Count)
            return;
        var rb = GetComponent<Rigidbody>();
        var dir = (Path.vectorPath[_currentWaypoint] - transform.position).normalized;
        dir *= Speed * Time.deltaTime;
        rb.velocity = dir;
        var distance = Vector3.Distance(transform.position, Path.vectorPath[_currentWaypoint]);
        if (distance < NextWayPointDistance)
        {
            _currentWaypoint++;
        }
    }


    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            Path = p;
            _currentWaypoint = 0;
        }
    }

    private IEnumerator GenerateBacteria()
    {
        for (var i=0; i < MaxNumberOfBacterias; i++)
        {


            yield return new WaitForSeconds(BacteriaGenerationTime);
            var numberOfInitialBacterias =
                GameObject.Find("GameController").GetComponent<InfectionController>().NumberOfInitialBacterias;

            var terrain = FindObjectOfType<Terrain>();
            var terrainSize = terrain.terrainData.size;
            var terrainPosition = terrain.GetPosition();

            var minLimit = new Vector2(terrainPosition.x, terrainPosition.z);
            var maxLimit = new Vector2(terrainPosition.x + terrainSize.x, terrainPosition.z + terrainSize.z);

            var position = new Vector2(Mathf.Infinity, Mathf.Infinity);

            var bacteriaSize = gameObject.GetComponentInChildren<Renderer>().bounds.size;

            while (position.x + bacteriaSize.x < minLimit.x || position.x - bacteriaSize.x > maxLimit.x ||
                   position.y + bacteriaSize.z < minLimit.y || position.y - bacteriaSize.z > maxLimit.y)
            {
                var randVector = Random.insideUnitCircle;
                position.x = randVector.x*(terrainSize.x/(numberOfInitialBacterias*4)) + transform.position.x;
                position.y = randVector.y*(terrainSize.z/(numberOfInitialBacterias*4)) + transform.position.z;
            }
            var childBacteria =
                Instantiate(_bacteria, new Vector3(position.x, 1f, position.y), Quaternion.identity) as GameObject;
            if (childBacteria != null)
            {
                var childBacteriaController = childBacteria.GetComponent<BacteriaController>();
                childBacteriaController.IsParent = false;
                childBacteriaController.ParentBacteria = gameObject;
                childBacteria.name = name + "(child)";
            }
                
        }
    }
}
