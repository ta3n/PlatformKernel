using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberty.Application.Utils
{
    public static class EnumUtil
    {
        public static T GetValue<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name);
        }

        public static T? IntToEnum<T>(int? value) where T : struct, Enum
        {
            if (value == null)
                return null;

            T enumValue = default(T);

            foreach (T item in Enum.GetValues(typeof(T)))
            {
                int intValue = Convert.ToInt32(item);
                if ((value & intValue) == intValue)
                    enumValue = (T)Enum.ToObject(typeof(T), Convert.ToInt32(enumValue) | intValue);
            }

            return enumValue;
        }

        public static int? EnumToInt<T>(T? enumValue) where T : struct, Enum
        {
            if (enumValue == null)
                return null;

            int value = 0;

            foreach (T item in Enum.GetValues(typeof(T)))
            {
                if (enumValue.Value.HasFlag(item))
                    value |= Convert.ToInt32(item);
            }

            return value;
        }
    }
}
