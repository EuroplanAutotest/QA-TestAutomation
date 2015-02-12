using OpenQA.Selenium;

namespace QA.TestAutomation.Framework.Helpers.CustomIWebElements
{
    public class Textbox
    {
        private  IWebElement element;

        public Textbox(IWebElement _element)
        {
            element = _element;
        }

        public void SetValue(string value, bool needClear = true)
        {
            if (needClear)
                element.Clear();
            element.SendKeys(value);
        }

        public string GetValue()
        {
           // return element.Text;
            return element.GetAttribute("value");
        }
    }
}
