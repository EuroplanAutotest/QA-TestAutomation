using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;

namespace QA.TestAutomation.Framework.Helpers.CustomIWebElements
{
    public class Checkbox
    {
         private static IWebElement element;

        public Checkbox(IWebElement _element)
        {
            element = _element;
        }

        public void Check(bool viaJs = false)
        {
            if (!element.Selected)
            {
                if (viaJs)
                    element.ClickViaJs();
                else
                    element.Click();
            }
        }
        public void Uncheck(bool viaJs = false)
        {
            if (element.Selected)
            {
                if (viaJs)
                    element.ClickViaJs();
                else
                    element.Click();
            }
        }
    }
}
