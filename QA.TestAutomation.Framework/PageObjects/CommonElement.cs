namespace QA.TestAutomation.Framework.PageObjects
{
    public class CommonElement
    {
        public string Locator;
        public Locators LocatorType;

        public CommonElement(string locator, Locators type)
        {
            Locator = locator;
            LocatorType = type;
        }
    }
}
