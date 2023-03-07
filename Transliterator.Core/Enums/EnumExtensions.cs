using System.ComponentModel;

namespace Transliterator.Core.Enums
{
    public static class EnumExtensions
    {
        public static string GetDescriptionOrName(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            var field = type.GetField(name);
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            {
                return attribute.Description;
            }
            return name;
        }
    }
}
