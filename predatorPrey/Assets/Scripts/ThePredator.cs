using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThePredator : Agent {
	private MainAcademy academy;
	public float timeBetweenDecisionsAtInference;

    private predatorVisualSystem visualSystem;
    public float sightDistance;
    public float turnDegree;

    private GameObject currentTargetPrey;
    private GameObject lastKill;
    private int killCount; 

    private int  timeSinceLastAttack;
    private float timeSinceDecision;	
    private bool isCoolDown = false;

    public Transform Prey;
    private List<GameObject> killedPrey;

	private GameObject arena;
	private float arenaRadius;

	public GameObject monitor;


	Vector3 startingPosition;
	// Use this for initialization
	void Start () 
	{
		killedPrey = new List <GameObject>();
		startingPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
	}
	public override void InitializeAgent()
	{
		academy = FindObjectOfType(typeof(MainAcademy)) as MainAcademy;
        visualSystem = this.transform.Find("predatorVisualSystem").GetComponent<predatorVisualSystem>();
		arena = GameObject.Find("Arena");
		arenaRadius = arena.GetComponent<MeshCollider>().bounds.extents.x;
    }
/************************************************************************************************************/
/* Here we should reset the Predator if:								  
/*	1. The Predator kills the prey										   
/*	2. The Predator doesnt reach the prey in 2000 simulation steps         
/***********************************************************************************************************/

	public override void AgentReset()
	{
		if(killedPrey != null)
        {
            //Debug.Log(killedPrey.Count);
            foreach(GameObject prey in killedPrey)
            {
                prey.SetActive(true);
            }
        }
		currentTargetPrey = null;
		lastKill = null;
		killCount = 0;
		/*
		//reset position of predator
		if (isOutOfBounds () == true) 
		{
			
		
		    this.transform.position = startingPosition;
		}
		killCount = 0;
		*/
	}
/**************************************************************************************************************/
/* The predator should collect these observations 							 
/* 1.																			
/* 
/* 
/* 
/*************************************************************************************************************/	
	public override void CollectObservations()
	{
        AddVectorObs(detectVisibleObjects());
		AddVectorObs((this.transform.position.x) / arenaRadius);
		AddVectorObs((this.transform.position.z) / arenaRadius);

		Monitor.Log("Reward", GetCumulativeReward(), MonitorType.text, monitor.transform);

    }

    float[] detectVisibleObjects()
    {
        Collider[] visibleObjectColliders = Physics.OverlapSphere(this.transform.position, sightDistance);
        Dictionary<float, GameObject> visibleObjects = new Dictionary<float, GameObject>();

        for (int i = 0; i < visibleObjectColliders.Length; i++)
        {
            GameObject nextObject = visibleObjectColliders[i].gameObject;

            if (nextObject.tag != "prey") //Changing this in the prey agent script will allow it to detect predators too
            {
                continue;
            }

            Vector3 nextObjectPosition = nextObject.transform.position;
            float angleTowardObject = Vector3.SignedAngle(this.transform.forward * -1, this.transform.position - nextObjectPosition, this.transform.up);
			if (!visibleObjects.ContainsKey(angleTowardObject))
			{
				visibleObjects.Add(angleTowardObject, nextObject);
			}
            //Debug.Log("Adding: " + nextObjectTag + " towards: " + angleTowardObject);
            //visibleObjects.Add(angleTowardObject, nextObject);
        }

        return visualSystem.processVisualFeedback(visibleObjects);
    }

    //Check to see if the predator is close enough to any of the visible prey to attempt an attack
    //Returns a prey target that is close enough to attack or null if it is not close enough to any prey
    private GameObject isCloseEnoughToAttack()
    {
        GameObject targetPrey = null;

        visualSystem.visiblePrey.ForEach(delegate (GameObject nextPrey)
        {
            float distanceToPrey = Vector3.Distance(this.transform.position, nextPrey.transform.position);
            //Debug.Log("distance to prey = " + distanceToPrey);
            //if (distanceToPrey <= 5)
            if (distanceToPrey <= 5f)
            {
                //Debug.Log("Found prey close enough to attack!");
                targetPrey = nextPrey;
            }
        });

        return targetPrey;
    }

    //Give the probability of successfully capturing the prey. The influence of swarm size simulates the predator confusion effect.
    public float getPreyCaptureProbability(GameObject targetPrey)
    {
        int preyCloseToTarget = 0;

        visualSystem.visiblePrey.ForEach(delegate (GameObject prey)
        {
            float distanceBetweenPrey = Vector3.Distance(targetPrey.transform.position, prey.transform.position);

            //The prey will count itself as part of the swarm. This avoids dividing by zero below.
            if (distanceBetweenPrey <= 30)
            {
                preyCloseToTarget++;
            }
        });

        //Debug.Log("prey close to target = " + preyCloseToTarget);
        float captureProbability = 1f / preyCloseToTarget;
        //Debug.Log("preyCaptureProbability = " + captureProbability);
        return captureProbability;
    }

    //True if the attack was successful, otherwise false
    public bool attackPrey(GameObject targetPrey)
    {
        float chanceOfSuccess = getPreyCaptureProbability(targetPrey);
        float outcome = Random.value;

        if (outcome <= chanceOfSuccess)
        {
            lastKill = targetPrey;
            Debug.Log("attack success: chance of success = " + chanceOfSuccess + " , outcome = " + outcome);
            return true;
        }
        Debug.Log("attack fail: chance of success = " + chanceOfSuccess + " , outcome = " + outcome);
        return false;
    }

    /******************************************************************************************************/
    /******************************************************************************************************/
    public float speed = 10;
	private float previousDistance = float.MaxValue;
	public override void AgentAction(float[] vectorAction,string textAction)
	{
       
		int action = Mathf.FloorToInt (vectorAction [0]);
		float distanceToPrey = Vector3.Distance (this.transform.position, Prey.transform.position);

		Vector3 targetPos = this.transform.position;

        bool attackResult = false;
      
        //If Action == 0, then do nothing, otherwise move.
        if (action > 0)
        {
            //Rotate if turning
            if (action == 2)
            {
                this.transform.Rotate(Vector3.up, turnDegree * -1);
                //Debug.Log("Left");
            }
            else if (action == 3)
            {
                this.transform.Rotate(Vector3.up, turnDegree);
                //Debug.Log("Left");
            }

            //Move forward
            this.transform.position += this.transform.forward;
            //Debug.Log("Forward");
        }

        //Put the attack cooldown code here

        //Try to target a nearby prey
        currentTargetPrey = isCloseEnoughToAttack();
     
		if(visualSystem.visiblePrey.Count > 0)
		{
			AddReward(0.01f);
		}
      
        //If there is a prey close enough to attack, and we arent on cooldown
        if (currentTargetPrey != null && isCoolDown == false)
        {
			AddReward (0.3f);
           // We are in attack phase
           if(isCoolDown == false)
           { 
              attackResult = attackPrey(currentTargetPrey);

              if(attackResult)
              {
                 killCount++; 
                 
                 currentTargetPrey.SetActive(false); // disable the killed prey

                 killedPrey.Add(currentTargetPrey); // add the killed prey to a list

                 isCoolDown = true; // Will go on cool down for 10 simulation steps

                 AddReward(1.0f);

                 Debug.Log("kill count = " + killCount);
              }
              //if we failed to kill something 
              else
              {
                isCoolDown = true;
                AddReward(-1.0f);
              }

            }
        }
        //Debug.Log(GetStepCount());
        //small positive reward for digesting?
        if(isCoolDown == true)
        {
            timeSinceLastAttack++;
            if(timeSinceLastAttack == 10)
            {
                isCoolDown = false;
                timeSinceLastAttack = 0;
            }
        }

		if(isOutOfBounds() == true)
		{
			AddReward (-0.5f);
			Done();
		}
			

        //time penalty
        AddReward(-0.02f);    
    }

	public bool isOutOfBounds()
	{
		if (this.transform.position.x > 250 || this.transform.position.x < -250) 
		{
			return true;
		}

		if (this.transform.position.z > 250 || this.transform.position.z < -250) {
			return true;
		} 
		else 
		{
			return false;

		}

	}
}
