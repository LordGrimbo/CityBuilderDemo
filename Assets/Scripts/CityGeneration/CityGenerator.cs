using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CityGenerator : MonoBehaviour
{
    [Header("Road Settings")]
    public float laneWidth = 3f;
    public int lanes = 3;
    public int intersectionDivisions = 1;
    public float intersectionDistance = 10f;
    [Space]
    public float sideWalkWidth = 2f;
    public float sideWalkHeightFromRoad = .4f;

    [Header("Building Settings")]
    public float wallThickness = 0.3f;
    public float wallHeight = 2f;
    [Space]
    public int minStories = 2;
    public int maxStories = 5;

    Foundation[] foundations;

    [Header("City Materials")]
    public Material roadMaterial;
    public Material sideWalkMaterial;
    public Shader buildingShader;

    private void Start()
    {
        CreateCity();
    }

    private void CreateCity()
    {
        //Make Roads
        GameObject roadsObj = new GameObject("Roads", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        roadsObj.transform.SetParent(transform);
        roadsObj.GetComponent<MeshCollider>().convex = true;
        roadsObj.GetComponent<MeshFilter>().mesh = GenerateRoad();
        roadsObj.GetComponent<MeshRenderer>().material = roadMaterial;
        roadsObj.GetComponent<MeshFilter>().mesh.RecalculateNormals();

        //Make SideWalk
        GameObject sideWalkObj = new GameObject("Side Walk", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        sideWalkObj.transform.SetParent(transform);
        sideWalkObj.GetComponent<MeshCollider>().convex = true;
        sideWalkObj.GetComponent<MeshFilter>().mesh = GenerateSideWalk();
        sideWalkObj.GetComponent<MeshRenderer>().material = sideWalkMaterial;
        sideWalkObj.GetComponent<MeshFilter>().mesh.RecalculateNormals();

        //Create Buildings
        for (int f = 0; f < foundations.Length; f++)
        {
            GameObject buildingObj = new GameObject("Building_" + f);            
            
            buildingObj.AddComponent<Building>().Init(foundations[f], sideWalkWidth, sideWalkHeightFromRoad, intersectionDistance, wallHeight, wallThickness, minStories, maxStories, buildingShader);
        }
    }

    private Mesh GenerateRoad()
    {
        //Create Lists
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        Square[,] intersections;
        Square[] roads;

        //Intersections
        int interLength = intersectionDivisions;
        intersections = new Square[interLength, interLength];

        float xOffset = sideWalkWidth;
        float zOffset = sideWalkWidth;

        float sectionSize = laneWidth * lanes;
        int triangleIndex = 0;

        for (int x = 0; x < interLength; x++)
        {
            for (int z = 0; z < interLength; z++)
            {
                intersections[x, z] = new Square(new Vector3[4]
                {
                    new Vector3(xOffset, 0, zOffset + sectionSize),
                    new Vector3(xOffset + sectionSize, 0, zOffset + sectionSize),
                    new Vector3(xOffset, 0, zOffset),
                    new Vector3(xOffset + sectionSize, 0, zOffset)
                });

                tris.Add(0 + triangleIndex);
                tris.Add(1 + triangleIndex);
                tris.Add(2 + triangleIndex);
                tris.Add(2 + triangleIndex);
                tris.Add(1 + triangleIndex);
                tris.Add(3 + triangleIndex);

                triangleIndex += 4;
                zOffset += sectionSize + intersectionDistance;
            }

            zOffset = sideWalkWidth;
            xOffset += sectionSize + intersectionDistance;
        }

        //Road
        roads = new Square[(interLength - 1) * interLength * 2];
        int roadIndex = 0;

        //Along X Axis
        for (int x = 0; x < interLength - 1; x++)
        {
            for (int z = 0; z < interLength; z++)
            {
                Square inter1 = intersections[x, z];
                Square inter2 = intersections[x + 1, z];

                roads[roadIndex] = new Square(new Vector3[4] {
                        inter1.verts[1],
                        inter2.verts[0],
                        inter1.verts[3],
                        inter2.verts[2]
                    });

                tris.Add(0 + triangleIndex);
                tris.Add(1 + triangleIndex);
                tris.Add(2 + triangleIndex);
                tris.Add(2 + triangleIndex);
                tris.Add(1 + triangleIndex);
                tris.Add(3 + triangleIndex);

                triangleIndex += 4;
                roadIndex++;
            }
        }

        //Along Z Axis
        for (int x = 0; x < interLength; x++)
        {
            for (int z = 0; z < interLength - 1; z++)
            {
                Square inter1 = intersections[x, z];
                Square inter2 = intersections[x, z + 1];

                roads[roadIndex] = new Square(new Vector3[4] {
                        inter2.verts[2],
                        inter2.verts[3],
                        inter1.verts[0],
                        inter1.verts[1]
                    });

                tris.Add(0 + triangleIndex);
                tris.Add(1 + triangleIndex);
                tris.Add(2 + triangleIndex);
                tris.Add(2 + triangleIndex);
                tris.Add(1 + triangleIndex);
                tris.Add(3 + triangleIndex);

                triangleIndex += 4;
                roadIndex++;
            }
        }

        //Add Intersections
        for (int x = 0; x < interLength; x++)
        {
            for (int z = 0; z < interLength; z++)
            {
                for (int i = 0; i < intersections[x, z].verts.Length; i++)
                {
                    verts.Add(intersections[x, z].verts[i]);
                }
            }
        }

        //Add Roads
        for (int i = 0; i < roads.Length; i++)
        {
            for (int l = 0; l < roads[i].verts.Length; l++)
            {
                verts.Add(roads[i].verts[l]);
            }
        }

        //Remove Extra Verts
        List<Vector3> originalVerts = new List<Vector3>();

        for (int t = 0; t < tris.Count; t++)
        {
            Vector3 vert = verts[tris[t]];

            if (!originalVerts.Contains(vert))
            {
                originalVerts.Add(vert);
            }
        }

        for (int t = 0; t < tris.Count; t++)
        {
            Vector3 indexVert = verts[tris[t]];

            if (originalVerts.Contains(indexVert))
                tris[t] = originalVerts.IndexOf(indexVert);
        }

        //Mesh Uvs
        Vector2[] uvs = new Vector2[originalVerts.Count];

        for (int v = 0; v < originalVerts.Count; v++)
        {
            uvs[v] = new Vector2(originalVerts[v].x, originalVerts[v].z);
        }

        //Make Mesh
        Mesh mesh = new Mesh();

        mesh.vertices = originalVerts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }

    private Mesh GenerateSideWalk()
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        int triangleIndex = 0;

        //Make Exterior
        float outsideLength = sideWalkWidth + ((laneWidth * lanes) + (intersectionDistance)) * (intersectionDivisions-1) + (laneWidth * lanes);
        float xOffset = 0;
        float zOffset = 0;

        Square[] exCorners = new Square[4];

        for (int i = 0, x = 0;  x < 2; x++)
        {
            for (int z = 0; z < 2; z++)
            {
                exCorners[i] = new Square(new Vector3[4]{
                    new Vector3(xOffset, sideWalkHeightFromRoad, zOffset + sideWalkWidth),
                    new Vector3(xOffset + sideWalkWidth, sideWalkHeightFromRoad, zOffset + sideWalkWidth),
                    new Vector3(xOffset, sideWalkHeightFromRoad, zOffset),
                    new Vector3(xOffset + sideWalkWidth, sideWalkHeightFromRoad, zOffset)
                });

                tris.Add(triangleIndex + 0);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 2);
                tris.Add(triangleIndex + 2);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 3);

                zOffset += outsideLength;
                i++;
                triangleIndex += 4;
            }

            zOffset = 0;
            xOffset += outsideLength;
        }

        //Pave The Exterior
        for (int t = 0; t < 4; t += 2)
        {
            int subIndex = triangleIndex - (4 * t);
            int one = subIndex - 8;
            int two = subIndex - 4;

            tris.Add(two + 2);
            tris.Add(two + 3);
            tris.Add(one + 0);

            tris.Add(one + 0);
            tris.Add(two + 3);
            tris.Add(one + 1);
        }

        for (int t = 4; t <= 8; t += 4)
        {
            int subIndex = triangleIndex - (t);
            int one = subIndex - 8;
            int two = subIndex;

            tris.Add(one + 1);
            tris.Add(two + 0);
            tris.Add(one + 3);

            tris.Add(one + 3);
            tris.Add(two + 0);
            tris.Add(two + 2);
        }

        #region Exterior Walls
        //Add Exterior Walls
        Square[] exWalls = new Square[4];

        exWalls[0] = new Square(new Vector3[4]{
            new Vector3(sideWalkWidth, sideWalkHeightFromRoad, sideWalkWidth),
            new Vector3(sideWalkWidth, sideWalkHeightFromRoad, outsideLength),
            new Vector3(sideWalkWidth, 0, sideWalkWidth),
            new Vector3(sideWalkWidth, 0, outsideLength)
        });

        tris.Add(triangleIndex + 0);
        tris.Add(triangleIndex + 1);
        tris.Add(triangleIndex + 2);

        tris.Add(triangleIndex + 2);
        tris.Add(triangleIndex + 1);
        tris.Add(triangleIndex + 3);

        triangleIndex += 4;

        exWalls[1] = new Square(new Vector3[4]{
            new Vector3(outsideLength, sideWalkHeightFromRoad, outsideLength),
            new Vector3(outsideLength, sideWalkHeightFromRoad, sideWalkWidth),
            new Vector3(outsideLength, 0, outsideLength),
            new Vector3(outsideLength, 0, sideWalkWidth)
        });

        tris.Add(triangleIndex + 0);
        tris.Add(triangleIndex + 1);
        tris.Add(triangleIndex + 2);

        tris.Add(triangleIndex + 2);
        tris.Add(triangleIndex + 1);
        tris.Add(triangleIndex + 3);

        triangleIndex += 4;

        exWalls[2] = new Square(new Vector3[4]{
            new Vector3(sideWalkWidth, sideWalkHeightFromRoad, outsideLength),
            new Vector3(outsideLength, sideWalkHeightFromRoad, outsideLength),
            new Vector3(sideWalkWidth, 0, outsideLength),
            new Vector3(outsideLength, 0, outsideLength)
        });

        tris.Add(triangleIndex + 0);
        tris.Add(triangleIndex + 1);
        tris.Add(triangleIndex + 2);

        tris.Add(triangleIndex + 2);
        tris.Add(triangleIndex + 1);
        tris.Add(triangleIndex + 3);

        triangleIndex += 4;

        exWalls[3] = new Square(new Vector3[4]{
            new Vector3(outsideLength, sideWalkHeightFromRoad, sideWalkWidth),
            new Vector3(sideWalkWidth, sideWalkHeightFromRoad, sideWalkWidth),
            new Vector3(outsideLength, 0, sideWalkWidth),
            new Vector3(sideWalkWidth, 0, sideWalkWidth)
        });

        tris.Add(triangleIndex + 0);
        tris.Add(triangleIndex + 1);
        tris.Add(triangleIndex + 2);

        tris.Add(triangleIndex + 2);
        tris.Add(triangleIndex + 1);
        tris.Add(triangleIndex + 3);

        triangleIndex += 4;
        #endregion

        //Make Interior
        int plots = intersectionDivisions - 1;
        Square[] inCorners = new Square[4 * plots * plots];
        Square[] inPaths = new Square[4 * plots * plots];
        Square[] inWalls = new Square[4 * plots * plots];

        foundations = new Foundation[plots * plots];
        int foundationIndex = 0;

        float startingPoint = sideWalkWidth + (laneWidth * lanes);

        xOffset = startingPoint;
        zOffset = startingPoint;

        for (int x = 0, i = 0; x < plots; x++)
        {
            for (int z = 0; z < plots; z++)
            {
                xOffset = startingPoint + (intersectionDistance * x) + ((laneWidth * lanes) * x);
                zOffset = startingPoint + (intersectionDistance * z) + ((laneWidth * lanes) * z);

                inCorners[i] = new Square(new Vector3[4]
                {
                    new Vector3(xOffset, sideWalkHeightFromRoad, zOffset + sideWalkWidth),
                    new Vector3(xOffset + sideWalkWidth, sideWalkHeightFromRoad, zOffset + sideWalkWidth),
                    new Vector3(xOffset, sideWalkHeightFromRoad, zOffset),
                    new Vector3(xOffset + sideWalkWidth, sideWalkHeightFromRoad, zOffset)
                });

                tris.Add(triangleIndex + 0);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 2);

                tris.Add(triangleIndex + 2);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 3);

                triangleIndex += 4;

                xOffset += intersectionDistance - sideWalkWidth;

                inCorners[i+1] = new Square(new Vector3[4]
                {
                    new Vector3(xOffset, sideWalkHeightFromRoad, zOffset + sideWalkWidth),
                    new Vector3(xOffset + sideWalkWidth, sideWalkHeightFromRoad, zOffset + sideWalkWidth),
                    new Vector3(xOffset, sideWalkHeightFromRoad, zOffset),
                    new Vector3(xOffset + sideWalkWidth, sideWalkHeightFromRoad, zOffset)
                });

                tris.Add(triangleIndex + 0);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 2);

                tris.Add(triangleIndex + 2);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 3);

                triangleIndex += 4;

                xOffset -= (intersectionDistance - sideWalkWidth);
                zOffset += intersectionDistance - sideWalkWidth;

                inCorners[i + 2] = new Square(new Vector3[4]
                {
                    new Vector3(xOffset, sideWalkHeightFromRoad, zOffset + sideWalkWidth),
                    new Vector3(xOffset + sideWalkWidth, sideWalkHeightFromRoad, zOffset + sideWalkWidth),
                    new Vector3(xOffset, sideWalkHeightFromRoad, zOffset),
                    new Vector3(xOffset + sideWalkWidth, sideWalkHeightFromRoad, zOffset)
                });

                tris.Add(triangleIndex + 0);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 2);

                tris.Add(triangleIndex + 2);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 3);

                triangleIndex += 4;

                xOffset += (intersectionDistance - sideWalkWidth);

                inCorners[i + 3] = new Square(new Vector3[4]
                {
                    new Vector3(xOffset, sideWalkHeightFromRoad, zOffset + sideWalkWidth),
                    new Vector3(xOffset + sideWalkWidth, sideWalkHeightFromRoad, zOffset + sideWalkWidth),
                    new Vector3(xOffset, sideWalkHeightFromRoad, zOffset),
                    new Vector3(xOffset + sideWalkWidth, sideWalkHeightFromRoad, zOffset)
                });

                tris.Add(triangleIndex + 0);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 2);

                tris.Add(triangleIndex + 2);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 3);

                triangleIndex += 4;

                //Pave Paths
                inPaths[i] = new Square(new Vector3[4]
                {
                    inCorners[i+2].verts[2],
                     inCorners[i+2].verts[3],
                      inCorners[i].verts[0],
                        inCorners[i].verts[1]
                });

                tris.Add(triangleIndex + 0);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 2);

                tris.Add(triangleIndex + 2);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 3);

                triangleIndex += 4;

                inPaths[i+1] = new Square(new Vector3[4]
                {
                    inCorners[i+3].verts[2],
                     inCorners[i+3].verts[3],
                      inCorners[i+1].verts[0],
                        inCorners[i+1].verts[1]
                });

                tris.Add(triangleIndex + 0);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 2);

                tris.Add(triangleIndex + 2);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 3);

                triangleIndex += 4;

                inPaths[i+2] = new Square(new Vector3[4]
                {
                    inCorners[i+0].verts[1],
                     inCorners[i+1].verts[0],
                      inCorners[i+0].verts[3],
                        inCorners[i+1].verts[2]
                });

                tris.Add(triangleIndex + 0);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 2);

                tris.Add(triangleIndex + 2);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 3);

                triangleIndex += 4;

                inPaths[i+3] = new Square(new Vector3[4]
                {
                    inCorners[i+2].verts[1],
                     inCorners[i+3].verts[0],
                      inCorners[i+2].verts[3],
                        inCorners[i+3].verts[2]
                });

                tris.Add(triangleIndex + 0);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 2);

                tris.Add(triangleIndex + 2);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 3);

                triangleIndex += 4;

                //Build Inside Walls
                inWalls[i] = new Square(new Vector3[4]
                {
                    inCorners[i].verts[2],
                    inCorners[i+1].verts[3],
                    new Vector3(inCorners[i].verts[2].x, 0, inCorners[i].verts[2].z),
                    new Vector3(inCorners[i+1].verts[3].x, 0, inCorners[i+1].verts[3].z)
                });

                tris.Add(triangleIndex + 0);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 2);

                tris.Add(triangleIndex + 2);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 3);

                triangleIndex += 4;

                inWalls[i+1] = new Square(new Vector3[4]
                {
                    inCorners[i+1].verts[3],
                    inCorners[i+3].verts[1],
                    new Vector3(inCorners[i+1].verts[3].x, 0, inCorners[i+1].verts[3].z),
                    new Vector3(inCorners[i+3].verts[1].x, 0, inCorners[i+3].verts[1].z)
                });

                tris.Add(triangleIndex + 0);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 2);

                tris.Add(triangleIndex + 2);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 3);

                triangleIndex += 4;

                inWalls[i + 2] = new Square(new Vector3[4]
                {
                    inCorners[i+3].verts[1],
                    inCorners[i+2].verts[0],
                    new Vector3(inCorners[i+3].verts[1].x, 0, inCorners[i+3].verts[1].z),
                    new Vector3(inCorners[i+2].verts[0].x, 0, inCorners[i+2].verts[0].z)
                });

                tris.Add(triangleIndex + 0);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 2);

                tris.Add(triangleIndex + 2);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 3);

                triangleIndex += 4;

                inWalls[i + 3] = new Square(new Vector3[4]
                {
                    inCorners[i+2].verts[0],
                    inCorners[i].verts[2],
                    new Vector3(inCorners[i+2].verts[0].x, 0, inCorners[i+2].verts[0].z),
                    new Vector3(inCorners[i].verts[2].x, 0, inCorners[i].verts[2].z)
                });

                tris.Add(triangleIndex + 0);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 2);

                tris.Add(triangleIndex + 2);
                tris.Add(triangleIndex + 1);
                tris.Add(triangleIndex + 3);

                triangleIndex += 4;

                //Make A Foundation
                foundations[foundationIndex] = new Foundation(
                    new Vector2(inCorners[i].verts[1].x, inCorners[i].verts[1].z),
                    new Vector2(inCorners[i+3].verts[2].x, inCorners[i + 3].verts[2].z)
                    );

                i += 4;
                foundationIndex++;
            }
        }

        //Add verts
        for (int c = 0; c < exCorners.Length; c++)
        {
            for (int v = 0; v < exCorners[c].verts.Length; v++)
            {
                verts.Add(exCorners[c].verts[v]);
            }
        }

        for (int c = 0; c < exWalls.Length; c++)
        {
            for (int v = 0; v < exWalls[c].verts.Length; v++)
            {
                verts.Add(exWalls[c].verts[v]);
            }
        }

        for (int c = 0; c < inCorners.Length; c++)
        {
            for (int v = 0; v < inCorners[c].verts.Length; v++)
            {
                verts.Add(inCorners[c].verts[v]);
            }
        }

        for (int c = 0; c < inPaths.Length; c++)
        {
            for (int v = 0; v < inPaths[c].verts.Length; v++)
            {
                verts.Add(inPaths[c].verts[v]);
            }
        }

        for (int c = 0; c < inWalls.Length; c++)
        {
            for (int v = 0; v < inWalls[c].verts.Length; v++)
            {
                verts.Add(inWalls[c].verts[v]);
            }
        }

        //Remove Extra Verts
        List<Vector3> originalVerts = new List<Vector3>();

        for (int t = 0; t < tris.Count; t++)
        {
            Vector3 vert = verts[tris[t]];

            if (!originalVerts.Contains(vert))
            {
                originalVerts.Add(vert);
            }
        }

        for (int t = 0; t < tris.Count; t++)
        {
            Vector3 indexVert = verts[tris[t]];

            if (originalVerts.Contains(indexVert))
                tris[t] = originalVerts.IndexOf(indexVert);
        }

        //Make Uvs
        Vector2[] uvs = new Vector2[originalVerts.Count];

        for (int v = 0; v < originalVerts.Count; v++)
        {
            uvs[v] = new Vector2(originalVerts[v].x, originalVerts[v].z);
        }

        //Create Mesh
        Mesh mesh = new Mesh();

        mesh.vertices = originalVerts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}

//City Structs
public struct Square
{
    public Vector3[] verts;

    public Square(Vector3[] verts)
    {
        this.verts = verts;
    }
}

public struct Foundation
{
    public Vector2 start;
    public Vector2 end;

    public Foundation(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
    }
}
