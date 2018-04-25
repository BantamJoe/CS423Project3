using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class generateArena : MonoBehaviour {

    public int dimension;       //512
    public int rows;            //3
    public int columns;         //3
    public float wallHeight;    //0.1
    public float heightScale;   //0.05
    public float refinement;    //0.01
    public Transform tree;
    public Transform rock;
    public float obstacleFill;  //50-100
    public float treeToRockRatio;
    public Terrain terrain;     //The terrain object this script is attached to
    Vector3 terrainPos;
    

    // Use this for initialization
    void Start () {

        TerrainData TD = terrain.terrainData;
        terrainPos = terrain.transform.position;

        generateTerrain(TD);
        
    }

    void generateTerrain(TerrainData TD)
    {
        float[,] perlinMap = new float[dimension, dimension];
        float nextPerlin;
        int perlinSeed = Random.Range(0, 10000);
        int innerArenaWidth = dimension / columns;
        int innerArenaHeight = dimension / rows;

        for (int x = 0; x < dimension; x++)
        {
            for (int y = 0; y < dimension; y++)
            {              
                nextPerlin = Mathf.PerlinNoise((x + perlinSeed) * refinement, (y + perlinSeed) * refinement);
                perlinMap[x, y] = nextPerlin * heightScale;                  
            }
        }

        TD.SetHeights(0, 0, perlinMap);

        placeWalls();
        placeRectangles();
    }

    void placeWalls()
    {
        int innerArenaWidth = dimension / columns;
        int innerArenaHeight = dimension / rows;

        for (int x = 0; x <= dimension; x += innerArenaWidth)
        {
            GameObject nextCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            nextCube.name = "wall";
            nextCube.tag = "obstacle";
            nextCube.transform.localScale = new Vector3(2f, wallHeight, dimension);
            nextCube.transform.position = new Vector3(terrainPos.x + x, wallHeight/2, terrainPos.z + dimension /2);
        }

        for (int z = 0; z <= dimension; z += innerArenaHeight)
        {
            GameObject nextCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            nextCube.name = "wall";
            nextCube.tag = "obstacle";
            nextCube.transform.localScale = new Vector3(dimension, wallHeight, 2f);
            nextCube.transform.position = new Vector3(terrainPos.x + dimension /2, wallHeight/2, terrainPos.z + z);
        }
    }

        void placeRectangles ()
    {
        int innerArenaWidth = dimension / columns;
        int innerArenaHeight = dimension / rows;

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
    }

    void placeTrees(float[,] height)
    {
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
    }
	
	// Update is called once per frame
	void Update () {
		
	}

}
