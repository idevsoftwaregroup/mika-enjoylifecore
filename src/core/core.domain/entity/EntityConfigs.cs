using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.domain.entity
{
    public static class EntityConfigs
    {
        
        // Add other static properties for your constants

        public static void LoadConfigs(string configFile)
        {
            if (File.Exists(configFile))
            {
                var json = File.ReadAllText(configFile);
                var configs = JsonConvert.DeserializeObject<EntityConfigsData>(json);

                
                // Set other static properties here
            }
            else
            {
                throw new FileNotFoundException("entityconfigs.json not found.");
            }
        }

         class EntityConfigsData
        {
            public int ComplexTitleMaxLength { get; set; }
        }
    }
}

    
