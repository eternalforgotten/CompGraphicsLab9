using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lab9
{
    class LightRasterization
    {
        public static int ProjMode;

        public static List<Point3D> MakeProj(List<Point3D> init) => new Projection().ProjectZBuff(init, ProjMode);

        public static List<List<Point3D>> Makerasterization(Figure figure)
        {
            List<List<Point3D>> listrasterization = new List<List<Point3D>>();
            
            foreach (var surf in figure.Surfaces)
            {
                List<Point3D> currsurface = new List<Point3D>();
                List<Point3D> surfacepoints = new List<Point3D>(); 
                for (int i = 0; i < surf.Count; i++)
                {
                    surfacepoints.Add(figure.Vertexes[surf[i]]);
                }

                List<List<Point3D>> triangles = Buffer.Triangulate(surfacepoints); 
                foreach (List<Point3D> triangle in triangles)
                {
                    currsurface.AddRange(RasterizeTriangle(MakeProj(triangle)));
                }
                listrasterization.Add(currsurface);
            }
            return listrasterization;
        }

       
        private static List<Point3D> RasterizeTriangle(List<Point3D> points)
        {
            List<Point3D> res = new List<Point3D>();

            points.Sort((point1, point2) => point1.Y.CompareTo(point2.Y)); 
            var rpoints = points.Select(point => (X: (int)Math.Round(point.X), Y: (int)Math.Round(point.Y), Z: (int)Math.Round(point.Z), L: point.light)).ToList();

            var xy0Toxy1 = Buffer.Interpolate(rpoints[0].Y, rpoints[0].X, rpoints[1].Y, rpoints[1].X);
            var xy1Toxy2 = Buffer.Interpolate(rpoints[1].Y, rpoints[1].X, rpoints[2].Y, rpoints[2].X);
            var xy0Toxy2 = Buffer.Interpolate(rpoints[0].Y, rpoints[0].X, rpoints[2].Y, rpoints[2].X);

            var yz0Toyz1 = Buffer.Interpolate(rpoints[0].Y, rpoints[0].Z, rpoints[1].Y, rpoints[1].Z);
            var yz1Toyz2 = Buffer.Interpolate(rpoints[1].Y, rpoints[1].Z, rpoints[2].Y, rpoints[2].Z);
            var yz0Toyz2 = Buffer.Interpolate(rpoints[0].Y, rpoints[0].Z, rpoints[2].Y, rpoints[2].Z);

            xy0Toxy1.RemoveAt(xy0Toxy1.Count - 1);
            List<int> xy02 = xy0Toxy1.Concat(xy1Toxy2).ToList();

            yz0Toyz1.RemoveAt(yz0Toyz1.Count - 1);
            List<int> yz02 = yz0Toyz1.Concat(yz1Toyz2).ToList();

            int middle = xy02.Count / 2;
            List<int> leftX, rightX, leftZ, rightZ;
            List<double> leftH = new List<double>(), rigthH = new List<double>();

            if (xy0Toxy2[middle] < xy02[middle])
            {
                leftX = xy0Toxy2;
                leftZ = yz0Toyz2;
                rightX = xy02;
                rightZ = yz02;
            }
            else
            {
                leftX = xy02;
                leftZ = yz02;
                rightX = xy0Toxy2;
                rightZ = yz0Toyz2;
            }

            int y0 = rpoints[0].Y;
            int y2 = rpoints[2].Y;

            
            var lTo01 = InterpolateForLight(rpoints[0].Y, rpoints[0].L, rpoints[1].Y, rpoints[1].L);
            var lTo12 = InterpolateForLight(rpoints[1].Y, rpoints[1].L, rpoints[2].Y, rpoints[2].L);
            var lTo02 = InterpolateForLight(rpoints[0].Y, rpoints[0].L, rpoints[2].Y, rpoints[2].L);

            lTo01.RemoveAt(lTo01.Count - 1);
                List<double> h012 = lTo01.Concat(lTo12).ToList();

                if (xy0Toxy2[middle] < xy02[middle])
                {
                    leftH = lTo02;
                    rigthH = h012;
                }
                else
                {
                    leftH = h012;
                    rigthH = lTo02;
                }
            

            for (int ind = 0; ind <= y2 - y0; ind++) 
            {                                        
                int XL = leftX[ind];
                int XR = rightX[ind];

                List<int> intCurrZ = Buffer.Interpolate(XL, leftZ[ind], XR, rightZ[ind]);
                
                    List<double> doubleCurrL = InterpolateForLight(XL, leftH[ind], XR, rigthH[ind]);
                    for (int x = XL; x < XR; x++)
                    {
                        res.Add(new Point3D(doubleCurrL[x - XL], x, y0 + ind,  intCurrZ[x - XL]));
                    }
                
                
            }

            return res; 
        }


        public static List<double> InterpolateForLight(int y0, double l0, int y1, double l1)
        {
            if (y0 == y1)
            {
                return new List<double> { l0 };
            }
            List<double> res = new List<double>();

            double step = (l1 - l0) / (y1 - y0);
            double value = l0;
            for (int i = y0; i <= y1; i++)
            {
                res.Add(value);
                value += step;
            }

            return res;
        }

        

    }
}

