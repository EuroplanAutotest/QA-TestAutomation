using System;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace QA.TestAutomation.Framework.Helpers.CustomIWebElements
{
    public class TableRow
    {
        private static IWebElement element;

        public TableRow(IWebElement _element)
        {
            element = _element;
        }

        private int GetColumnIndex(IWebElement tableCell, IWebDriver driver)
        {
            const string script = "return arguments[0].cellIndex";
            Int64 columnIndex = (Int64)((IJavaScriptExecutor)driver).ExecuteScript(script, tableCell);
            columnIndex++;
            int result;
            checked
            {
                result = (int)columnIndex;
            }
            return result;
        }

        public IWebElement GetCell(int columnIndex)
        {
            if (element.TagName != "tr")
            {
                throw new ArgumentException("Invalid element passed for row element");
            }
            string xpath = string.Format("td[{0}]", columnIndex);
            return element.FindElement(By.XPath(string.Format(xpath)));
        }

        public IWebElement GetCell(string column, IWebDriver driver)
        {
            if (element.TagName != "tr")
            {
                throw new ArgumentException("Invalid element passed for row element");
            }

            var headingRow = element.FindElement(By.XPath("ancestor::table[1]/thead/tr"));
            string xpathForTh = string.Format("th[contains(text(),'{0}')]", column);

            var th = headingRow.FindElement(By.XPath(xpathForTh));

            int columnIndex = GetColumnIndex(th, driver);

            string xpath = string.Format("td[{0}]", columnIndex);

            return element.FindElement(By.XPath(string.Format(xpath)));
        }

    }
}
