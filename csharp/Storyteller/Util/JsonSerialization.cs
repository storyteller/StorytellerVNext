﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Baseline;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Storyteller.Messages;
using Storyteller.Model;

namespace Storyteller.Util
{
    public static class JsonSerialization
    {
        // var converter = new StringEnumConverter();

        private static readonly LightweightCache<string, Type> _messageTypes = new LightweightCache<string, Type>(); 

        static JsonSerialization()
        {
            var types = typeof (JsonSerialization).GetTypeInfo().Assembly.GetExportedTypes()
                .Where(x => x.IsConcreteWithDefaultCtor() && x.IsConcreteTypeOf<ClientMessage>());

            types.Each(x =>
            {
                var message = Activator.CreateInstance(x).As<ClientMessage>();
                _messageTypes[message.Type] = x;
            });
        }

        public static Type TypeForJson(string json)
        {
            var token = JToken.Parse(json);
            var type = token.Value<string>("type");

            return (!type.IsEmpty() && _messageTypes.Has(type))
                ?_messageTypes[type]
                : null;
        }

        public static string ToJson(object o, bool indentedFormatting = false)
        {
            var serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.All
            };

            serializer.Converters.Add(new StringEnumConverter());

            if (indentedFormatting)
            {
                serializer.Formatting = Formatting.Indented;
            }

            var writer = new StringWriter();
            serializer.Serialize(writer, o);

            return writer.ToString();
        }

        public static string ToCleanJson(object o)
        {
            var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.None,  };
            serializer.Converters.Add(new StringEnumConverter());


            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8))
                {
                    serializer.Serialize(writer, o);

                    writer.Flush();

                    stream.Position = 0;

                    return stream.ReadAllText();
                }


            }
        }

        public static Task WriteCleanJson(Stream response, object o)
        {
            var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.None, };
            serializer.Converters.Add(new StringEnumConverter());

            var raw = new MemoryStream();

            using (var writer = new StreamWriter(raw, Encoding.UTF8))
            {
                serializer.Serialize(writer, o);

                writer.Flush();

                raw.Position = 3;

                return raw.CopyToAsync(response);


            }
        }

        public static string ToIndentedJson(object o)
        {
            var serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.None, Formatting = Formatting.Indented};
            serializer.Converters.Add(new StringEnumConverter());

            var writer = new StringWriter();
            serializer.Serialize(writer, o);

            return writer.ToString();
        }


        public static string FormatJson(this string json)
        {
            return JToken.Parse(json).ToString(Formatting.Indented);
        }

        public static T Deserialize<T>(string json)
        {
            var serializer = buildSerializer();

            return serializer.Deserialize<T>(new JsonTextReader(new StringReader(json)));
        }

        private static JsonSerializer buildSerializer()
        {
            var serializer = new JsonSerializer
                {TypeNameHandling = TypeNameHandling.Auto, SerializationBinder = new ForwardingSerializationBinder()};
            serializer.Converters.Add(new StringEnumConverter());
            return serializer;
        }

        public static object DeserializeMessage(string json)
        {
            var serializer = buildSerializer();

            var jsonTextReader = new JsonTextReader(new StringReader(json));

            var messageType = TypeForJson(json);

            return messageType != null 
                ? serializer.Deserialize(jsonTextReader, messageType) 
                : serializer.Deserialize(jsonTextReader);
        }
    }
    
    public class ForwardingSerializationBinder : DefaultSerializationBinder
    {
        private readonly Dictionary<string, Type> _aliases = new Dictionary<string, Type>
        {
            {"System.Collections.Generic.List`1[[StoryTeller.Model.Node, Storyteller]], System.Private.CoreLib", typeof(List<Node>)},
            {"System.Collections.Generic.List`1[[StoryTeller.Model.Node, Storyteller]]", typeof(List<Node>)},
            {"System.Collections.Generic.Dictionary`2[[System.String, System.Private.CoreLib],[System.String, System.Private.CoreLib]]", typeof(Dictionary<string, string>)},
            {"System.Collections.Generic.Dictionary`2[[System.String, System.Private.CoreLib],[System.Boolean, System.Private.CoreLib]]", typeof(Dictionary<string, bool>)}
        };
        
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (_aliases.TryGetValue(typeName, out var type)) return type;
            
            return base.BindToType(assemblyName, typeName);
        }
    }
}
