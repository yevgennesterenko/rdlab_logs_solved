using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace MessagePersister.Implementation
{ 
    // Define the "writer" element    
public class WriterConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("name", DefaultValue = "Arial", IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String Name
        {
            get
            {
                return (String)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("messageFolder", IsRequired = true)]
        public string LogFolder
        {
            get { return (string)this["messageFolder"]; }
            set { this["messageFolder"] = value; }
        }

        [ConfigurationProperty("messageFileName", DefaultValue = "Message", IsRequired = false)]
        public string LogFileName
        {
            get { return (string) this["messageFileName"]; }
            set { this["messageFileName"] = value; }
        }

        [ConfigurationProperty("dateTimeFormat", DefaultValue = "yyyyMMdd HHmmss fff", IsRequired = false)]
        public string DateTimeFormat
        {
            get { return (string)this["dateTimeFormat"]; }
            set { this["dateTimeFormat"] = value; }
        }

        [ConfigurationProperty("extension", DefaultValue = ".log", IsRequired = false)]
        public string Extension
        {
            get { return (string)this["extension"]; }
            set { this["extension"] = value; }
        }
    }

}
