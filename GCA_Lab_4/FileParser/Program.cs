using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MainLogic
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //TODO: add resource file
            var objFileReader = new ObjReader(@"..\..\..\..\cube\source\cube.obj");
            
            var objModel = objFileReader.ReadObjFile();            

            foreach (var item in objModel.GeometricVertices)
            {
                Console.WriteLine($"X - {item.X},\t Y - {item.Y},\t Z - {item.Z},\t W - {item.W}");
            }
            Console.WriteLine("- - - - - - - -");

            foreach (var item in objModel.TextureVertices)
            {
                Console.WriteLine($"U - {item.U},\t V - {item.V},\t W - {item.W}");
            }
            Console.WriteLine("- - - - - - - -");

            foreach (var item in objModel.NormalVertices)
            {
                Console.WriteLine($"I - {item.I},\t J - {item.J},\t K - {item.K}");
            }
            Console.WriteLine("- - - - - - - -");

            // TODO: different cases of print
            foreach (var item in objModel.Poligons)
            {     
                for (int i = 0; i < item.GeometricVerticesNumber.Count; i++)
                {
                    Console.WriteLine($"V - {item.GeometricVerticesNumber[i]},\t Vt - {item.TextureVerticesNumber[i]},\t " +
                                      $"Vn - {item.NormalVerticesNumber[i]}");
                }                
            }

            Console.ReadLine();
        }
    }
}
