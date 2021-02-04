using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityVolumeRendering;
using System;

using System.IO;
using System.Linq;

public class Options : MonoBehaviour
{
    private string dataPrefix = "./Assets/BrainTumor/Data/";
    private string completePath;

    private BrainData brainData;

    public TextAsset jsonFile;

    public Material matBrainSeg;
    private VolumeRenderedObject brain;
    private VolumeRenderedObject brainSeg;

    private GameObject boundingBox;
    private GameObject boundingBox2;
    private Renderer rend;

    private void Start()
    {
        brainData = ReadData();
        completePath = dataPrefix + brainData.dataType + "_" + brainData.dataIndex;
        Debug.Log(completePath);
    }

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
        boundingBox2 = GameObject.CreatePrimitive(PrimitiveType.Cube);

        float ratio = 1.0f / 256.0f;

        boundingBox.transform.position = new Vector3(brainData.boxCenter.z * ratio,
                                                    brainData.boxCenter.y * ratio,
                                                    brainData.boxCenter.x * ratio);

        boundingBox.transform.localScale = new Vector3(brainData.boxDimension.z * ratio,
                                                    brainData.boxDimension.y * ratio,
                                                    brainData.boxDimension.x * ratio);

        boundingBox2.transform.position = new Vector3(brainData.boxCenter2.z * ratio,
                                                    brainData.boxCenter2.y * ratio,
                                                    brainData.boxCenter2.x * ratio);

        boundingBox2.transform.localScale = new Vector3(brainData.boxDimension2.z * ratio,
                                                    brainData.boxDimension2.y * ratio,
                                                    brainData.boxDimension2.x * ratio);
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
        brain = Import(completePath + "_flair");
        UpdateBrain(brain);
    }

    public void OnSegmentationButtonClicked()
    {
        brainSeg = Import(completePath + "_seg");
        brainSeg.transform.GetChild(0).GetComponent<Renderer>().material = matBrainSeg;
        UpdateBrain(brainSeg);
    }
}