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
    

    // Use this for initialization
    void Start () {

        TerrainData TD = terrain.terrainData;

        generateTerrain(TD);
        
    }

    void generateTerrain(TerrainData TD)
    {
        float[,] perlinMap = new float[dimension, dimension];
        float[,,] splatMap = new float[dimension, dimension, TD.alphamapLayers];
        float nextPerlin;
        int perlinSeed = Random.Range(0, 10000);
        int innerArenaWidth = dimension / columns;
        int innerArenaHeight = dimension / rows;
        bool wall = false; //Used to change texture weights where there are walls

        Debug.Log("innerArenaWidth= " + innerArenaWidth);
        Debug.Log("innerArenaHeight= " + innerArenaHeight);

        for (int x = 0; x < dimension; x++)
        {
            for (int y = 0; y < dimension; y++)
            {
                wall = false;

                if (x < 5 || y < 5 || x > dimension - 5 || y > dimension - 5)
                {
                    wall = true;
                    perlinMap[x, y] = wallHeight;
                }
                else
                {

                    if (x > 5 && y > 5 && x < dimension - 5 && y < dimension - 5)
                    {
                        if (x % innerArenaWidth == 0)
                        {
                            wall = true;
                            perlinMap[x, y] = wallHeight;
                            perlinMap[x, y + 1] = wallHeight;
                            perlinMap[x, y + 2] = wallHeight;
                            perlinMap[x, y + 3] = wallHeight;
                            perlinMap[x, y + 4] = wallHeight;
                        }
                        else if (y % innerArenaHeight == 0)
                        {
                            wall = true;
                            perlinMap[x, y] = wallHeight;
                            perlinMap[x + 1, y] = wallHeight;
                            perlinMap[x + 2, y] = wallHeight;
                            perlinMap[x + 3, y] = wallHeight;
                            perlinMap[x + 4, y] = wallHeight;
                        }
                        else
                        {
                            nextPerlin = Mathf.PerlinNoise((x + perlinSeed) * refinement, (y + perlinSeed) * refinement);
                            perlinMap[x, y] = nextPerlin * heightScale;
                        }
                    }
                }

                for (int i = 0; i < TD.alphamapLayers; i++)
                {
                    if (wall)
                    {
                        if (i == 1)
                        {
                            //Color this point with wall texture
                            splatMap[x, y, i] = 1.0f;

                            //Color surrounding points with wall texture
                            for (int dx = 1; dx <= 3; dx++)
                            {
                                int nextXinc = x + dx;
                                int nextXdec = x - dx;

                                for (int dy = 1; dy <= 3; dy++)
                                {
                                    int nextyinc = y + dy;
                                    int nextydec = y - dy;

                                    if (nextXinc > 0 && nextXinc < dimension && nextyinc > 0 && nextyinc < dimension)
                                    {
                                        splatMap[nextXinc, nextyinc, i] = 1.0f;
                                        splatMap[nextXinc, y, i] = 1.0f;
                                        splatMap[x, nextyinc, i] = 1.0f;
                                    }

                                    if (nextXinc > 0 && nextXinc < dimension && nextydec > 0 && nextydec < dimension)
                                    {
                                        splatMap[nextXinc, nextydec, i] = 1.0f;
                                    }

                                    if (nextXdec > 0 && nextXdec < dimension && nextydec > 0 && nextydec < dimension)
                                    {
                                        splatMap[nextXdec, nextydec, i] = 1.0f;
                                        splatMap[nextXdec, y, i] = 1.0f;
                                        splatMap[x, nextydec, i] = 1.0f;
                                    }

                                    if (nextXdec > 0 && nextXdec < dimension && nextyinc > 0 && nextyinc < dimension)
                                    {
                                        splatMap[nextXdec, nextyinc, i] = 1.0f;
                                    }
                                }
                            }
                        }
                        else
                        {
                            splatMap[x, y, i] = 0.0f;
                        }

                    }
                    else
                    {
                        if (i == 0)
                        {
                            splatMap[x, y, i] = 1.0f;
                        }
                        else
                        {
                            splatMap[x, y, i] = 0.0f;
                        }

                    }
                }
            }
        }

        //Double check
        for (int j = 0; j < dimension; j++)
        {
            for (int k = 0; k < dimension; k++)
            {
                if (splatMap[j, k, 0] == 1.0f && splatMap[j, k, 1] == 1.0f)
                {
                    splatMap[j, k, 0] = 0.0f;
                }
            }
        }

        TD.SetHeights(0, 0, perlinMap);
        TD.SetAlphamaps(0, 0, splatMap);

        placeRectangles(perlinMap);
    }

    void placeRectangles (float[,] height)
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
                        Instantiate(tree, new Vector3(x, posY + 9, z), Quaternion.Euler(new Vector3(0, 0, 0)));
                    }        
                }
            }
        }
    }

    void placePerlinTrees(float[,] height)
    {
        float nextPerlin;
        int perlinSeed = Random.Range(0, 10000);

        for (int x = 50; x < dimension - 50; x++)
        {
            for (int z = 50; z < dimension - 50; z++)
            {
                nextPerlin = Mathf.PerlinNoise((x + perlinSeed) * refinement, (z + perlinSeed) * refinement);

                if (nextPerlin < obstacleFill)
                {
                    float posY = terrain.SampleHeight(new Vector3(x, 0, z));
                    float obstacleRoll = Random.Range(0.0f, 1.0f);

                    if (obstacleRoll > treeToRockRatio)
                    {
                        Instantiate(tree, new Vector3(x, posY, z), Quaternion.Euler(new Vector3(-90, Random.Range(-180, 180), 0)));
                    }
                    else
                    {
                        Instantiate(rock, new Vector3(x, posY, z), Quaternion.Euler(new Vector3(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180))));
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
                nextPerlin = Mathf.PerlinNoise((x + perlinSeed) * refinement, (z + perlinSeed) * refinement);
                float nextTreeRoll = Random.Range(0.0f, 100000);

                if(nextTreeRoll < obstacleFill)
                {
                    float posY = terrain.SampleHeight(new Vector3(x, 0, z));
                    float obstacleRoll = Random.Range(0.0f, 1.0f);

                    if(obstacleRoll > treeToRockRatio)
                    {
                        Instantiate(tree, new Vector3(x, posY, z), Quaternion.Euler(new Vector3(-90, Random.Range(-180, 180), 0)));
                    }
                    else
                    {
                        Instantiate(rock, new Vector3(x, posY, z), Quaternion.Euler(new Vector3(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180))));
                    }

                    
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

}
