namespace PasswordStore.Lib.Extension
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;

    public static class ObjectExtensions
    {
        public static string GetPropertyDisplayName(this Type type, string propertyName)
        {
            var property = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).FirstOrDefault(p => p.Name == propertyName);
            return property?.GetDisplayName();
        }
        
        public static string GetDisplayName(this PropertyInfo propertyInfo)
        {
            var displayAttr = propertyInfo.GetCustomAttribute<DisplayAttribute>();
            return displayAttr?.Name;
        }
    }
}