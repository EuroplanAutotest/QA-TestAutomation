using System;
using System.Configuration;

namespace QA.TestAutomation.Framework.Configuration
{
    public class TradeHoursConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("startTime", IsKey = true, IsRequired = false)]
        public DateTime StartTime
        {
            get { return (DateTime)base["startTime"]; }
            set { base["startTime"] = value; }
        }

        [ConfigurationProperty("endTime", IsKey = true, IsRequired = false)]
        public DateTime EndTime
        {
            get { return (DateTime)base["endTime"]; }
            set { base["endTime"] = value; }
        }

        [ConfigurationProperty("workingDays", IsKey = true, IsRequired = false)]
        public string WorkingDays
        {
            get { return (string)base["workingDays"]; }
            set { base["workingDays"] = value; }
        }

        public static TradeHoursConfiguration GetConfiguration()
        {
            var cfg = (TradeHoursConfiguration)ConfigurationManager.GetSection("tradeHours");
            if (cfg == null)
            {
                throw new ConfigurationErrorsException("tradeHours section is empty");
            }
            return cfg;
        }

       

       


    }
}
