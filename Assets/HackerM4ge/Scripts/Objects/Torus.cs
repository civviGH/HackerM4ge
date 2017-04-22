/**
 * Based on a script by Steffen (http://forum.unity3d.com/threads/torus-in-unity.8487/) (in $primitives_966_104.zip, originally named "Primitives.cs")
 *
 * Editted by Michael Zoller on December 6, 2015.
 * It was shortened by about 30 lines (and possibly sped up by a factor of 2) by consolidating math & loops and removing intermediate Collections.
 */
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Torus : MonoBehaviour
{

    public float segmentRadius = 1f;
    public float tubeRadius = 0.1f;
    public int numSegments = 32;
    public int numTubes = 12;

    void Start()
    {
        RefreshTorus();
    }

    public void RefreshTorus()
    {
        // Calculate size of a segment and a tube
        float segmentSize = 2 * Mathf.PI / (float)numSegments;
        float tubeSize = 2 * Mathf.PI / (float)numTubes;

        int numSegmentsPlusOne = numSegments + 1;
        int numTubesPlusOne = numTubes + 1;

        // Total vertices
        int totalVertices = numSegmentsPlusOne * numTubesPlusOne;

        // Total primitives
        int totalPrimitives = numSegments * numTubes * 2;

        // Total indices
        int totalIndices = totalPrimitives * 3;

        // Init the mesh
        Mesh mesh = new Mesh();

        // Init the vertex and triangle arrays
        Vector3[] vertices = new Vector3[totalVertices];
        Vector2[] uv = new Vector2[totalVertices];
        int[] triangleIndices = new int[totalIndices];

        // Create floats for our xyz coordinates
        float x, y, z;

        int ti = 0;

        // Begin loop that fills in both arrays
        for (int i = 0; i < numSegmentsPlusOne; i++)
        {
            // Find next (or first) segment offset
            int n = (i + 1) % numSegmentsPlusOne; // changed segmentList.Count to numSegments

            // Find the current and next segments
            int currentTubeOffset = i * numTubesPlusOne;
            int nextTubeOffset = n * numTubesPlusOne;

            //int ti = i * numTubes;

            for (int j = 0; j < numTubesPlusOne; j++)
            {
                // Find next (or first) vertex offset
                int m = (j + 1) % numTubesPlusOne; // changed currentTube.Count to numTubes

                // Find the 4 vertices that make up a quad
                int iv1 = currentTubeOffset + j;
                int iv2 = currentTubeOffset + m;
                int iv3 = nextTubeOffset + m;
                int iv4 = nextTubeOffset + j;

                // Calculate X, Y, Z coordinates.
                x = (segmentRadius + tubeRadius * Mathf.Cos(j * tubeSize)) * Mathf.Cos(i * segmentSize);
                z = (segmentRadius + tubeRadius * Mathf.Cos(j * tubeSize)) * Mathf.Sin(i * segmentSize);
                y = tubeRadius * Mathf.Sin(j * tubeSize);

                // Add the vertex to the vertex array
                vertices[iv1] = new Vector3(x, y, z);
                uv[iv1] = new Vector2((float)i / numSegments, (float)j / numTubes);
                Debug.LogFormat("i = {0}, j = {1}, iv1 = {2}, v = {3}, uv = {4}", i, j, iv1, vertices[iv1], uv[iv1]);

                if (n == 0 || m == 0)
                {
                    continue;
                }

                triangleIndices[ti++] = iv1;
                triangleIndices[ti++] = iv2;
                triangleIndices[ti++] = iv3;
                // Finish the quad
                triangleIndices[ti++] = iv3;
                triangleIndices[ti++] = iv4;
                triangleIndices[ti++] = iv1;
            }
        }
        mesh.vertices = vertices;
        mesh.triangles = triangleIndices;
        mesh.uv = uv;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals(); // added on suggestion of Eric5h5 & joaeba in the forum thread

        MeshFilter mFilter = GetComponent<MeshFilter>(); // tweaked to Generic
        mFilter.mesh = mesh;
    }
}
