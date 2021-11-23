using System;
using System.Collections.Generic;
using System.Text;

namespace SCSC.PlatformFunctions.Utilities
{
    internal class DurableEntityNameComparer: IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
        }
        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}
