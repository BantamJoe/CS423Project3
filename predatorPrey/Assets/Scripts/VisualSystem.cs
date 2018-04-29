using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualSystem : MonoBehaviour {

    public GameObject indicator;
    public int fieldOfView = 180;
    public int slices;
    public float distanceFromCenter;
    public Color preyDetectedColor;
    public Color nothingDetectedColor;
    public Color bothDetectedColor;
    public Color predatorDetectedColor;

    protected Transform parent;

    protected float sliceAngle;
    protected float halfSliceAngle;
    protected int visibleSlices;
    protected int slicesPerQuadrant;

    protected GameObject[] indicatorsRight;
    protected GameObject[] indicatorsLeft;
    protected Vector3[] indicatorPositions;

    protected bool[,] sliceStates;

	// Use this for initialization
	void Start () {

        parent = this.transform;
        sliceAngle = 360 / slices;
        halfSliceAngle = sliceAngle / 2;
        slicesPerQuadrant = slices / 4;
        visibleSlices = Mathf.RoundToInt(fieldOfView / sliceAngle);

        indicatorsRight = new GameObject[visibleSlices/2];
        indicatorsLeft = new GameObject[visibleSlices/2];
        indicatorPositions = new Vector3[visibleSlices];

        //Always has to remain the same for the vector space of the brain
        sliceStates = new bool[slices, 2]; //[slice index, [predator, prey]] 

        calculateIndicatorPositions();
        createIndicators();		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Returns an array of floats. Takes [slice,[prey, predator]] -> [val]. Formatted as: predator, prey, predator, prey, ....
    protected float[] convertSliceStatesToFloats(bool[,] sliceStates)
    {
        float[] sliceValues = new float[slices * 2];
        int index = 0;

        for (int i=0; i<sliceStates.Length/2; i++)
        {
            sliceValues[index] = sliceStates[i, 0] ? 1 : 0;
            index++;
            sliceValues[index] = sliceStates[i, 1] ? 1 : 0;
            index++;
        }

        return sliceValues;        
    }

    protected int angleToSlice(float angle)
    {
        int targetSlice;

        if (angle >= 0)
        {
            targetSlice = Mathf.FloorToInt(angle / sliceAngle);

            if (targetSlice >= visibleSlices / 2)
            {
                targetSlice--;
            }            
        }
        else
        {
            targetSlice = Mathf.CeilToInt(angle / sliceAngle) * -1;

            if (targetSlice >= visibleSlices / 2)
            {
                targetSlice--;
            }
        }        

        return targetSlice;
    }

    protected void updateIndicators()
    {
        int index;

        //Update all indicators
        for (int i = 0; i < visibleSlices; i++)
        {

            //Update indicators on the right side
            if (i < visibleSlices / 2)
            {
                index = i;
                if (sliceStates[i, 0] && sliceStates[i, 1])  //If this slice detected 1+ predator(s) AND 1+ prey 
                {
                    indicatorsRight[index].GetComponent<SpriteRenderer>().color = bothDetectedColor;
                }
                else if (sliceStates[i, 0])     //If this slice detected 1+ predator(s)
                {
                    indicatorsRight[index].GetComponent<SpriteRenderer>().color = predatorDetectedColor;
                }
                else if (sliceStates[i, 1])     //If this slice detected 1+ prey
                {
                    indicatorsRight[index].GetComponent<SpriteRenderer>().color = preyDetectedColor;
                }
                else      //If this slice detected nothing
                {
                    indicatorsRight[index].GetComponent<SpriteRenderer>().color = nothingDetectedColor;
                }

            }
            else
            {
               index = i - (visibleSlices / 2);
                //Debug.Log("index = " + index);
                if (sliceStates[i, 0] && sliceStates[i, 1])  //If this slice detected 1+ predator(s) AND 1+ prey 
                {
                    indicatorsLeft[index].GetComponent<SpriteRenderer>().color = bothDetectedColor;
                }
                else if (sliceStates[i, 0])     //If this slice detected 1+ predator(s)
                {
                    indicatorsLeft[index].GetComponent<SpriteRenderer>().color = predatorDetectedColor;
                }
                else if (sliceStates[i, 1])     //If this slice detected 1+ prey
                {
                    indicatorsLeft[index].GetComponent<SpriteRenderer>().color = preyDetectedColor;
                }
                else      //If this slice detected nothing
                {
                    indicatorsLeft[index].GetComponent<SpriteRenderer>().color = nothingDetectedColor;
                }
            }
            //Debug.Log("Slice: [" + i + ", " + sliceStates[i, 0] + " , " + sliceStates[i, 1] + "] : index = " + index);
        }

        //indicatorsRight[0].GetComponent<SpriteRenderer>().color = Color.red;
        //indicatorsLeft[0].GetComponent<SpriteRenderer>().color = Color.red;
    }

    protected void createIndicators()
    {
        Quaternion indicatorRotation = Quaternion.Euler(-90f,0f,0f);

        for(int i=0; i<visibleSlices; i++)
        {
            if (i < visibleSlices / 2)
            {
                indicatorsRight[i] = Instantiate(indicator, parent, false);
                indicatorsRight[i].transform.Translate(indicatorPositions[i].x, 2f, indicatorPositions[i].z);
                indicatorsRight[i].transform.Rotate(Vector3.right, -90, Space.Self);
                indicatorsRight[i].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.25f);
            }
            else
            {
                int index = i - (visibleSlices / 2);
                indicatorsLeft[index] = Instantiate(indicator, parent, false);
                indicatorsLeft[index].transform.Translate(indicatorPositions[i].x, 2f, indicatorPositions[i].z);
                indicatorsLeft[index].transform.Rotate(Vector3.right, -90, Space.Self);
                indicatorsLeft[index].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.25f);
            }   
        }
    }

    protected void calculateIndicatorPositions()
    {
        int index = 0;

        //Generate right half of indicator positions
        for (float i = halfSliceAngle; i < fieldOfView/2; i += sliceAngle)
        {
            float nextX = (Mathf.Cos(i * Mathf.Deg2Rad)) * distanceFromCenter;
            float nextZ = (Mathf.Sin(i * Mathf.Deg2Rad)) * distanceFromCenter;
            indicatorPositions[index] = new Vector3(nextX, 2f, nextZ);
            index++;
        }
        
        //Generate other half of indicator positions
        for (int i = 0; i < visibleSlices/2; i++)
        {
            float nextX = indicatorPositions[i].x;
            float nextZ = indicatorPositions[i].z*-1;
            indicatorPositions[index] = new Vector3(nextX, 2f, nextZ);
            index++;
        }
    }
}
