using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lab9
{
    class Lighting
    {
       
        public static Bitmap Gouraud(int width, int height, Figure figure, Color color, Point3D light, int projMode)
        {
            

            Dictionary<int, Point3D> normal = new Dictionary<int, Point3D>();
            for (int i = 0; i < figure.Vertexes.Count; i++)
            {
                List<List<int>> surfaces = figure.Surfaces.Where(x => x.Contains(i)).ToList();
                normal.Add(i, Vectors.NormalCalculation(surfaces, figure));
            }

            for (int i = 0; i < figure.Vertexes.Count; i++)
            {
                figure.Vertexes[i].light = ModelLambert(figure.Vertexes[i], normal[i], light);
            }

            LightRasterization.ProjMode = projMode;

            Bitmap newImg = new Bitmap(width, height);
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    newImg.SetPixel(i, j, Color.White);
                }
                    

            float[,] zbuff = new float[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    zbuff[i, j] = float.MinValue;
                }
                    

            
            List<List<Point3D>> rasterizedFigure = LightRasterization.Makerasterization(figure);

            var centerX = width / 2;
            var centerY = height / 2;

            
            var figureLeftX = rasterizedFigure.Where(surface => surface.Count != 0).Min(surface => surface.Min(vertex => vertex.X));
            var figureLeftY = rasterizedFigure.Where(surface => surface.Count != 0).Min(surface => surface.Min(vertex => vertex.Y));
            var figureRightX = rasterizedFigure.Where(surface => surface.Count != 0).Max(surface => surface.Max(vertex => vertex.X));
            var figureRightY = rasterizedFigure.Where(surface => surface.Count != 0).Max(surface => surface.Max(vertex => vertex.Y));
            var figureCenterX = (figureRightX - figureLeftX) / 2;
            var figureCenterY = (figureRightY - figureLeftY) / 2;

            for (int i = 0; i < rasterizedFigure.Count; i++)
            {
                List<Point3D> curr = rasterizedFigure[i]; 
                foreach (Point3D point in curr)
                {
                    int x = (int)(point.X + centerX - figureCenterX);
                    int y = (int)(point.Y + centerY - figureCenterY);
                    if (x < width && y < height && x > 0 && y > 0)
                    {
                        if (point.Z > zbuff[x, y])
                        {
                            zbuff[x, y] = point.Z;
                            newImg.SetPixel(x, y, Color.FromArgb((int)(color.R * point.light), (int)(color.G * point.light), (int)(color.B * point.light)));
                        }
                    }
                }
            }
            return newImg;
        }




        private static double ModelLambert(Point3D vertex, Point3D normal, Point3D plight)
        {
            Point3D lightplace = new Point3D(vertex.X - plight.X, vertex.Y - plight.Y, vertex.Z - plight.Z);
            double cos = Vectors.VectorCos(lightplace, normal);
            return (cos + 1) / 2;
        }

    }
}
