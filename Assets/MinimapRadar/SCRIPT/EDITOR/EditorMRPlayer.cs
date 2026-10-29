using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace MinimapRadar { 

	[CustomEditor(typeof(MRPlayer))]
	[CanEditMultipleObjects]
	public class EditorMRPlayer : Editor
	{
		SerializedProperty mDisplaySprite;
		SerializedProperty mColor;
		SerializedProperty mScaleRatio;
		SerializedProperty mOrientDisplay;

		SerializedProperty mUseVisionAngleDisplay;
		SerializedProperty mVisionAngleDegree;


		void OnEnable()
		{
			mDisplaySprite = serializedObject.FindProperty("mDisplaySprite");
			mColor = serializedObject.FindProperty("mColor");
			mScaleRatio = serializedObject.FindProperty("mScaleRatio");
			mOrientDisplay = serializedObject.FindProperty("mOrientDisplay");

			mUseVisionAngleDisplay = serializedObject.FindProperty("mUseVisionAngleDisplay");
			mVisionAngleDegree = serializedObject.FindProperty("mVisionAngleDegree");

		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			MRPlayer _baseScript = target as MRPlayer;

			int prevDisplaySpriteId = mDisplaySprite.objectReferenceInstanceIDValue;

			EditorGUILayout.PropertyField(mDisplaySprite);
			EditorGUILayout.PropertyField(mColor);
			EditorGUILayout.PropertyField(mScaleRatio);
			EditorGUILayout.PropertyField(mOrientDisplay);

			EditorGUILayout.PropertyField(mUseVisionAngleDisplay);
			if (_baseScript.mUseVisionAngleDisplay)
			{
				EditorGUILayout.PropertyField(mVisionAngleDegree);
			}

			serializedObject.ApplyModifiedProperties();

			if (prevDisplaySpriteId != mDisplaySprite.objectReferenceInstanceIDValue)
			{
				//sprite updated
				_baseScript.UpdateDisplay();
			}
		}
	}
}
