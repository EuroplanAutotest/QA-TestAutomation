using System;
using System.Linq;
using QA.TestAutomation.Framework.WebDriver;

namespace QA.TestAutomation.Framework.PageObjects
{
    public abstract class PageElement
    {
        public WebDriverContext WebDriverContext { get; private set; }

        protected PageElement(WebDriverContext webDriverContext)
        {
            WebDriverContext = webDriverContext;
        }

        public static T Create<T>(WebDriverContext webDriverContext)
            where T : PageElement
        {
            return (T)Create(typeof (T), webDriverContext);
        }

        public static PageElement Create(Type t, WebDriverContext webDriverContext)
        {
            var constructor = t.GetConstructors().Where(c =>
            {
                var pars = c.GetParameters();
                if (pars.Count() != 1) return false;
                if (pars[0].ParameterType != typeof(WebDriverContext)) return false;
                return true;
            }).SingleOrDefault();

            if (constructor == null) throw new InvalidOperationException("Your page class must implement default page constructor");
            return (PageElement)constructor.Invoke(new object[] { webDriverContext });
        }
    }
}
