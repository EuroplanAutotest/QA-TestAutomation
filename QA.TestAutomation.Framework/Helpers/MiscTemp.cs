using System;
using System.Threading;
using OpenQA.Selenium;

namespace QA.TestAutomation.Framework.Helpers
{
    public class MiscTemp
    {
        public static short ShortTimer = 5;
        public static short LongTimer = 20;

        public static void Wait(short sec)
        {
            Thread.Sleep(sec*1000);
        }

        // TODO: replace by webDriverContext
        /// <summary>
        /// Specifies the amount of time the driver should wait when searching for an element if it is not immediately present.  
        /// The timeout will be used everytime for FindElement and FindElements until another value is specified recalling this method again.
        /// </summary>
        /// <param name="webDriver">Webdriver instance that will use the specified timeout.</param>
        /// <param name="sec">Timeout in seconds. It is recommended to use MiscTemp.ShortTimer or MiscTemp.LongTimer.</param>
        public static void SetElementTimeout(IWebDriver webDriver, short sec)
        {
            webDriver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(sec));
        }

        // TODO: sreplace by webDriverContext
        /// <summary>
        /// Sets the amount of time to wait for an asynchronous script to finish execution before throwing an error. If the timeout is negative, then the script will be allowed to run indefinitely.  
        /// The timeout will be used everytime when executing JavaScript.
        /// </summary>
        /// <param name="webDriver">Webdriver instance that will use the specified timeout.</param>
        /// <param name="sec">Timeout in seconds. It is recommended to use MiscTemp.ShortTimer or MiscTemp.LongTimer.</param>
        public static void SetScriptTimeout(IWebDriver webDriver, short sec)
        {
            webDriver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(sec));
        }

        

    }

}
