using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using QA.TestAutomation.Framework.WebDriver;

namespace QA.TestAutomation.Framework.Helpers
{
    /// <summary>
    /// Provides possibility to execute multiple assertions
    /// </summary>
    public class AssertHelper : IDisposable
    {
        private readonly List<Exception> _exceptions = new List<Exception>();

        public static AssertHelper New
        {
            get {return new AssertHelper();}
        }

        public AssertHelper Verify(Action assert)
        {
            try
            {
                assert();
            }
            catch (Exception ex)
            {
                _exceptions.Add(ex);
            }

            return this;
        }

        public void Verify(bool addConsoleErrors = true)
        {
            if (_exceptions.Count == 0) return;

            var allErrors = new StringBuilder();
            foreach (var error in _exceptions)
            {
                allErrors.Append(error.Message);
            }

            if (addConsoleErrors && WebDriverContext.HasInstance)
            {
                var errors = PageHelper.GetJsConsoleErrors();
                if (errors.Count > 0)
                {
                    allErrors.AppendLine("JavaScript errors:");
                    foreach (Dictionary<string, object> error in errors)
                    {
                        object value;
                        if (error.TryGetValue("errorMessage", out value))
                        {
                            allErrors.AppendLine(value.ToString());
                        }
                    }
                }
            }

            Assert.Fail(allErrors.ToString());
        }

        public void Dispose()
        {
            Verify();
        }
    }
}
