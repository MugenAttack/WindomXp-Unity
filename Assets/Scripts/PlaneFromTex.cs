using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneFromTex
{
    public int pixelPerUnit = 100;
    
    public Mesh generateMeshFromTex(Texture2D tex)
    {
        float xPos = (float)(tex.width / pixelPerUnit) / 2;
        float yPos = (float)(tex.height / pixelPerUnit) / 2;

        Vector3[] vertex = new Vector3[4];
        vertex[0] = new Vector3(-xPos, yPos, 0);
        vertex[1] = new Vector3(xPos, yPos, 0);
        vertex[2] = new Vector3(xPos, -yPos, 0);
        vertex[3] = new Vector3(-xPos, -yPos, 0);

        Vector2[] UVs = new Vector2[4];
        UVs[0] = new Vector2(0, 0);
        UVs[1] = new Vector2(1.0f, 0);
        UVs[2] = new Vector2(1.0f, 1.0f);
        UVs[3] = new Vector2(0, 1.0f);

        int[] tris = new int[6];
        tris[0] = 0;
        tris[1] = 1;
        tris[2] = 3;
        tris[3] = 1;
        tris[4] = 2;
        tris[5] = 3;


        Mesh m = new Mesh();
        m.vertices = vertex;
        m.uv = UVs;
        m.triangles = tris;
        m.RecalculateNormals();
        return m;

    }



}
