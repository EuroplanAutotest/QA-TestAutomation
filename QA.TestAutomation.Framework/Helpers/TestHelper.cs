using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;

namespace QA.TestAutomation.Framework.Helpers
{
    public static class TestHelper
    {
        public static IDictionary ToDictionary(this TestCaseData testCaseData)
        {
            return (IDictionary) testCaseData.Arguments[0];
        }
    }
}
