using System.Configuration;

namespace QA.TestAutomation.Framework.Configuration
{
    // ReSharper disable InconsistentNaming
    public enum DriverNames
    {
        IE,
        Firefox,
        Chrome,
        Safari,
        PhantomJS,
        Andriod
    }
    // ReSharper restore InconsistentNaming

    public class DriverConfiguration : ConfigurationSection
    {
        public bool SizeSet
        {
            get { return Width > 0 && Height > 0; }
        }

        [ConfigurationProperty("timeout", IsKey = true, DefaultValue = 10, IsRequired = false)]
        public int Timeout
        {
            get { return (int)base["timeout"]; }
            set { base["timeout"] = value; }
        }

        [ConfigurationProperty("implicitTimeout", IsKey = true, DefaultValue = 30, IsRequired = false)]
        public int ImplicitTimeout
        {
            get { return (int)base["implicitTimeout"]; }
            set { base["implicitTimeout"] = value; }
        }

        [ConfigurationProperty("pageLoadTimeout", IsKey = true, DefaultValue = 30, IsRequired = false)]
        public int PageLoadTimeout
        {
            get { return (int)base["pageLoadTimeout"]; }
            set { base["pageLoadTimeout"] = value; }
        }

        [ConfigurationProperty("width", IsKey = true, IsRequired = false)]
        public int Width
        {
            get { return (int)base["width"]; }
            set { base["width"] = value; }
        }

        [ConfigurationProperty("height", IsKey = true, IsRequired = false)]
        public int Height
        {
            get { return (int)base["height"]; }
            set { base["height"] = value; }
        }

        [ConfigurationProperty("remoteUrl", IsKey = true, IsRequired = false)]
        public string RemoteUrl
        {
            get { return (string)base["remoteUrl"]; }
            set { base["remoteUrl"] = value; }
        }

        [ConfigurationProperty("takeScreenshots", IsKey = true, IsRequired = false)]
        public bool TakeScreenshots
        {
            get { return (bool)base["takeScreenshots"]; }
            set { base["takeScreenshots"] = value; }
        }

        [ConfigurationProperty("screenshotDir", IsKey = true, IsRequired = false)]
        public string ScreenshotDir
        {
            get { return (string)base["screenshotDir"]; }
            set { base["screenshotDir"] = value; }            
        }

        [ConfigurationProperty("downloadDir", IsKey = true, IsRequired = false)]
        public string DownloadDir
        {
            get { return (string)base["downloadDir"]; }
            set { base["downloadDir"] = value; }
        }

        [ConfigurationProperty("targetDriver", IsKey = true, IsRequired = true)]
        public DriverNames TargetDriver
        {
            get { return ((DriverNames)(base["targetDriver"])); }
            set { base["targetDriver"] = value; }
        }

        [ConfigurationProperty("isRemote", IsKey = true, IsRequired = true)]
        public bool IsRemote
        {
            get { return ((bool)(base["isRemote"])); }
            set { base["isRemote"] = value; }
        }

        public static DriverConfiguration GetConfiguration()
        {
            var cfg = (DriverConfiguration)ConfigurationManager.GetSection("driverConfiguration");
            if (cfg == null)
            {
                throw new ConfigurationErrorsException("driverConfiguration section is required");
            }
            return cfg;
        }
    }
}