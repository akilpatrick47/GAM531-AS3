#Assignment 3

I used OpenTK library

The cube was rendered by first defining 8 unique 3D vertices (each with a position and color). Instead of repeating vertices, an index buffer (EBO) was used to tell OpenGL how to connect those vertices into 12 triangles (2 per face, 6 faces total).
A vertex shader transforms each vertex using three matrices:
Model – rotates the cube around the Y-axis over time.
View – simulates moving the camera back to see the cube.
Projection – applies perspective so the cube looks 3D.
The fragment shader takes interpolated colors from the vertices and fills in each pixel. Finally, depth testing was enabled so faces at the back of the cube don’t overwrite the front faces.
Together, these steps produce a smoothly rotating, colored 3D cube.
