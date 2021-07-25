using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(World))]
public class WorldEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var world = (World)target;

        if (GUILayout.Button("Generate"))
        {
            var chunk = new Chunk(world.ChunkSize, Vector3Int.zero, world.NoiseSettings);
            var meshData = chunk.GetGreedyMesh();

            SpawnChunk(meshData, chunk);
        }
    }

    void SpawnChunk(MeshData meshData, Chunk chunk)
    {
        var obj = new GameObject(chunk.Pos.ToString());
        obj.transform.position = chunk.Pos;

        obj.AddComponent<MeshFilter>().sharedMesh = meshData.ToMesh();
        obj.AddComponent<MeshRenderer>();
        obj.AddComponent<MeshCollider>();

        Debug.Log(chunk.GenerationTime);
    }
}
