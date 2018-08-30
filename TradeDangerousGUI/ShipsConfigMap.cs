using System.Configuration;

namespace TDHelper
{
    public class ShipConfig : ConfigurationElement
    {
        [ConfigurationProperty("padsizes", IsRequired = true)]
        public string PadSizes
        {
            get { return (string)base["padsizes"]; }
            set { base["padsizes"] = value; }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("shiptype", IsKey = true, IsRequired = true)]
        public string ShipType
        {
            get { return (string)base["shiptype"]; }
            set { base["shiptype"] = value; }
        }
    }

    public class ShipCollection : ConfigurationElementCollection
    {
        public ShipCollection()
        {
            this.AddElementName = "ship";
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as ShipConfig).ShipType;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ShipConfig();
        }

        public new ShipConfig this[string key]
        {
            get { return base.BaseGet(key) as ShipConfig; }
        }

        public ShipConfig this[int ind]
        {
            get { return base.BaseGet(ind) as ShipConfig; }
        }
    }

    public class ShipSection : ConfigurationSection
    {
        public const string sectionName = "ships";

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public ShipCollection ShipSettings
        {
            get
            {
                return this[""] as ShipCollection;
            }
        }

        public static ShipSection GetSection()
        {
            return (ShipSection)ConfigurationManager.GetSection(sectionName);
        }
    }
    /*
    public sealed class ShipsConfigMapConfigElement : ConfigurationElement
    {
        public sealed class ShipsConfigMapSection : ConfigurationSection
        {
            public static ShipsConfigMapSection Config { get; } = ConfigurationManager.GetSection("ships") as ShipsConfigMapSection;

            [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
            private ShipsConfigMapConfigElements Settings
            {
                get { return (ShipsConfigMapConfigElements)this[string.Empty]; }
                set { this[string.Empty] = value; }
            }

            public IEnumerable<ShipsConfigMapConfigElement> SettingsList
            {
                get { return Settings.Cast<ShipsConfigMapConfigElement>(); }
            }
        }

        [ConfigurationProperty("padsizes", IsRequired = true)]
        public string PadSizes
        {
            get { return (string)base["padsizes"]; }
            set { base["padsizes"] = value; }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("shiptype", IsKey = true, IsRequired = true)]
        public string ShipType
        {
            get { return (string)base["shiptype"]; }
            set { base["shiptype"] = value; }
        }
    }

    public sealed class ShipsConfigMapConfigElements : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ShipsConfigMapConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ShipsConfigMapConfigElement)element).ShipType;
        }
    }
    */
}