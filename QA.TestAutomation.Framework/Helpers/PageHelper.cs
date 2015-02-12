using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using QA.TestAutomation.Framework.Configuration;
using QA.TestAutomation.Framework.WebDriver;

namespace QA.TestAutomation.Framework.Helpers
{
    public static class PageHelper
    {
        private static int timeout;

        public static int Timeout
        {
            get
            {
                if (timeout == 0)
                {
                    timeout = DriverConfiguration.GetConfiguration().Timeout;
                    if (timeout == 0)
                    {
                        throw new ConfigurationErrorsException("Timeout property of driverConfiguration section is set to 0. Timeout must be > 0");
                    }
                }

                return timeout;
            }
        }

        public static void ClickViaJs(this IWebElement element)
        {
            var executor = (IJavaScriptExecutor)WebDriverContext.Current.WebDriver;
            if (executor is PhantomJSDriver)
            {
                element.Click();
            }
            else
            {
                executor.ExecuteScript("arguments[0].click()", element);
            }
        }

        /// <summary>
        /// Gets errors from js console. Now work only for firefox
        /// </summary>
        /// <returns>List of js console errors</returns>
        public static IList<object> GetJsConsoleErrors()
        {
            if (WebDriverContext.Current.WebDriver.GetType() != typeof(OpenQA.Selenium.Firefox.FirefoxDriver))
            {
                throw new NotSupportedException("Implemented only for Firefox");

            }

            return
                (IList<object>)
                (((IJavaScriptExecutor)WebDriverContext.Current.WebDriver).ExecuteScript(
                    "return window.JSErrorCollector_errors.pump()"));
        }

        /// <summary>
        /// Wait all IWebElements to be loaded
        /// </summary>
        /// <param name="element">Target element</param>
        /// <param name="webDriver">WebDriver instance</param>
        /// <exception cref="ConfigurationException"></exception>
        public static void Wait(object element, IWebDriver webDriver)
        {
            Wait(element, webDriver, Timeout);
        }

        /// <summary>
        /// Wait all IWebElements to be loaded
        /// </summary>
        /// <param name="element">Target element</param>
        /// <param name="timeout">Timeout in seconds</param>
        /// <param name="webDriver">WebDriver instance</param>
        /// <exception cref="ConfigurationException"></exception>
        public static void Wait(object element, IWebDriver webDriver, int timeout)
        {
            var props = element.GetType()
                               .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                               .Where(p =>
                                   (typeof(IWebElement).IsAssignableFrom(p.PropertyType) || typeof(IList<IWebElement>).IsAssignableFrom(p.PropertyType))
                                   && p.GetCustomAttributes(typeof(FindsByAttribute), true).Any())
                               .ToArray();

            foreach (var p in props)
            {
                var attrs = p.GetCustomAttributes(typeof(FindsByAttribute), true)
                    .OrderBy(a => ((FindsByAttribute)a).Priority)
                    .ToArray();

                IWebElement el = null;
                foreach (FindsByAttribute attr in attrs)
                {
                    try
                    {
                        el = WaitUntilLoaded(webDriver, attr, timeout);
                        if (el != null) break;
                    }
                    catch (WebDriverTimeoutException)
                    {
                    }                   
                }

                if (el != null) continue;

                var last = (FindsByAttribute)attrs.Last();
                throw new ConfigurationErrorsException(
                    String.Format("WebDriver was unable to find element by \"{0}\" using \"{1}\"", last.How,
                                  last.Using));
            }
        }

        /// <summary>
        /// Waits until element is loaded
        /// </summary>
        /// <param name="webDriver">WebDriver instance</param>
        /// <param name="attr">Locator</param>
        /// <returns>Loaded IWebElement</returns>
        public static IWebElement WaitUntilLoaded(IWebDriver webDriver, FindsByAttribute attr)
        {
            return WaitUntilLoaded(webDriver, attr, Timeout);
        }

        /// <summary>
        /// Waits until element is loaded
        /// </summary>
        /// <param name="webDriver">WebDriver instance</param>
        /// <param name="attr">Locator</param>
        /// <param name="timeout">Timeout in seconds</param>
        /// <returns>Loaded IWebElement</returns>
        public static IWebElement WaitUntilLoaded(IWebDriver webDriver, FindsByAttribute attr, int timeout)
        {
            return new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeout)).Until(w => FindElement(w, attr));
        }

        public static void WaitUntil(Func<bool> func)
        {
            WaitUntil(func, Timeout);
        }

        public static void WaitUntil(Func<bool> func, int timeout)
        {
            var start = DateTime.Now;
            var flag = false;
            while (!flag && (DateTime.Now - start).Seconds < timeout)
            {
                flag = func();
                Thread.Sleep(1000);
            }

            if (!flag)
            {
                throw new WebDriverTimeoutException();
            }
         }

        public static void WaitUntil(Func<bool> func, int timeout, string failMessage)
        {
            var start = DateTime.Now;
            var flag = false;
            while (!flag && (DateTime.Now - start).Seconds < timeout)
            {
                flag = func();
                Thread.Sleep(1000);
            }

            if (!flag)
            {
                Assert.Fail(failMessage);
            }
        }

        public static void WaitUntilTextEqualExpected(IWebElement element, string expectedValue, int timeout, string message)
        {
            var wait = new WebDriverWait(WebDriverContext.Current.WebDriver, TimeSpan.FromSeconds(timeout));
            try
            {
                wait.Until(driver => element.Text == expectedValue);
            }
            catch (WebDriverTimeoutException)
            {
                Assert.Fail(message);
            }
        }

        /// <summary>
        /// Waits until Text of element will be equal expected (actual for JQuery element)
        /// </summary>
        /// <param name="element">PageObjects element</param>
        /// <param name="expectedValue">Expected value</param>
        /// <param name="timeout">Timeout in seconds</param>
        /// <param name="message">Message if Text of element isn't equal expected</param>
        public static void ExplicitWaitJQuery(IWebElement element, string expectedValue, int timeout, string message)
        {
            var wait = new WebDriverWait(WebDriverContext.Current.WebDriver, TimeSpan.FromSeconds(timeout));
            var executor = (IJavaScriptExecutor)WebDriverContext.Current.WebDriver;
            try
            {
                wait.Until(_ => executor.ExecuteScript("return jQuery.active").ToString() == "0");
                wait.Until(_ => element.Text == expectedValue);
            }
            catch (WebDriverTimeoutException)
            {
                Assert.Fail(message);
            }
            catch (Exception e)
            {
                Assert.Fail("Unexpected exception: " + e.Message);
            }
        }

        /// <summary>
        /// Waits for IWebElement property to be loaded
        /// </summary>
        /// <typeparam name="TPage">Page or PageElemnt type</typeparam>
        /// <param name="expression">Property expression</param>
        /// <returns>Loaded IWebElement</returns>
        public static IWebElement WaitFor<TPage>(Expression<Func<TPage, IWebElement>> expression)
        {
            return WaitFor(expression, Timeout);
        }

        /// <summary>
        /// Waits for IWebElement property to be loaded
        /// </summary>
        /// <typeparam name="TPage">Page or PageElemnt type</typeparam>
        /// <param name="expression">Property expression</param>
        /// <param name="timeout">Timeout in seconds</param>
        /// <returns>Loaded IWebElement</returns>
        public static IWebElement WaitFor<TPage>(Expression<Func<TPage, IWebElement>> expression, int timeout)
        {
            return DoWaitFor(expression, timeout);
        }

        private static IWebElement DoWaitFor(LambdaExpression expression, int timeout)
        {
            try
            {
                var member = (MemberExpression) expression.Body;
                var attr = (FindsByAttribute) member.Member
                                                    .GetCustomAttributes(typeof (FindsByAttribute), false)
                                                    .First();

                return WaitUntilLoaded(WebDriverContext.Current.WebDriver, attr, timeout);
            }
            catch (InvalidCastException e)
            {
                throw new NotSupportedException("Expression must reference to propery", e);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException("FindsByAttribute must be set to property", e);
            }
        }

        public static IWebElement WaitFor<TPage>(Expression<Func<TPage, IList<IWebElement>>> expression)
        {
            return WaitFor(expression, Timeout);
        }

        public static IWebElement WaitFor<TPage>(Expression<Func<TPage, IList<IWebElement>>> expression, int timeout)
        {
            return DoWaitFor(expression, timeout);
        }

        /// <summary>
        /// Find element by attribute.
        /// How == Custom is not supported
        /// </summary>
        public static IWebElement FindElement(IWebDriver webDriver, FindsByAttribute attr)
        {
            By by;

            switch (attr.How)
            {
                case How.ClassName:
                    @by = By.ClassName(attr.Using); break;
                case How.CssSelector:
                    @by = By.CssSelector(attr.Using); break;
                case How.Id:
                    @by = By.Id(attr.Using); break;
                case How.LinkText:
                    @by = By.LinkText(attr.Using); break;
                case How.Name:
                    @by = By.Name(attr.Using); break;
                case How.PartialLinkText:
                    @by = By.PartialLinkText(attr.Using); break;
                case How.TagName:
                    @by = By.TagName(attr.Using); break;
                case How.XPath:
                    @by = By.XPath(attr.Using); break;

                default: throw new NotSupportedException();
            }

            return webDriver.FindElement(@by);
        }
    }
}
