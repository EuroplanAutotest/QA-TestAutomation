using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using QA.TestAutomation.Framework.Attributes;
using QA.TestAutomation.Framework.Configuration;
using TechTalk.SpecFlow;

namespace QA.TestAutomation.Framework.Tests
{
    public class TestsBase
    {
        private static readonly Regex BracesFilter = new Regex(@"\(.*\)");
        private UserElement _user;
        private DateTime _startTime;

        /// <summary>
        /// Checks if TestMethod contains UserAttribute.
        /// If attribute exists use this one, else try to find attribute at class.
        ///  If there is no attributes use User with name="Default"
        /// </summary>
        public UserElement User
        {
            get
            {
                if (_user == null)
                {
                    var type = GetType();
                    var testName = GetTestMethodName();
                    var name = BracesFilter.Replace(
                        testName,
                        string.Empty);

                    // First check if method contains attribute, then check class
                    UserAttribute u = null;
                    if (!string.IsNullOrEmpty(testName))
                    {
                        var method = type.GetMethod(name);
                        u = method == null
                            ? null
                            : (UserAttribute)method
                                .GetCustomAttributes(typeof (UserAttribute), true)
                                .FirstOrDefault();
                    }

                    if (u == null)
                    {
                        u = (UserAttribute) type
                                                .GetCustomAttributes(typeof (UserAttribute), true)
                                                .FirstOrDefault();
                    }

                    var str = u != null ? u.Name : "Default";
                    _user = EnvironmentsConfiguration.GetConfiguration().GetTargetEnvironment().Users[str];
                }

                return _user;
            }
        }

        public DateTime StartTime
        {
            get { return _startTime; }
            set { _startTime = value; }
        }

        /// <summary>
        /// Get WebDriverContext instance
        /// </summary>
        [SetUp]
        public virtual void SetUp()
        {
            _user = null;
            _startTime = DateTime.Now;
        }

        protected string GetTestMethodName()
        {
            var name =
                GetTestNameFromStackTrace<GivenAttribute>()
                ?? GetTestNameFromStackTrace<BeforeScenarioAttribute>()
                ?? GetTestNameFromStackTrace<TestAttribute>()
                ?? GetTestNameFromStackTrace<TestCaseAttribute>()
                ?? GetTestNameFromStackTrace<TestCaseSourceAttribute>();

            if (string.IsNullOrEmpty(name))
            {
                try
                {
                    name = TestContext.CurrentContext.Test.Name;
                }
                catch (NullReferenceException e)
                {
                    throw new ConfigurationErrorsException("TestContext.CurrentContext.Test.Name throws NullReferenceException. Probably, you need to update Resharper", e);
                }
               
            }

            return name;
        }

        protected static string GetTestNameFromStackTrace<T>()
            where T:Attribute
        {
            var st = new StackTrace();
            var frame = st.GetFrames()
                          .LastOrDefault(f => f.GetMethod().GetCustomAttributes(typeof (T), true).Any());

            if (frame == null)
            {
                return null;
            }

            var method = frame.GetMethod();
            return method.Name.Split('.').Last();
        }
    }
}