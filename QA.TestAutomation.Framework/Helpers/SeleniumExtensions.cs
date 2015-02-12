using OpenQA.Selenium;
using System.Linq;
using System.Collections.Generic;
using OpenQA.Selenium.Support.UI;
using QA.TestAutomation.Framework.Helpers.CustomIWebElements;

namespace QA.TestAutomation.Framework.Helpers
{
    public static class SeleniumExtensions
    {
        #region Convertion

        /// <summary>
        /// Converts IWebElement to SelectElement
        /// </summary>
        /// <param name="combobox">IWebElement representing combobox</param>
        /// <returns>SelectElement representing combobox</returns>
        public static SelectElement ToCombobox(this IWebElement combobox)
        {
            return new SelectElement(combobox);
        }

        public static Table ToTable(this IWebElement element)
        {
            return new Table(element);
        }

        public static Textbox ToTextbox(this IWebElement element)
        {
            return new Textbox(element);
        }
        public static Checkbox ToCheckbox(this IWebElement element)
        {
            return new Checkbox(element);
        }
        public static NumericUpDown ToNumericUpDown(this IWebElement element)
        {
            return new NumericUpDown(element);
        }

        public static TableRow ToTableRow(this IWebElement element)
        {
            return new TableRow(element);
        }

        #endregion Convertion

        #region IEnumerable<IWebElement> functions

        /// <summary>
        /// Clicks on all displayed elements one by one
        /// </summary>
        /// <param name="elements"></param>
        public static void ClickAllDisplayedElements(this IEnumerable<IWebElement> elements)
        {
            var displayedElements = from IWebElement e in elements
                                    where e.Displayed
                                    select e;

            foreach (var element in displayedElements)
                element.Click();
        }

        #endregion
       
    }
}
