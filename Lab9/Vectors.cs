using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab9
{
    class Vectors
    {
        public static Point3D VectorMultiplication(Point3D vector1, Point3D vector2)
        {
            float x = vector1.Y * vector2.Z - vector1.Z * vector2.Y;
            float y = vector1.Z * vector2.X - vector1.X * vector2.Z;
            float z = vector1.X * vector2.Y - vector1.Y * vector2.X;
            return new Point3D(x, y, z);
        }

        public static Point3D Normalize(List<int> surface, List<Point3D> vertexes)
        {
            Point3D firstVec = vertexes[surface[1]] - vertexes[surface[0]];
            Point3D secondVec = vertexes[surface[2]] - vertexes[surface[1]];
            if (secondVec.X == 0 && secondVec.Y == 0 && secondVec.Z == 0)
            {
                secondVec = vertexes[surface[3]] - vertexes[surface[2]];
            }
            return VectorMultiplication(firstVec, secondVec);
        }

        
        public static double Length(Point3D vec)
        {
            return Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
        }

        

        public static Point3D CalculateNormalForSurface(List<int> surface, Figure figure)
        {
            Point3D p0 = figure.Vertexes[surface[0]];
            Point3D p1 = figure.Vertexes[surface[1]];
            Point3D p2 = figure.Vertexes[surface[surface.Count - 1]];
            Point3D v1 = new Point3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Point3D v2 = new Point3D(p2.X - p0.X, p2.Y - p0.Y, p2.Z - p0.Z);
            return VectorMultiplication(v1, v2);
        }

        public static Point3D NormalCalculation(List<List<int>> surfaces, Figure figure)
        {
            Point3D res = new Point3D(0, 0, 0);
            foreach (var surface in surfaces)
            {
                res.X += CalculateNormalForSurface(surface, figure).X;
                res.Y += CalculateNormalForSurface(surface, figure).Y;
                res.Z += CalculateNormalForSurface(surface, figure).Z;
            }
            res.X /= surfaces.Count;
            res.Y /= surfaces.Count;
            res.Z /= surfaces.Count;
            return res;
        }

        public static double VectorCos(Point3D vec1, Point3D vec2)
        {
            var scalar = vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z * vec2.Z;
            var prodLength = Length(vec1) * Length(vec2);
            return scalar / prodLength;
        }



    }
}
