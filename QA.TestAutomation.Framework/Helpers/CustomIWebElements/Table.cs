using System;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace QA.TestAutomation.Framework.Helpers.CustomIWebElements
{
    public class Table
    {
        private static IWebElement element;

        public Table(IWebElement _element)
        {
            element = _element;
            if (element.TagName != "table")
            {
                throw new ArgumentException("Invalid element passed for table element");
            }
        }

        private IWebElement GetCellFromRow(IWebElement row, int colId)
        {
            if (row.TagName != "tr")
            {
                throw new ArgumentException("Invalid element passed for row element");
            }
            string xpath = string.Format("td[{0}]", colId);
            return row.FindElement(By.XPath(string.Format(xpath)));
        }

        private static int GetColumnIndex(IWebElement tableCell, IWebDriver driver)
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

       

        public List<IWebElement> GetAllRows()
        {
            try
            {
                var rows = element.FindElements(By.XPath("tbody/tr"));
                return new List<IWebElement>(rows);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IWebElement GetRow(int rowId)
        {
            return GetAllRows()[rowId];
        }

        public IWebElement GetCell(int rowId, int colId)
        {
            return GetCellFromRow(GetRow(rowId), colId);
        }
       

        public IWebElement GetCell(int rowId, string colName, IWebDriver driver)
        {
            IWebElement row = GetRow(rowId);

            if (row.TagName != "tr")
            {
                throw new ArgumentException("Invalid element passed for row element");
            }

            var headingRow = row.FindElement(By.XPath("ancestor::table[1]/thead/tr"));
            string xpathForTh = string.Format("th[contains(text(),'{0}')]", colName);

            var th = headingRow.FindElement(By.XPath(xpathForTh));

            int columnIndex = GetColumnIndex(th, driver);

            string xpath = string.Format("td[{0}]", columnIndex);

            return row.FindElement(By.XPath(string.Format(xpath)));

        }

    }
}
