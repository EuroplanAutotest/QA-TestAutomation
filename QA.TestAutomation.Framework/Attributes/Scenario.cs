using System;

namespace QA.TestAutomation.Framework.Attributes
{
    /// <summary>
    /// Test Scenario attribute mark test method with this attribute, when generating DSL
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Scenario : Attribute
    {
        public string Title { get; set; }

        public string[] Tags { get; set; }

        public Scenario(string title, params string[] tags)
        {
            Title = title;
            Tags = tags;
        }
    }
}
