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
// GECubixAnim applies OutBack-tweens scale animation on itself's GameObject.
// This class is used in GECubixDemo class.
// ######################################################################

public class GECubixAnim : MonoBehaviour
{
	// ########################################
	// Variables
	// ########################################

	#region Variables

	// Target Scale
	Vector3 m_TargetScale = new Vector3(0,0,0);
	// Start Scale
	Vector3 m_StartScale = new Vector3(0,0,0);

	// Animation ratio, it is between 0.0f to 1.0f
	float m_AnimRatioValue = 0;

	// Delay time in second
	float m_DelayTime = 0;

	// Time that passes the Delay
	float m_PassedTime = 0;

	// Total animation time in second
	float m_AnimTotalTime = 1;

	#endregion // Variables

	// ########################################
	// MonoBehaviour Functions
	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.html
	// ########################################

	#region MonoBehaviour

	// Use this for initialization
	void Start()
	{
		// Use local scale as target scale
		m_TargetScale = transform.localScale;

		// Scale always start with zero
		transform.localScale = m_StartScale;
	}

	// Update is called once per frame
	void Update()
	{
		// Update passed time
		m_PassedTime += Time.deltaTime;
		if (m_PassedTime<m_DelayTime)
			return;

		// Find animation ratio
		m_AnimRatioValue = (m_PassedTime - m_DelayTime) / m_AnimTotalTime;
		if (m_AnimRatioValue<1.0f)
		{
			// Update scale
			transform.localScale = UpdateAnim(m_StartScale, m_TargetScale, m_AnimRatioValue);
		}
		else if (m_AnimRatioValue>1.0f)
		{
			// make sure m_AnimRatioValue is not larger than 1.0f
			m_AnimRatioValue=1.0f;

			// Update scale
			transform.localScale = UpdateAnim(m_StartScale, m_TargetScale, m_AnimRatioValue);

			// Destroy this component when the animation is done. 
			Destroy(this);
		}
	}

	#endregion //MonoBehaviour

	// ########################################
	// Animation functions
	// ########################################

	#region Animation functions

	// Set the total animation time
	public void SetAnimTime(float TotalTime)
	{
		m_AnimTotalTime = TotalTime;
	}

	// Set delay time
	public void SetAnimDelay(float Delay)
	{
		m_DelayTime = Delay;
	}

	// Update animation according to animation ratio value
	private Vector3 UpdateAnim(Vector3 StarValue, Vector3 EndValue, float CurrentValue)
	{
		float ConstVal = 1.70158f;
		EndValue -= StarValue;
		CurrentValue = (CurrentValue / 1) - 1;
		return EndValue * ((CurrentValue) * CurrentValue * ((ConstVal + 1) * CurrentValue + ConstVal) + 1) + StarValue;
	}

	#endregion //Animation functions
}
