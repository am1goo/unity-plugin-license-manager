using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LicenseManager.Editor
{
    public static class LicenseEditorGUI
    {
        private static readonly Type _flagsAttribute = typeof(FlagsAttribute);
        private static readonly Dictionary<string, string[]> _cachedPaths = new Dictionary<string, string[]>();

        private static bool TryGetEnumFlags(SerializedProperty property, out Type enumType, out int enumFlags)
        {
            if (property.propertyType == SerializedPropertyType.Enum)
            {
                var parentType = property.serializedObject.targetObject.GetType();
                var type = GetTypeAtPropertyPath(parentType, property.propertyPath);
                var flags = type.GetCustomAttributes(_flagsAttribute, inherit: false);
                if (flags != null && flags.Length > 0)
                {
                    enumType = type;
                    enumFlags = property.intValue;
                    return true;
                }
                else
                {
                    enumType = default;
                    enumFlags = default;
                    return false;
                }
            }
            else
            {
                enumType = default;
                enumFlags = default;
                return false;
            }
        }

        public static float GetPropertyHeight(SerializedProperty property)
        {
            if (TryGetEnumFlags(property, out var enumType, out var enumFlags))
            {
                var height = 0f;

                var enumValues = 0;
                foreach (int value in Enum.GetValues(enumType))
                {
                    if ((enumFlags & value) != 0)
                    {
                        enumValues++;
                    }
                }

                if (enumValues > 0)
                {
                    height += EditorGUIUtility.singleLineHeight;
                    height += enumValues * EditorGUIUtility.singleLineHeight;
                }

                return height;
            }
            else
            {
                return EditorGUI.GetPropertyHeight(property);
            }
        }

        public static void PropertyField(Rect rect, SerializedProperty property)
        {
            Rect r = new Rect(rect);
            r.height = EditorGUIUtility.singleLineHeight;

            if (TryGetEnumFlags(property, out var enumType, out var enumFlags))
            {
                EditorGUI.LabelField(r, "Remarks:");
                r.y += r.height;

                EditorGUI.indentLevel++;
                foreach (int value in Enum.GetValues(enumType))
                {
                    if ((enumFlags & value) != 0)
                    {
                        EditorGUI.LabelField(r, Enum.GetName(enumType, value));
                        r.y += r.height;
                    }
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUI.PropertyField(rect, property);
            }
        }

        private static Type GetTypeAtPropertyPath(this Type type, string path)
        {
            if (!_cachedPaths.TryGetValue(path, out var exist))
            {
                exist = path.Split('.');
                _cachedPaths.Add(path, exist);
            }

            var parentType = type;
            var mi = GetMember(type, path);
            for (int i = 0; i < exist.Length; ++i)
            {
                var memberName = exist[i];
                var child = GetMember(parentType, memberName);
                if (child != null)
                {
                    var childType = GetMemberType(child);
                    if (childType.IsArray)
                    {
                        parentType = childType.GetElementType();
                        i += 2;
                    }
                    else if (childType.IsGenericType)
                    {
                        parentType = childType.GetGenericArguments()[0];
                        i += 2;
                    }
                    else
                    {
                        parentType = childType;
                    }
                }
                else
                    return null;
            }
            return parentType;
        }

        private static MemberInfo GetMember(Type type, string path)
        {
            var fi = type.GetField(path, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi != null)
            {
                return fi.FieldType;
            }

            var pi = type.GetProperty(path, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (pi != null)
            {
                return pi.PropertyType;
            }

            return null;
        }

        private static Type GetMemberType(MemberInfo mi)
        {
            if (mi is TypeInfo ti)
            {
                return ti.UnderlyingSystemType;
            }
            else if (mi is FieldInfo fi)
            {
                return fi.FieldType;
            }
            else if (mi is PropertyInfo pi)
            {
                return pi.PropertyType;
            }
            else
            {
                return null;
            }
        }
    }
}
