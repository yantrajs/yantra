using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test262Runner
{
    class Config
    {

        public string Description { get; set; }

        public Response Negative { get; set; }

        public string[] Flags { get; set; }

        public string[] Features { get; set; }
        public bool Ignore { 
            get {
                if (Flags != null)
                {
                    if (Flags.Contains("noStrict"))
                        return true;
                }
                if (Features != null)
                {
                    if (Features.Contains("async-iteration"))
                        return true;
                }
                return false;
            }
        }

        public class Response
        {
            public string Phase { get; set; }
            public string Type { get; set; }
        }

        public static Config Parse(string  code)
        {
            var start = code.IndexOf("/*---");
            code = code.Substring(start + 2);
            start = code.IndexOf("---*/");
            code = code.Substring(0, start);

            YamlDotNet.Serialization.DeserializerBuilder db = new YamlDotNet.Serialization.DeserializerBuilder()
                .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties();

            return db.Build().Deserialize<Config>(code);
        }


    }
}
