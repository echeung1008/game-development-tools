using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlueMuffinGames.Utility
{
    [Serializable]
    public class EnumDictionary<TEnum, TValue> : IEnumerable<KeyValuePair<TEnum, TValue>>
    where TEnum : Enum
    {
        // Unity-serialized backing store: exactly one value per enum entry (by ordinal index)
        [SerializeField] private List<TValue> values = new();

        public TValue this[TEnum key]
        {
            get { EnsureSize(); return values[ToIndex(key)]; }
            set { EnsureSize(); values[ToIndex(key)] = value; }
        }

        public bool TryGetValue(TEnum key, out TValue value)
        {
            EnsureSize();
            int i = ToIndex(key);
            if (i < 0 || i >= values.Count) { value = default; return false; }
            value = values[i];
            return true;
        }

        public IReadOnlyDictionary<TEnum, TValue> Dictionary
        {
            get
            {
                EnsureSize();
                Dictionary<TEnum, TValue> result = new();
                foreach (var e in Enum.GetValues(typeof(TEnum)))
                {
                    if (e is not TEnum key) continue;

                    result.Add(key, this[key]);
                }
                return result;
            }
        }

        public IReadOnlyList<TValue> Values
        {
            get { EnsureSize(); return values; }
        }

        public int Count
        {
            get { EnsureSize(); return values.Count; }
        }

        // --- internal helpers ---
        private static int ToIndex(TEnum key)
        {
            TEnum[] array = Enum.GetValues(typeof(TEnum)) as TEnum[];
            for (int i = 0; i < array.Length; i++)
            {
                if (key.Equals(array[i])) return i;
            }
            return -1;
        }
        private static TEnum EnumAtIndex(int index)
        {
            Array array = Enum.GetValues(typeof(TEnum));
            if (index < 0 || index >= array.Length) { return default; }
            if (array.GetValue(index) is TEnum e) return e;
            return default;
        }

        private void EnsureSize()
        {
            int enumCount = Enum.GetValues(typeof(TEnum)).Length;
            if (values == null) values = new List<TValue>(enumCount);

            while (values.Count < enumCount) values.Add(default);
            while (values.Count > enumCount) values.RemoveAt(values.Count - 1);
        }

        #region IEnumerable
        public IEnumerator<KeyValuePair<TEnum, TValue>> GetEnumerator() => new EnumDictionaryEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class EnumDictionaryEnumerator : IEnumerator<KeyValuePair<TEnum, TValue>>
        {
            private EnumDictionary<TEnum, TValue> thisDictionary;
            private int index;

            public EnumDictionaryEnumerator(EnumDictionary<TEnum, TValue> dictionary)
            {
                thisDictionary = dictionary;
                index = -1;
            }

            public KeyValuePair<TEnum, TValue> Current => new KeyValuePair<TEnum, TValue>(EnumAtIndex(index), thisDictionary.Values.ElementAt(index));

            object IEnumerator.Current => Current;

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                index++;
                if (index >= thisDictionary.Count) index = -1;
                return index != -1;
            }

            public void Reset()
            {
                index = -1;
            }
        }
        #endregion
    }
}
