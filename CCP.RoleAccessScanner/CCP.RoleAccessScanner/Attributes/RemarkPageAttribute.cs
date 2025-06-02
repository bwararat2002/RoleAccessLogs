using System;

namespace CCP.RoleAccessScanner.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class RemarkPageAttribute : Attribute
    {
        public string Title { get; }

        public RemarkPageAttribute(string title)
        {
            Title = title;
        }
    }
}
