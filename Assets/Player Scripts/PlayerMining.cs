using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMining : MonoBehaviour
{
    public Transform cam;
    public World world;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Mine();

        if (Input.GetMouseButtonDown(1))
            Build();
    }

    void Mine()
    {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit))
        {
            var hitPoint = cam.position + cam.forward * (hit.distance + 0.5f);

            var chunk = world.ChunkBeingEdited(hit.collider.name);
            var hitPos = new Vector3Int(Mathf.RoundToInt(hitPoint.x), Mathf.RoundToInt(hitPoint.y), Mathf.RoundToInt(hitPoint.z));
            var blockPos = hitPos - chunk.Pos;

            try
            {
                chunk.Blocks[chunk.GetCell(blockPos.x, blockPos.y, blockPos.z)].Type = BlockType.Air;

                var newMeshData = chunk.GetGreedyMesh();
                world.EditChunk(newMeshData, chunk);
            }
            catch (System.Exception) { }
        }
    }

    void Build()
    {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit))
        {
            var hitPoint = cam.position + cam.forward * (hit.distance - 0.5f);

            var chunk = world.ChunkBeingEdited(hit.collider.name);
            var hitPos = new Vector3Int(Mathf.RoundToInt(hitPoint.x), Mathf.RoundToInt(hitPoint.y), Mathf.RoundToInt(hitPoint.z));
            var blockPos = hitPos - chunk.Pos;

            try
            {
                chunk.Blocks[chunk.GetCell(blockPos.x, blockPos.y, blockPos.z)].Type = BlockType.Grass;

                var newMeshData = chunk.GetGreedyMesh();
                world.EditChunk(newMeshData, chunk);
            }
            catch (System.Exception) { }
        }
    }
}
