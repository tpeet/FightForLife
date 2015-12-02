using UnityEngine;
using System.Collections;

public class Common : MonoBehaviour
{

    public static int BacteriaGenerationTime = 1;


    public static bool ShiftKeyDown()
    {
        return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
    }



}
