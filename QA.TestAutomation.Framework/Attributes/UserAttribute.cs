using System;

namespace QA.TestAutomation.Framework.Attributes
{
    /// <summary>
    /// Mark test method or Given step to point user, provided in config section. User will be available via TestBase.User property
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple = false)]
    public class UserAttribute : Attribute
    {
        public UserAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; protected set; }
    }
}
