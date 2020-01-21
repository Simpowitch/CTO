using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{

    public MeshFilter mesh;
    List<Vector3> vertices;
    private void Start()
    {
        vertices = new List<Vector3>(mesh.mesh.vertices);
        List<Vector3> verticesNoDuplicates = new List<Vector3>();

        for (int i = 0; i < vertices.Count; i++)
        {
            if (!verticesNoDuplicates.Contains(vertices[i]))
            {
                verticesNoDuplicates.Add(vertices[i]);
            }
        }
        vertices = verticesNoDuplicates;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < vertices.Count; i++)
        {
            Gizmos.DrawSphere(mesh.transform.position + vertices[i], 0.1f);
            print(i);
        }
    }
}
