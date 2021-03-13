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

#endregion // Namespaces

// ######################################################################
// GECubixObjectList class stores Gameobject to demostrate.
// This class is used in GECubixSampleObject class.
// ######################################################################
[System.Serializable]
public class GECubixObjectList
{
	// ########################################
	// Variables
	// ########################################

	#region Variables

	// Root Gameobject to demostrate
	public GameObject GO;

	// GameObjects to be animated when the sample appears.
	public List<GameObject> AnimateList;

	#endregion // Variables
}

// ######################################################################
// GECubixSampleObject class stores Gameobject to demostrate in each scene.
// It also setup GameObjects in SampleObjects when the scene starts.
// This class is attached with Camera in each demo scene.
// ######################################################################
public class GECubixSampleObject : MonoBehaviour
{
	// ########################################
	// Variables
	// ########################################

	#region Variables

	// List of root of the Sample Gameobject in the scene.
	// Some scene may has many Samples to demostrate.
	public List<GECubixObjectList> SampleObjects;

	#endregion // Variables

	// ########################################
	// MonoBehaviour Functions
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.html
	// ########################################

	#region MonoBehaviour

	// Use this for initialization
	void Start()
	{
		// Find GECubixDemo component in the scene.
		GECubixDemo pDemo = Transform.FindObjectOfType<GECubixDemo>();
		if (pDemo)
		{
			// Hide all GameObjects in SampleObjects when scene is starting.
			for (int i = 0; i<SampleObjects.Count; i++)
			{
				SampleObjects[i].GO.SetActive(false);
			}
		}
		for (int i = 0; i<SampleObjects.Count; i++)
		{
			// If some SampleObjects.AnimateList contains no GameObject then automatically fill it.
			if (SampleObjects[i].AnimateList.Count==0)
			{
				// Add all children in the current SampleObjects.GO into SampleObjects.AnimateList.
				foreach (Transform child in SampleObjects[i].GO.transform)
				{
					SampleObjects[i].AnimateList.Add(child.gameObject);
				}
			}
		}
	}

	#endregion // MonoBehaviour
}
