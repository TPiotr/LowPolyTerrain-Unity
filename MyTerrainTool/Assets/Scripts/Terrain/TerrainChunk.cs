using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class TerrainChunk : MonoBehaviour {
    
    public static int ChunkSize = 64; //vertices count on one axis (total vertices count of chunk is pow(ChunkSize + 1, 2))
    
    public int ChunkX, ChunkY;

    [SerializeField]
    private ChunkMeshData ChunkData;

    [SerializeField]
    private float[] Heights;

    [SerializeField]
    private Mesh Mesh;
    
    [SerializeField]
    private bool NeedsUpdate;
    
    [SerializeField]
    private bool UpdateColors;

    [System.Serializable]
    private class ChunkMeshData
    {
        [SerializeField]
        public Vector3[] vertices;

        [SerializeField]
        public int[] triangles;

        [SerializeField]
        public Color32[] colors;

        [SerializeField]
        public Vector2[] uv;

        public int vertex_index = 0;
        public void AddTriangle(int a, int b, int c)
        {
            triangles[vertex_index] = a;
            triangles[vertex_index + 1] = b;
            triangles[vertex_index + 2] = c;

            vertex_index += 3;
        }
    }

    public void InitMesh()
    {
        GetComponent<MeshFilter>().mesh = Mesh = new Mesh();
        Mesh.name = "TerrainChunk";
        Mesh.MarkDynamic();
    }
    
    public void Update()
    {
        if(UpdateColors)
        {
            Mesh.colors32 = ChunkData.colors; 
            UpdateColors = false;
        }
    }

    public void GenerateMeshData(int ChunkX, int ChunkY, HeightsGenerator generator)
    {
        this.ChunkX = ChunkX;
        this.ChunkY = ChunkY;

        bool create = ChunkData == null;

        if (ChunkData == null)
            ChunkData = new ChunkMeshData();

        int ThisChunkSize = ChunkSize + 1;

        if (create)
        {
            ChunkData.vertices = new Vector3[ThisChunkSize * ThisChunkSize];
            Heights = new float[ThisChunkSize * ThisChunkSize];
            ChunkData.triangles = new int[(ThisChunkSize - 1) * (ThisChunkSize - 1) * 6];
            ChunkData.colors = new Color32[ThisChunkSize * ThisChunkSize];
            ChunkData.uv = new Vector2[ThisChunkSize * ThisChunkSize];
        }
        
        for(int vertex_index = 0, i = 0; i <= ChunkSize; i++)
        {
            for(int j = 0; j <= ChunkSize; j++, vertex_index++)
            {
                if (create)
                {
                    float h = generator.GetHeight(i + (ChunkX * (ChunkSize)), j + (ChunkY * (ChunkSize)));
                    SetHeight(i, j, h);

                    Color color = generator.GetColor(i + (ChunkX * (ChunkSize)), j + (ChunkY * (ChunkSize)), h);
                    SetColor(i, j, color);

                    ChunkData.vertices[vertex_index] = new Vector3();

                    ChunkData.uv[vertex_index] = new Vector2((float) i / (float) ThisChunkSize, (float) j / (float) ThisChunkSize);
                } 
                
                float height = GetHeight(i, j);
                ChunkData.vertices[vertex_index].Set(i, height, j);

                if (i < ChunkSize && j < ChunkSize)
                {
                    ChunkData.AddTriangle(vertex_index, vertex_index + ThisChunkSize + 1, vertex_index + ThisChunkSize);
                    ChunkData.AddTriangle(vertex_index + ThisChunkSize + 1, vertex_index, vertex_index + 1);
                }
            }
        }

        ChunkData.vertex_index = 0;

        NeedsUpdate = true;
    }

    public void GenerateAndUpdateMesh()
    {
        GenerateMeshData(ChunkX, ChunkY, null);
        UpdateMesh();
    }

    //update Mesh data
    public void UpdateMesh()
    {
        GetComponent<MeshFilter>().mesh = Mesh = new Mesh();
        Mesh.name = "TerrainChunk";

        Mesh.vertices = ChunkData.vertices;
        Mesh.triangles = ChunkData.triangles;
        Mesh.colors32 = ChunkData.colors;
        Mesh.uv = ChunkData.uv;
        
        //update physics Mesh
        gameObject.GetComponent<MeshCollider>().sharedMesh = GetMesh();

        Mesh.RecalculateBounds();
        Mesh.RecalculateNormals();

        NeedsUpdate = false;
    }

    /*private void LowPolyMesh()
    {
        Vector3[] old_verts = Mesh.vertices;
        int[] triangles_new = Mesh.triangles;
        Vector3[] vertices_new = new Vector3[triangles_new.Length];
        Color32[] colors_new = new Color32[triangles_new.Length];

        for (int i = 0; i < triangles_new.Length; i++)
        {
            vertices_new[i] = old_verts[Mesh.triangles[i]];
            colors_new[i] = Mesh.colors32[Mesh.triangles[(i / 3) * 3]];

            triangles_new[i] = i;
        }
        Mesh.vertices = vertices_new;
        Mesh.triangles = triangles_new;
        Mesh.colors32 = colors_new;
        

        Mesh.RecalculateBounds();
        Mesh.RecalculateNormals();
    }*/

    public bool IsMeshUpdateNeeded()
    {
        return NeedsUpdate;
    }

    public Mesh GetMesh()
    {
        return Mesh;
    }

    public float[] GetHeights()
    {
        return Heights;
    }

    public float GetHeight(int x, int y)
    {
        return Heights[y * (ChunkSize + 1) + x];
    }

    public void SetHeight(int x, int y, float val)
    {
        Heights[y * (ChunkSize + 1) + x] = val;
    }

    public Color32[] GetColors()
    {
        return ChunkData.colors;
    }

    public Color32 GetColor(int y, int x)
    {
        return ChunkData.colors[y * (ChunkSize + 1) + x];
    }

    public void SetColor(int y, int x, Color color)
    {
        if (x > ChunkSize + 1 || y > ChunkSize + 1)
            return;

        ChunkData.colors[y * (ChunkSize + 1) + x] = color;

        UpdateColors = true;
    }

    public int[] GetTriangles()
    {
        return ChunkData.triangles;
    }
}
