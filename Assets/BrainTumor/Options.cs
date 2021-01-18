using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityVolumeRendering;
using System;

using System.IO;
using System.Linq;

public class Options : MonoBehaviour
{
    public string dataIndex;
    public TextAsset jsonFile;

    private VolumeRenderedObject brain;
    private VolumeRenderedObject brainSeg;

    private GameObject boundingBox;
    private Renderer rend;
    private Bounds bounds;

    public void OnBoundingBoxButtonClicked()
    {
        boundingBox = GameObject.CreatePrimitive(PrimitiveType.Cube);

        BrainData loadedData = ReadData();
        boundingBox.transform.position = loadedData.boxCenter;
        boundingBox.transform.localScale = loadedData.boxDimension;

        // output brain size
        //rend = brain.transform.GetComponent<Renderer>();
        //bounds = brain.transform.GetComponent<MeshFilter>().mesh.bounds;

        //Vector3 center = rend.bounds.center;
        //float radius = rend.bounds.extents.magnitude;

        //float x = rend.bounds.extents.x;
        //float y = rend.bounds.extents.y;
        //float z = rend.bounds.extents.z;

        //Debug.Log("radius: " + radius);
        //Debug.Log("x: " + x);
        //Debug.Log("y: " + y);
        //Debug.Log("z: " + z);

        //Vector3 c = bounds.center;
        //float r = bounds.extents.magnitude;
        //Debug.Log("r: " + r);
    }

    public BrainData ReadData()
    {
        return JsonUtility.FromJson<BrainData>(jsonFile.text);
    }

    private VolumeRenderedObject Import(string dir)
    {
        if (Directory.Exists(dir))
        {
            bool recursive = true;

            // Read all files
            IEnumerable<string> fileCandidates = Directory.EnumerateFiles(dir, "*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Where(p => p.EndsWith(".dcm", StringComparison.InvariantCultureIgnoreCase) || p.EndsWith(".dicom", StringComparison.InvariantCultureIgnoreCase) || p.EndsWith(".dicm", StringComparison.InvariantCultureIgnoreCase));

            if (fileCandidates.Any())
            {
                DICOMImporter importer = new DICOMImporter(fileCandidates, Path.GetFileName(dir));
                VolumeDataset dataset = importer.Import();
                if (dataset != null)
                {
                    VolumeRenderedObject vo = VolumeObjectFactory.CreateObject(dataset);
                    vo.transform.Rotate(new Vector3(180, 0, 0));
                    return vo;
                }
            }
            else
            {
                Debug.LogError("Could not find any DICOM files to import.");
            }
        }
        return null;
    }

    public void OnImportButtonClicked()
    {
        brain = Import(dataIndex + "_flair");
    }

    public void OnSegmentationButtonClicked()
    {
        brainSeg = Import(dataIndex + "_seg");
    }
}