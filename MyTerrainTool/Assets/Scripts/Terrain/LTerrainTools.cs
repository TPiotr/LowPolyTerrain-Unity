using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LTerrainTools : MonoBehaviour {

    public RaycastHit LastRaycastHit;
    public LTerrain terrain;

    public static int SCULPT_TOOL = 0;
    public static int SMOOTH_TOOL = 1;
    public static int COLOR_TOOL = 2;
    public static int GRAB_COLOR_TOOL = 3;
    public static string[] ALL_TOOLS = { "Sculpt", "Smooth", "Colors", "Grab Color" };

    public int CurrentTool = SCULPT_TOOL;

    //gizmo
    public Color sphere_color = new Color(.6f, .7f, .8f, .3f);
    public float CurrentRadius = 1;

    //sculpt vars
    public float SculptStrenght = .5f;
    public bool UpDirection = false;

    //smooth vars
    public float SmoothStrenght = .5f;

    //colors vars
    public Color32 PaintingColor = Color.white;
    public float PaintingStrenght = .3f;
    
    //mouse states
    private bool LeftMouseDown;

    //tick speed
    private int last_tick_time;
    private int TICK_AFTER = 20;
    
    public void Tick () {

        if (terrain == null)
        {
            terrain = GetComponent<LTerrain>();

            if(terrain == null)
                Debug.LogError("LTerrain instance not found");
        }

        if (Environment.TickCount - last_tick_time > TICK_AFTER)
        {
            Vector3 point = LastRaycastHit.point;
            
            point = transform.InverseTransformPoint(point); //LastRaycastHit.transform.InverseTransformPoint(point);

            if (LeftMouseDown && CurrentTool == SCULPT_TOOL)
            {
                List<TerrainChunk> affected_chunks = new List<TerrainChunk>();

                int triangle_index = LastRaycastHit.triangleIndex;
                TerrainChunk triangle_chunk = LastRaycastHit.collider.gameObject.GetComponent<TerrainChunk>();

                int vertex_index = triangle_chunk.GetTriangles()[triangle_index * 3 + 2];
                int x = vertex_index % (TerrainChunk.ChunkSize + 1) + (triangle_chunk.ChunkY * (TerrainChunk.ChunkSize + 1));
                int y = vertex_index / (TerrainChunk.ChunkSize + 1) + (triangle_chunk.ChunkX * (TerrainChunk.ChunkSize + 1));

                int radius = Mathf.CeilToInt(CurrentRadius);
                for (int i = -radius / 2; i < radius / 2; i++)
                {
                    for (int j = -radius / 2; j < radius / 2; j++)
                    {
                        int local_x = i + y;
                        int local_y = j + x;

                        float height_add = .1f * ((UpDirection) ? 1 : -1) * SculptStrenght;

                        TerrainChunk affected_chunk = AddHeight(local_x, local_y, height_add);

                        if (!affected_chunks.Contains(affected_chunk) && affected_chunk != null)
                            affected_chunks.Add(affected_chunk);
                    }
                }

                foreach (TerrainChunk chunk in affected_chunks)
                    chunk.GenerateAndUpdateMesh();
            }
            else if(LeftMouseDown && CurrentTool == SMOOTH_TOOL)
            {
                List<TerrainChunk> affected_chunks = new List<TerrainChunk>();

                int triangle_index = LastRaycastHit.triangleIndex;
                TerrainChunk triangle_chunk = LastRaycastHit.collider.gameObject.GetComponent<TerrainChunk>();

                int vertex_index = triangle_chunk.GetTriangles()[triangle_index * 3 + 2];
                int x = vertex_index % (TerrainChunk.ChunkSize + 1) + (triangle_chunk.ChunkY * (TerrainChunk.ChunkSize + 1));
                int y = vertex_index / (TerrainChunk.ChunkSize + 1) + (triangle_chunk.ChunkX * (TerrainChunk.ChunkSize + 1));

                float total_height = 0f;
                float heights_count = 0f;

                int radius = Mathf.CeilToInt(CurrentRadius);
                for (int i = -radius / 2; i < radius / 2; i++)
                {
                    for (int j = -radius / 2; j < radius / 2; j++)
                    {
                        int local_x = i + y;
                        int local_y = j + x;
                        
                        if (GetChunk(local_x, local_y) != null)
                        {
                            float height = GetHeight(local_x, local_y);

                            total_height += height;
                            heights_count++;
                        }
                    }
                }

                float final_height = total_height / heights_count;
                for (int i = -radius / 2; i < radius / 2; i++)
                {
                    for (int j = -radius / 2; j < radius / 2; j++)
                    {
                        int local_x = i + y;
                        int local_y = j + x;
                        
                        if (GetChunk(local_x, local_y) != null)
                        {
                            float current_height = GetHeight(local_x, local_y);
                            TerrainChunk chunk = AddHeight(local_x, local_y, (final_height - current_height) * SmoothStrenght);

                            if (!affected_chunks.Contains(chunk) && chunk != null)
                                affected_chunks.Add(chunk);
                        }
                    }
                }

                foreach (TerrainChunk chunk in affected_chunks)
                    chunk.GenerateAndUpdateMesh();
            }
            else if(LeftMouseDown && CurrentTool == COLOR_TOOL)
            {
                List<TerrainChunk> affected_chunks = new List<TerrainChunk>();

                int triangle_index = LastRaycastHit.triangleIndex;
                TerrainChunk triangle_chunk = LastRaycastHit.collider.gameObject.GetComponent<TerrainChunk>();
                
                int vertex_index = triangle_chunk.GetTriangles()[triangle_index * 3 + 2];
                int x = vertex_index % (TerrainChunk.ChunkSize + 1) + (triangle_chunk.ChunkY * (TerrainChunk.ChunkSize + 1));
                int y = vertex_index / (TerrainChunk.ChunkSize + 1) + (triangle_chunk.ChunkX * (TerrainChunk.ChunkSize + 1));

                int radius = Mathf.CeilToInt(CurrentRadius);
                for(int i = -radius/2; i <= radius/2; i++)
                {
                    for(int j = -radius/2; j <= radius/2; j++)
                    {
                        float dst = (float)radius / Mathf.Sqrt((i*i) + (j*j));
                        dst = dst / (float) radius;

                        if (i == 0 && j == 0)
                            dst = 0.7f;

                        int local_x = i + y;
                        int local_y = j + x;

                        float alpha = (dst) * (PaintingStrenght / 2f);
                        
                        Color new_color = Color.Lerp(GetColor(local_x, local_y), PaintingColor, alpha);
                        TerrainChunk affected_chunk = SetColor(local_x, local_y, new_color);

                        if (!affected_chunks.Contains(affected_chunk) && affected_chunk != null)
                            affected_chunks.Add(affected_chunk);
                    }
                }

                foreach (TerrainChunk chunk in affected_chunks)
                    chunk.GenerateAndUpdateMesh();
            }
            else if(LeftMouseDown && CurrentTool == GRAB_COLOR_TOOL)
            {
                int triangle_index = LastRaycastHit.triangleIndex;
                TerrainChunk triangle_chunk = LastRaycastHit.collider.gameObject.GetComponent<TerrainChunk>();

                int vertex_index = triangle_chunk.GetTriangles()[triangle_index * 3 + 2];
                
                PaintingColor = triangle_chunk.GetColors()[vertex_index];

                CurrentTool = COLOR_TOOL;
            }
            last_tick_time = Environment.TickCount;
        }
    }

    public void ProcessEvent(Event e)
    {
        if (e.isKey)
        {
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.LeftControl)
                {

                }
            }
            else if (e.type == EventType.KeyUp)
            {
                if (e.keyCode == KeyCode.LeftControl)
                {

                }
            }
        }

        if (e.isMouse)
        {
            if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0)
            {
                LeftMouseDown = true;
                
                e.Use();
            }
            else if(e.type == EventType.MouseUp && e.button == 0)
            {
                LeftMouseDown = false;
            }

            /*else if (e.type == EventType.ScrollWheel)
            {
                CurrentRadius += e.delta.y;
                CurrentRadius = Mathf.Clamp(CurrentRadius, 0, 10);

                e.Use();
            }*/
        }
    }

    public void ConnectChunksEdges()
    {
        for(int i = 0; i < terrain.ChunksCount; i++)
        {
            for(int j = 0; j < terrain.ChunksCount; j++)
            {
                TerrainChunk this_chunk = terrain.GetChunk(i, j);
                if(this_chunk != null)
                {
                    TerrainChunk chunk_up = terrain.GetChunk(i, j + 1);
                    if (chunk_up != null)
                    {
                        for (int k = 0; k < TerrainChunk.ChunkSize; k++)
                        {
                            this_chunk.SetHeight(k, TerrainChunk.ChunkSize, chunk_up.GetHeight(k, 0));
                        }
                    }

                    TerrainChunk chunk_down = terrain.GetChunk(i, j - 1);
                    if (chunk_down != null)
                    {
                        for (int k = 0; k < TerrainChunk.ChunkSize; k++)
                        {
                            this_chunk.SetHeight(k, 0, chunk_down.GetHeight(k, TerrainChunk.ChunkSize));
                        }
                    }

                    TerrainChunk chunk_left = terrain.GetChunk(i - 1, j);
                    if (chunk_left != null)
                    {
                        for (int k = 0; k < TerrainChunk.ChunkSize; k++)
                        {
                            this_chunk.SetHeight(0, k, chunk_left.GetHeight(TerrainChunk.ChunkSize, k));
                        }
                    }

                    TerrainChunk chunk_right = terrain.GetChunk(i + 1, j);
                    if (chunk_right != null)
                    {
                        for (int k = 0; k < TerrainChunk.ChunkSize; k++)
                        {
                            this_chunk.SetHeight(TerrainChunk.ChunkSize, k, chunk_right.GetHeight(0, k));
                        }
                    }
                }

                this_chunk.GenerateAndUpdateMesh();
            }
        }
    }
    
    private TerrainChunk SetHeight(float local_x, float local_z, float height)
    {
        int Size = TerrainChunk.ChunkSize + 1;

        int x = (int) ((local_x % Size / Size) * Size);
        int z = (int) ((local_z % Size / Size) * Size);

        int chx = (int) local_x / Size;
        int chz = (int) local_z / Size;
        
        TerrainChunk chunk = terrain.GetChunk(chx, chz);

        if (chunk == null)
            return null;

        chunk.SetHeight(x, z, height);
        return chunk;
    }

    private TerrainChunk AddHeight(float local_x, float local_z, float add_value)
    {
        int Size = TerrainChunk.ChunkSize + 1;

        int x = (int)((local_x % Size / Size) * Size);
        int z = (int)((local_z % Size / Size) * Size);

        int chx = (int)local_x / Size;
        int chz = (int)local_z / Size;
        
        TerrainChunk chunk = terrain.GetChunk(chx, chz);

        if (chunk == null)
            return null;

        chunk.SetHeight(x, z, chunk.GetHeight(x, z) + add_value);
        return chunk;
    }

    private float GetHeight(float local_x, float local_z)
    {
        int Size = TerrainChunk.ChunkSize + 1;

        int x = (int)((local_x % Size / Size) * Size);
        int z = (int)((local_z % Size / Size) * Size);

        int chx = (int)local_x / Size;
        int chz = (int)local_z / Size;
        
        TerrainChunk chunk = terrain.GetChunk(chx, chz);

        if (chunk == null)
            return 0;

        return chunk.GetHeight(x, z);
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

    private TerrainChunk GetChunk(float local_x, float local_z)
    {
        int Size = TerrainChunk.ChunkSize + 1;

        int x = (int)((local_x % Size / Size) * Size);
        int z = (int)((local_z % Size / Size) * Size);

        int chx = (int)local_x / Size;
        int chz = (int)local_z / Size;
        
        TerrainChunk chunk = terrain.GetChunk(chx, chz);
        return chunk;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = sphere_color;
        Gizmos.DrawSphere(LastRaycastHit.point, CurrentRadius);
    }
}
