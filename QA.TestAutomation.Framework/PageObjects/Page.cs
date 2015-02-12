using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.PageObjects;
using QA.TestAutomation.Framework.Configuration;
using QA.TestAutomation.Framework.Helpers;
using QA.TestAutomation.Framework.WebDriver;

namespace QA.TestAutomation.Framework.PageObjects
{
    /// <summary>
    /// Generic Page Object Class. Incapsulate Web Driver low-level logic.
    /// Public properies of derived classes would be considered as Page Elements.
    /// </summary>
    public class Page : PageElement
    {
        #region Vars

        private readonly string _baseUrl;
        
        private readonly string _url;

        private Dictionary<Type, PageElement> _pageElements;

        public static Page Current { get; private set; }

        protected IWebDriver WebDriver
        {
            get
            {
                return WebDriverContext.WebDriver;
            }
        }

        #endregion

        #region Props

        /// <summary>
        /// Current Page url. Can be helpfull if redirect has occured
        /// </summary>
        public string CurrentUrl
        {
            get { return WebDriver.Url; }
        }

        #endregion

        #region ConstructorAndInitialization

        public Page(WebDriverContext webDriverContext, string baseUrl, string url)
            : base(webDriverContext)
        {
            if (String.IsNullOrEmpty(baseUrl)) throw new ArgumentException("baseUrl can't be null or empty", "baseUrl");
            
            _baseUrl = baseUrl;
            _url = url;
            _pageElements = new Dictionary<Type, PageElement>();
            InitPageElementProperties();
            PageFactory.InitElements(WebDriver, this);
            foreach (var pageElement in _pageElements)
            {
                PageFactory.InitElements(WebDriver, pageElement);
            }

            Current = this;
        }

        internal void InitPageElementProperties()
        {
            // transfer properties to PageElements dictionary
            var props = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty |
                               BindingFlags.Instance)
                .Where(p => typeof(PageElement).IsAssignableFrom(p.PropertyType))
                .ToList();

            foreach (var propertyInfo in props)
            {
                var val = (PageElement)propertyInfo.GetValue(this, null) ?? Create(propertyInfo.PropertyType, WebDriverContext);

                if (!_pageElements.Keys.Contains(val.GetType()))
                {
                    _pageElements.Add(val.GetType(), val);
                }
            }
        }

        public static T Create<T>(WebDriverContext webDriverContext, string baseUrl, string url)
            where T : Page
        {
            var constructor = typeof(T).GetConstructors().Where(c =>
            {
                var pars = c.GetParameters();
                if (pars.Count() < 3) return false;
                if (pars[0].ParameterType != typeof(WebDriverContext)) return false;
                if (pars[1].ParameterType != typeof(string)) return false;
                if (pars[2].ParameterType != typeof(string)) return false;
                return true;
            }).SingleOrDefault();

            if (constructor == null) throw new InvalidOperationException("Your page class must implement default page constructor");
            return (T)constructor.Invoke(new object[] { webDriverContext, baseUrl, url });
        }

        #endregion

        /// <summary>
        /// Wait for all IWebElement properies of page instance to be loaded.
        /// </summary>
        /// <param name="withElements">Wait all page elements to be loaded or just load page IWebElement properties</param>
        /// <returns>this</returns>
        public Page WaitUntilLoaded()
        {
            PageHelper.Wait(this, WebDriver);
            return this;
        }

        /// <summary>
        /// Navigate to url, provided
        /// </summary>
        /// <returns>this</returns>
        public Page Navigate()
        {
            var u = new Uri(new Uri(_baseUrl), _url);
            WebDriver.Url = u.ToString();

            WebDriver.Navigate();
            return this;
        }

        public Page Refresh()
        {
            WebDriver.Navigate().Refresh();
            return this;
        }

        public T ContinueWith<T>()
            where T : Page
        {
            return Create<T>(WebDriverContext, _baseUrl, WebDriverContext.WebDriver.Url.Replace(_baseUrl, String.Empty));
        }

        public T GoTo<T>(string baseUrl, string url)
            where T : Page
        {
            return Create<T>(WebDriverContext, baseUrl, url);
        }

        public T GoTo<T>(string url)
            where T : Page
        {
            return GoTo<T>(EnvironmentsConfiguration.GetConfiguration().CurrentEnvironmentBaseUrl, url);
        }

        /*
         * Методы, использующие JS
         */

        #region JS and Angular JS

        public void SetAutid(string csspath, string autId)
        {
            var s = WebDriver.ExecuteJavaScript<string>("$('" + csspath + "').attr('autId','" + autId + "'); return '0';");
        }

        public string GetScope(string csspath, string element)
        {
            return WebDriver.ExecuteJavaScript<string>("return $('" + csspath + "').scope().model." + element + ";");
        }

        public bool GetScopeIsSelected(string csspath, string element)
        {
            return WebDriver.ExecuteJavaScript<bool>("return $('" + csspath + "').scope().model." + element + ";");
        }


        /// <summary>
        /// Returns angular JS element value - which checkbox is checked now.
        /// </summary>
        /// <param name="checkbox">WebElement - checkbox - any of the checkboxes of the same scope.</param>
        /// <returns>string - value of the active checkbox from Angular</returns>
        public string CheckboxValue(IWebElement checkbox)
        {
            return WebDriver.ExecuteJavaScript<string>("return angular.element(arguments[0]).scope().Model.application.type;", checkbox);
        }

        #endregion

        #region CommonElements

        public static Dictionary<string, CommonElement> CommonControls { get; set; }

        public void CommonElementAction(string controlName, string keys)
        {
            GetElement(CommonControls[controlName]).SendKeys(keys);
        }

        public void CommonElementAction(string controlName)
        {
            GetElement(CommonControls[controlName]).Click();
        }

        public IWebElement GetElement(CommonElement element)
        {
            switch (element.LocatorType)
            {
                case Locators.ClassName:
                    return WebDriver.FindElement(By.ClassName(element.Locator));
                case Locators.CssSelector:
                    return WebDriver.FindElement(By.CssSelector(element.Locator));
                case Locators.Id:
                    return WebDriver.FindElement(By.Id(element.Locator));
                case Locators.LinkText:
                    return WebDriver.FindElement(By.LinkText(element.Locator));
                case Locators.Name:
                    return WebDriver.FindElement(By.Name(element.Locator));
                case Locators.PartialLinkText:
                    return WebDriver.FindElement(By.PartialLinkText(element.Locator));
                case Locators.TagName:
                    return WebDriver.FindElement(By.TagName(element.Locator));
                case Locators.XPath:
                    return WebDriver.FindElement(By.XPath(element.Locator));
                default:
                    return null;
            }
        }
        #endregion

        /*
         * Общая логика выбора значений из выпадающих списков
         */

        #region Selectors (Dropdown boxes)

        public static Dictionary<string, CommonElement> DropDowns { get; set; }

        public string ParentContainter(string dropBoxName)
        {
            return DropDowns[dropBoxName].Locator;
        }

        /// <summary>
        /// Repeats the given sequence of actions if StaleElementReferenceException occured.
        /// </summary>
        /// <param name="testMethod">The sequence of actions that can cause StaleElementReferenceException.</param>
        /// <returns>True if the testMethod actions have been completed successfully. And false if exception hapepnned.</returns>
        public bool WaitForElement(Action testMethod)
        {
            try
            {
                testMethod();
            }
            catch (StaleElementReferenceException)
            {
                testMethod();
            }
            catch (ElementNotVisibleException)
            {
                return false;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Makes 100 attempts to catch StaleElementReferenceException.
        /// </summary>
        /// <param name="selectorClick">Action on a web element that sets preconditions to catch StaleElementReferenceException.</param>
        /// <param name="testMethod">The sequence of actions that can cause StaleElementReferenceException.</param>
        /// <returns>True if the testMethod actions completed successfully. False if all 100 attempts where not successfull.</returns>
        public bool RepeatElementSelect(Action selectorClick, Action testMethod)
        {
            MiscTemp.SetElementTimeout(WebDriver, MiscTemp.LongTimer);
            var a = false;
            short j = 0;
            do
            {
                selectorClick();
                var b = WaitForElement(testMethod);
                a = b;
                j++;
            } while (!a && j < 100);

            MiscTemp.SetElementTimeout(WebDriver, MiscTemp.ShortTimer);

            return a;
        }


        public IWebElement DropDown(string parentContainer)
        {
            return WebDriver.FindElement(By.CssSelector(parentContainer));
        }

        public void Marker(string textToMark, string parentContainer)
        {
            var cssSelector = parentContainer + " > div > ul > li:contains(\u0022" + textToMark + "\u0022)";
            const string autId = "Marked";
            SetAutid(cssSelector, autId);
        }

        public IWebElement SelectThis
        {
            get
            {
                return WebDriver.FindElement(By.CssSelector("[autId='Marked']"));
            }
        }
        #endregion
        
    }
}
