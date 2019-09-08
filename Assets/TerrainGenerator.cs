using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int mDivisions = 128;
    public float mSize = 100;
    public float mHeight = 40;

    Vector3[] mVerts;
    int mVertCount;

    public Shader shader;

    // Start is called before the first frame update
    void Start()
    {
        // Generate terrain by using the diamond square algorithm
        // Source tutorial: https://www.youtube.com/watch?v=1HV8GbFnCik

        MeshFilter terrainMesh = gameObject.AddComponent<MeshFilter>();
        terrainMesh.mesh = generateTerrain();


        MeshRenderer terrainMeshRenderer = gameObject.AddComponent<MeshRenderer>();
        terrainMeshRenderer.material.shader = shader;
    }

    Mesh generateTerrain()
    {
        Mesh mesh = new Mesh();
        mesh.name = "TerrainMesh";

        mVertCount = (mDivisions + 1) * (mDivisions + 1);
        mVerts = new Vector3[mVertCount];
        // Each vert needs an uv
        Vector2[] uvs = new Vector2[mVertCount];
        // Number of triangles
        int[] tris = new int[mDivisions * mDivisions * 2 * 3];
        float halfSize = mSize * 0.5f;
        float divisionSize = mSize / mDivisions;

        int triOffset = 0;

        // Terrain will be a square
        for (int i = 0; i <= mDivisions; i++)
        {
            for (int j = 0; j <= mDivisions; j++)
            {
                // Set up verts positions
                // One-dimensional array to represent two-dimentional matrix
                // Build mesh from top-left to bottom-right, build each line from left to right
                mVerts[i * (mDivisions + 1) + j] = new Vector3(-halfSize + j * divisionSize, 0.0f,
                    halfSize - i * divisionSize);
                uvs[i * (mDivisions + 1) + j] = new Vector2((float)i / mDivisions, (float)j / mDivisions);

                // Build triangles
                if (i < mDivisions && j < mDivisions)
                {
                    // Top left vert
                    int topLeft = i * (mDivisions + 1) + j;
                    // Bottom left vert
                    int botLeft = (i + 1) * (mDivisions + 1) + j;

                    tris[triOffset] = topLeft;
                    tris[triOffset + 1] = topLeft + 1;
                    tris[triOffset + 2] = botLeft + 1;

                    tris[triOffset + 3] = topLeft;
                    tris[triOffset + 4] = botLeft + 1;
                    tris[triOffset + 5] = botLeft;

                    triOffset += 6;
                }
            }
        }

        // Diamond square: square top left
        mVerts[0].y = Random.Range(-mHeight, mHeight);
        // Diamond square: square top right
        mVerts[mDivisions].y = Random.Range(-mHeight, mHeight);
        // Diamond square: square bottom left
        mVerts[mVerts.Length - 1 - mDivisions].y = Random.Range(-mHeight, mHeight);
        // Diamond square: square bottom right
        mVerts[mVerts.Length - 1].y = Random.Range(-mHeight, mHeight);

        // Number of iterations to go through (number of squares to be divided down)
        int iterations = (int)Mathf.Log(mDivisions, 2);
        // There is only one square first
        int numSquares = 1;
        int squareSize = mDivisions;
        for (int i = 0; i < iterations; i++)
        {
            int row = 0;
            for (int j = 0; j < numSquares; j++)
            {
                int col = 0;
                for (int k = 0; k < numSquares; k++)
                {

                    diamondSquare(row, col, squareSize, mHeight);

                    // Move to the next square
                    col += squareSize;
                }
                // Move to the next square
                row += squareSize;
            }
            // Increase the number of squares
            numSquares *= 2;
            // Reduce every square's size by half
            squareSize /= 2;
            // Reduce the maximum height
            mHeight *= 0.5f;
        }

        mesh.vertices = mVerts;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();


        return mesh;

    }

    void diamondSquare(int row, int col, int size, float offset)
    {
        // The half size of a square
        int halfSize = (int)(size * 0.5f);
        int topLeft = row * (mDivisions + 1) + col;
        int botLeft = (row + size) * (mDivisions + 1) + col;
        // The midpoint of the square
        int midPoint = (int)(row + halfSize) * (mDivisions + 1) + (int)(col + halfSize);

        // The diamond step
        mVerts[midPoint].y = (mVerts[topLeft].y + mVerts[topLeft + size].y + mVerts[botLeft].y + mVerts[botLeft + size].y)
            / 4 // Calculate the average four corner points
            + Random.Range(-offset, offset); // Plus a random values

        // The north midpoint
        mVerts[topLeft + halfSize].y = (mVerts[topLeft].y + mVerts[topLeft + size].y + mVerts[midPoint].y)
            / 3 + Random.Range(-offset, offset); // Average plus a random value
        // The west midpoint
        mVerts[midPoint - halfSize].y = (mVerts[topLeft].y + mVerts[botLeft].y + mVerts[midPoint].y)
            / 3 + Random.Range(-offset, offset);
        // The east midpoint
        mVerts[midPoint + halfSize].y = (mVerts[topLeft + size].y + mVerts[botLeft + size].y + mVerts[midPoint].y)
            / 3 + Random.Range(-offset, offset);
        // The south midpoint
        mVerts[botLeft + halfSize].y = (mVerts[botLeft].y + mVerts[botLeft + size].y + mVerts[midPoint].y)
            / 3 + Random.Range(-offset, offset);
    }
}
