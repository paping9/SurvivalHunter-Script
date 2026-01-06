using System;

namespace Utils.Numberics
{
    [Serializable]
    public struct Float3Vector
    {
        public float x, y, z;

        public static readonly Float3Vector Zero = new Float3Vector()
        {
            x = 0.0f,
            y = 0.0f,
            z = 0.0f
        };
        
        public static readonly Float3Vector NaN = new Float3Vector()
        {
            x = float.NaN,
            y = float.NaN,
            z = float.NaN
        };
    }
}