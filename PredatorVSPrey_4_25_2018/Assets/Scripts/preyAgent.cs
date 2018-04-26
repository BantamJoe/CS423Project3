using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class preyAgent : Agent {

	PredatorPreyAcademy academy;

    private VisualSystem visualSystem;
    public float sightDistance;

    void Awake()
	{
		// There is one brain in the scene so this should find our brain.
		//brain = FindObjectOfType<Brain>();
	    //academy = FindObjectOfType<PredatorPreyAcademy>();
	}

	public override void InitializeAgent()
	{
		base.InitializeAgent();

        visualSystem = this.transform.Find("visualIndicators").GetComponent<VisualSystem>();

	}

	public override void CollectObservations()
	{
        //AddVectorObs((this.transform.position.x - arenaSize) / arenaSize);
        //AddVectorObs((this.transform.position.z - arenaSize) / arenaSize);

        //Monitor.Log("Reward", GetCumulativeReward(), MonitorType.text, this.transform);

        detectVisibleObjects();
    }

    bool[,] detectVisibleObjects()
    {
        Collider[] visibleObjectColliders = Physics.OverlapSphere(this.transform.position, sightDistance);
        Dictionary<float, string> visibleObjects = new Dictionary<float, string>();

        for (int i = 0; i < visibleObjectColliders.Length; i++)
        {
            GameObject nextObject = visibleObjectColliders[i].gameObject;
            string nextObjectTag = nextObject.tag;

            if (nextObjectTag == "obstacle") //Changing this in the prey agent script will allow it to detect predators too
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
            visibleObjects.Add(angleTowardObject, nextObjectTag);
        }

        return visualSystem.processVisualFeedback(visibleObjects);
    }
   

	/// <summary>
	/// Moves the agent according to the selected action.
	/// </summary>
	public void MoveAgent(float[] act)
	{

	}

	/// <summary>
	/// Called every step of the engine. Here the agent takes an action.
	/// </summary>
	public override void AgentAction(float[] vectorAction, string textAction)
	{
		// Move the agent using the action.
		MoveAgent(vectorAction);
	}

	/// <summary>
	/// In the editor, if "Reset On Done" is checked then AgentReset() will be 
	/// called automatically anytime we mark done = true in an agent script.
	/// </summary>
	public override void AgentReset()
	{

		//transform.position = GetRandomSpawnPos();
		//agentRB.velocity = Vector3.zero;
		//agentRB.angularVelocity = Vector3.zero;
	}
		
}
