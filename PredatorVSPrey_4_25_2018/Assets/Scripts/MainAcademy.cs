using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAcademy : Academy {
	[HideInInspector]
	public List<GameObject> prey;
	[HideInInspector]
	public int[] players;

	public GameObject predator;


	public GameObject agentPref;
	public GameObject preyPref;

	GameObject[] objects;



	public override void InitializeAcademy()
	{
		objects = new GameObject[2] {agentPref, preyPref};
	}
	public override void AcademyReset()
	{
		foreach (GameObject actor in prey)
		{
			DestroyImmediate(actor);
		}

		prey.Clear();

		HashSet<int> numbers = new HashSet<int>();

		/** Reset the position of all the prey **/
		for (int i = 0; i < players.Length; i++)
		{

			GameObject actorObj = Instantiate(objects[players[i]]);
			actorObj.transform.position = new Vector3 (Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
			prey.Add(actorObj);
		}

		// Reset the position of the predator
		predator.transform.position = new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);

	}
		
}
