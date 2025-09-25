#Assignment 3

I used OpenTK library

The cube was rendered by first defining 8 unique 3D vertices (each with a position and color). Instead of repeating vertices, an index buffer (EBO) was used to tell OpenGL how to connect those vertices into 12 triangles (2 per face, 6 faces total).
A vertex shader transforms each vertex using three matrices:
Model – rotates the cube around the Y-axis over time.
View – simulates moving the camera back to see the cube.
Projection – applies perspective so the cube looks 3D.
The fragment shader takes interpolated colors from the vertices and fills in each pixel. Finally, depth testing was enabled so faces at the back of the cube don’t overwrite the front faces.
Together, these steps produce a smoothly rotating, colored 3D cube.

<img width="726" height="522" alt="image" src="https://github.com/user-attachments/assets/2fd110db-464e-49d5-b452-87851fc7815a" />
<img width="648" height="482" alt="image" src="https://github.com/user-attachments/assets/fc106251-29e2-44b8-99a6-71232ac8c7b5" />
<img width="623" height="397" alt="image" src="https://github.com/user-attachments/assets/159e0527-547e-4cc5-9564-528ce6c8bda1" />

