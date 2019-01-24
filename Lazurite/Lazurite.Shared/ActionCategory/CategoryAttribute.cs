using System;
using System.Linq;
using System.Reflection;

namespace Lazurite.Shared.ActionCategory
{
    public class CategoryAttribute: Attribute
    {
        public CategoryAttribute(Category category) => Category = category;

        public Category Category { get; private set; }

        public static Category Get(Type type)
        {
            var attr = type.GetTypeInfo().GetCustomAttributes(typeof(CategoryAttribute), true).FirstOrDefault();
            if (attr == null)
                return Category.Other;
            else
                return ((CategoryAttribute)attr).Category;
        }
    }
}
