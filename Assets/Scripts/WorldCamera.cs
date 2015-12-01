﻿using System;
using UnityEngine;
using System.Collections;

/*
 * Controls the camera within the game world
 * Code by Unitychat (https://www.youtube.com/watch?v=aqwxc_jD3Uw)
 */
public class WorldCamera : MonoBehaviour {

    [Serializable]
	public struct BoxLimit 
	{
		public float LeftLimit;
		public float RightLimit;
		public float TopLimit;
		public float BottomLimit;
	}

    

	public static BoxLimit mouseScrollLimits;
	public static WorldCamera Instance;

	private float _cameraMoveSpeed = 60f;
	private float _shiftBonus = 45f;


    public float zoomSpeed = 100;

    public float MinFOV = 3;
    public float MaxFOV = 50;

	public float MouseBoundary = 25f;
    public BoxLimit cameraLimits;

    public bool MoveByMouse;

    public Terrain WorldTerrain;
    public float WorldTerrainPadding = 100f;

    void Awake() {
		Instance = this;
	}

	void Start () {
        // How far can the camera move?
        //cameraLimits.LeftLimit = -120f;
        //cameraLimits.RightLimit = -35f;
        //cameraLimits.TopLimit = -22f;
        //cameraLimits.BottomLimit = -150f;
        cameraLimits.LeftLimit = WorldTerrain.transform.position.x + WorldTerrainPadding;
        cameraLimits.RightLimit = WorldTerrain.terrainData.size.x - WorldTerrainPadding;
        cameraLimits.TopLimit = WorldTerrain.terrainData.size.z - WorldTerrainPadding;
        cameraLimits.BottomLimit = WorldTerrain.transform.position.z + WorldTerrainPadding;

        // When does the mouse tell the game to move the camera?
        mouseScrollLimits.LeftLimit = MouseBoundary;
		mouseScrollLimits.RightLimit = MouseBoundary;
		mouseScrollLimits.TopLimit = MouseBoundary;
		mouseScrollLimits.BottomLimit = MouseBoundary;
	}


	void Update () {
		if (CheckIfUserCameraInput())
		{
			var desiredTranslation = GetDesiredTranslation();

			if(!IsDesiredPositionOverBoundaries(desiredTranslation))
				this.transform.Translate(desiredTranslation);
        }


        // let mouse scroll change FOV
	    var cameraComp = transform.FindChild("Camera").GetComponent<Camera>();
	    var fov = cameraComp.fieldOfView;
	    fov += Input.GetAxis("Mouse ScrollWheel")*zoomSpeed*Time.deltaTime;
	    fov = Mathf.Clamp(fov, MinFOV, MaxFOV);
	    cameraComp.fieldOfView = fov;

	}

	public bool CheckIfUserCameraInput()
	{
	    var keyboardMove = AreCameraKeyboardButtonsPressed ();
		var mouseMove = IsMousePositionWithinBoundaries ();

		return (keyboardMove || mouseMove);
	}

	public Vector3 GetDesiredTranslation()
	{
	    var desiredTranslation = new Vector3();
		var moveSpeed = 0f;

		if (Common.ShiftKeyDown())
			moveSpeed = (_cameraMoveSpeed + _shiftBonus) * Time.deltaTime;
		else
			moveSpeed = _cameraMoveSpeed * Time.deltaTime;

		// move by keyboard
		if (Input.GetKey (KeyCode.W) || (Input.mousePosition.y > (Screen.height - mouseScrollLimits.TopLimit) && MoveByMouse))
			desiredTranslation += Vector3.forward*moveSpeed;

		if (Input.GetKey (KeyCode.S) || (Input.mousePosition.y < mouseScrollLimits.BottomLimit && MoveByMouse))
            desiredTranslation += Vector3.back * moveSpeed;

        if (Input.GetKey (KeyCode.A) || (Input.mousePosition.x < mouseScrollLimits.LeftLimit && MoveByMouse))
            desiredTranslation += Vector3.left * moveSpeed;

        if (Input.GetKey (KeyCode.D) || (Input.mousePosition.x > (Screen.width - mouseScrollLimits.RightLimit) && MoveByMouse))
            desiredTranslation += Vector3.right * moveSpeed;


  //      //move by mouse
  //      if (Input.mousePosition.x < mouseScrollLimits.LeftLimit && MoveByMouse)
		//	desiredX = -moveSpeed;

		//if (Input.mousePosition.x > (Screen.width - mouseScrollLimits.RightLimit) && MoveByMouse)
		//	desiredX = moveSpeed;

		//if (Input.mousePosition.y < mouseScrollLimits.BottomLimit && MoveByMouse)
		//	desiredZ = -moveSpeed;
		
		//if (Input.mousePosition.y > (Screen.height - mouseScrollLimits.TopLimit) && MoveByMouse)
		//	desiredZ = moveSpeed;

        return desiredTranslation;
	}





	#region Helper functions

	public bool IsDesiredPositionOverBoundaries (Vector3 desiredTranslation)
	{
	    var desiredWorldPosition = this.transform.TransformPoint(desiredTranslation);

        var result = (
			(desiredWorldPosition.x < cameraLimits.LeftLimit) || 
			(desiredWorldPosition.x > cameraLimits.RightLimit) || 
			(desiredWorldPosition.z > cameraLimits.TopLimit) || 
			(desiredWorldPosition.z < cameraLimits.BottomLimit)
			);
	    return result;
	}

	public static bool AreCameraKeyboardButtonsPressed()
	{
		return (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.D));
	}

	public static bool IsMousePositionWithinBoundaries()
	{
		return ((Input.mousePosition.x < mouseScrollLimits.LeftLimit && Input.mousePosition.x > -5) ||
		        (Input.mousePosition.x > (Screen.width - mouseScrollLimits.RightLimit) && Input.mousePosition.x < (Screen.width+5)) ||
		        (Input.mousePosition.y < mouseScrollLimits.BottomLimit && Input.mousePosition.y > -5) ||
		        (Input.mousePosition.y > (Screen.height - mouseScrollLimits.TopLimit) && Input.mousePosition.y < (Screen.height+5)));
	}
	#endregion

}
