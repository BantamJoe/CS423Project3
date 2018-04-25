using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class preyAgent : Agent {

	public GameObject arena;
    Vector3 arenaPos;

	PredatorPreyAcademy academy;

	Rigidbody agentRB;
	RayPerception rayPer;

    float arenaSize;
    float arenaMinX;
    float arenaMaxX;
    float arenaMinZ;
    float arenaMaxZ;

    GameObject myPredator;
    float initialDistanceToPrey;
    float currentDistanceToPredator = 9999f;

    int initialFrameCount;

    void Awake()
	{
		// There is one brain in the scene so this should find our brain.
		//brain = FindObjectOfType<Brain>();
	    //academy = FindObjectOfType<PredatorPreyAcademy>();
	}

	public override void InitializeAgent()
	{
		base.InitializeAgent();

		// Cache the agent rigidbody
		agentRB = GetComponent<Rigidbody>();

        // Get the ground's bounds

        arenaPos = arena.transform.position;
        arenaSize = arena.GetComponent<Collider>().bounds.size.x/2;
        arenaMinX = arenaSize * -1;
        arenaMaxX = arenaSize;

        arenaMinZ = arenaSize * -1;
        arenaMaxZ = arenaSize;

        getMyPredator();
	}

	public override void CollectObservations()
	{
        AddVectorObs((this.transform.position.x - arenaSize) / arenaSize);
        AddVectorObs((this.transform.position.z - arenaSize) / arenaSize);

        //Monitor.Log("Reward", GetCumulativeReward(), MonitorType.text, this.transform);
    }

    /// <summary>
    /// Use the ground's bounds to pick a random spawn position.
    /// </summary>
    public Vector3 GetRandomSpawnPos()
	{
        
        bool foundNewSpawnLocation = false;
		Vector3 randomSpawnPos = Vector3.zero;
		while (foundNewSpawnLocation == false)
		{
            float randomPosX = Random.Range(arenaMinX, arenaMaxX);

            float randomPosZ = Random.Range(arenaMinZ, arenaMaxZ);

            randomSpawnPos = new Vector3(arenaPos.x + randomPosX, 1.2f, arenaPos.z + randomPosZ);
			if (Physics.CheckBox(randomSpawnPos, new Vector3(2.5f, 0.01f, 2.5f)) == false)
			{
                float distanceToPredator = Vector3.Distance(myPredator.transform.position, randomSpawnPos);
              
               if(distanceToPredator <= 20)
               {
                    foundNewSpawnLocation = true;
               }                                           
			}
		}

        initialFrameCount = Time.frameCount;

        return randomSpawnPos;
	}

    void getMyPredator()
    {

        myPredator = arena.transform.Find("Predator").gameObject;

        /*
        GameObject[] predatorArray = GameObject.FindGameObjectsWithTag("predator");
        float nextPredatorX;
        float nextPredatorZ;

        foreach (GameObject predator in predatorArray)
        {
            nextPredatorX = predator.transform.position.x;
            nextPredatorZ = predator.transform.position.z;

            if (nextPredatorX <= terrainPos.x + arenaMaxX && nextPredatorX >= terrainPos.x + arenaMinX && nextPredatorZ <= terrainPos.z + arenaMaxZ && nextPredatorZ >= terrainPos.z + arenaMinZ)
            {
                myPredator = predator;
            }
        }
        */
    }

    public void hitAnObstacle()
    {
        AddReward(-1f);
    }

	/// <summary>
	/// Moves the agent according to the selected action.
	/// </summary>
	public void MoveAgent(float[] act)
	{

		Vector3 dirToGo = Vector3.zero;
		Vector3 rotateDir = Vector3.zero;

        float movement = Mathf.Clamp(act[0], -1, 1);
        float rotation = Mathf.Clamp(act[1], -1, 1);

        
        if(movement > 0)
        {
            dirToGo = transform.forward * 1f;
        }
        else if(movement < 0)
        {
            dirToGo = transform.forward * -1f;
        }
        
        if (rotation > 0)
        {
            rotateDir = transform.up * -1;
        }
        else if (rotation < 0)
        {
            rotateDir = transform.up * 1f;
        }

		transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
		agentRB.AddForce(dirToGo * academy.agentRunSpeed,
			ForceMode.VelocityChange);
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

		transform.position = GetRandomSpawnPos();
		agentRB.velocity = Vector3.zero;
		agentRB.angularVelocity = Vector3.zero;
	}
		
}
