namespace Blorc.OpenIdConnect
{
    using System;

    public static class TypeExtensions
    {
        public static bool IsPrimitiveEx(this Type type)
        {
            return type.IsPrimitive || type.IsEnum
                                    || type == typeof(decimal)
                                    || type == typeof(string)
                                    || type == typeof(Guid)
                                    || type == typeof(Uri)
                                    || type == typeof(TimeSpan)
                                    || type == typeof(DateTime);
        }
    }
}
