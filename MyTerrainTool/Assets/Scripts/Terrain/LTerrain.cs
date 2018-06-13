using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[ExecuteInEditMode]
public class LTerrain : MonoBehaviour {

    [SerializeField]
    private List<TerrainChunk> TerrainChunks;

    public int ChunksCount;
    public Material Material;
    
    public void CreateTerrain(HeightsGenerator generator)
    {
        for (int i = 0; i < ChunksCount; i++)
        {
            for(int j = 0; j < ChunksCount; j++)
            {
                TerrainChunk chunk = CreateChunkObject(i, j);
                TerrainChunks.Add(chunk);

                chunk.gameObject.transform.parent = gameObject.transform;
                chunk.gameObject.transform.localPosition = new Vector3(i * TerrainChunk.ChunkSize, 0, j * TerrainChunk.ChunkSize);

                chunk.gameObject.GetComponent<MeshRenderer>().material = Material;
                
                chunk.InitMesh();
                chunk.GenerateMeshData(i, j, generator);
            }
        }
    }

    public void Update()
    {
        foreach(TerrainChunk chunk in TerrainChunks)
        {
            if (chunk.IsMeshUpdateNeeded())
                chunk.UpdateMesh();
        }
    }

    public void Delete()
    {
        foreach(TerrainChunk t in TerrainChunks)
        {
            if (t == null)
                continue;

            DestroyImmediate(t.gameObject);
        }
        TerrainChunks.Clear();
    }
    
    private TerrainChunk CreateChunkObject(int x, int y)
    {
        GameObject new_chunk = new GameObject("TerrainChunk(X: " + x + " Y: " + y + ")");
        new_chunk.AddComponent<TerrainChunk>();

        return new_chunk.GetComponent<TerrainChunk>();
    }

    public List<TerrainChunk> GetChunks()
    {
        return TerrainChunks;
    }

    public TerrainChunk GetChunk(int chx, int chz)
    {
        TerrainChunk chunk = null;
        foreach (TerrainChunk t in TerrainChunks)
        {
            int tchx = (int) t.transform.localPosition.x / TerrainChunk.ChunkSize;
            int tchz = (int) t.transform.localPosition.z / TerrainChunk.ChunkSize;

            if(tchx == chx && tchz == chz)
            {
                return t;
            }
        }

        return chunk;
    }
}
