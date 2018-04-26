using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualSystem : MonoBehaviour {

    public GameObject indicator;
    public int slices;
    public float distanceFromCenter;
    public Color predatorDetectedColor;
    public Color preyDetectedColor;
    public Color bothDetectedColor;
    public Color nothingDetectedColor;

    private Transform parent;
    private float sliceAngle;
    private float halfSliceAngle;
    private int slicesPerQuadrant;
    private GameObject[] indicatorsRight;
    private GameObject[] indicatorsLeft;
    private Vector3[] indicatorPositions;

    private bool[,] sliceStates;

	// Use this for initialization
	void Start () {

        parent = this.transform;
        sliceAngle = 360 / slices;
        halfSliceAngle = sliceAngle / 2;
        slicesPerQuadrant = slices / 4;
        indicatorsRight = new GameObject[slices/2];
        indicatorsLeft = new GameObject[slices/2];
        indicatorPositions = new Vector3[slices];
        sliceStates = new bool[slices, 2]; //[slice index, [predator, prey]]

        calculateIndicatorPositions();
        createIndicators();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool[,] processVisualFeedback(Dictionary<float, string> detectedObjects)
    {
        //Get fresh new array
        sliceStates = new bool[slices, 2];

        foreach(KeyValuePair<float, string> kvp in detectedObjects){
            float nextAngle = kvp.Key;
            string nextObject = kvp.Value;
            int nextSlice = angleToSlice(nextAngle);

            //If angles is negative, increment next slice
            if(nextAngle < 0)
            {
                nextSlice += slices / 2;
            }

            if (nextObject == "predator")
            {
                sliceStates[nextSlice, 0] = true;
            }

            if (nextObject == "prey")
            {
                sliceStates[nextSlice, 1] = true;
            }
        }

        updateIndicators();

        return sliceStates;
    }

    private int angleToSlice(float angle)
    {
        int targetSlice;

        if (angle >= 0)
        {
            targetSlice = Mathf.FloorToInt(angle / sliceAngle);

            if (targetSlice >= slices / 2)
            {
                targetSlice--;
            }            
        }
        else
        {
            targetSlice = Mathf.CeilToInt(angle / sliceAngle) * -1;

            if (targetSlice >= slices / 2)
            {
                targetSlice--;
            }            
        }

        return targetSlice;
    }

    public void updateIndicators()
    {
        int index;

        //Update all indicators
        for (int i = 0; i < slices; i++)
        {

            //Update indicators on the right side
            if (i < slices / 2)
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
               index = i - (slices / 2);
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
    }

    void createIndicators()
    {
        Quaternion indicatorRotation = Quaternion.Euler(-90f,0f,0f);

        for(int i=0; i<slices; i++)
        {
            if (i < slices / 2)
            {
                indicatorsRight[i] = Instantiate(indicator, parent, false);
                indicatorsRight[i].transform.Translate(indicatorPositions[i].x, 2f, indicatorPositions[i].z);
                indicatorsRight[i].transform.Rotate(Vector3.right, -90, Space.Self);
                indicatorsRight[i].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.25f);
            }
            else
            {
                int index = i - (slices / 2);
                indicatorsLeft[index] = Instantiate(indicator, parent, false);
                indicatorsLeft[index].transform.Translate(indicatorPositions[i].x, 2f, indicatorPositions[i].z);
                indicatorsLeft[index].transform.Rotate(Vector3.right, -90, Space.Self);
                indicatorsLeft[index].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.25f);
            }   
        }
    }

    void calculateIndicatorPositions()
    {
        int index = 0;

        //Generate right half of indicator positions
        for (float i = halfSliceAngle; i < 180; i += sliceAngle)
        {
            float nextX = (Mathf.Cos(i * Mathf.Deg2Rad)) * distanceFromCenter;
            float nextZ = (Mathf.Sin(i * Mathf.Deg2Rad)) * distanceFromCenter;
            indicatorPositions[index] = new Vector3(nextX, 2f, nextZ);
            index++;
        }
        
        //Generate other half of indicator positions
        for (int i = 0; i < slices/2; i++)
        {
            float nextX = indicatorPositions[i].x;
            float nextZ = indicatorPositions[i].z*-1;
            indicatorPositions[index] = new Vector3(nextX, 2f, nextZ);
            index++;
        }
    }
}
