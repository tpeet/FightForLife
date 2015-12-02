using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    public int NumberOfInitialMacrophages = 5;

    // how much bacterias can it destroy
    public int InitialMacrophageHealth = 100;

    public GameObject Macrophage;

    void Start () {
        var worldTerrain = FindObjectOfType<Terrain>();
        var terrainWidth = worldTerrain.terrainData.size.x;
        var terrainLength = worldTerrain.terrainData.size.z;
        for (int i = 0; i < NumberOfInitialMacrophages; i++)
        {
            var startX = (terrainWidth / NumberOfInitialMacrophages) * i;
            var endX = (terrainWidth / NumberOfInitialMacrophages) * (i + 1);
            var allowedAreaSizeX = (endX - startX) / 2;
            var allowedAreaBeggingX = worldTerrain.GetPosition().x + startX + allowedAreaSizeX / 2;
            var allowedAreaEndingX = worldTerrain.GetPosition().x + startX + 3 * allowedAreaSizeX / 2;
            var positionX = Random.Range(allowedAreaBeggingX, allowedAreaEndingX);

            var allowedAreaBegginingY = worldTerrain.GetPosition().z +(terrainLength / 12);
            var allowedAreaEndingY = worldTerrain.GetPosition().z + (terrainLength / 4);
            var positionZ = Random.Range(allowedAreaBegginingY, allowedAreaEndingY);
            var macrophage = Instantiate(Macrophage, new Vector3(positionX, 0.05f, positionZ), Quaternion.identity) as GameObject;
            if (macrophage != null)
            {
                macrophage.name = "Macrophage " + (i + 1);
            }

        }
    }

	void Update () {
	
	}
}
