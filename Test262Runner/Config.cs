using System;
using System.Collections.Generic;
using System.Text;

namespace Test262Runner
{
    class Config
    {

        public string Description { get; set; }

        public Response Negative { get; set; }

        public string[] Flags { get; set; }

        public string[] Features { get; set; }

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
