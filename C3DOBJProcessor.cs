#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/*
    C3DOBJProcessor script works after .obj models are imported 
    or modified within the Unity Editor.

    Intention is to extend Unity default model importer to support 
    Vertex Colors from .obj files created with Crocotile3D perfectly.

    It also changes "Scene" naming (non-object-tiles default group of Crocotile3D)
    to filename_Scene, otherwise as the Unity Project grows it becomes impossible to 
    search for individual Meshes if they are all called "Scene".

    Tested on 2018.4.36f1 LTS, newer versions might need code changes due to deprecations.

    CONSIDERATIONS AND LIMITATIONS:
    1 - Crocotile3D Object hierarchy needs to be alphanumerically ordered by YOU!
            - Only Objects, Instances' order don't matter.
            - Order them descendingly.
            - Pair equally named Objects together in the hierarchy, yet acknowledge
                that they will get merged into a single Mesh by Unity.
            - Layer names don't matter but the overall Object hierarchy still
            needs to be ordered.
            - Crocotile3D Hierarchy Examples:
                - Example of CORRECT Crocotile3D hierarchy:
                    - Superlayer Z
                        - Object A
                        - Object A
                        - Object B
                    - Object C
                - Example of INCORRECT Crocotile3D hierarchy:
                    - Superlayer Z
                        - Object A
                        - Object A
                        - Object C
                    - Object B
            - I have found no way to workaround this limitation, it probably needs
                a whole ModelImporter created from scratch which is out of my scope.

    2 - Don't call any Crocotile3D Object exactly "Scene", including case variations.
            - "Scene" is the default group (non-object-tiles group) of Crocotile3D
                the order where the vertex data ends if an Object is called "Scene"
                gets hard to be determined in many cases. Avoid it.

    3 - This script cannot distinguish .obj files created with Crocotile3D than other
        modeling softwares. If you have .obj files created from other
        modeling software imported or modified, THIS SCRIPT WILL PROBABLY FAIL!
            - Feel free to extend the code and add conditions that will prevent the script to 
            affect each .obj files of your project. 
            - Example: Name your Croco .obj files as filename_c3d.obj and add a condition check 
            for that "c3d" in code.

    4 - If the Unity project doesn't have Color Space set to Gamma in the Player Settings 
        vertex colors will result different than Crocotile 3D. (More subtle)
            - Gamma is the default Color Space on new projects by Unity.
            - There might be a workaround in ModelImporter functions to avoid this, check documentation.
            - Other solution might involve to change how the Colors are parsed in this script. 

    5 - It requires PreserveHierarchy, OptimizeMesh and WeldVertices from ModelImporter settings set to false
        so Unity doesn't mismatch with the .obj file vertex order. 
        (This script does this automatically for you on the OnPreprocessModel () function)

    6 - Performance on multiple .obj importing or modifications has not been extensively tested.

    7 - USE THIS AT YOUR OWN RISK! Backup your .obj files if you have no way of reimport them before
        adding this script to your Unity project!

    8 - Feel free to use this script however you want, yet remember that the original intention was
        to help Unity and Crocotile3D users for FREE.

    This is my gift for the Crocotile3D community, you can catch me on the official discord.
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
        /* 
            You can add extra modelImporter settings here. 
            Bear in mind Preserve Hierarchy, Optimize Mesh, and Weld Vertices
            need to be false for vertex colors to be loaded correctly.
        */
    }

    void OnPostprocessModel (GameObject gameObject)
    {
        if (!File.Exists (_fileFullPath) || !_extension.Equals (".obj")) return;

        string[] lines = File.ReadAllLines (_fileFullPath);

        Transform root = gameObject.transform;

        // - MESH FILTERS ORDER ALGORITHM HERE -
        List<MeshFilter> meshFilters = root.GetComponentsInChildren<MeshFilter> (true).ToList<MeshFilter> ();
        MeshFilter scene = meshFilters.Find (m => m.name.Equals ("Scene"));
        if (scene)
        {
            // If "Scene" Object exists place it first to read the vertex data.
            meshFilters.Remove (scene);
            meshFilters.Insert (0, scene);
            // - GAME OBJECT "SCENE" NAME CHANGE -  
            meshFilters [0].name = gameObject.name + "_Scene";
        }

        // Search the .obj file for first "v" line position to start reading there.
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
                }
                currFileLine++;
            }
            currentMesh.SetColors (vertexColors);
            vertexColors.Clear (); 
        }

        if (!errorFound) Debug.Log (gameObject.name + ".obj Vertex Colors loaded successfully!");
    }

}
#endif
