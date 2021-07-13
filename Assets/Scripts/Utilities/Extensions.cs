using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Extensions
{
    public static void AutoCheck(this MonoBehaviour checkedObject)
    {
        SerializedProperty prop = new SerializedObject(checkedObject).GetIterator();

        while (prop.NextVisible(true))
        {
            if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue == null)
            {
                Debug.LogError($"\"{prop.displayName}\" property on \"{checkedObject.name}\" is null.");
            }
        }
    }
}
