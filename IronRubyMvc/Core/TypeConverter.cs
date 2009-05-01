namespace System.Web.Mvc.IronRuby.Core
{
    public class TypeConverter
    {
        public static T ConvertTo<T>(object value)
        {
            return (T)ConvertType(value, typeof(T));
        }

        public static object ConvertType(object value, Type targetType)
        {
            if (value == null)
                return null;

            if (value.GetType() == targetType)
                return value;
            
            if (targetType.IsValueType)
            {
                if (!targetType.IsGenericType)
                {
                    return targetType.IsEnum ? Enum.ToObject(targetType, value) : Convert.ChangeType(value, targetType);
                }

                if (targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var realType = targetType.GetGenericArguments()[0];

                    return ConvertType(value, realType);
                }
            }

            return Convert.ChangeType(value, targetType);
        }
    }
}