using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (HeightScaler))]
[CanEditMultipleObjects]
public class HeightScalerEditor : Editor {

    SerializedProperty _factor;
    SerializedProperty _include;

    private void OnEnable () {

        _factor = serializedObject.FindProperty ("_scaleFactor");
        _include = serializedObject.FindProperty ("_includeThis");

    }

    public override void OnInspectorGUI () {

        base.OnInspectorGUI ();

        serializedObject.Update ();

        if (GUILayout.Button ("Apply")) {

            HeightScaler comp = target as HeightScaler;
            Queue<Transform> queue = new Queue<Transform> ();
            if (_include.boolValue) queue.Enqueue (comp.transform);
            else {

                foreach (Transform child in comp.transform) {

                    queue.Enqueue (child);

                }

            }
            while (queue.Count > 0) {

                Transform t = queue.Dequeue ();
                t.localPosition = new Vector3 (t.localPosition.x, t.localPosition.y * _factor.floatValue, t.localPosition.z);

                foreach (Transform child in t) {

                    queue.Enqueue (child);

                }

            }

        }

        serializedObject.ApplyModifiedProperties ();

    }

}
