using System;
using System.Globalization;
using NUnit.Framework;

namespace QA.TestAutomation.Framework.Attributes
{
    /// <summary>
    /// Provides Feature (user story) description, mark test class with this attribute, when generating DSL
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FeatureAttribute : DescriptionAttribute
    {
        public string Title { get; private set; }

        public string Story
        {
            get { return string.Format("As {0}\nI Want {1}\nSo that {2}", As, IWant, SoThat); }
        }

        public string As { get; set; }

        // ReSharper disable InconsistentNaming
        public string IWant { get; set; }
        // ReSharper restore InconsistentNaming

        public string SoThat { get; set; }
        
        public CultureInfo CultureInfo { get; private set; }

        public FeatureAttribute(string title):base(title)
        {
            Title = title;
            CultureInfo = new CultureInfo("en-US");
        }

        public FeatureAttribute(string title, CultureInfo cultureInfo)
            : base(title)
        {
            Title = title;
            CultureInfo = cultureInfo;
        }
    }
}
