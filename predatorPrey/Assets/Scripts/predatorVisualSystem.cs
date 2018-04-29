using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class predatorVisualSystem : VisualSystem {

    public List<GameObject> visiblePrey = new List<GameObject>();

    //Process the visible objects to update retinal slice states and check for attack opportunities
	public float[] processVisualFeedback(Dictionary<float, GameObject> detectedObjects)
    {
        //Get fresh new array
        sliceStates = new bool[slices, 2];
        visiblePrey.Clear();

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

            if (nextObject.tag == "prey")
            {
                sliceStates[nextSlice, 1] = true;
                //Cant call this without processing all prey because wont get right swarm size
                //preyCloseEnoughToAttack = isCloseEnoughToAttack(nextObject);
                visiblePrey.Add(nextObject);
            }
        }

        updateIndicators();

        return convertSliceStatesToFloats(sliceStates);
    }
}
