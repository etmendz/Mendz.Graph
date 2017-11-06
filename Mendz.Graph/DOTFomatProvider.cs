using System;

namespace Mendz.Graph
{
    /// <summary>
    /// Represents the default DOT format provider.
    /// </summary>
    public class DOTFormatProvider : IFormatProvider
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return new DOTFormatter();
            }
            return null;
        }
    }
}
