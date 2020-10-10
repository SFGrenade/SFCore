using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SFCore
{
    public class ReflectionPublicConverter<T> : JsonConverter where T : new()
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            T ret = (T)value;
            Type retType = ret.GetType();
            writer.WriteStartObject();

            foreach (FieldInfo tmpFieldInfo in retType.GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                writer.WritePropertyName(tmpFieldInfo.Name);
                serializer.Serialize(writer, tmpFieldInfo.GetValue(value));
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject item = JObject.Load(reader);

                T ret = new T();
                foreach (FieldInfo tmpFieldInfo in objectType.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (item[tmpFieldInfo.Name] == null) return null;

                    tmpFieldInfo.SetValue(ret, item[tmpFieldInfo.Name].ToObject(item[tmpFieldInfo.Name].GetType()));
                }

                return ret;
            }
            else
            {
                JArray array = JArray.Load(reader);

                var users = array.ToObject<IList<Vector2>>();

                return users;
            }

            return null;
        }
    }
    public class ReflectionPrivateConverter<T> : JsonConverter where T : new()
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            T ret = (T)value;
            Type retType = ret.GetType();
            writer.WriteStartObject();

            foreach (FieldInfo tmpFieldInfo in retType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                writer.WritePropertyName(tmpFieldInfo.Name);
                serializer.Serialize(writer, tmpFieldInfo.GetValue(value));
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject item = JObject.Load(reader);

                T ret = new T();
                foreach (FieldInfo tmpFieldInfo in objectType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (item[tmpFieldInfo.Name] == null) return null;

                    tmpFieldInfo.SetValue(ret, item[tmpFieldInfo.Name].ToObject(item[tmpFieldInfo.Name].GetType()));
                }

                return ret;
            }
            else
            {
                JArray array = JArray.Load(reader);

                var users = array.ToObject<IList<Vector2>>();

                return users;
            }

            return null;
        }
    }
    public class ReflectionConverter<T> : JsonConverter where T : new()
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            T ret = (T)value;
            Type retType = ret.GetType();
            writer.WriteStartObject();

            foreach (FieldInfo tmpFieldInfo in retType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                writer.WritePropertyName(tmpFieldInfo.Name);
                serializer.Serialize(writer, tmpFieldInfo.GetValue(value));
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject item = JObject.Load(reader);

                T ret = new T();
                foreach (FieldInfo tmpFieldInfo in objectType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (item[tmpFieldInfo.Name] == null) return null;

                    tmpFieldInfo.SetValue(ret, item[tmpFieldInfo.Name].ToObject(item[tmpFieldInfo.Name].GetType()));
                }

                return ret;
            }
            else
            {
                JArray array = JArray.Load(reader);

                var users = array.ToObject<IList<Vector2>>();

                return users;
            }

            return null;
        }
    }
}
