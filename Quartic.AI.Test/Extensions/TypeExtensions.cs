namespace Quartic.AI.Test.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Quartic.AI.Test.Attributes;

    public static class TypeExtensions
    {
        public static PropertyInfo[] GetFilteredProperties(this Type type)
        {
            return type.GetProperties().Where(property => !Attribute.IsDefined(property, typeof(DoNotIncludeInReflectionAttribute))).ToArray();
        }

        public static PropertyInfo[] GetExemptProperties(this Type type)
        {
            return type.GetProperties().Where(property => Attribute.IsDefined(property, typeof(ExemptAttribute))).ToArray();
        }

        public static PropertyInfo[] GetIntegerProperties(this Type type)
        {
            return type.GetProperties().Where(property => Attribute.IsDefined(property, typeof(IntegerAttribute))).ToArray();
        }

        public static PropertyInfo[] GetIntegerWhereNotExemptProperties(this Type type)
        {
            return type.GetProperties().Where(property => Attribute.IsDefined(property, typeof(IntegerAttribute)) && !Attribute.IsDefined(property, typeof(ExemptAttribute))).ToArray();
        }

        public static PropertyInfo[] GetStringProperties(this Type type)
        {
            return type.GetProperties().Where(property => Attribute.IsDefined(property, typeof(StringAttribute))).ToArray();
        }

        public static PropertyInfo[] GetStringWhereNotExemptProperties(this Type type)
        {
            return type.GetProperties().Where(property => Attribute.IsDefined(property, typeof(StringAttribute)) && !Attribute.IsDefined(property, typeof(ExemptAttribute))).ToArray();
        }

        public static PropertyInfo[] GetDateTimeProperties(this Type type)
        {
            return type.GetProperties().Where(property => Attribute.IsDefined(property, typeof(DateTimeAttribute))).ToArray();
        }

        public static PropertyInfo[] GetDateTimeWhereNotExemptProperties(this Type type)
        {
            return type.GetProperties().Where(property => Attribute.IsDefined(property, typeof(DateTimeAttribute)) && !Attribute.IsDefined(property, typeof(ExemptAttribute))).ToArray();
        }
    }
}