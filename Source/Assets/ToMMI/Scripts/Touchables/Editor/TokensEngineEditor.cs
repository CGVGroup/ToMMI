/*
 * @author Francesco Strada
 */

using UnityEditor;
using UnityEngine;
using Touchables.TokenEngine;

namespace Touchables.Editor
{
    [CustomEditor(typeof(TokensEngine))]
    public class TokensEngineEditor : UnityEditor.Editor
    {
        string[] tokenTypes = { "3 x 3", "4 x 4", "5 x 5" };
        string[] distanceMetrics = { "Pixels", "Centimeters" };

        public override void OnInspectorGUI()
        {
            TokensEngineProperties props = ((TokensEngine)target).GetPars();

            //DrawDefaultInspector();

            EditorGUILayout.BeginHorizontal();
            props.Type = EditorGUILayout.Popup("Token Type: ", props.Type, tokenTypes, EditorStyles.popup);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();
            GUILayout.Label("Operation Metrics", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            props.MeanSquare = EditorGUILayout.Toggle("Mean Square", props.MeanSquare);
            //EditorGUIUtility.labelWidth = 80;
            props.ComputePixels = EditorGUILayout.Popup("Distances: ", props.ComputePixels, distanceMetrics, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
            props.ContinuousMeanSquare = EditorGUILayout.Toggle("Continuous", props.ContinuousMeanSquare);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();
            GUILayout.Label("Token Update Thresholds", EditorStyles.boldLabel);
            props.TranslationThr = EditorGUILayout.Slider("Translation", props.TranslationThr, 0.1f, 5.0f);
            props.RotationThr = EditorGUILayout.Slider("Rotation", props.RotationThr, 0.1f, 5.0f);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            props.Target60FPS = EditorGUILayout.Toggle("Target 60FPS", props.Target60FPS);
            EditorGUILayout.EndHorizontal();



        }
    }
}
