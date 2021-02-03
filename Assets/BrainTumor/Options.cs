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

    public Material matBrainSeg;
    private VolumeRenderedObject brain;
    private VolumeRenderedObject brainSeg;

    private GameObject boundingBox;
    private Renderer rend;

    private void UpdateBrain(VolumeRenderedObject vro)
    {
        if (vro != null)
        {
            /* update renderer and bounds */
            // volume data is stored in child gameobject
            rend = vro.transform.GetChild(0).GetComponent<Renderer>();

            Vector3 center = rend.bounds.center;
            float radius = rend.bounds.extents.magnitude;

            float x = rend.bounds.extents.x;
            float y = rend.bounds.extents.y;
            float z = rend.bounds.extents.z;

            // update position so that the world origin point is at the corner of brain
            vro.transform.position = new Vector3(x, y, z);
        }
    }

    public void OnBoundingBoxButtonClicked()
    {
        boundingBox = GameObject.CreatePrimitive(PrimitiveType.Cube);

        BrainData loadedData = ReadData();
        // scale of X and Z is 1.0, it contains 240 layers
        float ratio = 1.0f / 256.0f;
        boundingBox.transform.position = new Vector3(loadedData.boxCenter.z * ratio,
                                                    loadedData.boxCenter.y * ratio,
                                                    loadedData.boxCenter.x * ratio);

        boundingBox.transform.localScale = new Vector3(loadedData.boxDimension.z * ratio,
                                                    loadedData.boxDimension.y * ratio,
                                                    loadedData.boxDimension.x * ratio);

        Debug.Log(loadedData.boxCenter2.x); // output a vector3
        Debug.Log(loadedData.dataIndex); // output a vector3
        Debug.Log(loadedData.dataType); // output a vector3
    }

    private BrainData ReadData()
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
        UpdateBrain(brain);
    }

    public void OnSegmentationButtonClicked()
    {
        brainSeg = Import(dataIndex + "_seg");
        brainSeg.transform.GetChild(0).GetComponent<Renderer>().material = matBrainSeg;
        UpdateBrain(brainSeg);
    }
}