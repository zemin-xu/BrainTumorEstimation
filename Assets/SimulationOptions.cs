using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationOptions : MonoBehaviour
{
    public GameObject brain;

    public TextAsset jsonFile;

    private GameObject boundingBox;
    private Renderer rend;
    private Bounds bounds;

    private void Start()
    {
        boundingBox = GameObject.CreatePrimitive(PrimitiveType.Cube);

        BrainData loadedData = ReadData();
        boundingBox.transform.position = loadedData.boxCenter;
        boundingBox.transform.localScale = loadedData.boxDimension;

        // output brain size
        rend = brain.GetComponent<Renderer>();
        bounds = brain.GetComponent<MeshFilter>().mesh.bounds;

        Vector3 center = rend.bounds.center;
        float radius = rend.bounds.extents.magnitude;

        float x = rend.bounds.extents.x;
        float y = rend.bounds.extents.y;
        float z = rend.bounds.extents.z;

        Debug.Log("radius: " + radius);
        Debug.Log("x: " + x);
        Debug.Log("y: " + y);
        Debug.Log("z: " + z);

        Vector3 c = bounds.center;
        float r = bounds.extents.magnitude;
        Debug.Log("r: " + r);
    }

    /*
    private void OnDrawGizmosSelected()
    {
        // A sphere that fully encloses the bounding box.

        Vector3 center = rend.bounds.center;
        float radius = rend.bounds.extents.magnitude;

        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(center, radius);
        Debug.Log("radius: " + radius);
        Gizmos.DrawWireCube(center, new Vector3(radius, radius, radius));

        Vector3 c = bounds.center;
        float r = bounds.extents.magnitude;

        Debug.Log("mesh radius: " + r);
        Gizmos.DrawWireCube(c, new Vector3(r, r, r));
    }
    */

    public void ImportModel(GameObject model)
    {
    }

    public BrainData ReadData()
    {
        return JsonUtility.FromJson<BrainData>(jsonFile.text);
    }

    public void ChangeTransform()
    {
        if (brain != null)
        {
        }
    }
}