
//Detect when the prey has got "eaten/touched" by the predator. 
//Detect when the prey has touched an obstacle. 
//Put this script onto the prey. There's nothing you need to set in the editor.
//Make sure the prey is tagged with prey in the editor.

using UnityEngine;
using System.Collections;

public class PreyDetectOld : MonoBehaviour
{
	[HideInInspector]
	/// <summary>
	/// The associated agent.
	/// This will be set by the agent script on Initialization. 
	/// Don't need to manually set.
	/// </summary>
	public WolfAgent agent;  //

	void OnCollisionEnter(Collision col)
	{
		// Touched prey
		if (col.gameObject.CompareTag("prey"))
		{
			agent.caughtPrey();
		}

        //Touched Obstacle
        if (col.gameObject.CompareTag("obstacle"))
        {
            agent.hitAnObstacle();
        }
    }

}

