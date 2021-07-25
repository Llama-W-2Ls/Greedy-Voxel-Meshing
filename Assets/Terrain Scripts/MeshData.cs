using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshData
{
    public List<Vector3> Vertices = new List<Vector3>();
    public List<int> Triangles = new List<int>();
    public List<Vector2> UVs = new List<Vector2>();

    public void AddCube(Vector3 StartPos, Vector3 EndPos)
    {
        var offset = new Vector3(0.5f, 0.5f, 0.5f);
        var startPos = StartPos - offset;
        var endPos = EndPos + offset;

        var vertices = new Vector3[]
        {
            new Vector3(startPos.x, startPos.y, startPos.z), // left bottom back
            new Vector3(endPos.x, startPos.y, startPos.z), // right bottom back
            new Vector3(endPos.x, endPos.y, startPos.z), // right top back
            new Vector3(startPos.x, endPos.y, startPos.z), // left top back
            new Vector3(startPos.x, startPos.y, endPos.z), // left bottom front
            new Vector3(endPos.x, startPos.y, endPos.z), // right bottom front
            new Vector3(endPos.x, endPos.y, endPos.z), // right top front 
            new Vector3(startPos.x, endPos.y, endPos.z) // left top front
        };

        var uvs = Textures.instance.TextureDict[BlockType.Grass];

        AddQuad(vertices[0], vertices[1], vertices[2], vertices[3], true); // back
        AddQuad(vertices[4], vertices[5], vertices[6], vertices[7], false); // front
        AddQuad(vertices[0], vertices[4], vertices[5], vertices[1], true); // bottom
        AddQuad(vertices[3], vertices[7], vertices[6], vertices[2], false); // top
        AddQuad(vertices[4], vertices[0], vertices[3], vertices[7], true); // left
        AddQuad(vertices[5], vertices[1], vertices[2], vertices[6], false); // right

        UVs.AddRange(uvs[BlockFace.Back]);
        UVs.AddRange(uvs[BlockFace.Front]);
        UVs.AddRange(uvs[BlockFace.Bottom]);
        UVs.AddRange(uvs[BlockFace.Top]);
        UVs.AddRange(uvs[BlockFace.Left]);
        UVs.AddRange(uvs[BlockFace.Right]);
    }

    void AddQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, bool outward)
    {
        var triangles = outward ? new int[] { 2, 1, 0, 3, 2, 0 } : new int[] { 0, 1, 2, 0, 2, 3 };
        AddTriangles(triangles, Vertices.Count);

        Vertices.AddRange(new Vector3[] { a, b, c, d });
    }

    public void AddTriangles(int[] triangles, int increment)
    {
        foreach (var triangle in triangles)
        {
            Triangles.Add(triangle + increment);
        }
    }

    public Mesh ToMesh()
    {
        var mesh = new Mesh()
        {
            vertices = Vertices.ToArray(),
            triangles = Triangles.ToArray(),
            uv = UVs.ToArray()
        };

        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.Optimize();

        return mesh;
    }
}
