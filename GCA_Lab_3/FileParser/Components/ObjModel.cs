using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainLogic.Components;

namespace MainLogic
{
    public class ObjModel
    {
        public List<GeometricVertex> GeometricVertices;
        public List<TextureVertex> TextureVertices;
        public List<NormalVertex> NormalVertices;
        public List<Poligon> Poligons;

        public ObjModel()
        {
            GeometricVertices = new List<GeometricVertex>();
            TextureVertices = new List<TextureVertex>();
            NormalVertices = new List<NormalVertex>();
            Poligons = new List<Poligon>();
        }
    }
}
