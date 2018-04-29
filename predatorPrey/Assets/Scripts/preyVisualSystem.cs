using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class preyVisualSystem : VisualSystem {

    private float cumulativeSwarmDensity;
    private int currentSwarmDensity;
    private int swarmDensitySamples;

    //Checks to see if this object is a member of this object's swarm
    private bool memberOfSwarm(GameObject potentialMember)
    {
        float distanceApart = Vector3.Distance(this.transform.parent.transform.position, potentialMember.transform.position);
        //Debug.Log("Distance from potential swarm member to me = " + distanceApart);

        if (distanceApart <= 30)
        {
            return true;
        }

        return false;
    }

    //Sets the current swarm density and adds the density to a running total of cumulative swarm density and increments the sample counter.
    //This book keeping will make it easy to get average swarm density for the simulation later.
    private void setCurrentSwarmDensity(int swarmDensity)
    {
        currentSwarmDensity = swarmDensity;
        cumulativeSwarmDensity += swarmDensity;
        swarmDensitySamples++;
        //Debug.Log("current swarm density = " + currentSwarmDensity);
    }

    public int getCurrentSwarmDensity()
    {
        return currentSwarmDensity;
    }

    public float getAverageSwarmDensity()
    {
        return cumulativeSwarmDensity / swarmDensitySamples;
    }

    public float[] processVisualFeedback(Dictionary<float, GameObject> detectedObjects)
    {
        //Get fresh new array
        sliceStates = new bool[slices, 2];
        int newSwarmDensity = 0;

        foreach (KeyValuePair<float, GameObject> kvp in detectedObjects)
        {
            float nextAngle = kvp.Key;
            GameObject nextObject = kvp.Value;
            int nextSlice = angleToSlice(nextAngle);

            //If the next angle is not in the field of view, skip it
            if (Mathf.Abs(nextAngle) > fieldOfView / 2)
            {
                continue;
            }

            //If angles is negative, increment next slice
            if (nextAngle < 0)
            {
                nextSlice += visibleSlices / 2;
            }

            if (nextObject.tag == "predator")
            {
                sliceStates[nextSlice, 0] = true;
            }

            if (nextObject.tag == "prey")
            {
                sliceStates[nextSlice, 1] = true;

                if (memberOfSwarm(nextObject))
                {
                    newSwarmDensity++;
                }
            }
        }

        setCurrentSwarmDensity(newSwarmDensity);
        updateIndicators();

        return convertSliceStatesToFloats(sliceStates);
    }
}
