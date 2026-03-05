using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberty.Application.Utils
{
    public static class EntityUtil
    {
        public static string CreateCode() => Guid.NewGuid().ToString();
    }
}
