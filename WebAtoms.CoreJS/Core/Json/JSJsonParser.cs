using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    internal static class JSJsonParser
    {
        public static JSValue Parse(string str, Func<JSValue, string, JSValue> r)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(str));
            return Read(reader, r);
        }
        static JSValue ReadObject(JsonTextReader reader, Func<JSValue, string, JSValue> r)
        {
            var j = new JSObject();
            // read properties...
            while(reader.Read())
            {
                switch(reader.TokenType)
                {
                    case JsonToken.EndObject:
                        return j;
                    case JsonToken.PropertyName:
                        var name = (string)reader.Value;
                        var value = Read(reader, r);
                        j[name] = value;
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid token {reader.Value} at {reader.LineNumber},{reader.LinePosition}");
                }
            }
            return j;
        }
        static JSValue ReadArray(JsonTextReader reader, Func<JSValue, string, JSValue> r)
        {
            var j = new JSArray();
            // read properties...
            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
            {
                j.Add(Read(reader, r));
            }
            return j;
        }
        static JSValue Read(
            JsonTextReader reader, 
            Func<JSValue, string, JSValue> r)
        {
            switch (reader.TokenType)
            {
                case JsonToken.None:
                    break;
                case JsonToken.StartObject:
                    return ReadObject(reader, r);
                case JsonToken.StartArray:
                    return ReadArray(reader, r);
                case JsonToken.StartConstructor:
                    break;
                case JsonToken.PropertyName:
                    break;
                case JsonToken.Comment:
                    break;
                case JsonToken.Raw:
                    break;
                case JsonToken.Integer:
                    return new JSNumber((long)reader.Value);
                case JsonToken.Float:
                    return new JSNumber((double)reader.Value);
                case JsonToken.String:
                    return new JSString((string)reader.Value);
                case JsonToken.Boolean:
                    return ((bool)reader.Value) ? JSBoolean.True : JSBoolean.False;
                case JsonToken.Null:
                    return JSNull.Value;
                case JsonToken.Undefined:
                    return JSUndefined.Value;
                case JsonToken.EndObject:
                    break;
                case JsonToken.EndArray:
                    break;
                case JsonToken.EndConstructor:
                    break;
                case JsonToken.Date:
                    break;
                case JsonToken.Bytes:
                    break;
            }
            throw new NotSupportedException($"Unexpected token {reader.Value}({reader.TokenType}) at {reader.LineNumber},{reader.LinePosition}");
        }



    }
}
