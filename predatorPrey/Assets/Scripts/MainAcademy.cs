using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAcademy : Academy 
{
	private GameObject arena;

	private GameObject[] predator;
	private GameObject[] prey;
	public int arenaSpawnBuffer; //So that we don't spawn agents right next to the edge

	private float arenaRadius;

	public override void InitializeAcademy()
	{
		predator = GameObject.FindGameObjectsWithTag("predator");
		prey = GameObject.FindGameObjectsWithTag("prey");
		arena = GameObject.Find("Arena");
		arenaRadius = arena.GetComponent<MeshCollider>().bounds.size.x-arenaSpawnBuffer;
	}
	public override void AcademyReset()
	{
		GameObject nextPrey;

		/** Reset the position of all the prey **/
		for (int i = 0; i < prey.Length; i++)
		{
			nextPrey = prey[i];

			nextPrey.SetActive(true);
			nextPrey.transform.Rotate(Vector3.up, Random.Range(-180, 180));
			nextPrey.transform.position = getNewSpawnPoint();
		}

		//Reset the position of the predator. Array should have length=1;
		for (int i = 0; i < predator.Length; i++)
		{
			// Reset the position of the predator
			predator[i].transform.Rotate(Vector3.up, Random.Range(-180, 180));
			predator[i].transform.position = getNewSpawnPoint();
		}


	}

	private Vector3 getNewSpawnPoint()
	{

		Vector3 spawnPoint = new Vector3((Random.value * arenaRadius)-arenaRadius/2, 0.5f, (Random.value * arenaRadius) - arenaRadius / 2);

		return spawnPoint;
	}
		
}
