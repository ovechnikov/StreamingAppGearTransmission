using OpenGLWinForms.Geometry;
using OpenGLWinForms.Shaders;
using OpenGLWinForms.Structs;
using System.Runtime.InteropServices;
using GLbitfield = uint;
using GLdouble = double;
using GLenum = uint;
using GLfloat = float;
using GLint = int;
using GLsizei = int;
using GLubyte = byte;
using GLuint = uint;

namespace OpenGLWinForms;

internal class OpenGlDriver : IDisposable
{
    #region OpenGL Definitions
    private const uint PFD_DRAW_TO_WINDOW = 0x00000004;
    private const uint PFD_SUPPORT_OPENGL = 0x00000020;
    private const uint PFD_DOUBLEBUFFER = 0x00000001;
    private const byte PFD_TYPE_RGBA = 0;
    private const byte PFD_MAIN_PLANE = 0;


    private const GLenum        GL_BACK             = 0x0405;
    private const GLbitfield    GL_COLOR_BUFFER_BIT = 0x00004000;
    private const GLbitfield    GL_DEPTH_BUFFER_BIT = 0x00000100;
    private const GLenum        GL_DEPTH_TEST       = 0x0B71;
    private const GLenum        GL_SMOOTH           = 0x1D01;
    private const GLenum        GL_PROJECTION       = 0x1701;
    private const GLenum        GL_MODELVIEW        = 0x1700;
    private const GLenum        GL_NORMALIZE        = 0x0BA1;
    private const GLenum        GL_COMPILE          = 0x1300;
    private const GLenum        GL_QUADS            = 0x0007;
    private const GLenum        GL_LINE_STRIP       = 0x0003;
    private const GLenum        GL_TRIANGLE_FAN     = 0x0006;
    private const GLenum        GL_POINTS           = 0x0000;
    private const GLenum        GL_RGBA             = 0x1908;
    private const GLenum        GL_LINES            = 0x0001;
    private const GLenum        GL_QUAD_STRIP       = 0x0008;
    private const GLenum        GL_VERSION          = 0x1F02;
    private const GLenum        GL_EXTENSIONS       = 0x1F03;
    private const GLenum        GL_LIGHT0           = 0x4000;
    private const GLenum        GL_UNSIGNED_BYTE    = 0x1401;

    #endregion

    private const string userDll    = "user32.dll";
    private const string gdiDll     = "gdi32.dll";
    private const string openglDll  = "opengl32.dll";
    private const string gluDll     = "glu32.dll";

    #region Windows API Functions
    [DllImport(userDll)]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport(userDll)]
    public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hdc);

    [DllImport(gdiDll)]
    private static extern int ChoosePixelFormat(IntPtr hdc, ref PixelFormatDescriptor ppfd);

    [DllImport(gdiDll)]
    private static extern int SetPixelFormat(IntPtr hdc, int format, ref PixelFormatDescriptor ppfd);

    [DllImport(gdiDll)]
    public static extern int SwapBuffers(IntPtr hdc);
    #endregion

    #region OpenGL Functions
    [DllImport(openglDll)]
    public static extern void glBegin(uint mode);

    [DllImport(openglDll)]
    public static extern void glClear(uint mask);

    [DllImport(openglDll)]
    public static extern void glEnd();

    [DllImport(openglDll)]
    public static extern IntPtr glGetString(uint name);

    [DllImport(openglDll)]
    public static extern IntPtr wglCreateContext(IntPtr hdc);

    [DllImport(openglDll)]
    public static extern int wglDeleteContext(IntPtr hglrc);

    [DllImport(openglDll)]
    public static extern int wglMakeCurrent(IntPtr hdc, IntPtr hglrc);

    [DllImport(openglDll)]
    public static extern void glVertex2f(float x, float y);

    [DllImport(openglDll)]
    public static extern void glVertex3f(GLfloat x, GLfloat y, GLfloat z);

    [DllImport(openglDll)]
    public static extern GLuint glGenLists(GLsizei range);

    [DllImport(openglDll)]
    public static extern void glNewList(GLuint list, GLenum mode);

    [DllImport(openglDll)]
    public static extern void glLineWidth(GLfloat width);

    [DllImport(openglDll)]
    public static extern void glEndList();

    [DllImport(openglDll)]
    public static extern void glPointSize(GLfloat size);

    [DllImport(openglDll)]
    public static extern void glColor3f(GLfloat red, GLfloat green, GLfloat blue);

    [DllImport(openglDll)]
    public static extern void glVertex3d(GLdouble x, GLdouble y, GLdouble z);

    [DllImport(openglDll)]
    public static extern void glPushMatrix();

    [DllImport(openglDll)]
    public static extern void glRotatef(GLfloat angle, GLfloat x, GLfloat y, GLfloat z);

    [DllImport(openglDll)]
    public static extern void glRotated(GLdouble angle, GLdouble x, GLdouble y, GLdouble z);

    [DllImport(openglDll)]
    public static extern void glNormal3d(GLdouble nx, GLdouble ny, GLdouble nz);

    [DllImport(openglDll)]
    public static extern void glTexCoord2d(GLdouble s, GLdouble t);

    [DllImport(openglDll)]
    public static extern void glPopMatrix();

    [DllImport(openglDll)]
    public static extern void glCallList(GLuint list);

    [DllImport(openglDll)]
    private static extern void glScalef(GLfloat x, GLfloat y, GLfloat z);

    [DllImport(openglDll)]
    public static extern void glTranslated(GLdouble x, GLdouble y, GLdouble z);

    [DllImport(openglDll)]
    public static extern void glTranslatef(GLfloat x, GLfloat y, GLfloat z);

    [DllImport(openglDll)]
    public static extern void glFlush();

    [DllImport(openglDll)]
    public static extern void glEnable(GLenum cap);

    [DllImport(openglDll)]
    public static extern void glDisable(GLenum cap);

    [DllImport(openglDll)]
    public static extern void glDrawBuffer(GLenum mode);

    [DllImport(openglDll)]
    public static extern void glMatrixMode(GLenum mode);

    [DllImport(openglDll)]
    public static extern void glLoadIdentity();

    [DllImport(openglDll)]
    public static extern void glViewport(GLint x, GLint y, GLsizei width, GLsizei height);

    [DllImport(openglDll)]
    public static extern void glShadeModel(GLenum mode);

    [DllImport(openglDll)]
    public static extern void glMultMatrixf(ref GLfloat[] m);

    [DllImport(openglDll)]
    public static extern IntPtr wglGetProcAddress(string functionName);

    [DllImport(openglDll)]
    public static extern GLenum glGetError();

    [DllImport(openglDll)]
    public static extern GLuint glCreateProgram();

    [DllImport(openglDll)]
    public static extern void glReadPixels(GLint x, GLint y, GLsizei width, GLsizei height, GLenum format, GLenum type, [Out] IntPtr pixels);

    #endregion

    #region GLU functions
    [DllImport(gluDll)]
    public static extern void gluPerspective(GLdouble fovy, GLdouble aspect, GLdouble zNear, GLdouble zFar);

    [DllImport(gluDll)]
    public static extern void gluLookAt(GLdouble eyex, GLdouble eyey, GLdouble eyez, GLdouble centerx, GLdouble centery, GLdouble centerz, GLdouble upx, GLdouble upy, GLdouble upz);

    [DllImport(gluDll)]
    public static extern void gluOrtho2D(GLdouble left, GLdouble right, GLdouble bottom, GLdouble top);
    #endregion

    #region Required delegates
    // Reference: https://registry.khronos.org/OpenGL-Refpages/gl4/

    private delegate void   GlAttachShaderDelegate(GLuint program, GLuint shader);
    private delegate void   GlCompileShaderDelegate(GLuint shader);
    private delegate GLuint GlCreateProgramDelegate();
    private delegate GLuint GlCreateShaderDelegate(GLenum shaderType);
    private delegate void   GlLinkProgramDelegate(GLuint program);
    private delegate void   GlUseProgramDelegate(GLuint program);
    private delegate void   GlUniform1fDelegate(GLint location, GLfloat v0);
    private delegate void   GlUniform3fDelegate(GLint location, GLfloat v0, GLfloat v1, GLfloat v2);
    private delegate void   GlUniform1iDelegate(GLint location, GLint v0);

    // void glGetShaderInfoLog(GLuint shader, GLsizei maxLength, GLsizei *length, GLchar* infoLog);
    private delegate void   GlGetShaderInfoLogDelegate(GLuint shader, GLsizei maxLength, out GLsizei length, IntPtr infoLog);

    // void glGetShaderiv(GLuint shader, GLenum pname, GLint *params);
    private delegate void   GlGetShaderivDelegate(GLuint shader, GLenum pname, out GLint param);

    // GLint glGetUniformLocation(GLuint program, const GLchar* name)
    private delegate GLint  GlGetUniformLocationDelegate(GLuint program, IntPtr name);

    // void glShaderSource(GLuint shader, GLsizei count, const GLchar**string, const GLint* length);
    private delegate void   GlShaderSourceDelegate(GLuint shader, GLsizei count, IntPtr stringSource, IntPtr length);

    private GlGetUniformLocationDelegate _glGetUniformLocation;
    private GLuint program;
    #endregion

    #region Legacy variables - Get rid of it!
    private const GLfloat AXES_WIDTH = 3.0f;    // line width for the axes

    private const int Ms_Slowdown = 100000000;

    private const float LENFRAC = 0.10f;        // fraction of the length to use as height of the characters
    private const float BASEFRAC = 1.10f;       // fraction of length to use as start location of the characters


    private static GLuint AxesList;		    // list to hold the axes
    private static GLuint DebugLinesList;   // Debugging Lines display list

    // Gear transmission input parameters
    private const int GEAR_NUMTEETH1 = 23;
    private const int GEAR_NUMTEETH2 = 47;
    private const double GEAR_RADIUS1 = 10;
    private const double GEAR_TEETH_HGT = 2;
    private const double GEAR_THICKNESS = 2;
    private const int GEAR_ARMS1 = 3;
    private const int GEAR_ARMS2 = 5;

    private const double GEAR_DUMMY_COEFFICIENT = 0.35;
    private const double GEAR_GLOBAL_TOLERANCE = 0.0000001;
    private const int GEAR_POLYGONS = 20;

    #endregion

    private readonly Form _parentForm;
    private readonly IntPtr _windowHandle;
    private IntPtr _hdlrDeviceContext;
    private IntPtr _hdlrRenderContext;

    private GLuint _dlGear1;    // Gear 1 display list
    private GLuint _dlGear2;    // Gear 2 display list

    public const float MINSCALE = 0.001f;   // minimum allowable scale factor
    public const float ANGFACT = 1.0f;
    public const float ScaleFactorStep = 0.001f;

    public int mouseX, mouseY;     // mouse values
    public float rotX, rotY;       // rotation angles in degrees
    public float scale = 0.02f;    // scaling factor

    public OpenGlDriver(Form parentForm)
    {
        _parentForm = parentForm;
        _windowHandle = _parentForm.Handle;
    }

    public Task StartRenderingThread(CancellationToken token)
    => Task.Run(() =>
    {
        // Get Device Context
        _hdlrDeviceContext = GetDC(_windowHandle);

        // Set Pixel Format
        PixelFormatDescriptor pfd = new()
        {
            nSize = 40,
            nVersion = 1,
            dwFlags = PFD_DRAW_TO_WINDOW | PFD_SUPPORT_OPENGL | PFD_DOUBLEBUFFER,
            iPixelType = PFD_TYPE_RGBA,
            cColorBits = 32,
            cDepthBits = 24,
            iLayerType = PFD_MAIN_PLANE,
        };

        int pixelFormat = ChoosePixelFormat(_hdlrDeviceContext, ref pfd);
        _ = SetPixelFormat(_hdlrDeviceContext, pixelFormat, ref pfd);
        Console.WriteLine("Pixel Format: " + pixelFormat);

        // Create OpenGL Rendering Context
        _hdlrRenderContext = wglCreateContext(_hdlrDeviceContext);
        _ = wglMakeCurrent(_hdlrDeviceContext, _hdlrRenderContext);

        IntPtr ver = glGetString(GL_VERSION);
        Console.WriteLine("Detected OpenGL: " + Marshal.PtrToStringAnsi(ver));


        #region Shaders Stuff
        // Shaders Stuff
        IntPtr extensionsPtr = glGetString(GL_EXTENSIONS);
        String extensions = Marshal.PtrToStringAnsi(extensionsPtr)!;
        //foreach (string s in extensions.Split(' '))
        //{
        //    Console.WriteLine(s);
        //}
        if (extensions.Contains("GL_ARB_vertex_shader"))
        {
            Console.WriteLine("Supports Vertex Shader");
        }
        if (extensions.Contains("GL_ARB_fragment_shader"))
        {
            Console.WriteLine("Supports Fragment Shader");
        }
        if (extensions.Contains("GL_ARB_geometry_shader4"))
        {
            Console.WriteLine("Supports Geometry Shader");
        }

        VertexShader vertexShader = new();
        FragmentShader fragmentShader = new();

        // Obtaining required function pointers
        IntPtr ptrGlCreateShader = wglGetProcAddress("glCreateShader");
        IntPtr ptrGlShaderSource = wglGetProcAddress("glShaderSource");
        IntPtr ptrGlCompileShader = wglGetProcAddress("glCompileShader");
        IntPtr ptrGlGetShaderiv = wglGetProcAddress("glGetShaderiv");
        IntPtr ptrGlCreateProgram = wglGetProcAddress("glCreateProgram");
        IntPtr ptrGlAttachShader = wglGetProcAddress("glAttachShader");
        IntPtr ptrGlLinkProgram = wglGetProcAddress("glLinkProgram");
        IntPtr ptrGlUseProgram = wglGetProcAddress("glUseProgram");
        IntPtr ptrGlGetUniformLocation = wglGetProcAddress("glGetUniformLocation");
        IntPtr ptrGlUniform1f = wglGetProcAddress("glUniform1f");
        IntPtr ptrGlUniform3f = wglGetProcAddress("glUniform3f");
        IntPtr ptrGlUniform3i = wglGetProcAddress("glUniform1i");

        // Assigning delegates
        GlCreateShaderDelegate          glCreateShader          = Marshal.GetDelegateForFunctionPointer<GlCreateShaderDelegate>(ptrGlCreateShader);
        GlShaderSourceDelegate          glShaderSource          = Marshal.GetDelegateForFunctionPointer<GlShaderSourceDelegate>(ptrGlShaderSource);
        GlCompileShaderDelegate         glCompileShader         = Marshal.GetDelegateForFunctionPointer<GlCompileShaderDelegate>(ptrGlCompileShader);
        GlGetShaderivDelegate           glGetShaderiv           = Marshal.GetDelegateForFunctionPointer<GlGetShaderivDelegate>(ptrGlGetShaderiv);
        GlCreateProgramDelegate         glCreateProgram         = Marshal.GetDelegateForFunctionPointer<GlCreateProgramDelegate>(ptrGlCreateProgram);
        GlAttachShaderDelegate          glAttachShader          = Marshal.GetDelegateForFunctionPointer<GlAttachShaderDelegate>(ptrGlAttachShader);
        GlLinkProgramDelegate           glLinkProgram           = Marshal.GetDelegateForFunctionPointer<GlLinkProgramDelegate>(ptrGlLinkProgram);
        //GlGetUniformLocationDelegate    glGetUniformLocation    = Marshal.GetDelegateForFunctionPointer<GlGetUniformLocationDelegate>(ptrGlGetUniformLocation);
        GlUseProgramDelegate            glUseProgram            = Marshal.GetDelegateForFunctionPointer<GlUseProgramDelegate>(ptrGlUseProgram);
        GlUniform1fDelegate             glUniform1f             = Marshal.GetDelegateForFunctionPointer<GlUniform1fDelegate>(ptrGlUniform1f);
        GlUniform3fDelegate             glUniform3f             = Marshal.GetDelegateForFunctionPointer<GlUniform3fDelegate>(ptrGlUniform3f);
        GlUniform1iDelegate             glUniform1i             = Marshal.GetDelegateForFunctionPointer<GlUniform1iDelegate>(ptrGlUniform3i);

        _glGetUniformLocation = Marshal.GetDelegateForFunctionPointer<GlGetUniformLocationDelegate>(ptrGlGetUniformLocation);

        // Create Vertex Shader
        if (ptrGlCreateShader == IntPtr.Zero)
        {
            Console.WriteLine("ERROR: Failed to load glCreateShader!");
        }
        else
        {
            Console.WriteLine("glCreateShader is loaded successfully");
        }

        GLuint vert = glCreateShader(0x8B31);
        GLenum glErr = glGetError();
        if (glErr != 0)
        {
            Console.WriteLine("Error code: " + glErr);
        }

        // Vertex Shader Source
        if (ptrGlShaderSource == IntPtr.Zero)
        {
            Console.WriteLine("ERROR: Failed to load glCreateShader!");
        }
        else
        {
            Console.WriteLine("glShaderSource is loaded successfully");
        }

        byte[] vertSrcBytes = System.Text.Encoding.ASCII.GetBytes(vertexShader.SourceCode);
        GCHandle gCHandle = GCHandle.Alloc(vertSrcBytes, GCHandleType.Pinned);
        IntPtr vertSrcPtr = gCHandle.AddrOfPinnedObject();

        int vertSrcLength = vertexShader.SourceCode.Length;

        IntPtr[] stringPointerArray = [vertSrcPtr];
        GCHandle pointerHandle = GCHandle.Alloc(stringPointerArray, GCHandleType.Pinned);
        glShaderSource(vert, 1, pointerHandle.AddrOfPinnedObject(), IntPtr.Zero);

        gCHandle.Free();
        pointerHandle.Free();

        glErr = glGetError();
        if (glErr != 0)
        {
            Console.WriteLine("Error code: " + glErr);
        }

        // Compile Vertex Shader

        glCompileShader(vert);
        glErr = glGetError();
        if (glErr != 0)
        {
            Console.WriteLine("Error code: " + glErr);
        }

        glGetShaderiv(vert, 0x8B81, out int cerr);
        Console.WriteLine("Vertex shader compilation output: " + cerr);

        // Create Fragment Shader
        GLuint frag = glCreateShader(0x8B30);
        glErr = glGetError();
        if (glErr != 0)
        {
            Console.WriteLine("Error code: " + glErr);
        }

        // Fragment Shader Source
        byte[] fragSrcBytes = System.Text.Encoding.ASCII.GetBytes(fragmentShader.SourceCode);
        GCHandle fraggCHandle = GCHandle.Alloc(fragSrcBytes, GCHandleType.Pinned);
        IntPtr fragSrcPtr = fraggCHandle.AddrOfPinnedObject();

        int fragSrcLength = fragmentShader.SourceCode.Length;

        IntPtr[] fragStringPointerArray = [fragSrcPtr];
        GCHandle fragPointerHandle = GCHandle.Alloc(fragStringPointerArray, GCHandleType.Pinned);
        glShaderSource(frag, 1, fragPointerHandle.AddrOfPinnedObject(), IntPtr.Zero);

        fraggCHandle.Free();
        fragPointerHandle.Free();

        glErr = glGetError();
        if (glErr != 0)
        {
            Console.WriteLine("Error code: " + glErr);
        }

        // Compile Fragment Shader
        glCompileShader(frag);
        glGetShaderiv(frag, 0x8B81, out cerr);
        Console.WriteLine("Fragment shader compilation output: " + cerr);

        // Create the shader program, attach the vertex and fragment shaders and link the program.

        program = glCreateProgram();
        Console.WriteLine("Program reference value: " + program);

        glAttachShader(program, vert);
        glAttachShader(program, frag);
        glLinkProgram(program);

        // Get the locations of uniform variables
        GLint scaleLoc  = GetUniformLocation("uScale");
        GLint colorLoc  = GetUniformLocation("uColor");
        GLint posLoc    = GetUniformLocation("uXpos");
        GLint angleLoc  = GetUniformLocation("uAngle");
        GLint rotXLoc   = GetUniformLocation("uRotX");
        GLint rotYLoc   = GetUniformLocation("uRotY");

        #endregion

        #region Geometry
        Gear gear1 = new()
        {
            NumTeeth = GEAR_NUMTEETH1,
            Radius = GEAR_RADIUS1,
            TeethHeight = GEAR_TEETH_HGT,
            Thickness = 2,
            Arms = 3
        };

        Gear gear2 = new()
        {
            NumTeeth = GEAR_NUMTEETH2,
            Radius = GEAR_RADIUS1 * GEAR_NUMTEETH2 / GEAR_NUMTEETH1,
            TeethHeight = GEAR_TEETH_HGT,
            Thickness = 2,
            Arms = 5
        };

        gear1.GenerateGeometry();
        gear2.GenerateGeometry();

        _dlGear1 = LoadQuadGeometry(gear1);
        _dlGear2 = LoadQuadGeometry(gear2);

        #endregion

        GLsizei v0 = 0;
        byte[] pixelData = [];
        GCHandle pixelDataHandle = GCHandle.Alloc(pixelData, GCHandleType.Pinned);
        IntPtr pixelDataPtr = pixelDataHandle.AddrOfPinnedObject();

        Task streaming = Task.Run(() => StreamingDriver.StartWebSocketServer(token));

        // Rendering Loop
        while (!token.IsCancellationRequested)
        {
            // OpenGL rendering code goes here
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT); // Clear background
            glEnable(GL_DEPTH_TEST);    // Enable Z-buffer

            // set the viewport to a square centered in the window:
            GLsizei v = Math.Min(_parentForm.ClientSize.Width, _parentForm.ClientSize.Height);
            if (v != v0)
            {
                pixelDataHandle.Free();
                pixelData = new byte[v * v * 4];
                v0 = v;
                pixelDataHandle = GCHandle.Alloc(pixelData, GCHandleType.Pinned);
                pixelDataPtr = pixelDataHandle.AddrOfPinnedObject();
            }
            GLint xl = (_parentForm.ClientSize.Width - v) / 2;
            GLint yb = (_parentForm.ClientSize.Height - v) / 2;
            glViewport(xl, yb, v, v);

            // This call ensures that transformations don't stack up each render cycle
            // set the viewing volume:
            // remember that the Z clipping  values are actually
            // given as DISTANCES IN FRONT OF THE EYE
            glMatrixMode(GL_PROJECTION);
            //glLoadIdentity();
            //gluPerspective(70.0d, 1.00d, 0.1d, 1000.0d);

            // place the objects into the scene:
            //glMatrixMode(GL_MODELVIEW);
            //glLoadIdentity();

            // set the eye position, look-at position, and up-vector:
            //gluLookAt(0.0d, 0.0d, 30.0d, 0.0d, 0.0d, 0.0d, 0.0d, 1.0d, 0.0d);

            // Use shader pipeline
            glUseProgram(program);

            // Scale the scene
            glUniform1f(scaleLoc, scale);

            // Rotate the scene
            glUniform1f(rotXLoc, (float)(rotX * Math.PI / 180));
            glUniform1f(rotYLoc, (float)(rotY * Math.PI / 180));

            // Drawing Gear 1
            glUniform3f(colorLoc, 0.039f, 0.492f, 0.547f);
            glUniform1f(posLoc, -(float)GEAR_RADIUS1);
            glUniform1f(angleLoc, (float)(2 * Math.PI * (DateTime.Now.Ticks & 0b01111111111111111111111111111111) / Ms_Slowdown));

            glCallList(_dlGear1);

            // Drawing Gear 2
            glUniform3f(colorLoc, 0.715f, 0.254f, 0.055f);
            glUniform1f(posLoc, (float)(GEAR_TEETH_HGT + GEAR_RADIUS1 * GEAR_NUMTEETH2 / GEAR_NUMTEETH1));
            glUniform1f(angleLoc, -(float)(2 * Math.PI * (DateTime.Now.Ticks & 0b01111111111111111111111111111111) / Ms_Slowdown * GEAR_NUMTEETH1 / GEAR_NUMTEETH2));

            glCallList(_dlGear2);

            // Read the pixel data from the framebuffer
            glReadPixels(0, 0, v, v, GL_RGBA, GL_UNSIGNED_BYTE, pixelDataPtr);
            StreamingDriver.SendPixelData(pixelData);

            // Swap buffers to display the rendered image
            _ = SwapBuffers(_hdlrDeviceContext);
        }
        pixelDataHandle.Free();
    },
    token);

    private GLint GetUniformLocation(string variable)
    {
        byte[] varBytes = System.Text.Encoding.ASCII.GetBytes(variable);
        GCHandle varGCHandle = GCHandle.Alloc(varBytes, GCHandleType.Pinned);
        IntPtr varPtr = varGCHandle.AddrOfPinnedObject();
        GLint varLoc = _glGetUniformLocation(program, varPtr);
        varGCHandle.Free();
        return varLoc;
    }

    public static GLuint LoadQuadGeometry(IQuadGeometry model)
    {
        GLuint dList = glGenLists(1);
        glNewList(dList, GL_COMPILE);
        foreach (QuadFace face in model.Faces)
        {
            glBegin(GL_QUAD_STRIP);
            foreach(GLPoint p in face)
            {
                glNormal3d(p.nx, p.ny, p.nz);
                glTexCoord2d(p.s, p.t);
                glVertex3d(p.x, p.y, p.z);
            }
            glEnd();
        }
        glEndList();
        return dList;
    }

     public void Dispose()
    {
        // Clean up OpenGL context when done
        _ = wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
        _ = wglDeleteContext(_hdlrRenderContext);
        _ = ReleaseDC(_windowHandle, _hdlrDeviceContext);
    }
}
