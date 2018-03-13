using UnityEngine;
using System.Collections.Generic;

using Extant;

using SharedComponents.Global.GameProperties;

public class MapController : MonoComponent
{
    GameObject[,] terrainBlocks;

    void Start()
    {
        ResetMap(1, 1);
    }

    void Update()
    {

    }

    public void ResetMap(int newNumBlocksX, int newNumBlocksY)
    {
        if (terrainBlocks != null)
        {
            foreach (var b in terrainBlocks)
            {
                GameObject.Destroy(b);
            }
        }

        Object terrainBlockObject = Resources.Load(ResourceList.Map.TerrainBlock);

        terrainBlocks = new GameObject[newNumBlocksX, newNumBlocksY];
        for (int i = 0; i < newNumBlocksX; i++)
        {
            for (int j = 0; j < newNumBlocksY; j++)
            {
                terrainBlocks[i, j] = (GameObject)Instantiate(terrainBlockObject, new Vector3(i * 10, 0, j * 10), Quaternion.identity);
            }
        }

        /*
        TerrainData terrainData = new TerrainData();

        terrainData.size = new Vector3(newNumBlocksX * MapDefaults.TERRAINBLOCK_WIDTH, 600, newNumBlocksY * MapDefaults.TERRAINBLOCK_WIDTH);

        terrainData.heightmapResolution = 128;
        terrainData.baseMapResolution = 128;
        terrainData.SetDetailResolution(128, 16);

        terrain = Terrain.CreateTerrainGameObject(terrainData);
        terrain.transform.position = new Vector3(0, 0, 0);
         * */

        Log.Log("Reset map to size: [" + newNumBlocksX + "," + newNumBlocksY + "]");
    }

    public void SetTerrainBlock(int i, int j, float[,] heightMap)
    {

        Log.Log("Set terrain block: [" + i + "," + j + "]");
    }
}

class TerrainBlock
{
    public readonly int i;
    public readonly int j;
    public readonly float[,] heightMap;

    public TerrainBlock(int i, int j, float[,] heightMap)
    {
        this.i = i;
        this.j = j;
        this.heightMap = heightMap;
    }
}
