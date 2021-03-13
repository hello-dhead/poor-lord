// Cubix
// Version: 1.0.1
// Compatilble: Unity 5.6.1 or higher, see more info in Readme.txt file.
//
// Developer:			Gold Experience Team (https://www.assetstore.unity3d.com/en/#!/search/page=1/sortby=popularity/query=publisher:4162)
// Unity Asset Store:	https://www.assetstore.unity3d.com/#!/content/92184
//
// Please direct any bugs/comments/suggestions to geteamdev@gmail.com

#region Namespaces

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

#endregion // Namespaces

// ######################################################################
// GECubixSamepleScene class stores name of each demo scene and camera orbit flag.
// This class is used in GECubixDemo class.
// ######################################################################

[System.Serializable]
public class GECubixSamepleScene
{
	// ########################################
	// Variables
	// ########################################

	#region Variables

	// Scene's name
	public string Name = "";

	// Camera is orbitable if this variable is true
	public bool OrbitCamera = false;

	#endregion // Variables
}

// ######################################################################
// GECubixDemo class handle user input to control the demo scene.
// This class is attached with Canvas in 00_Demo scene.
// ######################################################################
public class GECubixDemo : MonoBehaviour
{
	// ########################################
	// Variables
	// ########################################

	#region Variables

	// UI Buttons
	public Button btnHome;
	public Button btnPreviouse;
	public Button btnNext;
	public Button btnRotateCamera;

	// UI Texts
	public Text txtOrder;
	public Text txtScene;

	// List of sample scene, which are 01 to 09 scenes.
	public List<GECubixSamepleScene> m_SampleScenes;
	// Current Scene Index.
	private int m_CurrentScene = 0;

	// List of sample GameObject in the current demo scene.
	private GECubixSampleObject m_SampleObject=null;
	// Current Sample Index.
	private int m_CurrentSample = 0;
	// Total Sample Index which always equal to m_SampleObject.SampleObjects.Count
	private int m_TotalSample = 0;

	// Gameobject to keep over scene change, which are Canvas_Camera, Canvas and EventSystem in 00_Demo scene.
	public List<GameObject> m_DoNotDestroy;

	// Orbit Camera
	private Camera m_OrbitCamera = null;
	// Should the current camera orbit.
	private bool m_Orbit = true;
	// Speed of orbital
	private float m_OrbitSpeed = 10f;
	// Center position of orbital
	private Vector3 m_OrbitTarget = Vector3.zero;

	// Zoom status
	private bool wasZoomingLastFrame = false;
	private Vector2[] lastZoomPositions;
	// FOV of Camera
	private float m_MinFOV = 30;
	private float m_MaxFOV = 30;
	// Mouse zooming speed
	private float m_MouseZoomSpeed = 1000f;
	// Touch zooming speed
	private float m_TouchesZoomSpeed = 10f;

	// Total bound of the current sample, the center of the bound will be store in m_OrbitTarget
	private Bounds m_TotalBounds;

	#endregion // Variables

	// ########################################
	// MonoBehaviour Functions
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.html
	// ########################################

	#region MonoBehaviour

	// Awake is called when the script instance is being loaded
	void Awake()
	{
		// The Canvas_Camera, Canvas and EventSystem opjects will not be destroyed over the demostration.
		for (int i = 0; i<m_DoNotDestroy.Count; i++)
		{
			DontDestroyOnLoad(m_DoNotDestroy[i]);
		}
	}

	// Use this for initialization
	void Start()
	{
		// Reset all Indices.
		m_CurrentScene = 0;
		m_CurrentSample = 0;
		m_TotalSample = 0;

		// Add listener function to UI Buttons.
		btnHome.onClick.AddListener(btnHomeOnClick);
		btnPreviouse.onClick.AddListener(btnPreviouseOnClick);
		btnNext.onClick.AddListener(btnNextOnClick);
		btnRotateCamera.onClick.AddListener(btnRotateCameraOnClick);

		// If there are scenes in m_SampleScenes then load the first Sample Scene.
		if (m_SampleScenes.Count>0)
			SceneManager.LoadScene(m_SampleScenes[0].Name);

		// Change Sample to the first one in the loaded Sample Scene.
		ChangeSample(true);
	}

	// Update is called once per frame
	void Update()
	{
		// Response to H key, toggle the Canvas_Camera.
		if (Input.GetKeyUp(KeyCode.H))
		{
			Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
			foreach (Camera cam in cameras)
			{
				if (cam.gameObject.layer == LayerMask.NameToLayer("UI"))
				{
					cam.enabled = !cam.enabled;
					break;
				}
			}
		}
		// Response to A key and Left-arrow key, shows the previouse Sample.
		if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
		{
			btnPreviouseOnClick();
		}
		// Response to D key and Right-arrow key, shows the next Sample.
		if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
		{
			btnNextOnClick();
		}
		// Response to S key and Down-arrow key, zoom out.
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
		{
			ZoomCamera(-1.0f * Time.deltaTime, m_MouseZoomSpeed);
		}
		// Response to W key and Up-arrow key, zoom in.
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
		{
			ZoomCamera(+1.0f * Time.deltaTime, m_MouseZoomSpeed);
		}
	}

	// 	LateUpdate is called every frame, if the Behaviour is enabled.
	void LateUpdate()
	{
		// Orbits the Camera if m_OrbitCamera variable is true
		if (m_OrbitCamera != null)
		{
			if (m_Orbit==true)
			{
				GECubixOrbitCamera pGECubixOrbitCamera = m_OrbitCamera.GetComponent<GECubixOrbitCamera>();
				if (pGECubixOrbitCamera == null)
				{
					Orbit();
				}
				else if (Input.GetMouseButtonDown(0)==false)
				{
					Orbit();
				}

			}

			HandleZoom();
		}
	}

	#endregion //MonoBehaviour

	// ########################################
	// UI response functions
	// ########################################


	#region UI response functions

	// User clicks on the Home button
	void btnHomeOnClick()
	{
		// Reset scene index and sample index.
		m_CurrentScene=0;
		m_CurrentSample=0;

		// Load the first Sample
		ChangeSample(true);
	}

	// User clicks on the Previouse button
	void btnPreviouseOnClick()
	{
		// Decrese the Sample index
		m_CurrentSample--;

		// Load Sample
		ChangeSample(false);
	}

	// User clicks on the Next button
	void btnNextOnClick()
	{
		// Increase the Sample index
		m_CurrentSample++;

		// Load Sample
		ChangeSample(false);
	}

	// User clicks on the RotateCamera button
	void btnRotateCameraOnClick()
	{
		// Toggle the orbit Camera
		m_Orbit = !m_Orbit;
		if (m_Orbit)
		{
			// Change the appearance of Rotate UI Button to full color and opaque.
			btnRotateCamera.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		}
		else
		{
			// Change the appearance of Rotate UI Button to disabled color.
			btnRotateCamera.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 1.0f);
		}
	}

	#endregion //UI response functions

	// ########################################
	// Sample and Scene funtions
	// ########################################

	#region Sample and Scene funtions

	// Change Sample
	// The ForceLoadScene parameter forces to reload scene even it is the current active scene.
	void ChangeSample(bool ForceLoadScene)
	{
		// Check if there needs to load another scene for showing the next or previous Sample.
		bool ShouldLoadScene = false;
		if (m_SampleObject)
		{
			// Set to value before load next scene in the list in m_SampleScenes variable.
			if (m_CurrentSample>=m_TotalSample)
			{
				m_CurrentSample = 0;
				m_CurrentScene++;
				if (m_CurrentScene>=m_SampleScenes.Count)
				{
					m_CurrentScene=0;
				}
				ShouldLoadScene = true;
			}
			// Set to value before load previous scene in the list in m_SampleScenes variable.
			else if (m_CurrentSample<0)
			{
				m_CurrentScene--;
				if (m_CurrentScene<0)
				{
					m_CurrentScene=m_SampleScenes.Count-1;
				}
				ShouldLoadScene = true;
			}
		}
		else
		{
			// Always load the first scene if m_SampleObject is null.
			m_CurrentScene=0;
			m_CurrentSample=0;
			ShouldLoadScene = true;
		}

		// If ForceLoadScene is true then always load scene
		if (ForceLoadScene)
		{
			ShouldLoadScene = true;
		}

		if (ShouldLoadScene==true)
		{
			// Load new scene
			StartCoroutine(LoadScene());
		}
		else
		{
			// Change to another Sample in the same scene.
			LoadSample();
		}

	}

	// Load new scene
	IEnumerator LoadScene()
	{
		// Get current scene's name.
		string currentScene = SceneManager.GetActiveScene().name;
		// Load new scene if the desire scene is not the current scene.
		if (currentScene!= m_SampleScenes[m_CurrentScene].Name)
		{
			// Load new scene with SceneManager.
			SceneManager.LoadScene(m_SampleScenes[m_CurrentScene].Name);

			// Wait until loading is done.
			yield return new WaitWhile(() => currentScene == SceneManager.GetActiveScene().name);
		}

		// Some scene has many Samples, get Samples list in the scene.
		m_SampleObject = GameObject.FindObjectOfType<GECubixSampleObject>();
		if (m_SampleObject)
		{
			// Keep Sample count of the new scene
			m_TotalSample = m_SampleObject.SampleObjects.Count;

			// Set Sample index to be in range of Sample count
			if (m_CurrentSample<0)
				m_CurrentSample = m_TotalSample-1;
		}

		// Check for OrbitCamera variable of the new scene
		if (m_SampleScenes[m_CurrentScene].OrbitCamera==false)
		{
			// Set camera to be fix position.
			m_OrbitCamera = null;
			m_OrbitTarget = new Vector3(0, 0, 0);

			m_Orbit = false;
			btnRotateCamera.enabled = false;

			// Change the Camera Rotate UI to disabled apperance.
			btnRotateCamera.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 0.5f);
		}
		else
		{
			// Set up the camera to be orbital
			SetOrbitCamera();
			SetTargetToOrbit(m_SampleObject.SampleObjects[m_CurrentSample].GO);

			m_Orbit = true;
			btnRotateCamera.enabled = true;

			// Change the Camera Rotate UI to enabled apperance.
			btnRotateCamera.GetComponent<Image>().color = new Color(1, 1, 1, 1);
		}

		// Load Sample
		LoadSample();
	}

	// Load Sample and display it with tweener animations
	void LoadSample()
	{
		for (int i = 0; i<m_SampleObject.SampleObjects.Count; i++)
		{
			GameObject go = m_SampleObject.SampleObjects[i].GO;
			if (go!=null)
			{
				// If desired Sample then set it to active and animate its children.
				if (i==m_CurrentSample)
				{
					// Update Scene and Sample UI Texts
					txtOrder.text = "Scene: " + (m_CurrentScene+1).ToString() + "/" + m_SampleScenes.Count;
					if (m_SampleObject.SampleObjects.Count>1)
						txtOrder.text += " - (" + (m_CurrentSample+1).ToString() + "/" + m_SampleObject.SampleObjects.Count.ToString() + ")";
					txtScene.text = m_SampleObject.SampleObjects[m_CurrentSample].GO.name;

					go.SetActive(true);

					// Animate children GameObject in the AnimateList.
					Animate(m_SampleObject.SampleObjects[m_CurrentSample].AnimateList);
				}
				// Inactive other Samples
				else
				{
					go.SetActive(false);
				}
			}
		}
	}

	#endregion //Sample and Scene funtions

	// ########################################
	// Animate function
	// ########################################

	#region Animate function

	// Play tweener animations as on the demo video, https://youtu.be/PKEM3Pjy9Kk
	void Animate(List<GameObject> AnimateList)
	{
		int Count = 0;

		// Add tweener into GameObjects in the AnimateList variable
		foreach (GameObject child in AnimateList)
		{

			// This line is for animating objects as on the demo video, https://youtu.be/PKEM3Pjy9Kk
			GECubixAnim pGECubixAnim = child.gameObject.AddComponent<GECubixAnim>();
			pGECubixAnim.SetAnimTime(Random.Range(0.25f, 0.75f));
			pGECubixAnim.SetAnimDelay(Count*(1.5f/AnimateList.Count));

			Count++;
		}

	}

	#endregion //Animate function

	// ########################################
	// Orbit Camera functions
	// ########################################

	#region Orbit Camera functions

	// Rest the orbit camera.
	public void SetOrbitCamera()
	{
		Camera[] Cameras = Camera.allCameras;
		foreach (Camera cam in Cameras)
		{
			if (cam.gameObject.layer!=LayerMask.NameToLayer("UI"))
			{
				m_OrbitCamera = cam;

				// Set FOV ranges.
				m_MinFOV = m_OrbitCamera.fieldOfView;
				m_MaxFOV = m_OrbitCamera.fieldOfView* 2.5f;

				return;
			}
		}

		m_OrbitCamera = null;
	}

	// Set the center of orbital.
	void SetTargetToOrbit(GameObject go)
	{
		// Find total bound of the given GameObject.
		MeshRenderer goMeshRenderer = go.GetComponent<MeshRenderer>();
		if (goMeshRenderer)
			m_TotalBounds = goMeshRenderer.bounds;
		else
			m_TotalBounds = go.GetComponentInChildren<MeshRenderer>().bounds;

		// Starts off with the first bounds in the hierarchy.
		AddChildrenToBounds(go.transform);

		// Set the center of orbital with center of total bound.
		m_OrbitTarget = m_TotalBounds.center;
	}

	Vector3 GetTargetOrbit()
	{
		return m_OrbitTarget;
	}

	// Orbit the camera.
	void Orbit()
	{
		// Update the camera position.
		OrbitCamera(m_OrbitTarget, m_OrbitSpeed, 0);
	}

	// Update the camera position
	public void OrbitCamera(Vector3 target, float y_rotate, float x_rotate)
	{
		// Update the camera position.
		Vector3 angles = m_OrbitCamera.transform.eulerAngles;
		angles.z = 0;
		m_OrbitCamera.transform.eulerAngles = angles;
		m_OrbitCamera.transform.RotateAround(target, Vector3.up, y_rotate * Time.deltaTime);
		m_OrbitCamera.transform.RotateAround(target, Vector3.left, x_rotate * Time.deltaTime);

		// Update the camera's LookAt position.
		m_OrbitCamera.transform.LookAt(target);
	}

	#endregion //Orbit Camera functions

	// ########################################
	// Boundary functions
	// ########################################

	#region Boundary functions

	// A recursive function to calculate total bound of a GameOject hierarchy.
	void AddChildrenToBounds(Transform trans)
	{
		MeshRenderer transMeshRenderer = trans.gameObject.GetComponent<MeshRenderer>();
		if (transMeshRenderer != null)
		{
			// Encapsulate the bounds.
			m_TotalBounds.Encapsulate(transMeshRenderer.bounds.min);
			m_TotalBounds.Encapsulate(transMeshRenderer.bounds.max);
		}

		// Go recursive in the hierachy.
		foreach (Transform grandChild in trans)
		{
			AddChildrenToBounds(grandChild);
		}
	}

	#endregion //Boundary functions

	// ########################################
	// Zoom functions
	// ########################################

	#region Zoom functions

	// Response to user input
	void HandleZoom()
	{

		// Check if user zooming by touch screen.
		bool TouchOverrided = false;
		if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer)
		{
			if (Input.touchCount==2)
			{
				Vector2[] newPositions = new Vector2[]{Input.GetTouch(0).position, Input.GetTouch(1).position};
				if (!wasZoomingLastFrame)
				{
					lastZoomPositions = newPositions;
					wasZoomingLastFrame = true;
				}
				else
				{
					// Zoom based on the distance between the new positions compared to the 
					// distance between the previous positions.
					float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
					float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
					float ZoomFactor = newDistance - oldDistance;

					ZoomCamera(ZoomFactor, m_TouchesZoomSpeed);
					TouchOverrided = true;

					lastZoomPositions = newPositions;
				}
			}
			else
			{
				wasZoomingLastFrame = false;
			}
		}

		// Check if user is zooming by mouse.
		if (TouchOverrided==false)
		{
			// Get axis of mouse scroll wheel.
			float ZoomFactor = Input.GetAxis("Mouse ScrollWheel");

			// Update zoom
			ZoomCamera(ZoomFactor, m_MouseZoomSpeed);
		}
	}

	// Update zoom camera
	// ZoomFactor is FOV value, Speed is the speed of zooming
	void ZoomCamera(float ZoomFactor, float Speed)
	{
		// Calculate and change FOV of the camera
		m_OrbitCamera.fieldOfView -= ZoomFactor * Speed * Time.deltaTime;

		// Prevents value from exceeding specified range.
		m_OrbitCamera.fieldOfView = Mathf.Clamp(m_OrbitCamera.fieldOfView, m_MinFOV, m_MaxFOV);
	}

	#endregion //Zoom functions
}
