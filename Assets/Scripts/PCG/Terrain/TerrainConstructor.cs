using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainConstructor {

    public static float GetAltitude(EnvironmentGenome genome, float x, float z) {
        if(!genome.terrainGenome.useAltitude) {
            return 0f;
        }
        float altitude = 0f;
        float distMultiplier = 0.02f;
        float dist = new Vector2(x, z).magnitude * distMultiplier;
             
        float amplitude = 60f;
        float total = 0f;
        for (int i = 0; i < genome.terrainGenome.terrainWaves.Length; i++) {
            float height = NoisePrime.Simplex3D(new Vector3(x, genome.terrainGenome.terrainWaves[i].z, z), genome.terrainGenome.terrainWaves[i].x * Mathf.Pow(2f, i)).value * (amplitude * genome.terrainGenome.terrainWaves[i].y / Mathf.Pow(2f, i) * dist);
            total += height;
        }
        altitude = total / genome.terrainGenome.terrainWaves.Length;

        // StartPositions!!!
        for(int j = 0; j < genome.agentStartPositionsList.Count; j++) {
            float distToSpawn = (new Vector2(x, z) - new Vector2(genome.agentStartPositionsList[j].agentStartPosition.x, genome.agentStartPositionsList[j].agentStartPosition.z)).magnitude;
            if (distToSpawn < 12f) {
                altitude = Mathf.Lerp(0f, altitude, Mathf.Max(distToSpawn - 4f, 0f) / 8f);
            }
        }

        return altitude;

        // OLD!!!!
        /*
        
        float total = 0f;
        for (int i = 0; i < genome.terrainGenome.terrainWaves.Length; i++) {
            if (i % 2 == 0) {
                total += Mathf.Sin(x * genome.terrainGenome.terrainWaves[i].x + genome.terrainGenome.terrainWaves[i].z) * genome.terrainGenome.terrainWaves[i].y / genome.terrainGenome.terrainWaves[i].x; // divide by frequency
            }
            else {
                total += Mathf.Sin(z * genome.terrainGenome.terrainWaves[i].x + genome.terrainGenome.terrainWaves[i].z) * genome.terrainGenome.terrainWaves[i].y / genome.terrainGenome.terrainWaves[i].x;
            }
        }
        float height = total / genome.terrainGenome.terrainWaves.Length;
        float distToSpawn = new Vector2(x, z).magnitude;
        if (distToSpawn < 12f) {
            height = Mathf.Lerp(0f, height, Mathf.Max(distToSpawn - 4f, 0f) / 8f);
        }
        return height;*/
    }

    public static Mesh GetTerrainMesh(EnvironmentGenome genome, int xResolution, int zResolution, float xCenter, float zCenter, float xSize, float zSize) {
                
        float xQuadSize = xSize / (float)xResolution;
        float zQuadSize = zSize / (float)zResolution;
        Vector3 offset = new Vector3(xSize * 0.5f, 0f, zSize * 0.5f);

        Vector3[] vertices;

        Mesh mesh = new Mesh();

        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xResolution + 1) * (zResolution + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, z = 0; z <= zResolution; z++) {
            for (int x = 0; x <= xResolution; x++, i++) {
                float altitude = GetAltitude(genome, x * xQuadSize - offset.x + xCenter, z * zQuadSize - offset.z + zCenter);
                vertices[i] = new Vector3(x * xQuadSize, altitude, z * zQuadSize) - offset;
                uv[i] = new Vector2((float)x / xResolution, (float)z / zResolution);
                tangents[i] = tangent;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[xResolution * zResolution * 6];
        for (int ti = 0, vi = 0, z = 0; z < zResolution; z++, vi++) {
            for (int x = 0; x < xResolution; x++, ti += 6, vi++) {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xResolution + 1;
                triangles[ti + 5] = vi + xResolution + 2;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}
