using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using NUnit.Framework;
using QA.TestAutomation.Framework.Configuration;
using QA.TestAutomation.Framework.PageObjects;
using QA.TestAutomation.Framework.WebDriver;

namespace QA.TestAutomation.Framework.Tests
{
    /// <summary>
    /// Base test class. Hadles Web Driver context.
    /// </summary>
    public class WebDriverTestsBase : TestsBase
    {
        public const string DateFormat = "yyyy-MM-dd_HH.mm";

        #region Vars

        protected WebDriverContext WebDriverContext { get; set; }

        #endregion

        #region Setup & TearDown

        public override void SetUp()
        {
            base.SetUp();
            if (WebDriverContext.HasInstance)
            {
                WebDriverContext.Current.Dispose();    
            }

            WebDriverContext.ReadConfig();
            WebDriverContext = WebDriverContext.Current;
        }
        
        /// <summary>
        /// Dispose WebDriverContext instance
        /// </summary>
        [TearDown]
        public virtual void TearDown()
        {
            if (WebDriverContext.HasInstance)
            {
                var instance = WebDriverContext.Current;
                
                lock (instance)
                {
                    try
                    {
                        if (instance.CanTakeScreenshot &&
                            (TestContext.CurrentContext.Result.State == TestState.Failure ||
                             TestContext.CurrentContext.Result.State == TestState.Error))
                        {
                            TakeScreenshot(instance);
                        }
                    }
                    finally
                    {
                        instance.Dispose();
                    }
                }
            }
        }

        #endregion

        #region Private

        protected void TakeScreenshot(WebDriverContext instance)
        {
            var cfg = DriverConfiguration.GetConfiguration();
            if (!cfg.TakeScreenshots)
            {
                return;
            }

            var testName = TestContext.CurrentContext.Test.Name;
            var invalids = Path.GetInvalidFileNameChars();
            testName = invalids.Aggregate(testName, (current, inv) => current.Replace(inv.ToString(), string.Empty));

            if (testName.Length > 200)
            {
                testName = testName.Substring(0, 200) + "...";
            }

            var path = Path.Combine(
                cfg.ScreenshotDir,
                testName);

            var di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                Directory.CreateDirectory(path);
            }

            path = Path.Combine(
                di.FullName,
                StartTime.ToString(DateFormat) + ".png");

            instance.TakeScreenshot().SaveAsFile(
                path,
                ImageFormat.Png);
        }

        #endregion

        #region Public

        #endregion
    }

    /// <summary>
    /// Base test class. Prepares Page Object instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class WebDriverTestsBase<T> : WebDriverTestsBase
        where T:Page
    {
        #region Vars

        public virtual bool UseNavigate { get { return true; } }

        /// <summary>
        /// Page object instance
        /// </summary>
        protected T Page { get; set; }

        /// <summary>
        /// Relative Url to target Page Object
        /// </summary>
        protected virtual string Url
        {
            get { return string.Empty; }
        }

        #endregion

        public override void SetUp()
        {
            base.SetUp();
        }
    }
}
