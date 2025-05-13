/*
 * Copyright 2024 (c) Leia Inc.  All rights reserved.
 *
 * NOTICE:  All information contained herein is, and remains
 * the property of Leia Inc. and its suppliers, if any.  The
 * intellectual and technical concepts contained herein are
 * proprietary to Leia Inc. and its suppliers and may be covered
 * by U.S. and Foreign Patents, patents in process, and are
 * protected by trade secret or copyright law.  Dissemination of
 * this information or reproduction of this materials strictly
 * forbidden unless prior written permission is obtained from
 * Leia Inc.
 */
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LeiaMedia))]
public class LeiaMediaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        foreach (var targetObj in targets)
        {
            SerializedObject serializedObj = new SerializedObject(targetObj);
            SerializedProperty mediaTypeProp = serializedObj.FindProperty("mediaType");
            SerializedProperty sbsTextureProp = serializedObj.FindProperty("sbsTexture");
            SerializedProperty videoPlayerProp = serializedObj.FindProperty("videoPlayer");

            serializedObj.Update();

            EditorGUILayout.PropertyField(mediaTypeProp);

            if (mediaTypeProp.enumValueIndex == 0)
            {
                EditorGUILayout.PropertyField(sbsTextureProp);
            }
            else if (mediaTypeProp.enumValueIndex == 1)
            {
                EditorGUILayout.PropertyField(videoPlayerProp);
            }
            else
            {
                Debug.LogWarning(string.Format("Unexpected enumValueIndex for mediaTypeProp: {0}", mediaTypeProp.enumValueIndex));
            }

            serializedObj.ApplyModifiedProperties();
        }
    }
}