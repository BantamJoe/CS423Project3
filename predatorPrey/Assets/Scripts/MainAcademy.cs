using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAcademy : Academy 
{
    private GameObject[] preyRespawns;
    private GameObject[] predatorRespawns;

    private GameObject preyPrefab;
    private GameObject predatorPrefab;

    private  ThePredator thePredator;

    public override void InitializeAcademy()
    {
    	
    }

    public override void AcademyReset()
    {
    	//Debug.Log(thePredator.killedPrey.Count);
       
       //RespawnPrey();
       //RespawnPredator();
    }

    void RespawnPrey()
    {
    	if (preyRespawns == null)
        {
            preyRespawns = GameObject.FindGameObjectsWithTag("prey");
          
        }

        foreach (GameObject respawn in preyRespawns)
        {
        	respawn.SetActive(true);
            //Instantiate(preyPrefab, respawn.transform.position, respawn.transform.rotation);
        }
        //swarm.SetActive(true)

    }

    void RespawnPredator()
    {
    	if (predatorRespawns == null)
        {
            predatorRespawns = GameObject.FindGameObjectsWithTag("predator");
        }

        foreach (GameObject respawn in predatorRespawns)
        {
        	respawn.SetActive(true);
            //Instantiate(predatorPrefab, respawn.transform.position, respawn.transform.rotation);
        }

    }

		
}
