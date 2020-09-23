using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    internal static class JSJsonParser
    {
        private static void Next(this JsonTextReader reader)
        {
            if (!reader.Read())
                throw new InvalidOperationException("Invalid or empty json");
        }

        public static JSValue Parse(string str, JsonParserReceiver r)
        {
            JsonTextReader reader = new JsonTextReader(new StringReader(str));
            reader.Next();
            return Read(reader, r);
        }
        static JSValue ReadObject(JsonTextReader reader, JsonParserReceiver r)
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

                        reader.Next();

                        var value = Read(reader, r);

                        value = r?.Invoke((name, value)) ?? value;
                        if (value.IsUndefined)
                        {
                            continue;
                        }
                        j[name] = value;
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid token {reader.Value} at {reader.LineNumber},{reader.LinePosition}");
                }
            }
            return j;
        }
        static JSValue ReadArray(JsonTextReader reader, JsonParserReceiver r)
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
            JsonParserReceiver r)
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
