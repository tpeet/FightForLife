using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Instantiates some number of Bacterias, assigns them to random places (inside certain area)
public class InfectionController : MonoBehaviour
{

    public GameObject Bacteria;
    public Terrain WorldTerrain;
    public int NumberOfInitialBacterias;

    public List<BacteriaController> ParentBacterias;

	// Use this for initialization
	void Start ()
	{
	    var terrainWidth = WorldTerrain.terrainData.size.x;
	    var terrainLength = WorldTerrain.terrainData.size.z;
	    for (int i = 0; i < NumberOfInitialBacterias; i++)
	    {
	        var startX = (terrainWidth/NumberOfInitialBacterias)*i;
	        var endX = (terrainWidth/NumberOfInitialBacterias)*(i + 1);
	        var allowedAreaSizeX = (endX - startX)/2;
	        var allowedAreaBeggingX = WorldTerrain.GetPosition().x + startX + allowedAreaSizeX/2;
            var allowedAreaEndingX = WorldTerrain.GetPosition().x + startX + 3*allowedAreaSizeX/2;
            var positionX = Random.Range(allowedAreaBeggingX, allowedAreaEndingX);

	        var allowedAreaBegginingY = WorldTerrain.GetPosition().z + (2*terrainLength/3) + (terrainLength/12);
	        var allowedAreaEndingY = WorldTerrain.GetPosition().z + (2*terrainLength/3) + (terrainLength / 4);
	        var positionZ = Random.Range(allowedAreaBegginingY, allowedAreaEndingY);
            var bacteria = Instantiate(Bacteria, new Vector3(positionX, 0.05f, positionZ), Quaternion.identity) as GameObject;
	        if (bacteria != null)
	        {
                bacteria.GetComponent<BacteriaController>().IsParent = true;
                bacteria.name = "Bacteria" + (i + 1);
                ParentBacterias.Add(bacteria.GetComponent<BacteriaController>());
            }

	    }
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
