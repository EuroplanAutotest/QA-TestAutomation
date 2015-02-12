using System.Configuration;

namespace QA.TestAutomation.Framework.Configuration
{
    [ConfigurationCollection(typeof(UserElement), AddItemName = "user")]
    public class UserCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new UserElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((UserElement)(element)).Name;
        }

        public new UserElement this[string index]
        {
            get { return (UserElement)BaseGet(index); }
        }
    }

    public class UserElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return ((string)(base["name"])); }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("login", IsRequired = true)]
        public string Login
        {
            get { return ((string)(base["login"])); }
            set { base["login"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get { return ((string)(base["password"])); }
            set { base["password"] = value; }
        }
    }
}
