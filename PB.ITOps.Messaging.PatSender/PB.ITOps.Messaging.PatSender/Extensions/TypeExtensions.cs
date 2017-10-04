using System;
using System.Reflection;

namespace PB.ITOps.Messaging.PatSender.Extensions
{
    public static class TypeExtensions
    {
        public static string SimpleQualifiedName(this Type t)
        {
            return string.Concat(t.FullName, ", ", t.GetTypeInfo().Assembly.GetName().Name);
        }
    }
}