using System;
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

	void Awake() {
		Instance = this;
	}

	void Start () {
		// How far can the camera move?
		cameraLimits.LeftLimit = -120f;
		cameraLimits.RightLimit = -35f;
		cameraLimits.TopLimit = -22f;
		cameraLimits.BottomLimit = -150f;

		// When does the mouse tell the game to move the camera?
		mouseScrollLimits.LeftLimit = MouseBoundary;
		mouseScrollLimits.RightLimit = MouseBoundary;
		mouseScrollLimits.TopLimit = MouseBoundary;
		mouseScrollLimits.BottomLimit = MouseBoundary;
	}


	void Update () {
		if (CheckIfUserCameraInput())
		{
			var cameraDesiredMove = GetDesiredTranslation();

			if(!IsDesiredPositionOverBoundaries(cameraDesiredMove))
				this.transform.Translate(cameraDesiredMove);
        }


        // let mouse scroll change FOV
	    var cameraComp = GetComponent<Camera>();
	    var fov = cameraComp.fieldOfView;
	    fov += Input.GetAxis("Mouse ScrollWheel")*zoomSpeed;
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
		var desiredX = 0f;
		var desiredZ = 0f;
		var moveSpeed = 0f;

		if (Input.GetKey (KeyCode.LeftShift) || Input.GetKey (KeyCode.RightShift))
			moveSpeed = (_cameraMoveSpeed + _shiftBonus) * Time.deltaTime;
		else
			moveSpeed = _cameraMoveSpeed * Time.deltaTime;

		// move by keyboard
		if (Input.GetKey (KeyCode.W))
			desiredZ = moveSpeed;

		if (Input.GetKey (KeyCode.S))
			desiredZ = -moveSpeed;

		if (Input.GetKey (KeyCode.A))
			desiredX = -moveSpeed;

		if (Input.GetKey (KeyCode.D))
			desiredX = moveSpeed;


		//move by mouse
		if (Input.mousePosition.x < mouseScrollLimits.LeftLimit)
			desiredX = -moveSpeed;

		if (Input.mousePosition.x > (Screen.width - mouseScrollLimits.RightLimit))
			desiredX = moveSpeed;

		if (Input.mousePosition.y < mouseScrollLimits.BottomLimit)
			desiredZ = -moveSpeed;
		
		if (Input.mousePosition.y > (Screen.height - mouseScrollLimits.TopLimit))
			desiredZ = moveSpeed;

        return new Vector3(desiredX, desiredZ, 0);
	}





	#region Helper functions

	public bool IsDesiredPositionOverBoundaries (Vector3 desiredPosition)
	{
        var result = (
			((this.transform.position.x + desiredPosition.x) < cameraLimits.LeftLimit) || 
			((this.transform.position.x + desiredPosition.x) > cameraLimits.RightLimit) || 
			((this.transform.position.z + desiredPosition.y) > cameraLimits.TopLimit) || 
			((this.transform.position.z + desiredPosition.y) < cameraLimits.BottomLimit)
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
