using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Common : MonoBehaviour
{

    public static int BacteriaGenerationTime = 1;

    public static bool ShiftKeyDown()
    {
        return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }

    public struct Boundary
    {
        public float xMin, xMax, yMin, yMax;
    }

    public static float GetBacteriaGroupRadius()
    {
        var terrain = FindObjectsOfType<Terrain>().FirstOrDefault(x => x.name == "Terrain");
        var terrainSize = terrain.terrainData.size;
        var numberOfInitialBacterias = GameObject.Find("GameController").GetComponent<InfectionController>().NumberOfInitialBacterias;
        return terrainSize.x / (numberOfInitialBacterias * 4);
    }

    public static void DestroyCharacter(GameObject character)
    {
        MouseController.CurrentlySelectedUnits.Remove(character);
        MouseController.UnitsOnScreen.Remove(character);
        MouseController.UnitsInDrag.Remove(character);
        Destroy(character);
    }


    public static List<GameObject> GetAllPlayerCharacters()
    {
        var allCharacters = GameObject.FindGameObjectsWithTag("Macrophage").ToList();
        allCharacters.AddRange(GameObject.FindGameObjectsWithTag("Neutrophil"));
        allCharacters.AddRange(GameObject.FindGameObjectsWithTag("Healer"));
        return allCharacters;
    }

    public static float MapValues(float x, float inMin, float inMax, float outMin, float outMax)
    {
        return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
