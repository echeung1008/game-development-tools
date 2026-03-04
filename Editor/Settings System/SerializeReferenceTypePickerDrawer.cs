#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BlueMuffinGames.Tools.SettingsSystem
{
    [CustomPropertyDrawer(typeof(SerializeReferenceTypePickerAttribute), true)]
    public sealed class SerializeReferenceTypePickerDrawer : PropertyDrawer
    {
        private static readonly Dictionary<Type, Type[]> Cache = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            var baseType = GetManagedReferenceBaseType(fieldInfo.FieldType);
            var types = GetConcreteTypes(baseType);

            // Header row: label + type popup
            var line = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var labelRect = new Rect(line.x, line.y, EditorGUIUtility.labelWidth, line.height);
            var popupRect = new Rect(line.x + EditorGUIUtility.labelWidth, line.y, line.width - EditorGUIUtility.labelWidth, line.height);

            EditorGUI.LabelField(labelRect, label);

            var currentType = property.managedReferenceValue?.GetType();
            var currentIndex = IndexOf(types, currentType);

            var displayNames = BuildDisplayNames(types);
            var newIndex = EditorGUI.Popup(popupRect, currentIndex, displayNames);

            if (newIndex != currentIndex)
            {
                property.managedReferenceValue = newIndex <= 0 ? null : Activator.CreateInstance(types[newIndex - 1]);
            }

            // Draw the selected object's fields (if any)
            if (property.managedReferenceValue != null)
            {
                var y = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                var end = property.GetEndProperty();

                var it = property.Copy();
                it.NextVisible(true); // enter managed ref

                EditorGUI.indentLevel++;
                while (!SerializedProperty.EqualContents(it, end))
                {
                    float h = EditorGUI.GetPropertyHeight(it, true);
                    var r = new Rect(position.x, y, position.width, h);
                    EditorGUI.PropertyField(r, it, true);
                    y += h + EditorGUIUtility.standardVerticalSpacing;

                    if (!it.NextVisible(false))
                        break;
                }
                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
                return EditorGUI.GetPropertyHeight(property, label, true);

            float h = EditorGUIUtility.singleLineHeight; // your popup row

            if (property.managedReferenceValue != null)
            {
                h += EditorGUIUtility.standardVerticalSpacing;

                var end = property.GetEndProperty();
                var it = property.Copy();
                it.NextVisible(true);

                while (!SerializedProperty.EqualContents(it, end))
                {
                    h += EditorGUI.GetPropertyHeight(it, true) + EditorGUIUtility.standardVerticalSpacing;
                    if (!it.NextVisible(false))
                        break;
                }
            }

            return h;
        }

        private static Type GetManagedReferenceBaseType(Type fieldType)
        {
            // Field itself may be concrete; we want “what does this field accept?”
            return fieldType;
        }

        private static Type[] GetConcreteTypes(Type baseType)
        {
            if (baseType == null) return Array.Empty<Type>();

            if (Cache.TryGetValue(baseType, out var cached))
                return cached;

            // TypeCache is fast and editor-friendly.
            var derived = TypeCache.GetTypesDerivedFrom(baseType)
                .Where(t =>
                    !t.IsAbstract &&
                    !t.IsGenericTypeDefinition &&
                    t.GetConstructor(Type.EmptyTypes) != null &&
                    (t.IsSerializable || Attribute.IsDefined(t, typeof(SerializableAttribute))))
                .OrderBy(t => t.FullName)
                .ToArray();

            Cache[baseType] = derived;
            return derived;
        }

        private static int IndexOf(Type[] types, Type current)
        {
            if (current == null) return 0; // “None”
            for (int i = 0; i < types.Length; i++)
                if (types[i] == current) return i + 1; // offset by 1 for “None”
            return 0;
        }

        private static string[] BuildDisplayNames(Type[] types)
        {
            var list = new List<string>(types.Length + 1) { "None" };
            list.AddRange(types.Select(t => t.Name));
            return list.ToArray();
        }
    }
}
#endif