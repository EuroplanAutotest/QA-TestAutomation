using System.Collections.ObjectModel;
using System.Drawing;
using OpenQA.Selenium;

namespace QA.TestAutomation.Framework.Helpers.CustomIWebElements
{
    public class ProxyElement : IWebElement
    {
        private static IWebElement element;

        public ProxyElement(IWebElement _element)
        {
            element = _element;
        }

        public IWebElement FindElement(By @by)
        {
            return element.FindElement(by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By @by)
        {
            return element.FindElements(by);
        }

        public void Clear()
        {
            element.Clear();
        }

        public void SendKeys(string text)
        {
            element.SendKeys(text);
        }

        public void Submit()
        {
            element.Submit();
        }

        public void Click()
        {
            element.Click();
        }

        public string GetAttribute(string attributeName)
        {
            return element.GetAttribute(attributeName);
        }

        public string GetCssValue(string propertyName)
        {
            return element.GetCssValue(propertyName);
        }

        public string TagName
        {
            get { return element.TagName; }
            private set { }
        }
        public string Text 
        {
            get { return element.Text; }
            private set { }
        } 
        public bool Enabled 
        {
            get { return element.Enabled; }
            private set { }
        }
        public bool Selected 
        {
            get { return element.Selected; }
            private set { }
        }
        public Point Location 
        {
            get { return element.Location; }
            private set { }
        }
        public Size Size 
        {
            get { return element.Size; }
            private set { }
        }
        public bool Displayed 
        {
            get { return element.Displayed; }
            private set { }
        }
    }
}
