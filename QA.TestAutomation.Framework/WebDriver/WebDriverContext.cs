using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using QA.TestAutomation.Framework.Configuration;
using QA.TestAutomation.Framework.PageObjects;

namespace QA.TestAutomation.Framework.WebDriver
{
    /// <summary>
    /// Handles 
    /// </summary>
    public sealed class WebDriverContext : CriticalFinalizerObject, IDisposable
    {
        private const string PhantomUserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_7_5) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.95 Safari/537.11";

        #region Vars

        private IWebDriver _webDriver;

        private readonly DriverConfiguration _driverConfiguration;

        private readonly EnvironmentsConfiguration _environmentsConfiguration;

        #endregion

        #region Constructor & Finalizer

        public WebDriverContext(DriverConfiguration driver, EnvironmentsConfiguration environment)
        {
            _driverConfiguration = driver;
            _environmentsConfiguration = environment;
        }

        ~WebDriverContext()
        {
            if (_webDriver != null)
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            if (_webDriver != null)
            {
                _webDriver.Close();
                _webDriver.Quit();
                _webDriver = null;
            }

            Current = null;
        }

        public static void ReadConfig()
        {
            if (Current != null)
            {
                throw new InvalidOperationException("Context is already initialized");
            }

            Current = new WebDriverContext(DriverConfiguration.GetConfiguration(), EnvironmentsConfiguration.GetConfiguration());
        }

        /// <summary>
        /// Lazy-loaded IWebDriver instance
        /// </summary>
        public IWebDriver WebDriver
        {
           get
           {
               if (_webDriver == null)
               {
                   _webDriver = _driverConfiguration.IsRemote ? CreateRemoteDriver() : CreateDriver();
                   if (_driverConfiguration.SizeSet)
                   {
                       _webDriver.Manage().Window.Size = new Size(_driverConfiguration.Width, _driverConfiguration.Height);
                       _webDriver.Manage().Window.Position = new Point(0, 0);
                   }
                   else
                   {
                       _webDriver.Manage().Window.Maximize();
                   }
               }

               return _webDriver;
           }
        }

        #endregion

        #region Private

        private IWebDriver CreateRemoteDriver()
        {
            DesiredCapabilities cpb;
            var url = _driverConfiguration.RemoteUrl;
            if (string.IsNullOrEmpty(url))
            {
                throw new ConfigurationErrorsException("'remoteUrl' attribute must be configured at driverConfiguration");
            }

            switch (_driverConfiguration.TargetDriver)
            {
                case DriverNames.IE:
                    cpb = DesiredCapabilities.InternetExplorer();
                    break;
                case DriverNames.Firefox:
                    cpb = DesiredCapabilities.Firefox();
                    break;
                case DriverNames.Chrome:
                    cpb = DesiredCapabilities.Chrome();
                    break;
                case DriverNames.PhantomJS:
                    cpb = DesiredCapabilities.PhantomJS();
                    break;
                case DriverNames.Safari:
                    cpb = DesiredCapabilities.Safari();
                    break;
                default:
                    throw new NotImplementedException();
            }

            cpb.IsJavaScriptEnabled = true;
            cpb.SetCapability("takesScreenshot", true);
            return new RemoteWebDriverWithScreenshots(new Uri(url), cpb);            
        }

        private IWebDriver CreateDriver()
        {
            IWebDriver webDriver;
            switch (_driverConfiguration.TargetDriver)
            {
                case DriverNames.IE:
                    webDriver = new InternetExplorerDriver(new InternetExplorerOptions()
                        {
                            IgnoreZoomLevel = true,
                            IntroduceInstabilityByIgnoringProtectedModeSettings = true
                        });
                    break;
                case DriverNames.Firefox:
                    var firefoxProfile = new FirefoxProfile();
                    firefoxProfile.AddExtension(@"JSErrorCollector.xpi");
                    if (!String.IsNullOrEmpty(DriverConfiguration.GetConfiguration().DownloadDir))
                    {
                        firefoxProfile.SetPreference("browser.download.dir", DriverConfiguration.GetConfiguration().DownloadDir);
                        firefoxProfile.SetPreference("browser.helperApps.alwaysAsk.force", false);
                        firefoxProfile.SetPreference("browser.download.folderList", 2);
                        firefoxProfile.SetPreference("services.sync.prefs.sync.browser.download.manager.showWhenStarting", false);
                        firefoxProfile.SetPreference("browser.download.useDownloadDir", true);
                        firefoxProfile.SetPreference("browser.helperApps.neverAsk.saveToDisk","image/png, application/vnd.ms-excel");
                    }
                    webDriver = new FirefoxDriver(firefoxProfile);
                    break;
                case DriverNames.Chrome:
                    webDriver = new OpenQA.Selenium.Chrome.ChromeDriver();
                    break;
                case DriverNames.PhantomJS:
                    var options = new PhantomJSOptions();
                        options.AddAdditionalCapability("user-agent", PhantomUserAgent);
                    webDriver = new PhantomJSDriver(options);
                    break;
                case DriverNames.Safari:
                    webDriver = new OpenQA.Selenium.Safari.SafariDriver();
                    break;
                default:
                    throw new NotImplementedException();
            }
            
            return webDriver;
        }

        #endregion

        #region Public

        public object ExecuteJavaScript(string script)
        {
            var executor = (IJavaScriptExecutor)WebDriver;
            return executor.ExecuteScript(script);
        }

        public bool CanTakeScreenshot
        {
            get { return WebDriver is ITakesScreenshot; }
        }

        public Screenshot TakeScreenshot()
        {
            return ((ITakesScreenshot) WebDriver).GetScreenshot();
        }    

        #endregion

        #region Static

        /// <summary>
        /// Context
        /// </summary>
        /// <returns>WebDriverContext singleton instance</returns>
        public static WebDriverContext Current { get; set; }

        public static bool HasInstance
        {
            get { return Current != null; }
        }

        /// <summary>
        /// Get "Default" user from config
        /// </summary>
        /// <returns></returns>
        public static UserElement GetDefaultUser()
        {
            var env = Current._environmentsConfiguration.GetTargetEnvironment();
            return env.Users["Default"];
        }

        /// <summary>
        /// Get user by name from config
        /// </summary>
        /// <returns></returns>
        public static UserElement GetUser(string user)
        {
            var env = Current._environmentsConfiguration.GetTargetEnvironment();
            return env.Users[user];
        }

        #endregion
    }
}
