using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Maxisoft.Utils.Collections.Queues
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
            return typeToConvert.IsAssignableFrom(typeof(Queue<>)) || typeof(Queue<>).IsAssignableFrom(typeToConvert) || base.CanConvert(typeToConvert);
        }


        private static void ReadDequeData(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, in Deque<T> q)
        {
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
            
        }

        public override Deque<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var q = new Deque<T>();
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var chunkSize = q.ChunkSize;
            var trimOnDeletion = q.TrimOnDeletion;
            var initialChunkRatio = 0.5f;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return q;
                }
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }
                string propertyName = reader.GetString();
                if (!reader.Read()) break;
                switch (propertyName)
                {
                    case nameof(Deque<T>.ChunkSize):
                        //note that if Data property came before ChunkSize the later is silently ignored
                        chunkSize = reader.GetInt64();
                        break;
                    case nameof(Deque<T>.TrimOnDeletion):
                        trimOnDeletion = reader.GetBoolean();
                        q.TrimOnDeletion = trimOnDeletion;
                        break;
                    case nameof(Deque<T>.InitialChunkRatio):
                        initialChunkRatio = reader.GetSingle();
                        q.InitialChunkRatio = initialChunkRatio;
                        break;
                    case "Data":
                        q = new Deque<T>(chunkSize, initialChunkRatio){ TrimOnDeletion = trimOnDeletion};
                        ReadDequeData(ref reader, typeToConvert, options, in q);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(propertyName), propertyName, null);
                }
            }
            throw new JsonException();
            return q;
        }

        public override void Write(Utf8JsonWriter writer, Deque<T> value, JsonSerializerOptions options)
        {
            
            writer.WriteStartObject();
            writer.WriteNumber(nameof(Deque<T>.ChunkSize), value.ChunkSize);
            writer.WriteBoolean(nameof(Deque<T>.TrimOnDeletion), value.TrimOnDeletion);
            writer.WriteNumber(nameof(Deque<T>.InitialChunkRatio), value.InitialChunkRatio);
            writer.WriteStartArray("Data");


            foreach (var item in value)
            {
                JsonSerializer.Serialize(writer, item, options);
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}