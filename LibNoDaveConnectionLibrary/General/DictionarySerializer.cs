using System.Xml.Serialization;
using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace DotNetSiemensPLCToolBoxLibrary.General
{
    public class DictionarySerializer<K, V>
    {
        public struct SerializableKeyValuePair<TKey, TValue>
        {
            public TKey Key;
            public TValue Value;
            public SerializableKeyValuePair(KeyValuePair<TKey, TValue> kvp)
            {
                this.Key = kvp.Key;
                this.Value = kvp.Value;
            }
        }

        private XmlSerializer Serializer = new XmlSerializer(typeof(List<SerializableKeyValuePair<K, V>>));

        public void Serialize(Dictionary<K, V> dictionary, XmlWriter serializationStream)
        {
            Serializer.Serialize(serializationStream, BuildItemList(dictionary));
        }
        public void Serialize(Dictionary<K, V> dictionary, TextWriter serializationStream)
        {
            Serializer.Serialize(serializationStream, BuildItemList(dictionary));
        }
        public void Serialize(Dictionary<K, V> dictionary, Stream serializationStream)
        {
            Serializer.Serialize(serializationStream, BuildItemList(dictionary));
        }

        private List<SerializableKeyValuePair<K, V>> BuildItemList(Dictionary<K, V> dictionary)
        {
            List<SerializableKeyValuePair<K, V>> list = new List<SerializableKeyValuePair<K, V>>();
            foreach (KeyValuePair<K, V> nonSerializableKVP in dictionary)
            {
                list.Add(new SerializableKeyValuePair<K, V>(nonSerializableKVP));
            }

            return list;

        }


        public Dictionary<K, V> Deserialize(XmlReader serializationStream)
        {
            List<SerializableKeyValuePair<K, V>> dictionaryItems = Serializer.Deserialize(serializationStream) as List<SerializableKeyValuePair<K, V>>;
            return BuildDictionary(dictionaryItems);
        }
        public Dictionary<K, V> Deserialize(TextReader serializationStream)
        {
            List<SerializableKeyValuePair<K, V>> dictionaryItems = Serializer.Deserialize(serializationStream) as List<SerializableKeyValuePair<K, V>>;
            return BuildDictionary(dictionaryItems);
        }
        public Dictionary<K, V> Deserialize(Stream serializationStream)
        {
            List<SerializableKeyValuePair<K, V>> dictionaryItems = Serializer.Deserialize(serializationStream) as List<SerializableKeyValuePair<K, V>>;
            return BuildDictionary(dictionaryItems);
        }

        private Dictionary<K, V> BuildDictionary(List<SerializableKeyValuePair<K, V>> dictionaryItems)
        {
            Dictionary<K, V> dictionary = new Dictionary<K, V>(dictionaryItems.Count);
            foreach (SerializableKeyValuePair<K, V> item in dictionaryItems)
            {
                dictionary.Add(item.Key, item.Value);
            }

            return dictionary;
        }

    }
}