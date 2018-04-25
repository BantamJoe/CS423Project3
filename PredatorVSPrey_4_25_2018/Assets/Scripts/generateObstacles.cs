using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generateObstacles : MonoBehaviour {

    public Transform tree;
    public Transform rock;
    public float obstacleFill;  //50-100
    public float treeToRockRatio;
    Vector3 arenaPos;
    

    // Use this for initialization
    void Start () {
        arenaPos = this.transform.position;        
    }

    void placeObstacles ()
    {
        /*
        int innerArenaWidth = 1;
        int innerArenaHeight = 1;

        for (int x = 20; x < dimension - 20; x++)
        {
            for (int z = 20; z < dimension - 20; z++)
            {
                float nextTreeRoll = Random.Range(0.0f, 100000);

                if (nextTreeRoll < obstacleFill)
                {
                    float posY = terrain.SampleHeight(new Vector3(x, 0, z));

                    //Stops obstacles from being placed on arena walls - for some reason an obstacle still appears on the wall very rarely....
                    int wallOffsetX = (x > innerArenaWidth) ? x % innerArenaWidth : innerArenaWidth % x;
                    int wallOffsetZ = (z > innerArenaHeight) ? z % innerArenaHeight : innerArenaHeight % z;

                    if (wallOffsetX > 10 || wallOffsetZ > 10 )
                    {
                        Instantiate(tree, new Vector3(terrainPos.x + x, posY + 9, terrainPos.z + z), Quaternion.Euler(new Vector3(0, 0, 0)));
                    }        
                }
            }
        }
        */
    }

    void placeTrees(float[,] height)
    {
        /*
        float nextPerlin;
        int perlinSeed = Random.Range(0, 10000);

        for (int x = 50; x < dimension-50; x++)
        {
            for (int z = 50; z < dimension-50; z++)
            {
                float nextTreeRoll = Random.Range(0.0f, 100000);

                if(nextTreeRoll < obstacleFill)
                {
                    float posY = terrain.SampleHeight(new Vector3(x, 0, z));
                    float obstacleRoll = Random.Range(0.0f, 1.0f);

                    if(obstacleRoll > treeToRockRatio)
                    {
                        Instantiate(tree, new Vector3(terrainPos.x + x, posY, terrainPos.z + z), Quaternion.Euler(new Vector3(-90, Random.Range(-180, 180), 0)));
                    }
                    else
                    {
                        Instantiate(rock, new Vector3(terrainPos.x + x, posY, terrainPos.z + z), Quaternion.Euler(new Vector3(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180))));
                    }

                    
                }
            }
        }
        */
    }
	
	// Update is called once per frame
	void Update () {
		
	}

}
