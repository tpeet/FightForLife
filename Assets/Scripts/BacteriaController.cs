using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Random = UnityEngine.Random;
using Pathfinding;

public class BacteriaController : MonoBehaviour
{


    public bool IsParent = false;
    public float BacteriaGenerationTime;
    public int MaxNumberOfBacterias;
    public int MaxHealthOfParent = 20;
    public int CurrentHealth = 20;
    
    public GameObject ParentBacteria;

    public Vector3 Target = Vector3.zero;

    private GameObject _bacteria;

    // pathfinding variables
    public float Speed = 100;
    public float NextWayPointDistance = 1;
    public Path Path;
    private Seeker _seeker;
    private int _currentWaypoint;
    private Terrain _terrain;



    private int _numberOfInitialBacterias;
    private Vector3 _terrainSize;

    void Start()
    {
        _bacteria = gameObject;
        _seeker = GetComponent<Seeker>();

        _terrain = FindObjectsOfType<Terrain>().FirstOrDefault(x => x.name == "Terrain");
        _terrainSize = _terrain.terrainData.size;
        _numberOfInitialBacterias = GameObject.Find("GameController").GetComponent<InfectionController>().NumberOfInitialBacterias;
        if (PlayerPrefs.GetInt("Difficulty") > 0)
            BacteriaGenerationTime = BacteriaGenerationTime / PlayerPrefs.GetInt("Difficulty");
        if (IsParent)
        {
            StartCoroutine(GenerateBacteria());

            var targetX = Random.Range(transform.position.x - 5, transform.position.x + 5);
            var targetZ = GameObject.FindGameObjectWithTag("Finish").transform.position.z - 25;
            Target = new Vector3(targetX, 0.5f, targetZ);
            _seeker.StartPath(transform.position, Target, OnPathComplete);
        }
        else
        {
            var parentBacteriaController = ParentBacteria.GetComponent<BacteriaController>();
            //var parentLocation = parentBacteriaController.transform.position;
            var locationDifference = parentBacteriaController.transform.position - transform.position;
            Target = parentBacteriaController.Target - locationDifference;
            //Target.z = Mathf.Clamp(Target.x, _terrain.GetPosition().z + 5,
            //    _terrain.GetPosition().z + _terrain.terrainData.size.z);
            Destroy(transform.GetChild(0).FindChild("BacteriaProjector").gameObject);
            _seeker.StartPath(transform.position, Target, OnPathComplete);


        }
       
    }

    void Update()
    {
        if (CurrentHealth < 0)
        {
            GameObject.Find("GameController")
                .GetComponent<InfectionController>()
                .ParentBacterias.Remove(this);
            Destroy(gameObject);
            ScoreController.BacteriasKilledThisLevel++;
            foreach (var child in GameObject.FindGameObjectsWithTag("Enemy").Where(x => x.GetComponent<BacteriaController>().ParentBacteria == gameObject))
            {
                Destroy(child);
                ScoreController.BacteriasKilledThisLevel++;
            }
            return;
        }
            
        if (Path == null || _currentWaypoint >= Path.vectorPath.Count )
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


        var terrainPosition = _terrain.GetPosition();

        var minLimit = new Vector2(terrainPosition.x, terrainPosition.z);
        var maxLimit = new Vector2(terrainPosition.x + _terrainSize.x, terrainPosition.z + _terrainSize.z);
        for (var i=0; i < MaxNumberOfBacterias; i++)
        {
            yield return new WaitForSeconds(BacteriaGenerationTime);

            var position = new Vector2(Mathf.Infinity, Mathf.Infinity);
            var bacteriaRenderer = gameObject.GetComponentInChildren<Renderer>();
            if (bacteriaRenderer == null)
                break;
            var bacteriaSize = bacteriaRenderer.bounds.size;

            while (position.x + bacteriaSize.x < minLimit.x || position.x - bacteriaSize.x > maxLimit.x ||
                   position.y + bacteriaSize.z < minLimit.y || position.y - bacteriaSize.z > maxLimit.y)
            {
                var randVector = Random.insideUnitCircle;
                position.x = randVector.x*(_terrainSize.x/(_numberOfInitialBacterias*4)) + transform.position.x;
                position.y = randVector.y*(_terrainSize.z/(_numberOfInitialBacterias*4)) + transform.position.z;
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
