using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace MainLogic.Components
{
    public class NormalVertex
    {
        private const int _minElementsLength = 4;

        public float I { get; set; }
        public float J { get; set; }
        public float K { get; set; }

        public NormalVertex() { }

        public void GetElementsFromArray(string[] line)
        {
            if (line.Length < _minElementsLength)
            {
                throw new ArgumentException("Few elements in line.");
            }

            if (!float.TryParse(line[1], NumberStyles.Any, CultureInfo.InvariantCulture, out float i))
            {
                throw new ArgumentException("Couldn't convert I to float.");
            }
            I = i;

            if (!float.TryParse(line[2], NumberStyles.Any, CultureInfo.InvariantCulture, out float j))
            {
                throw new ArgumentException("Couldn't convert J to float.");
            }
            J = j;

            if (!float.TryParse(line[3], NumberStyles.Any, CultureInfo.InvariantCulture, out float k))
            {
                throw new ArgumentException("Couldn't convert K to float.");
            }
            K = k;
        }
    }
}
