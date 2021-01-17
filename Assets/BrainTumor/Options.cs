using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityVolumeRendering;
using System;

using System.IO;
using System.Linq;

public class Options : MonoBehaviour
{
    public void OnImportButtonClicked(string dir)
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
                    VolumeObjectFactory.CreateObject(dataset);
            }
            else
                Debug.LogError("Could not find any DICOM files to import.");
        }
    }
}