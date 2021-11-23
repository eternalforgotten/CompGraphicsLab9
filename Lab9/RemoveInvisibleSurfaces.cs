using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab9
{
    class RemoveInvisibleSurfaces
    {
        

        public static List<List<int>> RemoveSurfaces(Figure figure, Point3D offset)
        {
            List<List<int>> res = new List<List<int>>();
            Point3D pointOfView = figure.Center() - offset;
            foreach (var surface in figure.Surfaces)
            {
                Point3D normalized = Vectors.Normalize(surface, figure.Vertexes);
                var cos = Vectors.VectorCos(normalized,pointOfView);  
                if (cos > 0)
                    res.Add(surface);
            }
            return res;
        }
        
    }
}
