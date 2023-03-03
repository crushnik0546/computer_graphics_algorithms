using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;

namespace MainLogic.Components
{
    public class Poligon
    {
        private const int _minElementsLength = 4;

        public List<int> TextureVerticesNumber;
        public List<int> GeometricVerticesNumber;
        public List<int> NormalVerticesNumber;

        public Poligon() {
            TextureVerticesNumber = new List<int>();
            GeometricVerticesNumber = new List<int>();
            NormalVerticesNumber = new List<int>();
        }

        public void GetElementsFromArray(string[] line)
        {
            if (line.Length < _minElementsLength)
            {
                throw new ArgumentException("Few elements in line.");
            }

            int numOfSingleSlash = new Regex("/").Matches(line[1]).Count;
            switch(numOfSingleSlash)
            {
                case 0:
                    for (int i = 1; i < line.Length; i++)
                    {
                        if (!int.TryParse(line[i], NumberStyles.Any, CultureInfo.InvariantCulture, out int v))
                        {
                            throw new ArgumentException("Couldn't convert V number to int.");
                        }
                        GeometricVerticesNumber.Add(v);
                    }                    
                    break;

                case 1:
                    for (int i = 1; i < line.Length; i++)
                    {
                        var item = line[i].Split('/');                        
                        if (item.Length < 2)
                        {
                            throw new ArgumentException("Few elements in line.");
                        }

                        if (!int.TryParse(item[0], NumberStyles.Any, CultureInfo.InvariantCulture, out int v))
                        {
                            throw new ArgumentException("Couldn't convert V number to int.");
                        }
                        GeometricVerticesNumber.Add(v);

                        if (!int.TryParse(item[1], NumberStyles.Any, CultureInfo.InvariantCulture, out int vt))
                        {
                            throw new ArgumentException("Couldn't convert Vt number to int.");
                        }
                        TextureVerticesNumber.Add(vt);
                    }
                    break;

                case 2:
                    int numOfDoubleSlash = new Regex("//").Matches(line[1]).Count;
                    if (numOfDoubleSlash == 0)
                    {
                        for (int i = 1; i < line.Length; i++)
                        {
                            var item = line[i].Split('/');
                            if (item.Length < 3)
                            {
                                throw new ArgumentException("Few elements in line.");
                            }

                            if (!int.TryParse(item[0], NumberStyles.Any, CultureInfo.InvariantCulture, out int v))
                            {
                                throw new ArgumentException("Couldn't convert V number to int.");
                            }
                            GeometricVerticesNumber.Add(v);

                            if (!int.TryParse(item[1], NumberStyles.Any, CultureInfo.InvariantCulture, out int vt))
                            {
                                throw new ArgumentException("Couldn't convert Vt number to int.");
                            }
                            TextureVerticesNumber.Add(vt);

                            if (!int.TryParse(item[2], NumberStyles.Any, CultureInfo.InvariantCulture, out int vn))
                            {
                                throw new ArgumentException("Couldn't convert Vn number to int.");
                            }
                            NormalVerticesNumber.Add(vn);
                        }
                    }
                    else
                    {
                        for (int i = 1; i < line.Length; i++)
                        {
                            var item = line[i].Split(new string[] { "//" }, StringSplitOptions.None);
                            if (item.Length < 2)
                            {
                                throw new ArgumentException("Few elements in line.");
                            }

                            if (!int.TryParse(item[0], NumberStyles.Any, CultureInfo.InvariantCulture, out int v))
                            {
                                throw new ArgumentException("Couldn't convert V number to int.");
                            }
                            GeometricVerticesNumber.Add(v);

                            if (!int.TryParse(item[1], NumberStyles.Any, CultureInfo.InvariantCulture, out int vt))
                            {
                                throw new ArgumentException("Couldn't convert Vt number to int.");
                            }
                            TextureVerticesNumber.Add(vt);

                            if (!int.TryParse(item[2], NumberStyles.Any, CultureInfo.InvariantCulture, out int vn))
                            {
                                throw new ArgumentException("Couldn't convert Vn number to int.");
                            }
                            NormalVerticesNumber.Add(vn);
                        }
                    }
                    break;
            }          
        }
    }
}
