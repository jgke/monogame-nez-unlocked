using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace GameImpl {
    class ShouldSerializeContractResolver : DefaultContractResolver {
        static ShouldSerializeContractResolver _instance;
        public static ShouldSerializeContractResolver Instance {
            get {
                if (_instance == null) {
                    _instance = new ShouldSerializeContractResolver();
                }
                return _instance;
            }
        }

        protected override IList<Newtonsoft.Json.Serialization.JsonProperty> CreateProperties(Type type, Newtonsoft.Json.MemberSerialization memberSerialization) {
            var props = type
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(p => CreateProperty(p, memberSerialization))
                .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Select(f => CreateProperty(f, memberSerialization)))
                .ToList();
            props.ForEach(p => { p.Writable = true; p.Readable = true; });
            return props;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
            var property = base.CreateProperty(member, memberSerialization);
            property.ShouldSerialize = _ => ShouldSerialize(member);
            return property;
        }

        internal static bool ShouldSerialize(MemberInfo memberInfo) {
            var propertyInfo = memberInfo as PropertyInfo;
            return propertyInfo == null;
        }
    }

    public static class Logger {
        public static string LogString(Object obj) {
            var options = new JsonSerializerSettings {
                ContractResolver = ShouldSerializeContractResolver.Instance
            };
            return JsonConvert.SerializeObject(obj, Formatting.Indented, options);
        }

        public static void Log(Object obj) {
            var jsonString = LogString(obj);

            Debug.WriteLine(jsonString);
            System.Console.WriteLine(jsonString);
        }
    }
}
