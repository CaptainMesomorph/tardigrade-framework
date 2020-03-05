using System;
using System.Linq;

namespace Tardigrade.Framework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the Type class.
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// Determine whether a type implements or derives from another (including open and unbound generic types).
        /// <a href="https://stackoverflow.com/questions/5461295/using-isassignablefrom-with-open-generic-types">Using IsAssignableFrom with 'open' generic types</a>
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <param name="fromType">Implemented or derived type.</param>
        /// <returns>True if the type implements or derives from the other; false otherwise.</returns>
        public static bool ImplementsOrDerives(this Type type, Type fromType)
        {
            if (fromType is null)
            {
                return false;
            }
            else if (!fromType.IsGenericType)
            {
                return fromType.IsAssignableFrom(type);
            }
            else if (!fromType.IsGenericTypeDefinition)
            {
                return fromType.IsAssignableFrom(type);
            }
            else if (fromType.IsInterface)
            {
                foreach (Type interfaceType in type.GetInterfaces())
                {
                    if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == fromType)
                    {
                        return true;
                    }
                }
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == fromType)
            {
                return true;
            }

            return type.BaseType?.ImplementsOrDerives(fromType) ?? false;
        }

        /// <summary>
        /// Get the name of the type. If the type is generic, replace the generic arity (`) with the name of the
        /// generic type parameter using a more human readable format. This method will handle nested generic types.
        /// <![CDATA[For example, AuditEntry<string> would return AuditEntry<string> instead of AuditEntry`T.]]>
        /// <a href="https://stackoverflow.com/questions/17480990/get-name-of-generic-class-without-tilde">Get name of generic class without tilde</a>
        /// <a href="https://stackoverflow.com/questions/3396300/get-type-name-without-full-namespace-in-c-sharp">Get type name without full namespace in C#</a>
        /// </summary>
        /// <param name="type">Type class.</param>
        /// <returns>Name of the type (without the generic arity for generic types).</returns>
        public static string NameWithGenericType(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            string typeName = $"{type.Name.Substring(0, type.Name.IndexOf('`'))}";
            string genericTypeParameters =
                $"<{string.Join(",", type.GetGenericArguments().Select(t => t.NameWithGenericType()))}>";

            return $"{typeName}{genericTypeParameters}";
        }

        /// <summary>
        /// Get the name of the type. If the type is generic, ignore the generic arity (`) associated with the type.
        /// <![CDATA[For example, AuditEntry<string> would return AuditEntry instead of AuditEntry`T.]]>
        /// <a href="https://stackoverflow.com/questions/17480990/get-name-of-generic-class-without-tilde">Get name of generic class without tilde</a>
        /// </summary>
        /// <param name="type">Type class.</param>
        /// <returns>Name of the type (without the generic arity for generic types).</returns>
        public static string NameWithoutGenericArity(this Type type)
        {
            return type.Name.Split('`')[0];
        }
    }
}