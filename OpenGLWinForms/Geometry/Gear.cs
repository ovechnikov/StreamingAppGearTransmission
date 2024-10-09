namespace OpenGLWinForms.Geometry;

internal class Gear : IQuadGeometry
{
    public int NumTeeth { get; set; }

    public double Radius { get; set; }

    public double TeethHeight { get; set; }

    public double Thickness { get; set; }

    public int Arms { get; set; }

    public static int Polygons = 20;

    public static double GlobalTolerance = 0.0000001;

    public static double DummyCoefficient = 0.35;

    public List<QuadFace> Faces { get; }

    public Gear()
    {
        Faces = [];
    }

    public void GenerateGeometry()
    {
        if (NumTeeth        == 0 ||
            Radius          <= 0 ||
            TeethHeight     <= 0 ||
            Thickness       <= 0 ||
            Arms            == 0 ||
            Polygons        == 0 ||
            GlobalTolerance <= 0 ||
            GlobalTolerance >= 0.1)
        {
            Console.WriteLine("Gear was not properly initialized");
            return;
        }

        GLPoint[] contactPoints     = new GLPoint[Polygons + 1];
        GLPoint[] smallCirclePoints = new GLPoint[Polygons + 1];
        GLPoint[] bigCirclePoints   = new GLPoint[Polygons + 1];

        // Calculating point on involute using binary search
        double pAlpha = 0;
        double qAlpha = Math.PI / 2;
        double tAlpha;
        double xt;
        double yt;
        double rt;
        do
        {
            tAlpha = (pAlpha + qAlpha) / 2;
            xt = Radius * (Math.Cos(tAlpha) + tAlpha * Math.Sin(tAlpha));
            yt = Radius * (Math.Sin(tAlpha) - tAlpha * Math.Cos(tAlpha));
            rt = Math.Sqrt(xt * xt + yt * yt);
            if (rt > Radius + TeethHeight)
            {
                qAlpha = tAlpha;
            }
            else
            {
                pAlpha = tAlpha;
            }
        } while (Math.Abs(rt - Radius - TeethHeight) > GlobalTolerance);

        double contactAlpha = Math.Atan(yt / xt);
        double toothAngle = 2 * Math.PI / NumTeeth;
        if (2 * contactAlpha > toothAngle)
        {
            Console.WriteLine("Incorrect Gear Parameters!");
            throw new Exception();
        }
        double thetaBig = (toothAngle - 2 * contactAlpha) * DummyCoefficient;
        double thetaSmall = (toothAngle - 2 * contactAlpha) * (1 - DummyCoefficient);

        for (int i = 0; i <= Polygons; i++)
        {
            double c = i * tAlpha / Polygons;
            contactPoints[i].x = Radius * (Math.Cos(c) + c * Math.Sin(c));
            contactPoints[i].y = Radius * (Math.Sin(c) - c * Math.Cos(c));
            contactPoints[i].z = 0;
            contactPoints[i].nx = Math.Sin(c);
            contactPoints[i].ny = -Math.Cos(c);
            contactPoints[i].nz = 0;
            contactPoints[i].s = 0;
        }

        for (int i = 0; i <= Polygons; i++)
        {
            double cSmall = -thetaSmall / 2 + i * thetaSmall / Polygons;
            smallCirclePoints[i].x = Radius * Math.Cos(cSmall);
            smallCirclePoints[i].y = Radius * Math.Sin(cSmall);
            smallCirclePoints[i].z = 0;
            smallCirclePoints[i].nx = Math.Cos(cSmall);
            smallCirclePoints[i].ny = Math.Sin(cSmall);
            smallCirclePoints[i].nz = 0;
            smallCirclePoints[i].s = 0;

            double cBig = thetaSmall / 2 + contactAlpha + i * thetaBig / Polygons;
            bigCirclePoints[i].x = (Radius + TeethHeight) * Math.Cos(cBig);
            bigCirclePoints[i].y = (Radius + TeethHeight) * Math.Sin(cBig);
            bigCirclePoints[i].z = 0;
            bigCirclePoints[i].nx = Math.Cos(cBig);
            bigCirclePoints[i].ny = Math.Sin(cBig);
            bigCirclePoints[i].nz = 0;
            bigCirclePoints[i].s = 0;
        }

        // Let's draw the gear in 3D
        QuadFace surface;
        for (double phi = 0; phi < 2 * Math.PI; phi += 2 * Math.PI / NumTeeth)
        {
            // Contact Surface left
            surface = [];
            for (int i = 1; i <= Polygons; i++)
            {
                surface.Add(contactPoints[i], phi + thetaSmall);
                surface.Add(contactPoints[i - 1], phi + thetaSmall);
                for (int j = 1; j <= Polygons; j++)
                {
                    GLPoint p0 = new(ref contactPoints[i]);
                    p0.z -= j * Thickness / Polygons;
                    p0.s = 0;
                    GLPoint p1 = new(ref contactPoints[i - 1]);
                    p1.z -= j * Thickness / Polygons;
                    p1.s = 0;
                    surface.Add(p0, phi + thetaSmall);
                    surface.Add(p1, phi + thetaSmall);
                }
            }
            Faces.Add(surface);

            // Contact Surface right
            surface = [];
            for (int i = 1; i <= Polygons; i++)
            {
                GLPoint p0 = new(ref contactPoints[i]);
                p0.y = -p0.y;
                GLPoint p1 = new(ref contactPoints[i - 1]);
                p1.y = -p1.y;
                surface.Add(p0, phi);
                surface.Add(p1, phi);
                for (int j = 1; j <= Polygons; j++)
                {
                    p0 = new(ref contactPoints[i]);
                    p0.y = -p0.y;
                    p0.z -= j * Thickness / Polygons;
                    p0.s = 0;
                    p1 = new(ref contactPoints[i - 1]);
                    p1.y = -p1.y;
                    p1.z -= j * Thickness / Polygons;
                    p1.s = 0;
                    surface.Add(p0, phi);
                    surface.Add(p1, phi);
                }
            }
            Faces.Add(surface);

            // Outer and Inner tooth radiuses
            surface = [];
            for (int i = 1; i <= Polygons; i++)
            {
                surface.Add(smallCirclePoints[i], phi + thetaSmall / 2);
                surface.Add(smallCirclePoints[i - 1], phi + thetaSmall / 2);
                for (int j = 1; j <= Polygons; j++)
                {
                    GLPoint p0 = new(ref smallCirclePoints[i]);
                    p0.z -= j * Thickness / Polygons;
                    p0.s = 0;
                    GLPoint p1 = new(ref smallCirclePoints[i - 1]);
                    p1.z -= j * Thickness / Polygons;
                    p1.s = 0;
                    surface.Add(p0, phi + thetaSmall / 2);
                    surface.Add(p1, phi + thetaSmall / 2);
                }
            }
            Faces.Add(surface);
            surface = [];
            for (int i = 1; i <= Polygons; i++)
            {
                surface.Add(bigCirclePoints[i], phi + thetaSmall / 2);
                surface.Add(bigCirclePoints[i - 1], phi + thetaSmall / 2);
                for (int j = 0; j <= Polygons; j++)
                {
                    GLPoint p0 = new(ref bigCirclePoints[i]);
                    p0.z -= j * Thickness / Polygons;
                    p0.s = 0;
                    GLPoint p1 = new(ref bigCirclePoints[i - 1]);
                    p1.z -= j * Thickness / Polygons;
                    p1.s = 0;
                    surface.Add(p0, phi + thetaSmall / 2);
                    surface.Add(p1, phi + thetaSmall / 2);
                }
            }
            Faces.Add(surface);

            // Filling Teeth Surfaces
            for (float zNorm = 1; zNorm >= -1; zNorm -= 2)
            {
                surface = [];
                for (int i = 0; i < Polygons; i++)
                {   // contactPoints loop
                    double tmpR = Math.Sqrt(contactPoints[i].x * contactPoints[i].x + contactPoints[i].y * contactPoints[i].y);
                    double tempR1 = Math.Sqrt(contactPoints[i + 1].x * contactPoints[i + 1].x + contactPoints[i + 1].y * contactPoints[i + 1].y);
                    double phii0R = Math.Atan(contactPoints[i].y / contactPoints[i].x);
                    double phii1R = 2 * contactAlpha + thetaBig - phii0R;
                    double phi0R1 = Math.Atan(contactPoints[i + 1].y / contactPoints[i + 1].x);
                    double phi1R1 = 2 * contactAlpha + thetaBig - phi0R1;

                    for (int j = 0; j <= Polygons; j++)
                    {
                        GLPoint p0 = new()
                        {
                            x = tmpR * Math.Cos(phii0R + j * (phii1R - phii0R) / Polygons),
                            y = tmpR * Math.Sin(phii0R + j * (phii1R - phii0R) / Polygons),
                            z = (zNorm == 1) ? 0 : -Thickness,
                            nx = 0,
                            ny = 0,
                            nz = zNorm,
                            s = 0
                        };

                        GLPoint p1 = new()
                        {
                            x = tempR1 * Math.Cos(phi0R1 + j * (phi1R1 - phi0R1) / Polygons),
                            y = tempR1 * Math.Sin(phi0R1 + j * (phi1R1 - phi0R1) / Polygons),
                            z = (zNorm == 1) ? 0 : -Thickness,
                            nx = 0,
                            ny = 0,
                            nz = zNorm,
                            s = 0
                        };

                        surface.Add(p0, phi + thetaSmall);
                        surface.Add(p1, phi + thetaSmall);
                    }
                }
                Faces.Add(surface);
            }

            // Drawing rim
            for (float zNorm = 1; zNorm >= -1; zNorm -= 2)
            {
                for (int i = 0; i < Polygons; i++)
                {
                    surface = [];
                    double tmpR = Radius - i * 0.2 / Polygons * Radius;
                    double tempRNext = Radius - (i + 1) * 0.2 / Polygons * Radius;
                    double phii0R = 0;
                    double phii1R = 2 * contactAlpha + thetaBig;

                    for (int j = 0; j <= Polygons; j++)
                    {
                        GLPoint p0 = new()
                        {
                            x = tmpR * Math.Cos(phii0R + j * (phii1R - phii0R) / Polygons),
                            y = tmpR * Math.Sin(phii0R + j * (phii1R - phii0R) / Polygons),
                            z = (zNorm == 1) ? 0 : -Thickness,
                            nx = 0,
                            ny = 0,
                            nz = zNorm,
                            s = 0
                        };

                        GLPoint p1 = new()
                        {
                            x = tempRNext * Math.Cos(phii0R + j * (phii1R - phii0R) / Polygons),
                            y = tempRNext * Math.Sin(phii0R + j * (phii1R - phii0R) / Polygons),
                            z = (zNorm == 1) ? 0 : -Thickness,
                            nx = 0,
                            ny = 0,
                            nz = zNorm,
                            s = 0
                        };

                        surface.Add(p0, phi + thetaSmall);
                        surface.Add(p1, phi + thetaSmall);
                    }
                    Faces.Add(surface);
                }

                for (int i = 0; i < Polygons; i++)
                {
                    surface = [];
                    double tmpR = Radius - i * 0.2 / Polygons * Radius;
                    double tmpRNext = Radius - (i + 1) * 0.2 / Polygons * Radius;
                    double phii0R = -thetaSmall;
                    double phii1R = 0;

                    for (int j = 0; j <= Polygons; j++)
                    {
                        GLPoint p0 = new()
                        {
                            x = tmpR * Math.Cos(phii0R + j * (phii1R - phii0R) / Polygons),
                            y = tmpR * Math.Sin(phii0R + j * (phii1R - phii0R) / Polygons),
                            z = (zNorm == 1) ? 0 : -Thickness,
                            nx = 0,
                            ny = 0,
                            nz = zNorm,
                            s = 0
                        };

                        GLPoint p1 = new()
                        {
                            x = tmpRNext * Math.Cos(phii0R + j * (phii1R - phii0R) / Polygons),
                            y = tmpRNext * Math.Sin(phii0R + j * (phii1R - phii0R) / Polygons),
                            z = (zNorm == 1) ? 0 : -Thickness,
                            nx = 0,
                            ny = 0,
                            nz = zNorm,
                            s = 0
                        };

                        surface.Add(p0, phi + thetaSmall);
                        surface.Add(p1, phi + thetaSmall);
                    }
                    Faces.Add(surface);
                }
            }

            // Filling inner radius, 1st part
            double tempR = 0.8 * Radius;
            double phi0R = 0;
            double phi1R = 2 * contactAlpha + thetaBig;

            for (int i = 1; i <= Polygons; i++)
            {
                surface = [];

                GLPoint p0 = new()
                {
                    x = tempR * Math.Cos(phi0R + (i - 1) * (phi1R - phi0R) / Polygons),
                    y = tempR * Math.Sin(phi0R + (i - 1) * (phi1R - phi0R) / Polygons),
                    z = 0,
                    nx = -Math.Cos(phi0R + (i - 1) * (phi1R - phi0R) / Polygons),
                    ny = -Math.Sin(phi0R + (i - 1) * (phi1R - phi0R) / Polygons),
                    nz = 0,
                    s = 0
                };

                GLPoint p1 = new()
                {
                    x = tempR * Math.Cos(phi0R + (i) * (phi1R - phi0R) / Polygons),
                    y = tempR * Math.Sin(phi0R + (i) * (phi1R - phi0R) / Polygons),
                    z = 0,
                    nx = -Math.Cos(phi0R + (i) * (phi1R - phi0R) / Polygons),
                    ny = -Math.Sin(phi0R + (i) * (phi1R - phi0R) / Polygons),
                    nz = 0,
                    s = 0
                };

                surface.Add(p0, phi + thetaSmall);
                surface.Add(p1, phi + thetaSmall);

                for (int j = 1; j <= Polygons; j++)
                {
                    p0.z -= Thickness / Polygons;
                    p0.s = 0;
                    p1.z -= Thickness / Polygons;
                    p1.s = 0;
                    surface.Add(p0, phi + thetaSmall);
                    surface.Add(p1, phi + thetaSmall);
                }
                Faces.Add(surface);
            }

            // Filling inner radius, 2nd part
            phi0R = -thetaSmall;
            phi1R = 0;

            for (int i = 1; i <= Polygons; i++)
            {
                surface = [];
                GLPoint p0 = new()
                {
                    x = tempR * Math.Cos(phi0R + (i - 1) * (phi1R - phi0R) / Polygons),
                    y = tempR * Math.Sin(phi0R + (i - 1) * (phi1R - phi0R) / Polygons),
                    z = 0,
                    nx = -Math.Cos(phi0R + (i - 1) * (phi1R - phi0R) / Polygons),
                    ny = -Math.Sin(phi0R + (i - 1) * (phi1R - phi0R) / Polygons),
                    nz = 0,
                    s = 0
                };

                GLPoint p1 = new()
                {
                    x = tempR * Math.Cos(phi0R + (i) * (phi1R - phi0R) / Polygons),
                    y = tempR * Math.Sin(phi0R + (i) * (phi1R - phi0R) / Polygons),
                    z = 0,
                    nx = -Math.Cos(phi0R + (i) * (phi1R - phi0R) / Polygons),
                    ny = -Math.Sin(phi0R + (i) * (phi1R - phi0R) / Polygons),
                    nz = 0,
                    s = 0
                };

                surface.Add(p0, phi + thetaSmall);
                surface.Add(p1, phi + thetaSmall);

                for (int j = 1; j <= Polygons; j++)
                {
                    p0.z -= Thickness / Polygons;
                    p0.s = 0;
                    p1.z -= Thickness / Polygons;
                    p1.s = 0;
                    surface.Add(p0, phi + thetaSmall);
                    surface.Add(p1, phi + thetaSmall);
                }
                Faces.Add(surface);
            }
        }

        // Drawing hob
        for (double phi = 1; phi <= 360; phi++)
        {
            surface = [];
            // outer circle
            GLPoint p0 = new()
            {
                x = Radius * 0.2 * Math.Cos((phi - 1) * Math.PI / 180),
                y = Radius * 0.2 * Math.Sin((phi - 1) * Math.PI / 180),
                z = 0,
                nx = Math.Cos((phi - 1) * Math.PI / 180),
                ny = Math.Sin((phi - 1) * Math.PI / 180),
                nz = 0,
                s = 0
            };

            GLPoint p1 = new()
            {
                x = Radius * 0.2 * Math.Cos((phi) * Math.PI / 180),
                y = Radius * 0.2 * Math.Sin((phi) * Math.PI / 180),
                z = 0,
                nx = Math.Cos((phi) * Math.PI / 180),
                ny = Math.Sin((phi) * Math.PI / 180),
                nz = 0,
                s = 0
            };

            surface.Add(p0);
            surface.Add(p1);

            for (int j = 1; j <= Polygons; j++)
            {
                p0.z -= Thickness / Polygons;
                p0.s = 0;
                p1.z -= Thickness / Polygons;
                p1.s = 0;
                surface.Add(p0);
                surface.Add(p1);
            }
            Faces.Add(surface);

            // inner circle
            surface = [];
            p0 = new()
            {
                x = Radius * 0.1 * Math.Cos((phi - 1) * Math.PI / 180),
                y = Radius * 0.1 * Math.Sin((phi - 1) * Math.PI / 180),
                z = 0,
                nx = -Math.Cos((phi - 1) * Math.PI / 180),
                ny = -Math.Sin((phi - 1) * Math.PI / 180),
                nz = 0,
                s = 0
            };

            p1 = new()
            {
                x = Radius * 0.1 * Math.Cos((phi) * Math.PI / 180),
                y = Radius * 0.1 * Math.Sin((phi) * Math.PI / 180),
                z = 0,
                nx = -Math.Cos((phi) * Math.PI / 180),
                ny = -Math.Sin((phi) * Math.PI / 180),
                nz = 0,
                s = 0
            };

            surface.Add(p0);
            surface.Add(p1);

            for (int j = 1; j <= Polygons; j++)
            {
                p0.z -= Thickness / Polygons;
                p0.s = 0;
                p1.z -= Thickness / Polygons;
                p1.s = 0;
                surface.Add(p0);
                surface.Add(p1);
            }
            Faces.Add(surface);

            // filling top surface
            surface = [];
            for (int j = 0; j <= Polygons; j++)
            {
                p0 = new()
                {
                    x = Radius * (0.2 - 0.1 * j / Polygons) * Math.Cos((phi - 1) * Math.PI / 180),
                    y = Radius * (0.2 - 0.1 * j / Polygons) * Math.Sin((phi - 1) * Math.PI / 180),
                    z = 0,
                    nx = 0,
                    ny = 0,
                    nz = 1,
                    s = 0
                };

                p1 = new()
                {
                    x = Radius * (0.2 - 0.1 * j / Polygons) * Math.Cos((phi) * Math.PI / 180),
                    y = Radius * (0.2 - 0.1 * j / Polygons) * Math.Sin((phi) * Math.PI / 180),
                    z = 0,
                    nx = 0,
                    ny = 0,
                    nz = 1,
                    s = 0
                };

                surface.Add(p0);
                surface.Add(p1);
            }
            Faces.Add(surface);

            // filling bottom surface
            surface = [];
            for (int j = 0; j <= Polygons; j++)
            {
                p0 = new()
                {
                    x = Radius * (0.2 - 0.1 * j / Polygons) * Math.Cos((phi - 1) * Math.PI / 180),
                    y = Radius * (0.2 - 0.1 * j / Polygons) * Math.Sin((phi - 1) * Math.PI / 180),
                    z = -Thickness,
                    nx = 0,
                    ny = 0,
                    nz = -1,
                    s = 0
                };

                p1 = new()
                {
                    x = Radius * (0.2 - 0.1 * j / Polygons) * Math.Cos((phi) * Math.PI / 180),
                    y = Radius * (0.2 - 0.1 * j / Polygons) * Math.Sin((phi) * Math.PI / 180),
                    z = -Thickness,
                    nx = 0,
                    ny = 0,
                    nz = -1,
                    s = 0
                };

                surface.Add(p0);
                surface.Add(p1);
            }
            Faces.Add(surface);
        }

        // Drawing arms of Wheel
        for (double phi = 0; phi < 2 * Math.PI; phi += 2 * Math.PI / Arms)
        {
            double phi0p0 = Math.Asin(0.05 / 0.2);
            double phi1p0 = -phi0p0;
            double phi0p1 = Math.Asin(0.05 / 0.8);
            double phi1p1 = -phi0p1;

            for (int i = 1; i <= Polygons; i++)
            {
                double phip0 = phi1p0 + i * (phi0p0 - phi1p0) / Polygons;
                double phip1 = phi1p1 + i * (phi0p1 - phi1p1) / Polygons;
                double phip0prev = phi1p0 + (i - 1) * (phi0p0 - phi1p0) / Polygons;
                double phip1prev = phi1p1 + (i - 1) * (phi0p1 - phi1p1) / Polygons;
                double xp0 = 0.2 * Radius * Math.Cos(phip0);
                double yp0 = 0.2 * Radius * Math.Sin(phip0);
                double xp0prev = 0.2 * Radius * Math.Cos(phip0prev);
                double yp0prev = 0.2 * Radius * Math.Sin(phip0prev);
                double xp1 = 0.8 * Radius * Math.Cos(phip1);
                double xp1prev = 0.8 * Radius * Math.Cos(phip1prev);

                // Drawing top surface
                surface = [];
                GLPoint p0 = new()
                {
                    x = xp0prev,
                    y = yp0prev,
                    z = 0,
                    nx = 0,
                    ny = 0,
                    nz = 1,
                    s = 0
                };

                GLPoint p1 = new()
                {
                    x = xp0,
                    y = yp0,
                    z = 0,
                    nx = 0,
                    ny = 0,
                    nz = 1,
                    s = 0
                };

                surface.Add(p0, phi);
                surface.Add(p1, phi);

                for (int j = 1; j <= Polygons; j++)
                {
                    p0.x = xp0prev + j * (xp1prev - xp0prev) / Polygons;
                    p0.y = yp0prev;
                    p0.s = 0;
                    p1.x = xp0 + j * (xp1 - xp0) / Polygons;
                    p1.y = yp0;
                    p1.s = 0;
                    surface.Add(p0, phi);
                    surface.Add(p1, phi);
                }
                Faces.Add(surface);

                // Drawing bot surface
                surface = [];
                p0 = new()
                {
                    x = xp0prev,
                    y = yp0prev,
                    z = -Thickness,
                    nx = 0,
                    ny = 0,
                    nz = -1,
                    s = 0
                };

                p1 = new()
                {
                    x = xp0,
                    y = yp0,
                    z = -Thickness,
                    nx = 0,
                    ny = 0,
                    nz = -1,
                    s = 0
                };

                surface.Add(p0, phi);
                surface.Add(p1, phi);

                for (int j = 1; j <= Polygons; j++)
                {
                    p0.x = xp0prev + j * (xp1prev - xp0prev) / Polygons;
                    p0.y = yp0prev;
                    p0.s = 0;
                    p1.x = xp0 + j * (xp1 - xp0) / Polygons;
                    p1.y = yp0;
                    p1.s = 0;
                    surface.Add(p0, phi);
                    surface.Add(p1, phi);
                }
                Faces.Add(surface);
            }

            for (int i = 1; i <= Polygons; i++)
            {
                double xp0 = 0.2 * Radius * Math.Cos(phi0p0);
                double yp0 = 0.2 * Radius * Math.Sin(phi0p0);
                double xp1 = 0.8 * Radius * Math.Cos(phi0p1);

                // Drawing top surface
                surface = [];

                GLPoint p0 = new()
                {
                    x = xp0,
                    y = yp0,
                    z = -(i - 1) * Thickness / Polygons,
                    nx = 0,
                    ny = 1,
                    nz = 0,
                    s = 0
                };

                GLPoint p1 = new()
                {
                    x = xp0,
                    y = yp0,
                    z = -i * Thickness / Polygons,
                    nx = 0,
                    ny = 1,
                    nz = 0,
                    s = 0
                };

                surface.Add(p0, phi);
                surface.Add(p1, phi);

                for (int j = 1; j <= Polygons; j++)
                {
                    p0.x = xp0 + j * (xp1 - xp0) / Polygons;
                    p0.y = yp0;
                    p0.s = 0;
                    p1.x = xp0 + j * (xp1 - xp0) / Polygons;
                    p1.y = yp0;
                    p1.s = 0;
                    surface.Add(p0, phi);
                    surface.Add(p1, phi);
                }
                Faces.Add(surface);

                // Drawing bot surface
                surface = [];
                p0 = new()
                {
                    x = xp0,
                    y = -yp0,
                    z = -(i - 1) * Thickness / Polygons,
                    nx = 0,
                    ny = -1,
                    nz = 0,
                    s = 0
                };

                p1 = new()
                {
                    x = xp0,
                    y = -yp0,
                    z = -i * Thickness / Polygons,
                    nx = 0,
                    ny = -1,
                    nz = 0,
                    s = 0
                };

                surface.Add(p0, phi);
                surface.Add(p1, phi);

                for (int j = 1; j <= Polygons; j++)
                {
                    p0.x = xp0 + j * (xp1 - xp0) / Polygons;
                    p0.y = -yp0;
                    p0.s = 0;
                    p1.x += (xp1 - xp0) / Polygons;
                    p1.y = -yp0;
                    p1.s = 0;
                    surface.Add(p0, phi);
                    surface.Add(p1, phi);
                }
                Faces.Add(surface);
            }
        }
    }
}
