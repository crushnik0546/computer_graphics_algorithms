using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace MainLogic.Components
{
    public class GeometricVertex
    {
        private const int _minElementsLength = 4;

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public GeometricVertex() { }

        public void GetElementsFromArray(string[] line)
        {
            if (line.Length < _minElementsLength)
            {
                throw new ArgumentException("Few elements in line.");
            }

            W = 1;

            if (!float.TryParse(line[1], NumberStyles.Any, CultureInfo.InvariantCulture, out float x))
            {
                throw new ArgumentException("Couldn't convert X to float.");
            }
            X = x;

            if (!float.TryParse(line[2], NumberStyles.Any, CultureInfo.InvariantCulture, out float y))
            {
                throw new ArgumentException("Couldn't convert Y to float.");
            }
            Y = y;

            if (!float.TryParse(line[3], NumberStyles.Any, CultureInfo.InvariantCulture, out float z))
            {
                throw new ArgumentException("Couldn't convert Z to float.");
            }
            Z = z;

            if (line.Length == _minElementsLength + 1)
            {
                if (!float.TryParse(line[4], NumberStyles.Any, CultureInfo.InvariantCulture, out float w))
                {
                    throw new ArgumentException("Couldn't convert W to float.");
                }
                W = w;
            }                                  
        }
    }
}
