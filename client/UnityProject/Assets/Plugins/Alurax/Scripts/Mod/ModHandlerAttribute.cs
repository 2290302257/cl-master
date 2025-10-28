using System;

namespace Alurax
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ModHandlerAttribute : System.Attribute
    {
        public string Name { get; private set; } = "Global";
        public int Order { get; private set; } = 0;
        
        public ModHandlerAttribute(){}
        
        public ModHandlerAttribute(int order)
        {
            Order = order;
        }
        
        public ModHandlerAttribute(object obj)
        {
            Name = obj.ToString();
        }
        
        public ModHandlerAttribute(object obj, int order)
        {
            Name = obj.ToString();
            Order = order;
        }
    }
}
