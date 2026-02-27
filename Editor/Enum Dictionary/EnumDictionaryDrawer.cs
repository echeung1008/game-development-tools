#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BlueMuffinGames.Utility
{
    /// <summary>
    /// Draws EnumDictionary<TEnum, TValue> as:
    /// - A single header
    /// - One foldout per enum value (key)
    /// - The TValue drawn inline (no extra nested foldout for structs/classes)
    /// </summary>
    [CustomPropertyDrawer(typeof(EnumDictionary<,>), true)]
    public class EnumDictionaryDrawer : PropertyDrawer
    {
        // Persist foldouts per-property-path in the editor session
        private static readonly Dictionary<string, bool[]> FoldoutCache = new();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!TryGetEnumAndValues(property, out var enumType, out var valuesProp))
                return EditorGUIUtility.singleLineHeight;

            var names = Enum.GetNames(enumType);
            EnsureArraySize(valuesProp, names.Length);

            var folds = GetFoldouts(property.propertyPath, names.Length);

            float h = EditorGUIUtility.singleLineHeight + 2; // header
            for (int i = 0; i < names.Length; i++)
            {
                h += EditorGUIUtility.singleLineHeight; // foldout row
                if (folds[i])
                {
                    var elem = valuesProp.GetArrayElementAtIndex(i);
                    h += GetInlineValueHeight(elem) + 2;
                }
            }

            return h + 2;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!TryGetEnumAndValues(property, out var enumType, out var valuesProp))
            {
                EditorGUI.LabelField(position, label.text, "EnumDictionary Drawer error.");
                return;
            }

            var names = Enum.GetNames(enumType);
            EnsureArraySize(valuesProp, names.Length);

            var folds = GetFoldouts(property.propertyPath, names.Length);

            var r = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(r, label);
            r.y += EditorGUIUtility.singleLineHeight + 2;

            EditorGUI.indentLevel++;

            for (int i = 0; i < names.Length; i++)
            {
                // Key foldout
                var foldRect = new Rect(r.x, r.y, r.width, EditorGUIUtility.singleLineHeight);
                folds[i] = EditorGUI.Foldout(foldRect, folds[i], names[i], true);
                r.y += EditorGUIUtility.singleLineHeight;

                if (!folds[i]) continue;

                // Value drawn inline (no extra nested foldout)
                var elem = valuesProp.GetArrayElementAtIndex(i);
                DrawInlineValue(ref r, elem);
                r.y += 2;
            }

            EditorGUI.indentLevel--;
        }

        private bool TryGetEnumAndValues(SerializedProperty property, out Type enumType, out SerializedProperty valuesProp)
        {
            enumType = null;
            valuesProp = property.FindPropertyRelative("values");
            if (valuesProp == null || !valuesProp.isArray) return false;

            var ft = fieldInfo?.FieldType;
            if (ft == null || !ft.IsGenericType) return false;

            var args = ft.GetGenericArguments();
            if (args == null || args.Length != 2) return false;

            enumType = args[0];
            return enumType != null && enumType.IsEnum;
        }

        private static void EnsureArraySize(SerializedProperty valuesProp, int size)
        {
            if (valuesProp.arraySize != size)
                valuesProp.arraySize = size;
        }

        private static bool[] GetFoldouts(string path, int count)
        {
            if (!FoldoutCache.TryGetValue(path, out var arr) || arr == null || arr.Length != count)
            {
                arr = new bool[count];
                FoldoutCache[path] = arr;
            }
            return arr;
        }

        /// <summary>
        /// Height of value drawn inline.
        /// If the value is a struct/class (Generic), draw all direct children without the "value foldout".
        /// Otherwise draw it normally.
        /// </summary>
        private static float GetInlineValueHeight(SerializedProperty valueProp)
        {
            if (valueProp.propertyType != SerializedPropertyType.Generic)
                return EditorGUI.GetPropertyHeight(valueProp, includeChildren: true);

            float h = 0f;

            var child = valueProp.Copy();
            var end = child.GetEndProperty();

            // Move to first child
            if (!child.NextVisible(true)) return 0f;

            // Iterate until end of this property
            while (!SerializedProperty.EqualContents(child, end))
            {
                h += EditorGUI.GetPropertyHeight(child, includeChildren: true) + 2;
                if (!child.NextVisible(false)) break;
            }

            return h;
        }

        /// <summary>
        /// Draw value inline.
        /// For Generic (struct/class), draws all direct children properties (including arrays) with labels,
        /// but without showing the parent foldout.
        /// </summary>
        private static void DrawInlineValue(ref Rect r, SerializedProperty valueProp)
        {
            if (valueProp.propertyType != SerializedPropertyType.Generic)
            {
                float h = EditorGUI.GetPropertyHeight(valueProp, includeChildren: true);
                EditorGUI.PropertyField(new Rect(r.x, r.y, r.width, h), valueProp, GUIContent.none, true);
                r.y += h;
                return;
            }

            var child = valueProp.Copy();
            var end = child.GetEndProperty();

            if (!child.NextVisible(true)) return;

            EditorGUI.indentLevel++;

            while (!SerializedProperty.EqualContents(child, end))
            {
                float h = EditorGUI.GetPropertyHeight(child, includeChildren: true);

                // Use the child's own display name (e.g., "_quotas")
                EditorGUI.PropertyField(new Rect(r.x, r.y, r.width, h), child, true);
                r.y += h + 2;

                if (!child.NextVisible(false)) break;
            }

            EditorGUI.indentLevel--;
        }
    }
}
#endif
