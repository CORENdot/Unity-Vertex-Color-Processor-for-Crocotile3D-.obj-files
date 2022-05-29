#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/*
    C3DOBJProcessor script loads Vertex Colors from .obj files created with Crocotile3D
    when they are imported or modified within the Unity Editor.

    Read all Considerations and Limitations carefully from GitHub readme if you get a case
    where it doesn't work properly:
    https://github.com/CORENdot/Unity-Vertex-Color-Processor-for-Crocotile3D-.obj-files

    - CORENdot
*/
public sealed class C3DOBJProcessor : AssetPostprocessor
{

    private string _fileName;
    private string _extension;
    private string _fileFullPath;

    void OnPreprocessModel ()
    {
        _fileName = assetPath.Replace ("Assets", "");
        _extension = _fileName.Substring (_fileName.Length - 4);
        _fileFullPath = Application.dataPath + _fileName;

        if (!File.Exists (_fileFullPath) || !_extension.Equals (".obj")) return;

        ModelImporter modelImporter = (ModelImporter) assetImporter;
        modelImporter.preserveHierarchy = false;
        modelImporter.optimizeMesh = false;
        modelImporter.weldVertices = false;
        modelImporter.importNormals = ModelImporterNormals.None;
        /* 
            You can add extra modelImporter settings here. 
            Bear in mind that:
            - Preserve Hierarchy = false
            - Optimize Mesh = false
            - Weld Vertices = false
            - Import Normals = None
            need to be for vertex colors to be loaded correctly.
        */
    }

    void OnPostprocessModel (GameObject gameObject)
    {
        if (!File.Exists (_fileFullPath) || !_extension.Equals (".obj")) return;

        string[] lines = File.ReadAllLines (_fileFullPath);

        /*
            Find the order at which Objects Vertex Data is written in the .obj file.
            .obj file "g" lines seem to hold the accurate order.
        */
        List<string> ids = new List<string> ();
        for (int i = 0; i < lines.Length; i++)
        {
            string[] tokens = lines [i].Split (' ');
            if (tokens [0].Equals ("g"))
            {
                ids.Add (tokens [1]);
            }   
        }

        // Remove id duplicates.
        ids = ids.Distinct (StringComparer.InvariantCultureIgnoreCase).ToList ();

        // - MESH FILTERS ORDER ALGORITHM -
        Transform root = gameObject.transform;
        List<MeshFilter> meshFilters = new List<MeshFilter> ();
        foreach (string id in ids)
        {
            foreach (Transform child in root)
            {
                if (child.name.Equals (id))
                {
                    meshFilters.Add (child.GetComponent<MeshFilter> ());
                }
            }
        }

        MeshFilter scene = meshFilters.Find (m => m.name.Equals ("Scene"));
        if (scene)
        {
            // If "Scene" Object exists place it first to read the vertex data.
            meshFilters.Remove (scene);
            meshFilters.Insert (0, scene);
        }

        /*
            Search the .obj file for first "v" line position to start reading there.
            Lines that start with "v" hold each vertex data.
        */
        int currFileLine = 0;
        for (int i = 0; i < lines.Length && currFileLine == 0; i++)
        {
            string[] tokens = lines [i].Split (' ');
            if (tokens [0].Equals ("v"))
            {
                currFileLine = i;
            }
        }

        bool errorFound = false;
        int totalMeshes = meshFilters.Count;
        List<Color> vertexColors = new List<Color> ();
        for (int i = 0; i < totalMeshes; i++)
        {      
            Mesh currentMesh = meshFilters [i].sharedMesh;
            
            // - MESH "SCENE" NAME CHANGE -  
            if (currentMesh.name.Equals ("Scene")) currentMesh.name = gameObject.name + "_Scene";

            // - MESH VERTEX COLOR READ ALGORITHM - 
            int meshEndLine = currFileLine + currentMesh.vertexCount; 
            for (int j = currFileLine; j < meshEndLine; j++)
            {
                string[] tokens = lines [j].Split (' ');
                try
                {
                    float r = float.Parse (tokens [4], CultureInfo.InvariantCulture);
                    float g = float.Parse (tokens [5], CultureInfo.InvariantCulture);
                    float b = float.Parse (tokens [6], CultureInfo.InvariantCulture);

                    vertexColors.Add (new Color (r, g, b, 1f));
                }
                catch (Exception error)
                {
                    Debug.Log ("C3DOBJProcessor error = " + error.StackTrace);
                    errorFound = true;
                    break;
                }
                currFileLine++;
            }
            currentMesh.SetColors (vertexColors);
            currentMesh.RecalculateNormals ();
            currentMesh.RecalculateTangents ();
            vertexColors.Clear ();
        }

        if (!errorFound) Debug.Log (gameObject.name + ".obj Vertex Colors loaded successfully!");
    }

}
#endif
