using System.Collections;

namespace OpenGLWinForms.Geometry;

internal class QuadFace : IEnumerable<GLPoint>
{
    private readonly List<GLPoint> _points;

    public QuadFace()
    {
        _points = new List<GLPoint>(16);
    }

    public QuadFace(int capacity)
    {
        _points = new List<GLPoint>(capacity);
    }

    public QuadFace(IList<GLPoint> points)
    {
        _points = new List<GLPoint>(points.Count * 2);
        _points.AddRange(points);
    }

    public IEnumerator<GLPoint> GetEnumerator()
    {
        return _points.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _points.GetEnumerator();
    }

    public void Add(GLPoint point)
    {
        _points.Add(point);
    }

    public void Add(GLPoint point, double pivotAngle)
    {
        double x1 = point.x * Math.Cos(pivotAngle) - point.y * Math.Sin(pivotAngle);
        double y1 = point.x * Math.Sin(pivotAngle) + point.y * Math.Cos(pivotAngle);
        point.x = x1;
        point.y = y1;
        x1 = point.nx * Math.Cos(pivotAngle) - point.ny * Math.Sin(pivotAngle);
        y1 = point.nx * Math.Sin(pivotAngle) + point.ny * Math.Cos(pivotAngle);
        point.nx = x1;
        point.ny = y1;
        _points.Add(point);
    }
}
