using System;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SerializableGuid))]
public class SerializableGuidDrawer : PropertyDrawer
{
    private float ySep = 20;
   
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        EditorGUI.BeginProperty(position, label, property);

        var value0 = property.FindPropertyRelative("Part1");
        var value1 = property.FindPropertyRelative("Part2");
        var value2 = property.FindPropertyRelative("Part3");
        var value3 = property.FindPropertyRelative("Part4");

        if (value0 != null && value1 != null && value2 != null && value3 != null)
        {
            void SetGuid(System.Guid guid)
            {
                byte[] bytes = guid.ToByteArray();
                value0.uintValue = BitConverter.ToUInt32(bytes, 0);
                value1.uintValue = BitConverter.ToUInt32(bytes, 4);
                value2.uintValue = BitConverter.ToUInt32(bytes, 8);
                value3.uintValue = BitConverter.ToUInt32(bytes, 12);
            }

            var guidStr = new StringBuilder()
                    .AppendFormat("{0:X8}", (uint)value0.intValue)
                    .AppendFormat("{0:X8}", (uint)value1.intValue)
                    .AppendFormat("{0:X8}", (uint)value2.intValue)
                    .AppendFormat("{0:X8}", (uint)value3.intValue)
                    .ToString();

            // Draw label
            position = EditorGUI.PrefixLabel(new Rect(position.x, position.y + ySep / 2, position.width, position.height), GUIUtility.GetControlID(FocusType.Passive), label);
            position.y -= ySep / 2; // Offsets position so we can draw the label for the field centered

           
            float currentY = position.yMin;
            Rect labelPos = new Rect(position.xMin, currentY, position.width, ySep - 2);
            EditorGUI.SelectableLabel(labelPos, guidStr);

            currentY += ySep;

            float buttonSize = position.width / 3; // Update size of buttons to always fit perfeftly above the string representation field

            // Buttons
            if (GUI.Button(new Rect(position.xMin, currentY, buttonSize, ySep - 2), "New"))
            {
                SetGuid(System.Guid.NewGuid());
            }
            if (GUI.Button(new Rect(position.xMin + buttonSize, currentY, buttonSize, ySep - 2), "Copy"))
            {
                EditorGUIUtility.systemCopyBuffer = guidStr;
            }
            if (GUI.Button(new Rect(position.xMin + buttonSize * 2, currentY, buttonSize, ySep - 2), "Empty"))
            {
                SetGuid(System.Guid.Empty);
            }

            value0.serializedObject.ApplyModifiedProperties();
            value1.serializedObject.ApplyModifiedProperties();
            value2.serializedObject.ApplyModifiedProperties();
            value3.serializedObject.ApplyModifiedProperties();

        }
        else
        {
            EditorGUI.SelectableLabel(position, "GUID Not Initialized");
        }

        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Field height never changes, so ySep * 2 will always return the proper hight of the field
        return ySep * 2;
    }
}