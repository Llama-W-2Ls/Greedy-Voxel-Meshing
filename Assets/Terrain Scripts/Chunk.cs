using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

public class Chunk
{
    public Block[] Blocks;
    public Vector3Int Size;
    public Vector3Int Pos;

    MeshData meshData;
    public long GenerationTime;

    public Chunk(Vector3Int size, Vector3Int pos, NoiseSettings settings)
    {
        Size = size;
        Pos = pos;

        InitializeBlocks(settings);
    }

    void InitializeBlocks(NoiseSettings settings)
    {
        Blocks = new Block[Size.x * Size.y * Size.z];

        Parallel.For(0, Size.x, (x) =>
        {
            {
                for (int z = 0; z < Size.z; z++)
                {
                    int yPos = Mathf.RoundToInt(Noise.GetValue(x + Pos.x, z + Pos.z, settings));

                    for (int y = 0; y < Size.y; y++)
                    {
                        var blockType = y < yPos ? BlockType.Grass : BlockType.Air;
                        Blocks[GetCell(x, y, z)] = new Block(new Vector3Int(x, y, z), blockType);
                    }
                }
            }
        });
    }

    /// <summary>
    /// Mesh data is generated using greedy meshing algorithm.
    /// Quite fast and mesh is very optimal
    /// </summary>
    /// <param name="callback"></param>
    public MeshData GetGreedyMesh()
    {
        var timer = new Stopwatch();
        timer.Start();

        var meshData = GreedyMeshing.OptimizeChunk(this);

        GenerationTime = timer.ElapsedMilliseconds;
        timer.Stop();

        return meshData;
    }

    public int GetCell(int x, int y, int z)
    {
        return z + Size.z * (y + Size.y * x);
    }
}