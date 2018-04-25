using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredatorAgent : Agent {

    public GameObject arena;
    public GameObject monitor;



    Vector3 arenaPos;

	PredatorPreyAcademy academy;

	Rigidbody agentRB;
	RayPerception rayPer;
    PreyDetect preyDetect;

    float arenaSize;
    float arenaMinX;
    float arenaMaxX;
    float arenaMinZ;
    float arenaMaxZ;

    GameObject myPrey;
    float initialDistanceToPrey;
    float currentDistanceToPrey = 9999f;

    bool wasApproachingPrey;
    bool approachingPrey;

    bool preyWasVisible;
    bool preyVisible;

    //int initialFrameCount;

    void Awake()
	{
		// There is one brain in the scene so this should find our brain.
		brain = FindObjectOfType<Brain>();
		academy = FindObjectOfType<PredatorPreyAcademy>();
	}

	public override void InitializeAgent()
	{
		base.InitializeAgent();
		rayPer = GetComponent<RayPerception>();

        preyDetect = GetComponent<PreyDetect>();
        preyDetect.agent = this;

        // Cache the agent rigidbody
        agentRB = GetComponent<Rigidbody>();

        arenaPos = arenaPos = arena.transform.position;
        arenaSize = arena.GetComponent<Collider>().bounds.size.x/2;

        arenaMinX = arenaSize*-1;
        arenaMaxX = arenaSize;

        arenaMinZ = arenaSize*-1;
        arenaMaxZ = arenaSize;

        getMyPrey();

        /*
        Debug.Log("arenaSize=" + arenaSize);
        Debug.Log("\tarenaMinX=" + arenaMinX);
        Debug.Log("\tarenaMaxX=" + arenaMaxX);
        Debug.Log("\tarenaMinZ=" + arenaMinZ);
        Debug.Log("\tarenaMaxZ=" + arenaMaxZ);
        */
    }

	public override void CollectObservations()
	{
        List<float> rayData1;
        List<float> rayData2;
		float rayDistance = 15f;
        float[] rayAngles = { 30f, 45f, 90f, 135f, 150f, 110f, 70f };

        string[] detectableObjects;
		detectableObjects = new string[] {"obstacle" };
        rayData1 = rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 1f, 0f);

        RaycastHit preyRay;
        if (Physics.Raycast(this.transform.position, myPrey.transform.position - this.transform.position, out preyRay, 100.0f))
        {
            if (preyRay.collider.CompareTag("prey"))
            {
                float angleToPrey = Vector3.AngleBetween(this.transform.forward, this.transform.position - myPrey.transform.position);

                //The predator can see the prey
                if(angleToPrey > 2.0f)
                {
                    approachingPrey = checkIfApproachingPrey();
                    preyVisible = true;
                    preyWasVisible = true; //Always true once prey is seen for the first time
                }
                else
                {
                    preyVisible = false;
                }
            }
        }    
        

        //Add obstacles observations
        AddVectorObs(rayData1);

        //Debug.Log("arenaSize = " + arenaSize);

        //Add predator positional observations
        /*
        AddVectorObs((this.transform.position.x + arenaSize) / arenaSize);
        AddVectorObs((this.transform.position.x - arenaSize) / arenaSize);
        AddVectorObs((this.transform.position.z + arenaSize) / arenaSize);
        AddVectorObs((this.transform.position.z - arenaSize) / arenaSize);
        */

        AddVectorObs(this.transform.position.x / arenaSize);
        AddVectorObs(this.transform.position.z / arenaSize);

        if (Time.frameCount % 60 == 0)
        {
            //Debug.Log("norm Xpos = " + this.transform.position.x / arenaSize);
            //Debug.Log("norm Zpos = " + this.transform.position.z / arenaSize);
            //Debug.Log("reg Xpos = " + this.transform.position.x);
            //Debug.Log("reg Zpos = " + this.transform.position.z);
            //Debug.Log("Current Distance To Prey = " + currentDistanceToPrey/70);
        }

        //If prey is visible add prey positional observation relative to predator
        if (preyVisible)
        {
            AddVectorObs(1);
            Vector3 preyRelativePosition = myPrey.transform.position - this.transform.position;
            AddVectorObs(preyRelativePosition.x / (arenaSize*2));
            AddVectorObs(preyRelativePosition.z / (arenaSize*2));
            AddVectorObs(currentDistanceToPrey/70); //WARNING: 70 is hard coded from: Arena width=50, sqrt(50^2 + 50^2)

            if (Time.frameCount % 60 == 0)
            {
                //Debug.Log("preyRelativePosition.x = " + preyRelativePosition.x / arenaSize);
                //Debug.Log("preyRelativePosition.z = " + preyRelativePosition.z / arenaSize);
            }
        }
        else
        {
            AddVectorObs(0);
            AddVectorObs(0); //Is this a good way of saying the predator does not know the prey location?
            AddVectorObs(0);
            AddVectorObs(0);
        }

        updateRewards();

        Monitor.Log("Reward", GetCumulativeReward(), MonitorType.text, monitor.transform);
    }
    
    bool checkIfApproachingPrey()
    {
        bool approachingPrey = false;
        float distanceToPrey;
        float changeInDistanceToPrey;
        Vector3 predatorPosition = this.transform.position;
        Vector3 preyPosition = myPrey.transform.position;

        distanceToPrey = Vector3.Distance(predatorPosition,preyPosition);

        /*
        changeInDistanceToPrey = currentDistanceToPrey - distanceToPrey; //Positive if the predator is getting closer

        if (Time.frameCount % 2 == 0)
        {
            Debug.Log("changeInDistanceToPrey = " + changeInDistanceToPrey);
            Debug.Log("Rounded: new = " + Mathf.Round(distanceToPrey) + " old= " + Mathf.Round(currentDistanceToPrey));
        }
        */

        if (Mathf.Round(distanceToPrey) < Mathf.Round(currentDistanceToPrey))
        {
            approachingPrey = true;
        }

        currentDistanceToPrey = distanceToPrey;

        return approachingPrey;          
    }

    public void getMyPrey()
    {
        myPrey = arena.transform.Find("Prey").gameObject;
    }

    private void updateRewards()
    {
        //If the prey is visible and the predator is approaching: + Reward
        if(preyVisible && approachingPrey)
        {
            AddReward(0.1f);
        }

        //If the prey is visible and the predator is not approaching: - Reward
        if(preyWasVisible && !approachingPrey){
            //AddReward(-0.01f);
        }

        //If the prey was visible and is no longer visible: - Reward
        if(!preyVisible && preyWasVisible)
        {
            AddReward(-0.001f);
        }

        //Could add a small negative reward each time to hurry things up!

        wasApproachingPrey = approachingPrey;
        //preyWasVisible = preyVisible;

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
				foundNewSpawnLocation = true;
			}
		}
        /*
        Debug.Log("terrainBounds.min.x= " + terrainBounds.min.x);
        Debug.Log("terrainBounds.max.x= " + terrainBounds.max.x);

        Debug.Log("terrainBounds.min.z= " + terrainBounds.min.z);
        Debug.Log("terrainBounds.max.z= " + terrainBounds.max.z);


        Debug.Log("randomSpawnPos.x= " + randomSpawnPos.x);
        Debug.Log("randomSpawnPos.y= " + randomSpawnPos.y);
        Debug.Log("randomSpawnPos.z= " + randomSpawnPos.z);
        */

        return randomSpawnPos;
	}

	/// <summary>
	/// Called when the agent touches the prey.
	/// </summary>
	public void caughtPrey()
	{
        //int framesToCatchPrey = Time.frameCount - initialFrameCount;
        //Debug.Log("caughtPrey: framesToCatchPrey= " + framesToCatchPrey);

        //float adjustedReward = 1.0f - ((framesToCatchPrey / initialDistanceToPrey)/30);

        // We use a reward of 5.
        //AddReward(adjustedReward);
        AddReward(50.0f);

		// By marking an agent as done AgentReset() will be called automatically.
		Done();
	}

    public void movedForward()
    {
        //AddReward(0.0001f);
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
        

        //dirToGo = transform.forward * movement;

        //rotateDir = transform.up * rotation;

        
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
		//MoveAgent(vectorAction);

		//float distanceToPrey = Vector3.Distance (this.transform.position, Target, position);
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
        myPrey.GetComponent<preyAgent>().AgentReset();
	}
		
}
