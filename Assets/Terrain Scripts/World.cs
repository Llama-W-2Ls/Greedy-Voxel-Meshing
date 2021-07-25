using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [Header("World Settings")]
    public int ViewDistanceInChunks;
    public Transform Player;
    public Dictionary<Vector3, ChunkObj> Chunks = new Dictionary<Vector3, ChunkObj>();

    [Header("Chunk Settings")]
    public Vector3Int ChunkSize;
    public NoiseSettings NoiseSettings;
    public Material material;

    bool LoadedWorld;
    int originalViewDistance;

    void Start()
    {
        GenerateWorld();
    }

    void Update()
    {
        ActivateChunks();
    }

    async void GenerateWorld()
    {
        while (Application.isPlaying)
        {
            var playerPos = GetPlayerPos();
            var meshes = new List<ChunkMesh>();

            if (!LoadedWorld)
            {
                originalViewDistance = ViewDistanceInChunks;
                ViewDistanceInChunks = 1;
            }

            await Task.Run(() =>
            {
                for (int x = -ViewDistanceInChunks * ChunkSize.x; x < ViewDistanceInChunks * ChunkSize.x; x += ChunkSize.x)
                {
                    for (int z = -ViewDistanceInChunks * ChunkSize.z; z < ViewDistanceInChunks * ChunkSize.z; z += ChunkSize.z)
                    {
                        var chunkPos = new Vector3Int(x + playerPos.x, 0, z + playerPos.z);

                        if (!Chunks.ContainsKey(chunkPos))
                        {
                            try
                            {
                                var chunk = new Chunk(ChunkSize, chunkPos, NoiseSettings);
                                var meshData = chunk.GetGreedyMesh();

                                meshes.Add(new ChunkMesh() { chunk = chunk, data = meshData });
                            }
                            catch (Exception e) { Debug.LogError(e.Message); }
                        }
                    }
                }
            });

            foreach (var mesh in meshes)
            {
                SpawnChunk(mesh.data, mesh.chunk);
                await Task.Delay(Mathf.RoundToInt(Time.deltaTime * 1000));
            }

            // Wait a frame
            await Task.Delay(Mathf.RoundToInt(Time.deltaTime * 1000));

            if (!LoadedWorld)
            {
                SpawnPlayer();
            }

            if (ViewDistanceInChunks != originalViewDistance)
                ViewDistanceInChunks++;

            LoadedWorld = true;
        }
    }

    void SpawnPlayer()
    {
        Player.gameObject.SetActive(true);
    }

    void ActivateChunks()
    {
        var playerPos = GetPlayerPos();

        foreach (var chunk in Chunks)
        {
            if (Vector3.Distance(chunk.Key, playerPos) < ViewDistanceInChunks * ChunkSize.x) 
            {
                chunk.Value.obj.SetActive(true);
                continue;
            }

            chunk.Value.obj.SetActive(false);
        }

        StaticBatchingUtility.Combine(gameObject);
    }

    void SpawnChunk(MeshData meshData, Chunk chunk)
    {
        if (Chunks.ContainsKey(chunk.Pos))
            return;

        var obj = new GameObject(chunk.Pos.ToString())
        {
            layer = 7
        };
        obj.transform.position = chunk.Pos;
        obj.transform.SetParent(transform);

        obj.AddComponent<MeshFilter>().sharedMesh = meshData.ToMesh();
        obj.AddComponent<MeshRenderer>().sharedMaterial = material;
        obj.AddComponent<MeshCollider>();

        var chunkObj = new ChunkObj()
        {
            chunk = chunk,
            obj = obj
        };

        Chunks.Add(chunk.Pos, chunkObj);
    }

    public void EditChunk(MeshData meshData, Chunk chunk)
    {
        if (!Chunks.ContainsKey(chunk.Pos))
            return;

        var obj = Chunks[chunk.Pos].obj;

        obj.GetComponent<MeshFilter>().sharedMesh = meshData.ToMesh();
        obj.GetComponent<MeshRenderer>().material = material;
        Destroy(obj.GetComponent<MeshCollider>());
        obj.AddComponent<MeshCollider>();

        var chunkObj = new ChunkObj()
        {
            chunk = chunk,
            obj = obj
        };

        Chunks[chunk.Pos] = chunkObj;
    }

    public Vector3Int GetPlayerPos()
    {
        // Round by chunk size
        return new Vector3Int
        (
            Mathf.RoundToInt(Player.position.x / ChunkSize.x) * ChunkSize.x,
            0,
            Mathf.RoundToInt(Player.position.z / ChunkSize.z) * ChunkSize.z
        );
    }

    public Chunk ChunkBeingEdited(string objName)
    {
        // Remove the parentheses
        if (objName.StartsWith("(") && objName.EndsWith(")"))
        {
            objName = objName.Substring(1, objName.Length - 2);
        }

        // split the items
        string[] sArray = objName.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return Chunks[result].chunk;
    }
}

public struct ChunkObj
{
    public Chunk chunk;
    public GameObject obj;
}

public struct ChunkMesh
{
    public Chunk chunk;
    public MeshData data;
}
