using System;
using System.Collections.Generic;
using System.Linq;

using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace MainLogic.Components
{
    public class TextureVertex
    {
        private const int _minElementsLength = 2;

        public float U { get; set; }
        public float V { get; set; }
        public float W { get; set; }       

        public TextureVertex() { }

        public void GetElementsFromArray(string[] line)
        {
            if (line.Length < _minElementsLength)
            {
                throw new ArgumentException("Few elements in line.");
            }

            V = 0;
            W = 0;

            if (!float.TryParse(line[1], NumberStyles.Any, CultureInfo.InvariantCulture, out float u))
            {
                throw new ArgumentException("Couldn't convert U to float.");
            }
            U = u;

            if (line.Length < _minElementsLength + 1)
            {
                return;
            }

            if (!float.TryParse(line[2], NumberStyles.Any, CultureInfo.InvariantCulture, out float v))
            {
                throw new ArgumentException("Couldn't convert V to float.");
            }
            V = v;

            if (line.Length < _minElementsLength + 2)
            {
                return;
            }

            if (!float.TryParse(line[3], NumberStyles.Any, CultureInfo.InvariantCulture, out float w))
            {
                throw new ArgumentException("Couldn't convert W to float.");
            }
            W = w;
        }
    }
}
