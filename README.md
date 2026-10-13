# Unity Optimization Tech Demo
This is a small portfolio project demonstrating various optimization techniques in the Unity engine. It consists of one top-down shooter scene which can accumulate 2000+ entities in a short amount of time, with hundreds of objects being created and destroyed every second. The key systems that allow this project to handle this many entities are the C# Job System, Burst Compiling, Object Pooling, Level of Detail (LOD), and GPU instancing.

## Key Features

##### Object Pooling
- Can be toggled on/off
- Instead of destroying objects when no longer, they are disabled and still kept in the scene
- When a new object is needed, the object pool component checks to see if an inactive object is available in the scene
- If an inactive object is available, re-use it. New objects are only instantiated if no disabled objects are available in the scene
- One Object Pool component handles the pool of one prefab
- The Object Pool components are created dynamically when needed and managed by a PoolingManager script which can be accessed globally via a Service Locator
- This eliminates any lag spikes that may occur due to garbage collection in memory, while also reducing CPU overhead by limiting the use of Instantiate() and Destroy(), which are very expensive functions

##### C# Job System and Burst Compiling
- (add info on Burst compiling and jobs and give details on its impact on performance)

##### Level of Detail (LOD)
- Can be toggled on/off
- Using Unity's Mesh LOD system, 3D models are replaced with alternate models with lower polygon counts based on their distance from the camera
- With this enabled and shadows disabled, the Vertex and Triangle count goes down by roughly 70%, resulting in a massive increase in performance

##### GPU Instancing
- Can be toggled on/off
- Groups the rendering of objects that share the same mesh and material into a single draw call
- This greatly increases performance when rendering large quantities of the same object by reducing the number of draw calls and therefore memory usage

##### Other Optimizations
- Simplified enemy pathing using vector math instead of NavMesh (can be toggled on/off)
- Physics updates on the enemies are called every 10 frames instead of every frame (can be toggled on/off)
- Shadow rendering disabled (can be toggled on/off)
- Centralized enemy logic: instead of calling update functions on every enemy instance individually, the update logic is handled inside of one central script called the EnemyManager

## Unity Profiler Data

##### Profiler and Debug Stats with Optimizations Enabled
(insert profiler screenshot here)
(insert debug stats screenshot here)

##### Profiler and Debug Stats with Optimizations Disabled
(insert profiler screenshot here)
(insert debug stats screenshot here)

## Project Info
- This project was made in the Unity engine and coded entirely in the C# language
- The demo can played in browser on itch.io at this link: https://zubi-dev.itch.io/the-horde

## Credits
- All the code for this project was written by me
- Character models: https://kaylousberg.itch.io/kaykit-adventurers
- Character animations: https://kaylousberg.itch.io/kaykit-character-animations
- UI sound effects: https://sample-kingdom.itch.io/free-cosy-ui-sfx

## Contact
- Discord: @zubi_dev
- Work email: zubairhittam@gmail.com
