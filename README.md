# Unity Vertex Color Processor for Crocotile3D .obj files

Original intention is to extend Unity importer to support Vertex Colors
from .obj files created with Crocotile3D perfectly.

It also adds other quality of life improvements that you can find described
below in the Why this is useful section.

# How it works

Place the script somewhere inside your Asset folder of your Unity project and next time
you import or modify an .obj file it will load the Vertex Colors automatically into the resulting 
Meshes.

# Considerations and limitations:

    1 - Avoid equally named Objects in the Crocotile3D Hierarchy. Including casing variations!
        - Only Object names matter, Layers and Instances can be repeated.
        - If for some reason you need to have Objects named equally put them next to each other
            in the hierarchy and see if it works. Remember that Unity Importer merges
            all Objects called equally together into a single Mesh!
        - This would require the vertex data read algorithm to jump across the .obj file searching
            for each object that has the same name instead of reading sequentially. Can be done 
            but it's out of my scope for now, i'm also not sure the performance tradeoff is worth.

    2 - If you have any Object called "Scene" or it's possible case variations place it at top
        of the hierarchy.
            - Bear in mind it will get merged together with all other non-object-tiles of your
            Crocotile3D project into a single Mesh as "Scene" it's the default existing Object!
                
    3 - If you have done the above steps and still get an error like "Mesh.colors is out of bounds"
        it might be cause your Crocotile3D project has intersecting vertices.
            - This means a tile vertex ends intersecting another tile triangle.
            - Either find the culprit vertex and "fix it" or change the exporting settings 
                of the .obj file with Merge Vertices checkbox disabled in Crocotile3D.
            
    4 - If the Unity project doesn't have Color Space set to Gamma in the Player Settings 
        vertex colors will result different than Crocotile 3D. (More subtle)
            - Gamma is the default Color Space on new projects by Unity.
            - There might be a workaround in ModelImporter settings for Linear space users, 
                check documentation.
            - Other solution might involve to change how the Colors are parsed in this script. 
            
    5 - The script requires PreserveHierarchy, OptimizeMesh and WeldVertices from ModelImporter settings
        set to false so Unity doesn't mismatch with the .obj file vertex order. 
        (This script does this automatically for you on the OnPreprocessModel () function)
        
    6 - This script cannot distinguish .obj files created with Crocotile3D than other
        modeling softwares. If you have .obj files created from other
        modeling software imported or modified, THIS SCRIPT WILL PROBABLY FAIL!
            - Feel free to extend the code and add conditions that will prevent the script to 
            affect each .obj files of your project. 
            - Example: Name your Croco .obj files as filename_c3d.obj and add a condition check 
            for that "c3d" in code.        
        
    7 - Performance on multiple .obj importing or modifications has not been extensively tested.
    
    8 - Use this AT YOUR OWN RISK! IT'S EXPERIMENTAL! My advice is to backup any .obj that you are 
        unable to recover, for whatever reason, before adding the script into your Unity project.

    9 - Enjoy!

# Why this is useful

It is not possible for Unity to load Vertex Colors from .obj files created with Crocotile3D by default, they only load correctly from .dae files.

This brings the problem that by using .dae the Use Groups functionality of .obj is lost and all Objects end merged together in the same Mesh unless individually exported. 

Which in big Crocotile3D projects might be very time consuming.

With this script you will have some quality of life improvements by using .obj files: 

- Vertex Colors correctly loaded.
- Each Object as an individual Mesh or all merged together at will (based on Use Groups checkbox of .obj export settings in Crocotile3D)
- Meshes having "filename_Scene" naming for better inspector searching.
- Matching Vertex Colors from Crocotile3D to Unity in Gamma color space.

# Last tested with

- Unity 2018.4.36f1 LTS, newer versions might need code changes due to deprecations.
- .obj files created with Crocotile3D 1.8.5

# Ending

This is my gift for the Crocotile3D community, you can catch me on the official Crocotile3D discord. Thank Alex from my part for the incredible piece of software that Crocotile3D is.
- CORENdot
