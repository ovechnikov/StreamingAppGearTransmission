namespace OpenGLWinForms;

public partial class MainForm : Form
{
    private OpenGlDriver _glDriver = null!;

    public MainForm()
    {
        MouseWheel += MouseWheelEventHandler;
        InitializeComponent();
    }

    private void MouseWheelEventHandler(object? sender, MouseEventArgs e)
    {
        if (e.Delta < 0)
        {
            _glDriver.scale -= OpenGlDriver.ScaleFactorStep;
            if (_glDriver.scale < OpenGlDriver.MINSCALE)
            {
                _glDriver.scale = OpenGlDriver.MINSCALE;
            }
        }
        else
        {
            _glDriver.scale += OpenGlDriver.ScaleFactorStep;
        }
    }

    private void MouseDownEventHandler(object sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
            case MouseButtons.Left:
            case MouseButtons.Middle:
                _glDriver.mouseX = e.Location.X;
                _glDriver.mouseY = e.Location.Y;
                ((MainForm)sender).MouseMove += MouseMovementEventHandler;
                break;
            case MouseButtons.Right:
                break;
        }
    }

    private void MouseUpEventHandler(object sender, MouseEventArgs e)
    {
        switch (e.Button)
        {
            case MouseButtons.Left:
            case MouseButtons.Middle:
                ((MainForm)sender).MouseMove -= MouseMovementEventHandler;
                break;
            case MouseButtons.Right:
                break;
        }
    }

    private void MouseMovementEventHandler(object? sender, MouseEventArgs e)
    {
        int dx = e.Location.X - _glDriver.mouseX;        // change in mouse coords
        int dy = e.Location.Y - _glDriver.mouseY;

        if (e.Button == MouseButtons.Left)
        {
            _glDriver.rotX += (OpenGlDriver.ANGFACT * dy);
            _glDriver.rotY += (OpenGlDriver.ANGFACT * dx);
        }

        if (e.Button == MouseButtons.Middle)
        {
            _glDriver.scale += OpenGlDriver.ScaleFactorStep * (dx - dy);

            // keep object from turning inside-out or disappearing:
            if (_glDriver.scale < OpenGlDriver.MINSCALE)
            {
                _glDriver.scale = OpenGlDriver.MINSCALE;
            }
        }

        // new current position
        _glDriver.mouseX = e.Location.X;
        _glDriver.mouseY = e.Location.Y;
    }

    private void LoadEventHandler(object sender, EventArgs e)
    {
        // Set up OpenGL context
        //IntPtr hdlrWindow = Handle;

        _glDriver = new(this);

        //Console.WriteLine("Window handler:" + hdlrWindow);
        CancellationTokenSource cts = new();
        CancellationToken token = cts.Token;

        Invoke(() => _glDriver.StartRenderingThread(token));
    }
}
