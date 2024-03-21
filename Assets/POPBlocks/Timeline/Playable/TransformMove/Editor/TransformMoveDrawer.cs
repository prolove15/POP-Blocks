using UnityEditor;
using UnityEngine;

namespace POPBlocks.Timeline.Playable.TransformMove.Editor
{
    [CustomPropertyDrawer(typeof(TransformMoveBehaviour))]
    public class TransformMoveDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int fieldCount = 2;
            return fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
//        SerializedProperty space = property.FindPropertyRelative("space");
//        SerializedProperty curveXProp = property.FindPropertyRelative("curveX");
//        SerializedProperty curveYProp = property.FindPropertyRelative("curveY");
//        SerializedProperty curveYPro = property.FindPropertyRelative("curvee");
//
//        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
//        EditorGUI.PropertyField(singleFieldRect, curveXProp);
//
//        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
//        EditorGUI.PropertyField(singleFieldRect, curveYProp);
        }
    }
}
