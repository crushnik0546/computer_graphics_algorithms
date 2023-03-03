using System;
using System.IO;
using MainLogic.Components;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MainLogic
{
    public class ObjReader
    {
        private readonly string _filePath;

        public ObjReader(string filePath)
        {
            _filePath = filePath;
        }

        public ObjModel ReadObjFile()
        {            
            if (!File.Exists(_filePath))
            {
                throw new ArgumentException("File doesn't exists.");
            }
            string[] lines = File.ReadAllLines(_filePath);
            ObjModel objModel = new ObjModel();
            foreach (string line in lines)
            {
                string[] lineParts = line.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

                if (lineParts.Length > 0)
                {
                    switch (lineParts[0].ToLower())
                    {
                        case "v":
                            var geometricVertex = new GeometricVertex();
                            geometricVertex.GetElementsFromArray(lineParts);
                            objModel.GeometricVertices.Add(geometricVertex);
                            break;
                        case "vt":
                            var textureVertex = new TextureVertex();
                            textureVertex.GetElementsFromArray(lineParts);
                            objModel.TextureVertices.Add(textureVertex);
                            break;
                        case "vn":
                            var normalVertex = new NormalVertex();
                            normalVertex.GetElementsFromArray(lineParts);
                            objModel.NormalVertices.Add(normalVertex);
                            break;
                        case "f":
                            var poligon = new Poligon();
                            poligon.GetElementsFromArray(lineParts);
                            objModel.Poligons.Add(poligon);
                            break;
                    }
                }
            }

            string diffuseFile = "Albedo Map";
            string normalFile = "Normal Map";
            string specularFile = "Specular Map";
            string mraoFile = "MRAO Map";

            objModel.diffuseTexture = LoadTexture(diffuseFile?.ToLower());
            objModel.normalTexture = LoadTexture(normalFile?.ToLower());
            objModel.specularTexture = LoadTexture(specularFile?.ToLower());
            objModel.mraoTexture = LoadTexture(mraoFile?.ToLower());

            objModel.cubeMap = LoadCubeTexture();

            return objModel;
        }
        private static Bitmap LoadTexture(string fileName)
        {
            if (fileName == null)
            {
                return null;
            }

            string[] files = Directory.GetFiles(@"..\..\..\..\ship\source\");
            foreach (string file in files)
            {
                if (file.ToLower().Contains(fileName))
                {
                    return Bitmap.FromFile(file) as Bitmap;
                }
            }

            return null;
        }

        private static List<Bitmap> LoadCubeTexture()
        {
            List<Bitmap> result = new List<Bitmap>();
            string[] files = Directory.GetFiles(@"..\..\..\..\cube_map\better_studio\");
            string px = "", nx = "", py = "", ny = "", pz = "", nz = "";
            foreach (string file in files)
            {
                if (file.ToLower().Contains("px"))
                {
                    px = file;
                } else if (file.ToLower().Contains("nx"))
                {
                    nx = file;
                }
                else if (file.ToLower().Contains("py"))
                {
                    py = file;
                }
                else if (file.ToLower().Contains("ny"))
                {
                    ny = file;
                }
                else if (file.ToLower().Contains("pz"))
                {
                    pz = file;
                }
                else if (file.ToLower().Contains("nz"))
                {
                    nz = file;
                }
            }

            if (px != "" && nx != "" && py != "" && ny != "" && pz != "" && nz != "")
            {
                result.Add(Bitmap.FromFile(px) as Bitmap);
                result.Add(Bitmap.FromFile(nx) as Bitmap);
                result.Add(Bitmap.FromFile(py) as Bitmap);
                result.Add(Bitmap.FromFile(ny) as Bitmap);
                result.Add(Bitmap.FromFile(pz) as Bitmap);
                result.Add(Bitmap.FromFile(nz) as Bitmap);
            }

            return result;
        }
    }
}
