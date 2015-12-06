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
}
