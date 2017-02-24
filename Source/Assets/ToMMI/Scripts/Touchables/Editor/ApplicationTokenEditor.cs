/*
 * @author Francesco Strada
 */

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Touchables.Editor
{
    [CustomEditor(typeof(ApplicationToken))]
    public class ApplicationTokenEditor : UnityEditor.Editor
    {
        private string[] ComponentsNames = new string[0];

        private List<MonoBehaviour> monos;

        public override void OnInspectorGUI()
        {
            ApplicationToken appToken = (ApplicationToken)target;
            //DrawDefaultInspector();

            EditorGUILayout.BeginVertical();
            appToken.TokenClass = EditorGUILayout.IntField("Token Class", appToken.TokenClass);

            appToken.Target = EditorGUILayout.ObjectField("Target", appToken.Target, typeof(GameObject), true);
            if (appToken.Target != null)
                FillComponentArray(appToken);

            appToken.selectedComponent = EditorGUILayout.Popup("Token Events", appToken.selectedComponent, ComponentsNames);
            //appToken.SetTokenEventsFunctions();

            EditorGUILayout.EndVertical();
        }

        private void FillComponentArray(ApplicationToken appToken)
        {
            GameObject targetGameObject = appToken.Target as GameObject;

            if (targetGameObject != null)
            {
                MonoBehaviour[] list = targetGameObject.GetComponents<MonoBehaviour>();
                monos = new List<MonoBehaviour>();
                foreach (MonoBehaviour mb in list)
                {
                    ITokenEvents tokenEvent = mb as ITokenEvents;
                    if (tokenEvent != null)
                    {
                        monos.Add(mb);
                    }
                }
                appToken.targetComponents = monos.ToArray();

                FillComponentNameArray(appToken);
            }
            else
                ComponentsNames = new string[0];
        }

        private void FillComponentNameArray(ApplicationToken appToken)
        {
            ComponentsNames = new string[appToken.targetComponents.Length];

            for (int i = 0; i < appToken.targetComponents.Length; i++)
            {
                ITokenEvents c = (ITokenEvents)appToken.targetComponents[i];
                ComponentsNames[i] = c.GetType().ToString();
            }
        }
    }
}
