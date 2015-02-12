using System.Configuration;

namespace QA.TestAutomation.Framework.Configuration
{
    [ConfigurationCollection(typeof(AppElement), AddItemName = "app")]
    public class AppsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AppElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AppElement)(element)).Name;            
        }

        public new AppElement this[string index]
        {
            get { return (AppElement)BaseGet(index); }
        }

        [ConfigurationProperty("default")]
        public string Default
        {
            get { return ((string)(base["default"])); }
            set { base["default"] = value; }
        }
    }

    public class AppElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return ((string)(base["name"])); }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return ((string)(base["url"])); }
            set { base["url"] = value; }
        }

    }
}
