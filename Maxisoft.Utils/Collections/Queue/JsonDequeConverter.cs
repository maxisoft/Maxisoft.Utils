using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Maxisoft.Utils.Collections.Queue
{
    public class JsonDequeConverterAttribute : JsonConverterAttribute
    {
        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            if (!(typeToConvert.IsGenericType &&
                  typeToConvert.GetGenericTypeDefinition() == typeof(Deque<>)))
            {
                Debug.Fail($"{typeToConvert} is not a valid Deque<>");
                return base.CreateConverter(typeToConvert);
            }

            Type elementType = typeToConvert.GetGenericArguments()[0];
            var type = typeof(DequeJsonConverter<>)
                .MakeGenericType(elementType);
            var property = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
            return (JsonConverter) property!.GetMethod.Invoke(null, new object[0]);
        }
    }

    public class DequeJsonConverter<T> : JsonConverter<Deque<T>>
    {
        private DequeJsonConverter()
        {
        }

        public static JsonConverter<Deque<T>> Instance { get; } = new DequeJsonConverter<T>();

        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert == typeof(Queue<>))
            {
                return true;
            }

            return base.CanConvert(typeToConvert);
        }

        public override Deque<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var q = new Deque<T>();
            if (reader.TokenType != JsonTokenType.StartArray || !reader.Read())
            {
                throw new JsonException();
            }

            while (reader.TokenType != JsonTokenType.EndArray)
            {
                q.PushBack(JsonSerializer.Deserialize<T>(ref reader, options));

                if (!reader.Read())
                {
                    throw new JsonException();
                }
            }

            return q;
        }

        public override void Write(Utf8JsonWriter writer, Deque<T> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();


            foreach (var item in value)
            {
                JsonSerializer.Serialize(writer, item, options);
            }

            writer.WriteEndArray();
        }
    }
}