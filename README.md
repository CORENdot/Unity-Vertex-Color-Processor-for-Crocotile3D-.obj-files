# Unity Vertex Color Processor for Crocotile3D .obj files

Original intention is to extend Unity 3D model importer to support Vertex Colors
from .obj files created with Crocotile3D perfectly.

It also adds other quality of life improvements that you can find described
below in the Why this is useful section.

# How it works

Place the script somewhere inside your Asset folder of your Unity project and next time
you import or modify an .obj file it will load the Vertex Colors automatically into the resulting 
Mesh/es.

# Considerations and Limitations

    1 - Avoid creating equally named Objects in the Crocotile3D Object Hierarchy (Scene tab). 
        - Including casing variations! (Example, EXAMPLE, eXaMpLe, etc...)
        - Only Object names matter, Instances and Layers names can be repeated.
        - If for some reason you need to have Objects named equally put them next to each other
            in the hierarchy to make it work.
            -   Yet bear in mind that Unity Importer merges Objects called equally into a single Mesh!

    2 - If you have created Object/s called "Scene" or it's possible casing variations place them at top
        of the Object Hierarchy in Crocotile3D (Scene tab).
            - This is a special case as "Scene" is the default existing Object in Crocotile3D, it get's
            applied to all non-grouped-tiles by default. 
                - Again, bear in mind that if you create Objects called "Scene" they will get merged with 
                the non-grouped-tiles into a single Mesh by Unity.
            
    3 - If you have done the above steps and still get an error like "Mesh.colors is out of bounds..." try
        exporting the .obj file with Merge Vertices disabled in the Crocotile3D export settings and 
        see if it works.
            - This is due to very specific cases that i still need to find out why the script breaks.
            
    4 - If the Unity project doesn't have Color Space set to Gamma in the Player Settings 
        vertex colors will result different than Crocotile 3D. (More subtle)
            - If you want to match Crocotile3D Vertex Colors with Unity in Color Space Linear mode you
            will have to modify the script, maybe a gamma correction is needed after parsing
            the Vertex Colors.
            
    5 - The script cannot distinguish .obj files created with Crocotile3D than other modeling softwares. 
            - Imported or modified .obj files created on other modeling softwares CAN MAKE THE SCRIPT FAIL!
            - Feel free to extend the code and add conditions that will prevent the script to 
            affect each .obj files of your project. 
            - Example: Name your Croco .obj files as filename_c3d.obj and add a condition check 
            for that "c3d" in code.        
    
    6 - Be aware, THIS SCRIPT IS EXPERIMENTAL! My advice is to backup any .obj that you are 
        unable to recover, for whatever reason, before adding the script into your Unity project. Performance
        on multiple .obj import and modifications has not extensively tested either.

    7 - Enjoy!

# Why this is useful

It is not possible for Unity to load Vertex Colors from .obj files created with Crocotile3D by default, they only load correctly from .dae files.

This rises the problem that by using .dae the Use Groups functionality of .obj export settings in Crocotile3D is lost and all Objects end merged together in the same Mesh (Scene) by the Unity Importer. 

The only workaround is to individually export each Object of the Crocotile3D project as .dae, which in big Crocotile3D projects is very time consuming.

With this script you will have some quality of life improvements by using .obj files:

- Vertex Colors correctly loaded from the .obj file into Unity resulting Mesh/es.
- Each Object as an individual Mesh or merged together at will based on:
    - Use Groups checkbox of .obj export settings in Crocotile3D enabled or disabled.
    - Multiple Objects named equally in the Object Hierarchy of Crocotile3D. (Scene tab)
- Meshes having "filename_Scene" naming for better inspector searching within Unity Editor.
- Matching Vertex Colors from Crocotile3D to Unity in Gamma color space.

# Last tested with

- Unity 2018.4.36f1 LTS
- Unity 2019.4.39f1 LTS
- .obj files created with Crocotile3D 1.8.5 - 1.8.8

# Ending

This is my gift for the Crocotile3D community, you can catch me on the official Crocotile3D discord. Thank Alex from my part for the incredible piece of software that Crocotile3D is.
- CORENdot
