using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.CFGParser
{
    public static class Helpers
    {
        public static float Sum(this Color color)
        {
            return color.a + color.b + color.r + color.g;
        }

        public static Color FromText(string text)
        {
            return new Color(int.Parse(text.Substring(0, 2), NumberStyles.HexNumber) / 255.0f,
                int.Parse(text.Substring(2, 2), NumberStyles.HexNumber) / 255.0f, int.Parse(text.Substring(4, 2), NumberStyles.HexNumber) / 255.0f);
        }

        // Find distance between item pos in tne
        public static TerrainChunk FindClosestTerrain(TerrainGenerator terrainChunksParent, Vector2 itemPos)
        {
            float minDistance = float.PositiveInfinity;
            TerrainChunk closetChunk = null;
            var chunks = terrainChunksParent.GetTerrainChunks();

            for (int i = 0; i < chunks.Length; i++)
            {
                var currDist = Vector2.Distance(itemPos, chunks[i].sampleCentre);

                if (currDist < minDistance)
                {
                    minDistance = currDist;
                    closetChunk = chunks[i];
                }
            }

            return closetChunk;
        }

        // TODO Naive! Could be done better
        public static VertexAndDist NearestVertexTo(MeshFilter chunk, Vector3 point)
        {
            const float minDist = 0.35f;

            // convert point to local space
            point = chunk.transform.InverseTransformPoint(point);

            Mesh mesh = chunk.mesh;
            float minDistanceSqr = Mathf.Infinity;
            Vector3 nearestVertex = Vector3.zero;
            int index = 0;
			
			// MUST save the vertices in a temp array. Accessing them from the mesh is
			// apparently very costly
			var tmpVerts = mesh.vertices;

            // scan all vertices to find nearest
            for (int i = 0; i < tmpVerts.Length; i++)
            {
                Vector3 diff = point - tmpVerts[i];
                float distSqr = diff.sqrMagnitude;

                if (distSqr < minDistanceSqr)
                {
                    minDistanceSqr = distSqr;
                    nearestVertex = tmpVerts[i];
                    index = i;

                    if (distSqr < minDist)
                    {
                        break; // More efficient
                    }
                }
            }

            // convert nearest vertex back to world space
            return new VertexAndDist(chunk.transform.TransformPoint(nearestVertex), 
                (minDistanceSqr == Mathf.Infinity) ? 0 : minDistanceSqr, index);
        }
    }

    public struct VertexAndDist
    {
        public Vector3 vertex;
        public float distance;
        public int index;

        public VertexAndDist(Vector3 vertex, float distance, int index)
        {
            this.vertex = vertex;
            this.distance = distance;
            this.index = index;
        }
    }
}
