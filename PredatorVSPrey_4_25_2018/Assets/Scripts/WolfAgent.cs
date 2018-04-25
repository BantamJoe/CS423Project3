using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfAgent : Agent {

	public Terrain terrain;
    public int rowsAndCols;
    public int currentArenaX;
    public int currentArenaZ;

    TerrainData terrainData;
    //TerrainCollider terrainCollider;
    Bounds terrainBounds;

	PredatorPreyAcademy academy;

	// Detects when the block touches the goal.
	[HideInInspector]
	public PreyDetectOld goalDetect;

	Rigidbody agentRB;
	RayPerception rayPer;

    float arenaSize;
    float arenaMinX;
    float arenaMaxX;
    float arenaMinZ;
    float arenaMaxZ;

    GameObject myPrey;
    float initialDistanceToPrey;
    float currentDistanceToPrey = 9999f;

    int initialFrameCount;

    void Awake()
	{
		// There is one brain in the scene so this should find our brain.
		brain = FindObjectOfType<Brain>();
		academy = FindObjectOfType<PredatorPreyAcademy>();
	}

	public override void InitializeAgent()
	{
		base.InitializeAgent();
		goalDetect = GetComponent<PreyDetectOld>();
		goalDetect.agent = this;
		rayPer = GetComponent<RayPerception>();

		// Cache the agent rigidbody
		agentRB = GetComponent<Rigidbody>();

        // Get the ground's bounds
        terrainData = terrain.terrainData;
        terrainBounds = terrainData.bounds;
        arenaSize = terrainBounds.size.x/rowsAndCols;

        arenaMinX = arenaSize * currentArenaX;
        arenaMaxX = arenaMinX + arenaSize;

        arenaMinZ = arenaSize * currentArenaZ;
        arenaMaxZ = arenaMinZ + arenaSize;

	}

	public override void CollectObservations()
	{

        AddVectorObs((this.transform.position.x - arenaSize) / arenaSize);
        AddVectorObs((this.transform.position.z - arenaSize) / arenaSize);
        

        Monitor.Log("Reward", GetCumulativeReward(), MonitorType.text, this.transform);
    }

    void checkIfApproachingPrey()
    {
        float distanceToPrey;
        
        Vector3 predatorPosition = this.transform.position;
        Vector3 preyPosition = myPrey.transform.position;
        distanceToPrey = Vector3.Distance(predatorPosition,preyPosition);

        if (distanceToPrey < currentDistanceToPrey)
        {

            //Debug.Log("Prey In Sight! Approaching");


            AddReward(0.01f);
        }
        else if (distanceToPrey > currentDistanceToPrey)
        {

            // Debug.Log("Prey In Sight! Retreating");

            AddReward(-0.01f);
        }

        currentDistanceToPrey = distanceToPrey;

        if (Time.frameCount % 120 == 0)
        {
            //Debug.Log("currentDistanceToPrey = " + currentDistanceToPrey);
        }            
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

            randomSpawnPos = new Vector3(randomPosX, 11.5f, randomPosZ);
			if (Physics.CheckBox(randomSpawnPos, new Vector3(2.5f, 0.01f, 2.5f)) == false)
			{
				foundNewSpawnLocation = true;
			}
		}
       

        calculateInitialPreyDistance(randomSpawnPos);
        initialFrameCount = Time.frameCount;

        return randomSpawnPos;
	}

    void calculateInitialPreyDistance(Vector3 newSpawnPoint)
    {

        GameObject[] preyArray = GameObject.FindGameObjectsWithTag("prey");
        float nextPreyX;
        float nextPreyZ;

        foreach (GameObject prey in preyArray)
        {
            nextPreyX = prey.transform.position.x;
            nextPreyZ = prey.transform.position.z;

            if (nextPreyX <= arenaMaxX && nextPreyX >= arenaMinX && nextPreyZ <= arenaMaxZ && nextPreyZ >= arenaMinZ)
            {
                myPrey = prey;
                initialDistanceToPrey = Vector3.Distance(newSpawnPoint,new Vector3(nextPreyX, 0, nextPreyZ));
                //Debug.Log("Found nearest prey! Distance = " + initialDistanceToPrey);
            }
        }
    }

	/// <summary>
	/// Called when the agent touches the prey.
	/// </summary>
	public void caughtPrey()
	{
        int framesToCatchPrey = Time.frameCount - initialFrameCount;
        //Debug.Log("caughtPrey: framesToCatchPrey= " + framesToCatchPrey);

        float adjustedReward = 1.0f - ((framesToCatchPrey / initialDistanceToPrey)/30);

        // We use a reward of 5.
        //AddReward(adjustedReward);
        AddReward(100.0f);

		// By marking an agent as done AgentReset() will be called automatically.
		Done();
        //myPrey.AgentReset();
	}

    public void movedForward()
    {
        //AddReward(0.0001f);
    }

    public void hitAnObstacle()
    {
        AddReward(-0.1f);
    }

	/// <summary>
	/// Moves the agent according to the selected action.
	/// </summary>
	public void MoveAgent(float[] act)
	{

        //AddReward(-0.01f);

		Vector3 dirToGo = Vector3.zero;
		Vector3 rotateDir = Vector3.zero;

        float movement = Mathf.Clamp(act[0], -1, 1);
        float rotation = Mathf.Clamp(act[1], -1, 1);

        if(movement > 0)
        {
            dirToGo = transform.forward * 1f;
            movedForward();
        }
        else if(movement < 0)
        {
            dirToGo = transform.forward * -1f;
        }

        if (rotation > 0)
        {
            rotateDir = transform.up * -1f;
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
