using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lab9
{
    class Texture
    {
        private static bool IsPointInTriangle(Point3D p1, Point3D p2, Point3D p3, Point3D point)
        {
            float x1 = p1.X;
            float x2 = p2.X;
            float x3 = p3.X;

            float y1 = p1.Y;
            float y2 = p2.Y;
            float y3 = p3.Y;

            float x0 = point.X;
            float y0 = point.Y;

            double a = (x1 - x0) * (y2 - y1) - (x2 - x1) * (y1 - y0);
            double b = (x2 - x0) * (y3 - y2) - (x3 - x2) * (y2 - y0);
            double c = (x3 - x0) * (y1 - y3) - (x1 - x3) * (y3 - y0);
            if (((int)a > 0 && (int)b > 0 && (int)c > 0) || ((int)a < 0 && (int)b < 0 && (int)c < 0))
                return true;
            return false;
        }
        public static Bitmap Texturize(int width, int height, Figure figure, Bitmap texture, int projType, Projection projection, Point3D offset)
        {
            var centerX = width / 2;
            var centerY = height / 2;
            List<List<Point3D>> rasterizedFigure = LightRasterization.Makerasterization(figure);
            var figureLeftX = rasterizedFigure.Where(surface => surface.Count != 0).Min(surface => surface.Min(vertex => vertex.X));
            var figureLeftY = rasterizedFigure.Where(surface => surface.Count != 0).Min(surface => surface.Min(vertex => vertex.Y));
            var figureRightX = rasterizedFigure.Where(surface => surface.Count != 0).Max(surface => surface.Max(vertex => vertex.X));
            var figureRightY = rasterizedFigure.Where(surface => surface.Count != 0).Max(surface => surface.Max(vertex => vertex.Y));
            var figureCenterX = (figureRightX - figureLeftX) / 2;
            var figureCenterY = (figureRightY - figureLeftY) / 2;
            Bitmap bitmap = new Bitmap(width, height);
            List<Point3D> points = projection.ProjectWithPoints(figure, projType);
            foreach (var surface in RemoveInvisibleSurfaces.RemoveSurfaces(figure, offset))
            {
                Point3D p1 = points[surface[0]];
                p1.U = 0;
                p1.V = 0;
                Point3D p2 = points[surface[1]];
                p2.U = 0;
                p2.V = 1;
                Point3D p3 = points[surface[2]];
                p3.U = 1;
                p3.V = 1;
                Point3D p4 = points[surface[2]];
                if (surface.Count == 4)
                {
                    p4 = points[surface[3]];
                    p4.U = 1;
                    p4.V = 0;
                }
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (IsPointInTriangle(p1, p2, p3, new Point3D(x, y)))
                        {
                            double pU;
                            double pV;

                            double L1 = ((p2.Y - p3.Y) * (x - p3.X) + (p3.X - p2.X) * (y - p3.Y)) / ((p2.Y - p3.Y) * (p1.X - p3.X) + (p3.X - p2.X) * (p1.Y - p3.Y));
                            double L2 = ((p3.Y - p1.Y) * (x - p3.X) + (p1.X - p3.X) * (y - p3.Y)) / ((p2.Y - p3.Y) * (p1.X - p3.X) + (p3.X - p2.X) * (p1.Y - p3.Y));
                            double L3 = 1 - L1 - L2;
                            if (L1 >= 0 && L2 >= 0 && L3 >= 0 && L1 <= 1 && L2 <= 1 && L3 <= 1)
                            {
                                pU = L1 * p1.U + L2 * p2.U + L3 * p3.U;
                                pV = L1 * p1.V + L2 * p2.V + L3 * p3.V;
                                Color pixel = texture.GetPixel((int)(pU * (texture.Width - 1)), (int)(pV * (texture.Height - 1)));
                                bitmap.SetPixel((int)(x + centerX - figureCenterX), (int)(y + centerY - figureCenterY), pixel);
                            }
                        }
                        if (surface.Count == 4)
                        {
                            if (IsPointInTriangle(p1, p3, p4, new Point3D(x, y, 0)))
                            {
                                double pU;
                                double pV;

                                double L1 = ((p4.Y - p3.Y) * (x - p3.X) + (p3.X - p4.X) * (y - p3.Y)) / ((p4.Y - p3.Y) * (p1.X - p3.X) + (p3.X - p4.X) * (p1.Y - p3.Y));
                                double L2 = ((p3.Y - p1.Y) * (x - p3.X) + (p1.X - p3.X) * (y - p3.Y)) / ((p4.Y - p3.Y) * (p1.X - p3.X) + (p3.X - p4.X) * (p1.Y - p3.Y));
                                double L3 = 1 - L1 - L2;
                                if (L1 >= 0 && L2 >= 0 && L3 >= 0 && L1 <= 1 && L2 <= 1 && L3 <= 1)
                                {
                                    pU = L1 * p1.U + L2 * p4.U + L3 * p3.U;
                                    pV = L1 * p1.V + L2 * p4.V + L3 * p3.V;
                                    Color pixel = texture.GetPixel((int)(pU * (texture.Width - 1)), (int)(pV * (texture.Height - 1)));
                                    bitmap.SetPixel((int)(x + centerX - figureCenterX), (int)(y + centerY - figureCenterY), pixel);
                                }
                            }
                        }
                    }
                }
            }
            return bitmap;
        }
    }
}
