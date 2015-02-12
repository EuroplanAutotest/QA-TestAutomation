using System;
using System.Configuration;

namespace QA.TestAutomation.Framework.Configuration
{
    public class EnvironmentsConfiguration : ConfigurationSection
    {
        public enum EnvironmentNames
        {
            Local,
            Dev,
            Qa,
            Demo,
            Production,
            Special
        }
        
        [ConfigurationProperty("environments")]
        public EnvironmentCollection Environments
        {
            get { return ((EnvironmentCollection)(base["environments"])); }
        }

        [ConfigurationProperty("targetEnvironment", IsKey = true, IsRequired = true)]
        public string TargetEnvironment
        {
            get { return ((string)(base["targetEnvironment"])); }
            set { base["targetEnvironment"] = value; }
        }

        public static EnvironmentsConfiguration GetConfiguration()
        {
            var cfg = (EnvironmentsConfiguration)ConfigurationManager.GetSection("environmentsConfiguration");
            if (cfg == null)
            {
                throw new ConfigurationErrorsException("environmentsConfiguration section is required");
            }
            return cfg;
        }
        // HACK
        public static string CurrentApplication { get; set; }

        public EnvironmentElement GetTargetEnvironment()
        {
            return Environments[TargetEnvironment];
        }

        public string CurrentEnvironmentBaseUrl
        {
            get
            {
                try
                {
                    return Environments[TargetEnvironment].GetBaseUrl(CurrentApplication);
                }
                catch (NullReferenceException e)
                {
                    throw new ConfigurationErrorsException(
                        string.Format("Target environment {0} is not found in config",
                        TargetEnvironment), e);
                }
            }
        }
    }

    public class EnvironmentElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return ((string)(base["name"])); }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("app")]
        public string App
        {
            get { return ((string)(base["app"])); }
            set { base["app"] = value; }
        }

        [ConfigurationProperty("users")]
        public UserCollection Users
        {
            get
            {
                return ((UserCollection)(base["users"]));
            }
        }

        [ConfigurationProperty("apps")]
        public AppsCollection Apps
        {
            get
            {
                return ((AppsCollection)(base["apps"]));
            }
        }

        internal string GetBaseUrl(string targetApp)
        {
            string cfgName = String.IsNullOrEmpty(targetApp)
                ? String.IsNullOrEmpty(App) ? Apps.Default : App
                : targetApp;
                             
            string baseUrl = null;
            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (AppElement app in Apps)
            // ReSharper restore LoopCanBeConvertedToQuery
            {
                if (String.IsNullOrEmpty(cfgName) || String.Equals(app.Name, cfgName, StringComparison.CurrentCultureIgnoreCase))
                {
                    baseUrl = app.Url;
                    break;
                }
            }

            if (baseUrl == null)
            {
                throw new ConfigurationErrorsException(String.Format("Not found app for '{0}'. Environment must contain at least one app. " , Name));
            }

            return baseUrl;
        }
    }

    [ConfigurationCollection(typeof(EnvironmentElement), AddItemName = "environment")]
    public class EnvironmentCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new EnvironmentElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EnvironmentElement)(element)).Name;
        }

        public new EnvironmentElement this[string index]
        {
            get { return (EnvironmentElement) BaseGet(index); }
        }

    }
}