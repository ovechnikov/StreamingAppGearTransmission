namespace OpenGLWinForms.Geometry;

internal struct GLPoint
{
    public double x, y, z;      // coordinates
    public double nx, ny, nz;   // surface normal
    public double s, t;         // texture coords

    public GLPoint() { }

    public GLPoint(ref GLPoint p)
    {
        x = p.x;
        y = p.y;
        z = p.z;
        nx = p.nx;
        ny = p.ny;
        nz = p.nz;
        s = p.s;
        t = p.t;
    }

    public void Translate(double x, double y, double z)
    {
        this.x += x;
        this.y += y;
        this.z += z;
    }

    public void Pivot(double angle)
    {
        double x1 = x * Math.Cos(angle) - y * Math.Sin(angle);
        double y1 = x * Math.Sin(angle) + y * Math.Cos(angle);
        x = x1;
        y = y1;
        x1 = nx * Math.Cos(angle) - ny * Math.Sin(angle);
        y1 = nx * Math.Sin(angle) + ny * Math.Cos(angle);
        nx = x1;
        ny = y1;
    }
}
