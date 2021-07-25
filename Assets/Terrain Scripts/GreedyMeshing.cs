using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GreedyMeshing
{
    static readonly HashSet<Vector3> scannedBlocks = new HashSet<Vector3>();

    public static MeshData OptimizeChunk(Chunk chunk)
    {
        scannedBlocks.Clear();

        var meshes = GetGreedyMeshes(chunk);

        var meshData = new MeshData();

        foreach (var mesh in meshes)
        {
            meshData.AddCube(mesh.Start, mesh.End);
        }

        return meshData;
    }

    static GreedyMesh[] GetGreedyMeshes(Chunk chunk)
    {
        var meshes = new List<GreedyMesh>();
        var currentMesh = new GreedyMesh();

        for (int z = 0; z < chunk.Size.z; z++)
        {
            for (int y = 0; y < chunk.Size.y; y++)
            {
                for (int x = 0; x < chunk.Size.x; x++)
                {
                    if (scannedBlocks.Count == chunk.Blocks.Length)
                        return meshes.ToArray();

                    var block = chunk.Blocks[chunk.GetCell(x, y, z)];
                    bool blockIsValid = IsValidBlock(block);

                    if (blockIsValid)
                    {
                        if (currentMesh.IsNull)
                        {
                            currentMesh.Start = block.Pos;
                            currentMesh.End = block.Pos;
                            currentMesh.IsNull = false;

                            scannedBlocks.Add(block.Pos);
                        }
                        else
                        {
                            currentMesh.End = block.Pos;
                            scannedBlocks.Add(block.Pos);
                        }
                    }
                    else if (!currentMesh.IsNull && !blockIsValid)
                    {
                        meshes.Add(ExtendOnY(currentMesh, chunk));
                        currentMesh = new GreedyMesh();
                    }
                }

                if (!currentMesh.IsNull)
                {
                    meshes.Add(ExtendOnY(currentMesh, chunk));
                    currentMesh = new GreedyMesh();
                }
            }
        }

        return meshes.ToArray();
    }

    static GreedyMesh ExtendOnY(GreedyMesh currentMesh, Chunk chunk)
    {
        var includedBlocks = new List<Vector3>();

        for (int y = currentMesh.Start.y + 1; y < chunk.Size.y; y++)
        {
            bool canExtendOnY = true;

            for (int x = currentMesh.Start.x; x <= currentMesh.End.x; x++)
            {
                var block = chunk.Blocks[chunk.GetCell(x, y, currentMesh.Start.z)];

                if (!IsValidBlock(block))
                {
                    canExtendOnY = false;
                    break;
                }
                else
                    includedBlocks.Add(block.Pos);
            }

            if (canExtendOnY)
            {
                currentMesh.End.y++;

                for (int i = 0; i < includedBlocks.Count; i++)
                {
                    scannedBlocks.Add(includedBlocks[i]);
                }
                includedBlocks.Clear();
            }
            else
                return ExtendOnZ(currentMesh, chunk);
        }

        return ExtendOnZ(currentMesh, chunk);
    }

    static GreedyMesh ExtendOnZ(GreedyMesh currentMesh, Chunk chunk)
    {
        var includedBlocks = new List<Vector3>();

        for (int z = currentMesh.Start.z + 1; z < chunk.Size.z; z++)
        {
            for (int y = currentMesh.Start.y; y <= currentMesh.End.y; y++)
            {
                bool canExtendOnY = true;

                for (int x = currentMesh.Start.x; x <= currentMesh.End.x; x++)
                {
                    var block = chunk.Blocks[chunk.GetCell(x, y, z)];

                    if (!IsValidBlock(block))
                    {
                        canExtendOnY = false;
                        break;
                    }
                    else
                        includedBlocks.Add(block.Pos);
                }

                if (!canExtendOnY)
                    return currentMesh;
            }

            for (int i = 0; i < includedBlocks.Count; i++)
            {
                scannedBlocks.Add(includedBlocks[i]);
            }
            includedBlocks.Clear();

            currentMesh.End.z++;
        }

        return currentMesh;
    }

    static bool IsValidBlock(Block block)
    {
        return block.Type != BlockType.Air && !scannedBlocks.Contains(block.Pos);
    }
}

public class GreedyMesh
{
    public Vector3Int Start;
    public Vector3Int End;

    public bool IsNull = true;
}
