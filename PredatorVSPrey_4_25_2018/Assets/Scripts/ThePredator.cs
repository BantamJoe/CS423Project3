using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThePredator : Agent {
	private PredatorPreyAcademy academy;
	public float timeBetweenDecisionsAtInference;

    private VisualSystem visualSystem;
    public float sightDistance;

    private float timeSinceDecision;
	bool caughtPrey = false;
	bool out_of_bounds = false;
	// Use this for initialization
	void Start () 
	{
		
	}
	public override void InitializeAgent()
	{
		academy = FindObjectOfType(typeof(PredatorPreyAcademy)) as PredatorPreyAcademy;
        visualSystem = this.transform.Find("visualIndicators").GetComponent<VisualSystem>();
    }

/************************************************************************************************************/
/* Here we should reset the Predator if:								  
/*	1. The Predator kills the prey										   
/*	2. The Predator doesnt reach the prey in 2000 simulation steps         
/***********************************************************************************************************/
	public Transform Prey;
	public override void AgentReset()
	{
		
		if (caughtPrey == true) 
		{
			// move the predator to a new random position
			//this.transform.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);

			// Here we should add code that removes(hides) the killed prey, to simulate it being killed
			this.transform.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
			//academy.AcademyReset();
		}

		/** Currently there is a bug where the predator(cube) cant detect the walls and therfore goes right throught it **/
		/** So i just reset the cubes position if it goes outside the bounds **/
		if (OutOfBounds() == true) 
		{
			//this.transform.position = new Vector3(0.0f, 0.5f, 0.0f);
		}

	
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

            if (nextObjectTag != "prey") //Changing this in the prey agent script will allow it to detect predators too
            {
                continue;
            }

            Vector3 nextObjectPosition = nextObject.transform.position;
            float angleTowardObject = Vector3.SignedAngle(this.transform.forward * -1, this.transform.position - nextObjectPosition, this.transform.up);
            //Debug.Log("Adding: " + nextObjectTag + " towards: " + angleTowardObject);
            visibleObjects.Add(angleTowardObject, nextObjectTag);
        }

        return visualSystem.processVisualFeedback(visibleObjects);
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
		if (action == 0) {
			targetPos = this.transform.position + new Vector3 (0f, 0, 1f);
			this.transform.position = targetPos;
			//Debug.Log ("Forward");
		}

		if (action == 1) {
			targetPos = this.transform.position + new Vector3 (0f, 0, -1f);
			this.transform.position = targetPos;
			//Debug.Log ("Down");
		}
		if (action == 2) {
			targetPos = this.transform.position + new Vector3 (-1f, 0, 0f);
			this.transform.position = targetPos;
			//Debug.Log ("Left");
		}
		if (action == 3) {
			targetPos = this.transform.position + new Vector3 (1f, 0, 0f);
			this.transform.position = targetPos;
			//Debug.Log ("Right");
		}
	
		if (distanceToPrey < 3.5f) {
			caughtPrey = true;
			Debug.Log (" We caught the prey");
			Done ();
			AddReward (1.0f);
		} else {
			caughtPrey = false;
		}

		// We got close to the prey
		previousDistance = distanceToPrey;
		if (distanceToPrey < previousDistance) {
			AddReward (0.1f);
		}
			
			
		//Time penalty
		AddReward (-0.01f);

	}
/******************************************************************************************************/
/**                 Returns true if the predator goes outside the arena walls                        **/
/******************************************************************************************************/
	public bool OutOfBounds()
	{
		if (this.transform.position.x < -4.7f || this.transform.position.x > 4.6f)
		{
			
			return true;
		}

		if (this.transform.position.z < -4.4f || this.transform.position.z > 4.7f) 
		{

			return true;
		}

		return false;
	}
/******************************************************************************************************/
/**              These functions are useful if you want tick the "On Demad Decisions"                **/
/******************************************************************************************************/
	/*
	public void FixedUpdate()
	{
		WaitTimeInference();
	}

	private void WaitTimeInference()
	{
		if (!academy.GetIsInference())
		{
			RequestDecision();
		}
		else
		{
			if (timeSinceDecision >= timeBetweenDecisionsAtInference)
			{
				timeSinceDecision = 0f;
				RequestDecision();
			}
			else
			{
				timeSinceDecision += Time.fixedDeltaTime;
			}
		}
	}

  */
}
