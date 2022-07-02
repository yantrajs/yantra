using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace YantraJS.Core
{
    internal class JSJsonParser: System.Text.Json.Serialization.JsonConverter<JSValue>
    {

        private JsonParserReceiver r;

        public JSJsonParser(JsonParserReceiver r)
        {
            this.r = r;
        }

        public static JSValue Parse(string str, JsonParserReceiver r)
        {
            return System.Text.Json.JsonSerializer.Deserialize<JSValue>(str, new System.Text.Json.JsonSerializerOptions { 
                Converters =
                {
                    new JSJsonParser(r)
                }
            });
        }
        //static JSValue ReadObject(JsonTextReader reader, JsonParserReceiver r)
        //{
        //    var j = new JSObject();
        //    // read properties...
        //    while(reader.Read())
        //    {
        //        switch(reader.TokenType)
        //        {
        //            case JsonToken.EndObject:
        //                return j;
        //            case JsonToken.PropertyName:
        //                var name = (string)reader.Value;

        //                reader.Next();

        //                var value = Read(reader, r);

        //                value = r?.Invoke((name, value)) ?? value;
        //                if (value.IsUndefined)
        //                {
        //                    continue;
        //                }
        //                j[name] = value;
        //                break;
        //            default:
        //                throw new InvalidOperationException($"Invalid token {reader.Value} at {reader.LineNumber},{reader.LinePosition}");
        //        }
        //    }
        //    return j;
        // }
        //static JSValue ReadArray(JsonTextReader reader, JsonParserReceiver r)
        //{
        //    var j = new JSArray();
        //    // read properties...
        //    while (reader.Read() && reader.TokenType != JsonToken.EndArray)
        //    {
        //        j.Add(Read(reader, r));
        //    }
        //    return j;
        //}
        //static JSValue Read(
        //    JsonTextReader reader,
        //    JsonParserReceiver r)
        //{
        //    switch (reader.TokenType)
        //    {
        //        case JsonToken.None:
        //            break;
        //        case JsonToken.StartObject:
        //            return ReadObject(reader, r);
        //        case JsonToken.StartArray:
        //            return ReadArray(reader, r);
        //        case JsonToken.StartConstructor:
        //            break;
        //        case JsonToken.PropertyName:
        //            break;
        //        case JsonToken.Comment:
        //            break;
        //        case JsonToken.Raw:
        //            break;
        //        case JsonToken.Integer:
        //            return new JSNumber((long)reader.Value);
        //        case JsonToken.Float:
        //            return new JSNumber((double)reader.Value);
        //        case JsonToken.String:
        //            return new JSString((string)reader.Value);
        //        case JsonToken.Boolean:
        //            return ((bool)reader.Value) ? JSBoolean.True : JSBoolean.False;
        //        case JsonToken.Null:
        //            return JSNull.Value;
        //        case JsonToken.Undefined:
        //            return JSUndefined.Value;
        //        case JsonToken.EndObject:
        //            break;
        //        case JsonToken.EndArray:
        //            break;
        //        case JsonToken.EndConstructor:
        //            break;
        //        case JsonToken.Date:
        //            break;
        //        case JsonToken.Bytes:
        //            break;
        //    }
        //    throw new NotSupportedException($"Unexpected token {reader.Value}({reader.TokenType}) at {reader.LineNumber},{reader.LinePosition}");
        //}

        public override JSValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.None:
                    throw new NotSupportedException($"Unable to read JSON Data");
                case JsonTokenType.StartObject:
                    return ReadObject(ref reader, options);
                case JsonTokenType.StartArray:
                    return ReadArray(ref reader, options);
                case JsonTokenType.String:
                    return new JSString(reader.GetString());
                case JsonTokenType.Number:
                    return new JSNumber(reader.GetDouble());
                case JsonTokenType.True:
                    return JSBoolean.True;
                case JsonTokenType.False:
                    return JSBoolean.False;
                case JsonTokenType.Null:
                    return JSNull.Value;
            }
            throw new NotSupportedException($"Unexpected JSON {reader.TokenType} at {reader.TokenStartIndex}");
        }

        private JSValue ReadArray(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var j = new JSArray();
            // read properties...
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                j.Add(Read(ref reader, typeof(JSValue), options));
            }
            return j;
        }

        private JSValue ReadObject(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            var j = new JSObject();
            // read properties...
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.EndObject:
                        return j;
                    case JsonTokenType.PropertyName:
                        var name = reader.GetString();
                        if (!reader.Read())
                            throw new InvalidOperationException($"Unable to read JSON");

                        var value = Read(ref reader, typeof(JSValue), options);

                        value = r?.Invoke((name, value)) ?? value;
                        if (value.IsUndefined)
                        {
                            continue;
                        }
                        j[name] = value;
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid token {reader.TokenType} at {reader.TokenStartIndex}");
                }
            }
            return j;
        }

        public override void Write(Utf8JsonWriter writer, JSValue value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
