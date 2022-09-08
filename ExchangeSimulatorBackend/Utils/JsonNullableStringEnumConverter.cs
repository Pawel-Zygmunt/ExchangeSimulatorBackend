

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeSimulatorBackend.Utils
{
    public class JsonNullableStringEnumConverter : JsonConverterFactory
    {
        readonly JsonStringEnumConverter stringEnumConverter;

        public JsonNullableStringEnumConverter(JsonNamingPolicy? namingPolicy = null, bool allowIntegerValues = true)
        {
            stringEnumConverter = new(namingPolicy, allowIntegerValues);
        }

        public override bool CanConvert(Type typeToConvert)
            => Nullable.GetUnderlyingType(typeToConvert)?.IsEnum == true;

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var type = Nullable.GetUnderlyingType(typeToConvert)!;
            return (JsonConverter?)Activator.CreateInstance(typeof(ValueConverter<>).MakeGenericType(type), stringEnumConverter.CreateConverter(type, options));
        }

        class ValueConverter<T> : JsonConverter<T?> where T : struct, Enum
        {
            readonly JsonConverter<T> converter;
            private readonly ConcurrentDictionary<ulong, JsonEncodedText> _nameCacheForWriting;
            private readonly ConcurrentDictionary<string, T>? _nameCacheForReading;

            private static readonly TypeCode s_enumTypeCode = Type.GetTypeCode(typeof(T));
            private const string ValueSeparator = ", ";
            private static readonly bool s_isSignedEnum = ((int)s_enumTypeCode % 2) == 1;
            private const int NameCacheSizeSoftLimit = 64;

            public ValueConverter(JsonConverter<T> converter)
            {
                this.converter = converter;
                _nameCacheForWriting = new ConcurrentDictionary<ulong, JsonEncodedText>();

                string[] names = Enum.GetNames<T>();
                T[] values = Enum.GetValues<T>();


                for (int i = 0; i < names.Length; i++)
                {
                    T value = values[i];
                    ulong key = ConvertToUInt64(value);
                    string name = names[i];

                    string jsonName = FormatJsonName(name, null);
                    _nameCacheForWriting.TryAdd(key, JsonEncodedText.Encode(jsonName, null));
                    _nameCacheForReading?.TryAdd(jsonName, value);
                }
            }

            public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                {
                    reader.Read();
                    return null;
                }
                return converter.Read(ref reader, typeof(T), options);
            }

            public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
            {
                ulong key = ConvertToUInt64(value);

                if (_nameCacheForWriting.TryGetValue(key, out JsonEncodedText formatted))
                {
                    writer.WriteStringValue(formatted);
                    return;
                }

                string original = value.ToString();

                if (IsValidIdentifier(original))
                {
                    // We are dealing with a combination of flag constants since
                    // all constant values were cached during warm-up.
                    Debug.Assert(original.Contains(ValueSeparator));

                    original = FormatJsonName(original, null);

                    if (_nameCacheForWriting.Count < NameCacheSizeSoftLimit)
                    {
                        formatted = JsonEncodedText.Encode(original, options.Encoder);
                        writer.WriteStringValue(formatted);
                        _nameCacheForWriting.TryAdd(key, formatted);
                    }
                    else
                    {
                        // We also do not create a JsonEncodedText instance here because passing the string
                        // directly to the writer is cheaper than creating one and not caching it for reuse.
                        writer.WriteStringValue(original);
                    }

                    return;
                }

                if (value == null)
                    writer.WriteNullValue();
                else
                    converter.Write(writer, value.Value, options);
            }

            //public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            //{

            //}

            //internal override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options, bool isWritingExtensionDataProperty)
            //{
            //    ulong key = ConvertToUInt64(value);

            //    if (options.DictionaryKeyPolicy == null && _nameCacheForWriting.TryGetValue(key, out JsonEncodedText formatted))
            //    {
            //        writer.WritePropertyName(formatted);
            //        return;
            //    }

            //    string original = value.ToString();

            //    if (IsValidIdentifier(original))
            //    {
            //        if (options.DictionaryKeyPolicy != null)
            //        {
            //            original = FormatJsonName(original, options.DictionaryKeyPolicy);
            //            writer.WritePropertyName(original);
            //            return;
            //        }

            //        original = FormatJsonName(original, _namingPolicy);

            //        if (_nameCacheForWriting.Count < NameCacheSizeSoftLimit)
            //        {
            //            formatted = JsonEncodedText.Encode(original, options.Encoder);
            //            writer.WritePropertyName(formatted);
            //            _nameCacheForWriting.TryAdd(key, formatted);
            //        }
            //        else
            //        {
            //            // We also do not create a JsonEncodedText instance here because passing the string
            //            // directly to the writer is cheaper than creating one and not caching it for reuse.
            //            writer.WritePropertyName(original);
            //        }

            //        return;
            //    }

            //    switch (s_enumTypeCode)
            //    {
            //        case TypeCode.Int32:
            //            writer.WritePropertyName(Unsafe.As<T, int>(ref value));
            //            break;
            //        case TypeCode.UInt32:
            //            writer.WritePropertyName(Unsafe.As<T, uint>(ref value));
            //            break;
            //        case TypeCode.UInt64:
            //            writer.WritePropertyName(Unsafe.As<T, ulong>(ref value));
            //            break;
            //        case TypeCode.Int64:
            //            writer.WritePropertyName(Unsafe.As<T, long>(ref value));
            //            break;
            //        case TypeCode.Int16:
            //            writer.WritePropertyName(Unsafe.As<T, short>(ref value));
            //            break;
            //        case TypeCode.UInt16:
            //            writer.WritePropertyName(Unsafe.As<T, ushort>(ref value));
            //            break;
            //        case TypeCode.Byte:
            //            writer.WritePropertyName(Unsafe.As<T, byte>(ref value));
            //            break;
            //        case TypeCode.SByte:
            //            writer.WritePropertyName(Unsafe.As<T, sbyte>(ref value));
            //            break;
            //        default:
            //            ThrowHelper.ThrowJsonException();
            //            break;
            //    }
            //}


            private static bool TryParseEnumCore(string? enumString, JsonSerializerOptions options, out T value)
            {
                // Try parsing case sensitive first
                bool success = Enum.TryParse(enumString, out T result) || Enum.TryParse(enumString, ignoreCase: true, out result);
                value = result;
                return success;
            }

            private static ulong ConvertToUInt64(object value)
            {
                Debug.Assert(value is T);
                ulong result = s_enumTypeCode switch
                {
                    TypeCode.Int32 => (ulong)(int)value,
                    TypeCode.UInt32 => (uint)value,
                    TypeCode.UInt64 => (ulong)value,
                    TypeCode.Int64 => (ulong)(long)value,
                    TypeCode.SByte => (ulong)(sbyte)value,
                    TypeCode.Byte => (byte)value,
                    TypeCode.Int16 => (ulong)(short)value,
                    TypeCode.UInt16 => (ushort)value,
                    _ => throw new InvalidOperationException(),
                };
                return result;
            }

            private static string FormatJsonName(string value, JsonNamingPolicy? namingPolicy)
            {
                if (namingPolicy is null)
                {
                    return value;
                }

                string converted;
                if (!value.Contains(ValueSeparator))
                {
                    converted = namingPolicy.ConvertName(value);
                }
                else
                {
                    string[] enumValues = SplitFlagsEnum(value);

                    for (int i = 0; i < enumValues.Length; i++)
                    {
                        string name = namingPolicy.ConvertName(enumValues[i]);
                        enumValues[i] = name;
                    }

                    converted = string.Join(ValueSeparator, enumValues);
                }

                return converted;
            }

            private static bool IsValidIdentifier(string value)
            {
                return (value[0] >= 'A' &&
                    (!s_isSignedEnum || !value.StartsWith(NumberFormatInfo.CurrentInfo.NegativeSign)));
            }

            private static string[] SplitFlagsEnum(string value)
            {
                return value.Split(ValueSeparator);
            }
        }
    }
}
