using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberty.Application.Utils
{
    public class StringUtil
    {

        public static string ToFirstUpper(string str)
        {

            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException();
            }

            var _array = str.ToCharArray();
            var up = char.ToUpper(_array[0]);
            _array[0] = up;
            return new string(_array);
        }
    }
}
