using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class preyAgent : Agent {

	PredatorPreyAcademy academy;

	private preyVisualSystem visualSystem;
	public float sightDistance;
	public float turnDegree;
	public float speed;
	//public GameObject monitor;

	private GameObject arena;
	private float arenaRadius;

	void Awake()
	{
		// There is one brain in the scene so this should find our brain.
		//brain = FindObjectOfType<Brain>();
		//academy = FindObjectOfType<PredatorPreyAcademy>();
	}

	public override void InitializeAgent()
	{
		base.InitializeAgent();

		visualSystem = this.transform.Find("preyVisualSystem").GetComponent<preyVisualSystem>();
		arena = GameObject.Find("Arena");
		arenaRadius = arena.GetComponent<MeshCollider>().bounds.extents.x;

	}

	public override void CollectObservations()
	{
		// Distance to edges of platform
		AddVectorObs((this.transform.position.x) / arenaRadius);
		AddVectorObs((this.transform.position.z) / arenaRadius);

		AddVectorObs(detectVisibleObjects());
		//Monitor.Log("Reward", GetCumulativeReward(), MonitorType.text, monitor.transform);
	}

	float[] detectVisibleObjects()
	{
		Collider[] visibleObjectColliders = Physics.OverlapSphere(this.transform.position, sightDistance);
		Dictionary<float, GameObject> visibleObjects = new Dictionary<float, GameObject>();

		for (int i = 0; i < visibleObjectColliders.Length; i++)
		{
			GameObject nextObject = visibleObjectColliders[i].gameObject;

			if (nextObject.tag == "obstacle") //Changing this in the prey agent script will allow it to detect predators too
			{
				continue;
			}

			Vector3 nextObjectPosition = nextObject.transform.position;

			//If detecting the prey's own collider, continue
			if(nextObjectPosition == this.transform.position)
			{
				continue;
			}

			float angleTowardObject = Vector3.SignedAngle(this.transform.forward * -1, this.transform.position - nextObjectPosition, this.transform.up);
			//Debug.Log("Adding: " + nextObjectTag + " towards: " + angleTowardObject);
			if (!visibleObjects.ContainsKey(angleTowardObject))
			{
				visibleObjects.Add(angleTowardObject, nextObject);
			}            
		}

		return visualSystem.processVisualFeedback(visibleObjects);
	}

	public void isDead()
	{
		//Debug.Log(this.transform.name + " is dead!");
		AddReward(-1f);
		this.gameObject.SetActive(false);
	}

	private bool positionOutofBounds()
	{
		Vector3 checkPosition = this.transform.position + this.transform.forward;

		//Debug.Log("nextPosition = (" + Mathf.Abs(checkPosition.x) + ", " + Mathf.Abs(checkPosition.z) + ")");

		if (Mathf.Abs(checkPosition.x) > arenaRadius || Mathf.Abs(checkPosition.z) > arenaRadius)
		{
			AddReward(-0.001f);
			//Debug.Log("Position out of bounds!!");
			return true;
		}

		return false;
	}


	/// <summary>
	/// Moves the agent according to the selected action.
	/// </summary>
	public void MoveAgent(float[] act)
	{
		int action = Mathf.FloorToInt(act[0]);

		//If Action == 0, then do nothing, otherwise move.
		if (action > 0)
		{
			//Rotate if turning
			if (action == 2)
			{
				this.transform.Rotate(Vector3.up, turnDegree * -1);
			}
			else if (action == 3)
			{
				this.transform.Rotate(Vector3.up, turnDegree);
			}

			if (!positionOutofBounds())
			{
				//Move forward
				this.transform.position += this.transform.forward * speed;
			}
		}

	}

	/// <summary>
	/// Called every step of the engine. Here the agent takes an action.
	/// </summary>
	public override void AgentAction(float[] vectorAction, string textAction)
	{
		// Move the agent using the action.
		MoveAgent(vectorAction);

		AddReward(0.0005f); // 1f/simulation time steps to encourage prey to survive
	}

	/// <summary>
	/// In the editor, if "Reset On Done" is checked then AgentReset() will be 
	/// called automatically anytime we mark done = true in an agent script.
	/// </summary>
	public override void AgentReset()
	{

	}

}
