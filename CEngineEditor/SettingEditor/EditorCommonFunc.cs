using UnityEngine;
using System.Collections;
using UnityEditor;
namespace CEngineEditor
{
    public class EditorCommonFunc : EditorWindow
    {
        public static string ColorFormat(string tex)
        {
            return "<color='#FFD700'>" + tex + "</color>";
        }

        public static string Replace(string s)
        {
            return s.Replace("\\", "/");
        }

        public static void Layout_DrawSeparator(Color color, float height = 0.5f)
        {
            Rect rect = GUILayoutUtility.GetLastRect();
            GUI.color = color;
            GUI.DrawTexture(new Rect(0f, rect.yMax, Screen.width, height), EditorGUIUtility.whiteTexture);
            GUI.color = Color.white;
            GUILayout.Space(height);
        }

        public static void Layout_DrawSeparatorV(Color color, float width = 2f)
        {
            Rect rect = GUILayoutUtility.GetLastRect();
            GUI.color = color;
            GUI.DrawTexture(new Rect(rect.xMax, rect.yMin, width, Screen.height), EditorGUIUtility.whiteTexture);
            GUI.color = Color.white;
            GUILayout.Space(width);
        }
    }
}
