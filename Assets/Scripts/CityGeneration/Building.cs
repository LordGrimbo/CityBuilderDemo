using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    Foundation foundation;
    float sideWalkWidth;
    float sideWalkheight;
    float maxLength;

    float wallHeight;
    float wallThickness;

    int stories;
    Shader shader;

    public void Init(Foundation foundation, float sideWalkWidth, float sideWalkheight, float length, float height, float width, int storyMin, int storyMax, Shader shader)
    {
        this.foundation = foundation;
        this.sideWalkWidth = sideWalkWidth;
        this.sideWalkheight = sideWalkheight;

        this.stories = Random.Range(storyMin, storyMax);
        this.maxLength = length - (sideWalkWidth * 2);

        this.wallHeight = height;
        this.wallThickness = width;
        this.shader = shader;

        CreateLevels();
    }

    private void CreateLevels()
    {
        float currentHeight = sideWalkheight;
        float maxDistance = (maxLength);
        float innerMaxDistance = maxLength - (wallThickness * 2);

        float xOffsetEx = foundation.start.x;
        float zOffsetEx = foundation.start.y;

        float xOffsetIn = foundation.start.x + wallThickness;
        float zOffsetIn = foundation.start.y + wallThickness;

        List<Vector3> verts = new List<Vector3>();
        Material mat = new Material(shader);
        mat.color = Color.gray;

        //Make Floor
        verts.Add(new Vector3(xOffsetEx, currentHeight, zOffsetEx + maxDistance));
        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight, zOffsetEx + maxDistance));
        verts.Add(new Vector3(xOffsetEx, currentHeight, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight, zOffsetEx));

        GameObject floor = new GameObject("Sub_" + name, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        Mesh floorMesh = new Mesh();

        floorMesh.vertices = verts.ToArray();
        floorMesh.triangles = ApplyTriangles(verts.Count);

        floor.GetComponent<MeshCollider>().convex = true;
        floor.GetComponent<MeshFilter>().mesh = floorMesh;
        floor.GetComponent<MeshRenderer>().material = mat;
        floor.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        floor.transform.SetParent(transform);

        //Create Entrance Level
        verts.Clear();
        float doorSize = maxLength / 3;

        #region Outer Door
        verts.Add(new Vector3(xOffsetEx, doorSize, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + doorSize, doorSize, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx, currentHeight, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + doorSize, currentHeight, zOffsetEx));

        verts.Add(new Vector3(xOffsetEx + maxDistance - doorSize, doorSize, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + maxDistance, doorSize, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + maxDistance - doorSize, currentHeight, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight, zOffsetEx));

        verts.Add(new Vector3(xOffsetEx, currentHeight + wallHeight, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight + wallHeight, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx, doorSize, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + maxDistance, doorSize, zOffsetEx));
        #endregion

        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight + wallHeight, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight + wallHeight, zOffsetEx + maxDistance));
        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight, zOffsetEx + maxDistance));

        verts.Add(new Vector3(xOffsetEx, currentHeight + wallHeight, zOffsetEx + maxDistance));
        verts.Add(new Vector3(xOffsetEx, currentHeight + wallHeight, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx, currentHeight, zOffsetEx + maxDistance));
        verts.Add(new Vector3(xOffsetEx, currentHeight, zOffsetEx));

        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight + wallHeight, zOffsetEx + maxDistance));
        verts.Add(new Vector3(xOffsetEx, currentHeight + wallHeight, zOffsetEx + maxDistance));
        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight, zOffsetEx + maxDistance));
        verts.Add(new Vector3(xOffsetEx, currentHeight, zOffsetEx + maxDistance));

        #region Inner Door
        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight + wallHeight, zOffsetIn));
        verts.Add(new Vector3(xOffsetIn, currentHeight + wallHeight, zOffsetIn));
        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, doorSize, zOffsetIn));
        verts.Add(new Vector3(xOffsetIn, doorSize, zOffsetIn));

        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, doorSize, zOffsetIn));
        verts.Add(new Vector3(xOffsetEx + maxDistance - doorSize, doorSize, zOffsetIn));
        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight, zOffsetIn));
        verts.Add(new Vector3(xOffsetEx + maxDistance - doorSize, currentHeight, zOffsetIn));

        verts.Add(new Vector3(xOffsetEx + doorSize, doorSize, zOffsetIn));
        verts.Add(new Vector3(xOffsetIn, doorSize, zOffsetIn));
        verts.Add(new Vector3(xOffsetEx + doorSize, currentHeight, zOffsetIn));
        verts.Add(new Vector3(xOffsetIn, currentHeight, zOffsetIn));
        #endregion

        verts.Add(new Vector3(xOffsetIn, currentHeight + wallHeight, zOffsetIn));
        verts.Add(new Vector3(xOffsetIn, currentHeight + wallHeight, zOffsetIn + innerMaxDistance));
        verts.Add(new Vector3(xOffsetIn, currentHeight, zOffsetIn));
        verts.Add(new Vector3(xOffsetIn, currentHeight, zOffsetIn + innerMaxDistance));

        verts.Add(new Vector3(xOffsetIn, currentHeight + wallHeight, zOffsetIn + innerMaxDistance));
        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight + wallHeight, zOffsetIn + innerMaxDistance));
        verts.Add(new Vector3(xOffsetIn, currentHeight, zOffsetIn + innerMaxDistance));
        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight, zOffsetIn + innerMaxDistance));

        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight + wallHeight, zOffsetIn + innerMaxDistance));
        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight + wallHeight, zOffsetIn));
        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight, zOffsetIn + innerMaxDistance));
        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight, zOffsetIn));

        //Door Arch
        verts.Add(new Vector3(xOffsetEx + doorSize, doorSize, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + doorSize, doorSize, zOffsetIn));
        verts.Add(new Vector3(xOffsetEx + doorSize, currentHeight, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + doorSize, currentHeight, zOffsetIn));

        verts.Add(new Vector3(xOffsetEx + maxDistance - doorSize, doorSize, zOffsetIn));
        verts.Add(new Vector3(xOffsetEx + maxDistance - doorSize, doorSize, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + maxDistance - doorSize, currentHeight, zOffsetIn));
        verts.Add(new Vector3(xOffsetEx + maxDistance - doorSize, currentHeight, zOffsetEx));

        verts.Add(new Vector3(xOffsetEx + doorSize, doorSize, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + maxDistance - doorSize, doorSize, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + doorSize, doorSize, zOffsetIn));
        verts.Add(new Vector3(xOffsetEx + maxDistance - doorSize, doorSize, zOffsetIn));
        

        GameObject levelOne = new GameObject("Sub_" + name, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        Mesh levelOneMesh = new Mesh();

        levelOneMesh.vertices = verts.ToArray();
        levelOneMesh.triangles = ApplyTriangles(verts.Count);

        levelOne.GetComponent<MeshFilter>().mesh = levelOneMesh;

        mat = new Material(shader);
        mat.color = Random.ColorHSV();
        levelOne.GetComponent<MeshCollider>().convex = true;
        levelOne.GetComponent<MeshRenderer>().material = mat;
        levelOne.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        levelOne.transform.SetParent(transform);

        currentHeight += wallHeight;

        //Make Rest
        for (int l = 0; l < stories-1; l++)
        {
            verts.Clear();

            #region Inner Wall
            verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight + wallHeight, zOffsetIn));
            verts.Add(new Vector3(xOffsetIn, currentHeight + wallHeight, zOffsetIn));
            verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight, zOffsetIn));
            verts.Add(new Vector3(xOffsetIn, currentHeight, zOffsetIn));

            verts.Add(new Vector3(xOffsetIn, currentHeight + wallHeight, zOffsetIn));
            verts.Add(new Vector3(xOffsetIn, currentHeight + wallHeight, zOffsetIn + innerMaxDistance));
            verts.Add(new Vector3(xOffsetIn, currentHeight, zOffsetIn));
            verts.Add(new Vector3(xOffsetIn, currentHeight, zOffsetIn + innerMaxDistance));

            verts.Add(new Vector3(xOffsetIn, currentHeight + wallHeight, zOffsetIn + innerMaxDistance));
            verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight + wallHeight, zOffsetIn + innerMaxDistance));
            verts.Add(new Vector3(xOffsetIn, currentHeight, zOffsetIn + innerMaxDistance));
            verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight, zOffsetIn + innerMaxDistance));

            verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight + wallHeight, zOffsetIn + innerMaxDistance));
            verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight + wallHeight, zOffsetIn));
            verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight, zOffsetIn + innerMaxDistance));
            verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight, zOffsetIn));
            #endregion

            verts.Add(new Vector3(xOffsetEx, currentHeight + wallHeight, zOffsetEx));
            verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight + wallHeight, zOffsetEx));
            verts.Add(new Vector3(xOffsetEx, currentHeight, zOffsetEx));
            verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight, zOffsetEx));

            verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight + wallHeight, zOffsetEx));
            verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight + wallHeight, zOffsetEx + maxDistance));
            verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight, zOffsetEx));
            verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight, zOffsetEx + maxDistance));

            verts.Add(new Vector3(xOffsetEx, currentHeight + wallHeight, zOffsetEx + maxDistance));
            verts.Add(new Vector3(xOffsetEx, currentHeight + wallHeight, zOffsetEx));
            verts.Add(new Vector3(xOffsetEx, currentHeight, zOffsetEx + maxDistance));
            verts.Add(new Vector3(xOffsetEx, currentHeight, zOffsetEx));

            verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight + wallHeight, zOffsetEx + maxDistance));
            verts.Add(new Vector3(xOffsetEx, currentHeight + wallHeight, zOffsetEx + maxDistance));
            verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight, zOffsetEx + maxDistance));
            verts.Add(new Vector3(xOffsetEx, currentHeight, zOffsetEx + maxDistance));

            GameObject subLevel = new GameObject("Sub_" + name, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
            Mesh subMesh = new Mesh();

            subMesh.vertices = verts.ToArray();
            subMesh.triangles = ApplyTriangles(verts.Count);

            subLevel.GetComponent<MeshFilter>().mesh = subMesh;

            mat = new Material(shader);
            mat.color = Random.ColorHSV();
            subLevel.GetComponent<MeshCollider>().convex = true;
            subLevel.GetComponent<MeshRenderer>().material = mat;
            subLevel.GetComponent<MeshFilter>().mesh.RecalculateNormals();
            subLevel.transform.SetParent(transform);

            currentHeight += wallHeight;
        }

        //Add Roof
        verts.Clear();

        #region Inner Wall
        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight + wallHeight, zOffsetIn));
        verts.Add(new Vector3(xOffsetIn, currentHeight + wallHeight, zOffsetIn));
        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight, zOffsetIn));
        verts.Add(new Vector3(xOffsetIn, currentHeight, zOffsetIn));

        verts.Add(new Vector3(xOffsetIn, currentHeight + wallHeight, zOffsetIn));
        verts.Add(new Vector3(xOffsetIn, currentHeight + wallHeight, zOffsetIn + innerMaxDistance));
        verts.Add(new Vector3(xOffsetIn, currentHeight, zOffsetIn));
        verts.Add(new Vector3(xOffsetIn, currentHeight, zOffsetIn + innerMaxDistance));

        verts.Add(new Vector3(xOffsetIn, currentHeight + wallHeight, zOffsetIn + innerMaxDistance));
        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight + wallHeight, zOffsetIn + innerMaxDistance));
        verts.Add(new Vector3(xOffsetIn, currentHeight, zOffsetIn + innerMaxDistance));
        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight, zOffsetIn + innerMaxDistance));

        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight + wallHeight, zOffsetIn + innerMaxDistance));
        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight + wallHeight, zOffsetIn));
        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight, zOffsetIn + innerMaxDistance));
        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight, zOffsetIn));
        #endregion

        verts.Add(new Vector3(xOffsetEx, currentHeight + wallHeight + wallThickness, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight + wallHeight + wallThickness, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx, currentHeight, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight, zOffsetEx));

        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight + wallHeight + wallThickness, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight + wallHeight + wallThickness, zOffsetEx + maxDistance));
        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight, zOffsetEx + maxDistance));

        verts.Add(new Vector3(xOffsetEx, currentHeight + wallHeight + wallThickness, zOffsetEx + maxDistance));
        verts.Add(new Vector3(xOffsetEx, currentHeight + wallHeight + wallThickness, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx, currentHeight, zOffsetEx + maxDistance));
        verts.Add(new Vector3(xOffsetEx, currentHeight, zOffsetEx));

        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight + wallHeight + wallThickness, zOffsetEx + maxDistance));
        verts.Add(new Vector3(xOffsetEx, currentHeight + wallHeight + wallThickness, zOffsetEx + maxDistance));
        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight, zOffsetEx + maxDistance));
        verts.Add(new Vector3(xOffsetEx, currentHeight, zOffsetEx + maxDistance));

        //Inner Roof
        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight + wallHeight, zOffsetIn + innerMaxDistance));
        verts.Add(new Vector3(xOffsetIn, currentHeight + wallHeight, zOffsetIn + innerMaxDistance));
        verts.Add(new Vector3(xOffsetIn + innerMaxDistance, currentHeight + wallHeight, zOffsetIn));
        verts.Add(new Vector3(xOffsetIn, currentHeight + wallHeight, zOffsetIn));

        //Outer Roof
        verts.Add(new Vector3(xOffsetEx, currentHeight + wallHeight + wallThickness, zOffsetEx + maxDistance));
        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight + wallHeight + wallThickness, zOffsetEx + maxDistance));
        verts.Add(new Vector3(xOffsetEx, currentHeight + wallHeight + wallThickness, zOffsetEx));
        verts.Add(new Vector3(xOffsetEx + maxDistance, currentHeight + wallHeight + wallThickness, zOffsetEx));

        GameObject roofLevel = new GameObject("Sub_" + name, typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        Mesh roofMesh = new Mesh();

        roofMesh.vertices = verts.ToArray();
        roofMesh.triangles = ApplyTriangles(verts.Count);

        roofLevel.GetComponent<MeshFilter>().mesh = roofMesh;

        mat = new Material(shader);
        mat.color = Random.ColorHSV();
        roofLevel.GetComponent<MeshCollider>().convex = true;
        roofLevel.GetComponent<MeshRenderer>().material = mat;
        roofLevel.GetComponent<MeshFilter>().mesh.RecalculateNormals();
        roofLevel.transform.SetParent(transform);

        currentHeight += wallHeight;
    }

    private int[] ApplyTriangles(int vertsCount)
    {
        List<int> tries = new List<int>();
        int triangleIndex = 0;

        vertsCount = vertsCount / 4;
        for (int t = 0; t < vertsCount; t++)
        {
            tries.Add(triangleIndex + 0);
            tries.Add(triangleIndex + 1);
            tries.Add(triangleIndex + 2);
            tries.Add(triangleIndex + 2);
            tries.Add(triangleIndex + 1);
            tries.Add(triangleIndex + 3);

            triangleIndex += 4;
        }

        return tries.ToArray();
    }
}
