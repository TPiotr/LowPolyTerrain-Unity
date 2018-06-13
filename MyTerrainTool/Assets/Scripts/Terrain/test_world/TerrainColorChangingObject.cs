using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainColorChangingObject : MonoBehaviour {

    public Color TargetColor;
    public int Radius = 8;
    public float LerpSpeed = 0.5f;
    public float MinDistance = 1f;

    public LayerMask TerrainMask;

    //run changing color mechanics for 3 seconds since first correct raycast
    public float RunForTime = 3.0f; 
    [SerializeField] [HideInInspector] private float RunTime = -10;
    
    private LTerrain terrain;
    private Vector3 origin;

	// Use this for initialization
	void Start () {
        terrain = FindObjectOfType<LTerrain>();
        origin = new Vector3();
	}
	
	// Update is called once per frame
	void Update () {
        //raycast directly down

        RaycastHit hit;
        origin.Set(transform.position.x, transform.position.y + 1, transform.position.z);
        Ray ray = new Ray(origin, Vector3.down);

        bool working = (RunTime == -10) || (Time.time - RunTime < RunForTime);
        
        if(working && Physics.Raycast(ray, out hit, MinDistance, TerrainMask))
        {
            GameObject hitted_object = hit.transform.gameObject;
            if (hitted_object.GetComponent<TerrainChunk>() != null)
            {
                if(RunTime == -10)
                {
                    RunTime = Time.time;
                }
                
                //update colors
                int triangle_index = hit.triangleIndex;
                TerrainChunk triangle_chunk = hit.collider.gameObject.GetComponent<TerrainChunk>();

                //calculate local x,y cords of hit triangle
                int vertex_index = triangle_chunk.GetTriangles()[triangle_index * 3 + 2];
                int x = vertex_index % (TerrainChunk.ChunkSize + 1) + (triangle_chunk.ChunkY * (TerrainChunk.ChunkSize + 1));
                int y = vertex_index / (TerrainChunk.ChunkSize + 1) + (triangle_chunk.ChunkX * (TerrainChunk.ChunkSize + 1));
                
                for (int i = -Radius; i <= Radius; i++)
                {
                    for (int j = -Radius; j <= Radius; j++)
                    {
                        float dst_to_center = Mathf.Sqrt((i * i) + (j * j));

                        if (dst_to_center > Radius / 2) //keep circle shape
                            continue;

                        float dst = (float)Radius / 2f / dst_to_center;
                       
                        if (i == 0 && j == 0) //avoid situation where dst var has val 1.0
                            dst = 0.7f;

                        int local_x = i + y;
                        int local_y = j + x;

                        float alpha = (dst) * (LerpSpeed);
                        alpha = Mathf.Clamp01(alpha);

                        Color old_color = GetColor(local_x, local_y);
                        Color new_color = Color.Lerp(old_color, TargetColor, Time.deltaTime * alpha);
                        
                        SetColor(local_x, local_y, new_color);
                    }
                }
            }
        }
    }

    private Color32 GetColor(float local_x, float local_z)
    {
        int Size = TerrainChunk.ChunkSize + 1;

        int x = (int)((local_x % Size / Size) * Size);
        int z = (int)((local_z % Size / Size) * Size);

        int chx = (int)local_x / Size;
        int chz = (int)local_z / Size;
        
        TerrainChunk chunk = terrain.GetChunk(chx, chz);

        if (chunk == null)
            return Color.white;

        return chunk.GetColor(x, z);
    }

    private TerrainChunk SetColor(float local_x, float local_z, Color32 color)
    {
        int Size = TerrainChunk.ChunkSize + 1;

        int x = (int)((local_x % Size / Size) * Size);
        int z = (int)((local_z % Size / Size) * Size);

        int chx = (int)local_x / Size;
        int chz = (int)local_z / Size;

        TerrainChunk chunk = terrain.GetChunk(chx, chz);

        if (chunk == null)
            return null;

        chunk.SetColor(x, z, color);
        return chunk;
    }
    
}
