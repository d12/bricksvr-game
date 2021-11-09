using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;

public static class GenerateObjs
{
    private const string SavePath = "BrickObjs";
    private static StringBuilder objStringBuilder = new StringBuilder();

    [MenuItem("Generator/Generate Brick OBJs")]
    public static void Generate()
    {
        List<BrickData.Brick> bricks = BrickData.AllBricks();
        foreach (BrickData.Brick brick in bricks)
        {
            Mesh mesh = brick.Mesh;
            OutputObj(mesh, brick.PrefabName);
        }

        Debug.Log(ColorInt.IntToColor32(-2434342));
    }

    // Start is called before the first frame update
    // void Start()
    // {
    //     BrickColor[] bricks = GetComponentsInChildren<BrickColor>();
    //
    //     ProcessBricks(bricks);
    //
    //     foreach (Color c in _usedColors.Keys)
    //     {
    //         OutputMtl(c);
    //     }
    //
    //     PrintToFile(objStringBuilder.ToString(), ObjFilePath);
    //     PrintToFile(mtlStringBuilder.ToString(), MtlFilePath);
    // }

    private static void OutputObj(Mesh mesh, string name)
    {
        objStringBuilder.Clear();
        OutputVertices(mesh);
        OutputTextureCoordinates(mesh);
        OutputNormals(mesh);
        OutputMaterialToUse(Color.white);
        OutputTriangles(mesh);
        PrintToFile(objStringBuilder.ToString(), $"{SavePath}/{name}.pobj");
    }

    private static void OutputVertices(Mesh mesh)
    {
        foreach (Vector3 vertex in mesh.vertices)
        {
            objStringBuilder.AppendLine($"v {vertex.x} {vertex.y} {vertex.z}");
        }
    }

    private static void OutputTextureCoordinates(Mesh mesh)
    {
        foreach (Vector2 uv in mesh.uv)
            objStringBuilder.AppendLine($"vt {uv.x} {uv.y}");
    }

    private static void OutputNormals(Mesh mesh)
    {
        foreach (Vector3 normal in mesh.normals)
        {
            objStringBuilder.AppendLine($"vn {normal.x} {normal.y} {normal.z}");
        }
    }

    private static void OutputMaterialToUse(Color color)
    {
        objStringBuilder.AppendLine($"usemtl {ColorUtility.ToHtmlStringRGB(color)}");
    }

    private static void OutputTriangles(Mesh mesh)
    {
        int[] triangles = mesh.triangles;
        int numberOfTriangles = triangles.Length / 3;

        // Note that the indexes in a .obj are 1-indexed, not 0 indexed.
        for (int i = 0; i < numberOfTriangles; i++)
        {
            int firstTriangleIndex = i * 3;

            objStringBuilder.AppendLine($"f {triangles[firstTriangleIndex] + 1}/{triangles[firstTriangleIndex]  + 1}/{triangles[firstTriangleIndex]  + 1} {triangles[firstTriangleIndex + 1]  + 1}/{triangles[firstTriangleIndex + 1]  + 1}/{triangles[firstTriangleIndex + 1]  + 1} {triangles[firstTriangleIndex + 2]  + 1}/{triangles[firstTriangleIndex + 2]  + 1}/{triangles[firstTriangleIndex + 2]  + 1}");
        }
    }

    private static void PrintVector3(Vector3 v)
    {
        Debug.Log($"{v.x:0.00}, {v.y:0.00}, {v.z:0.00}");
    }

    private static void PrintToFile(string output, string filePath)
    {
        StreamWriter writer = new StreamWriter(filePath, false);
        writer.Write(output);
        writer.Close();

        Debug.Log($"Wrote {filePath}");
    }
}