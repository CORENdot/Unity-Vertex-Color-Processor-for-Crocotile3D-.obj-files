# Unity Vertex Color Processor for Crocotile3D .obj files

Original intention is to extend Unity importer to support Vertex Colors
from .obj files created with Crocotile3D perfectly.

It also changes "Scene" naming (non-object-tiles default group of Crocotile3D)
to filename_Scene, otherwise as the Unity Project grows it becomes impossible to 
search for individual Meshes if they are all called "Scene".

Tested on 2018.4.36f1 LTS, newer versions might need code changes due to deprecations.

# How it works

Place the script somewhere inside your Asset folder of your Unity project and next time
you import or modify an .obj it will load the Vertex Colors automatically into the resulting 
Meshes.

# Considerations and limitations:

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
                
    3 - If you have done the above steps and still get an error like "Mesh.colors is out of bounds"
        it might be cause your Crocotile3D project has intersecting vertices.
            - Try exporting the .obj file with Merge Vertices checkbox disabled in Crocotile3D
            export settings.
            
    4 - This script cannot distinguish .obj files created with Crocotile3D than other
        modeling softwares. If you have .obj files created from other
        modeling software imported or modified, THIS SCRIPT WILL PROBABLY FAIL!
            - Feel free to extend the code and add conditions that will prevent the script to 
            affect each .obj files of your project. 
            - Example: Name your Croco .obj files as filename_c3d.obj and add a condition check 
            for that "c3d" in code.
            
    5 - If the Unity project doesn't have Color Space set to Gamma in the Player Settings 
        vertex colors will result different than Crocotile 3D. (More subtle)
            - Gamma is the default Color Space on new projects by Unity.
            - There might be a workaround in ModelImporter settings for Linear space users, 
                check documentation.
            - Other solution might involve to change how the Colors are parsed in this script. 
            
    6 - It requires PreserveHierarchy, OptimizeMesh and WeldVertices from ModelImporter settings set to false
        so Unity doesn't mismatch with the .obj file vertex order. 
        (This script does this automatically for you on the OnPreprocessModel () function)
        
    7 - Performance on multiple .obj importing or modifications has not been extensively tested.
    
    8 - Use this AT YOUR OWN RISK! IT'S EXPERIMENTAL! My advice is to backup any .obj that you are 
        unable to recover, for whatever reason, before adding the script into your Unity project.

    9 - Enjoy!

# Why this is useful

It is not possible for Unity to load Vertex Colors from .obj created with Crocotile3D by default, they only load correctly from .dae.

This brings the problem that by using .dae the Use Groups functionality of .obj is lost and all Objects end merged together in the same Mesh unless individually exported. Which in complex Crocotile3D projects might end being very time consuming.

With this script you will have the best of both worlds by using .obj files: Vertex Colors, each Object as an individual Mesh or all merged together at will (based on Use Groups checkbox of .obj export settings enabled or not), Meshes having "filename_Scene" naming for better inspector searching and who knows what might come next.

# Ending

This is my gift for the Crocotile3D community, you can catch me on the official Crocotile3D discord. Thank Alex from my part for the incredible Crocotile3D software that is.
- CORENdot
