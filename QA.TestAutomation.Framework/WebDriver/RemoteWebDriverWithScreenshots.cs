using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace QA.TestAutomation.Framework.WebDriver
{
    public class RemoteWebDriverWithScreenshots : RemoteWebDriver, ITakesScreenshot
    {
        public RemoteWebDriverWithScreenshots(ICommandExecutor commandExecutor, ICapabilities desiredCapabilities) : base(commandExecutor, desiredCapabilities)
        {
        }

        public RemoteWebDriverWithScreenshots(ICapabilities desiredCapabilities) : base(desiredCapabilities)
        {
        }

        public RemoteWebDriverWithScreenshots(Uri remoteAddress, ICapabilities desiredCapabilities) : base(remoteAddress, desiredCapabilities)
        {
        }

        public RemoteWebDriverWithScreenshots(Uri remoteAddress, ICapabilities desiredCapabilities, TimeSpan commandTimeout) : base(remoteAddress, desiredCapabilities, commandTimeout)
        {
        }

        public new Screenshot GetScreenshot()
        {
            var res = Execute(DriverCommand.Screenshot, new Dictionary<string, object>());
            if (res.Status != WebDriverResult.Success)
            {
                throw new InvalidOperationException(res.Status.ToString());
            }

            return new Screenshot(res.Value.ToString());
        }
    }
}
