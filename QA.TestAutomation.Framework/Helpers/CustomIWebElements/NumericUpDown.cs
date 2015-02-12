using OpenQA.Selenium;

namespace QA.TestAutomation.Framework.Helpers.CustomIWebElements
{
    public class NumericUpDown
    {
        private static IWebElement element;

        public NumericUpDown(IWebElement _element)
        {
            element = _element;
        }

        public void SetValue(string value)
        {
            element.Clear();
            var defaultValue = GetValue();
            if (defaultValue != null)
            {
                for (int i = 0; i < defaultValue.Length; i++)
                    element.SendKeys(Keys.Backspace);
            }
            element.SendKeys(value);
            element.SendKeys(Keys.Enter);
        }

        public string GetValue()
        {
            return element.GetAttribute("value");
            //return element.Text;
        }
    }
}
