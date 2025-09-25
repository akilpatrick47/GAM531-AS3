using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

class Program
{
    static void Main(string[] args)
    {
        var game = new Game();
        game.Run();
    }
}

class Game : GameWindow
{
    // Buffers
    private int _vao;
    private int _vbo;
    private int _ebo;
    private int _shader;

    // Cube vertices (positions + colors)
    private readonly float[] _vertices = {
        // Positions          // Colors
        -0.5f, -0.5f, -0.5f,  1f, 0f, 0f, // 0 back-bottom-left (red)
         0.5f, -0.5f, -0.5f,  0f, 1f, 0f, // 1 back-bottom-right (green)
         0.5f,  0.5f, -0.5f,  0f, 0f, 1f, // 2 back-top-right (blue)
        -0.5f,  0.5f, -0.5f,  1f, 1f, 0f, // 3 back-top-left (yellow)
        -0.5f, -0.5f,  0.5f,  1f, 0f, 1f, // 4 front-bottom-left (magenta)
         0.5f, -0.5f,  0.5f,  0f, 1f, 1f, // 5 front-bottom-right (cyan)
         0.5f,  0.5f,  0.5f,  1f, 1f, 1f, // 6 front-top-right (white)
        -0.5f,  0.5f,  0.5f,  0f, 0f, 0f  // 7 front-top-left (black)
    };

    // Indices (which vertices form triangles)
    private readonly uint[] _indices = {
        0, 1, 2, 0, 2, 3,   // back face
        4, 5, 6, 4, 6, 7,   // front face
        0, 3, 7, 0, 7, 4,   // left face
        1, 5, 6, 1, 6, 2,   // right face
        0, 1, 5, 0, 5, 4,   // bottom face
        3, 2, 6, 3, 6, 7    // top face
    };

    // Shaders
    private const string VertexShaderSource = @"
    #version 330 core
    layout(location = 0) in vec3 aPos;
    layout(location = 1) in vec3 aColor;
    out vec3 ourColor;

    uniform mat4 model;
    uniform mat4 view;
    uniform mat4 projection;

    void main()
    {
        gl_Position = projection * view * model * vec4(aPos, 1.0);
        ourColor = aColor;
    }";

    private const string FragmentShaderSource = @"
    #version 330 core
    in vec3 ourColor;
    out vec4 FragColor;
    void main()
    {
        FragColor = vec4(ourColor, 1.0);
    }";

    public Game() : base(GameWindowSettings.Default, new NativeWindowSettings()
    {
        Size = new Vector2i(800, 600),
        Title = "Rotating 3D Cube"
    })
    { }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Enable(EnableCap.DepthTest); // Enable depth so cube looks 3D

        // Compile shaders
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, VertexShaderSource);
        GL.CompileShader(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, FragmentShaderSource);
        GL.CompileShader(fragmentShader);

        _shader = GL.CreateProgram();
        GL.AttachShader(_shader, vertexShader);
        GL.AttachShader(_shader, fragmentShader);
        GL.LinkProgram(_shader);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        // VAO, VBO, EBO
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

        // Position attribute
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Color attribute
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        GL.UseProgram(_shader);

        // Use elapsed time from the frame event
        float time = (float)DateTime.Now.TimeOfDay.TotalSeconds;

        Matrix4 model = Matrix4.CreateRotationY(time) * Matrix4.CreateRotationX(time * 0.5f);
        Matrix4 view = Matrix4.CreateTranslation(0, 0, -3f);
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45f),
            Size.X / (float)Size.Y,
            0.1f,
            100f
        );

        // Send matrices to GPU
        int modelLoc = GL.GetUniformLocation(_shader, "model");
        int viewLoc = GL.GetUniformLocation(_shader, "view");
        int projLoc = GL.GetUniformLocation(_shader, "projection");

        GL.UniformMatrix4(modelLoc, false, ref model);
        GL.UniformMatrix4(viewLoc, false, ref view);
        GL.UniformMatrix4(projLoc, false, ref projection);

        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

        SwapBuffers();
    }


    protected override void OnUnload()
    {
        base.OnUnload();
        GL.DeleteBuffer(_vbo);
        GL.DeleteBuffer(_ebo);
        GL.DeleteVertexArray(_vao);
        GL.DeleteProgram(_shader);
    }
}
