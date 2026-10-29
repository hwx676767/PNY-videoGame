using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace MinimapRadar { 

	[CustomEditor(typeof(MinimapItem))]
	[CanEditMultipleObjects]
	public class EditorMinimapItem : Editor
	{
		SerializedProperty mDisplaySprite;
		SerializedProperty mColor;
		SerializedProperty mScaleRatio;
		SerializedProperty mOrientDisplay;
		
		SerializedProperty mStayOnMapBorder;
		//if mStayOnMapBorder true
		SerializedProperty mAlwaysStayOnMapBorder;
		SerializedProperty mDistanceHideFromBorder;   //determine the distance where the icon should hide anyway
		SerializedProperty mUseBorderFadeoutScale;
		SerializedProperty mBorderFadeoutScale;
		SerializedProperty mUseBorderFadeoutAlpha;
		SerializedProperty mBorderFadeoutAlpha;

		SerializedProperty mUseFadeoutAlpha;
		SerializedProperty mRatioStartFadeoutAlpha;
		SerializedProperty mFadeoutAlpha;
		SerializedProperty mUseFadeoutScale;
		SerializedProperty mRatioStartFadeoutScale;
		SerializedProperty mFadeoutScale;


		void OnEnable()
		{
			mDisplaySprite = serializedObject.FindProperty("mDisplaySprite");
			mColor = serializedObject.FindProperty("mColor");
			mScaleRatio = serializedObject.FindProperty("mScaleRatio");
			mOrientDisplay = serializedObject.FindProperty("mOrientDisplay");

			mStayOnMapBorder = serializedObject.FindProperty("mStayOnMapBorder");
			mAlwaysStayOnMapBorder = serializedObject.FindProperty("mAlwaysStayOnMapBorder");
			mDistanceHideFromBorder = serializedObject.FindProperty("mDistanceHideFromBorder");
			mUseBorderFadeoutScale = serializedObject.FindProperty("mUseBorderFadeoutScale");
			mBorderFadeoutScale = serializedObject.FindProperty("mBorderFadeoutScale");
			mUseBorderFadeoutAlpha = serializedObject.FindProperty("mUseBorderFadeoutAlpha");
			mBorderFadeoutAlpha = serializedObject.FindProperty("mBorderFadeoutAlpha");

			mUseFadeoutAlpha = serializedObject.FindProperty("mUseFadeoutAlpha");
			mRatioStartFadeoutAlpha = serializedObject.FindProperty("mRatioStartFadeoutAlpha");
			mFadeoutAlpha = serializedObject.FindProperty("mFadeoutAlpha");
			mUseFadeoutScale = serializedObject.FindProperty("mUseFadeoutScale");
			mRatioStartFadeoutScale = serializedObject.FindProperty("mRatioStartFadeoutScale");
			mFadeoutScale = serializedObject.FindProperty("mFadeoutScale");

		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			MinimapItem _baseScript = target as MinimapItem;

			int prevDisplaySpriteId = mDisplaySprite.objectReferenceInstanceIDValue;

			EditorGUILayout.PropertyField(mDisplaySprite);
			EditorGUILayout.PropertyField(mColor);
			EditorGUILayout.PropertyField(mScaleRatio);
			EditorGUILayout.PropertyField(mOrientDisplay);

			EditorGUILayout.PropertyField(mStayOnMapBorder);
			if (_baseScript.mStayOnMapBorder)
			{
				EditorGUILayout.PropertyField(mAlwaysStayOnMapBorder);
				if (!_baseScript.mAlwaysStayOnMapBorder)
				{
					EditorGUILayout.PropertyField(mDistanceHideFromBorder);
					EditorGUILayout.PropertyField(mUseBorderFadeoutScale);
					if (_baseScript.mUseBorderFadeoutScale)
					{
						EditorGUILayout.PropertyField(mBorderFadeoutScale);
					}
					EditorGUILayout.PropertyField(mUseBorderFadeoutAlpha);
					if (_baseScript.mUseBorderFadeoutAlpha)
					{
						EditorGUILayout.PropertyField(mBorderFadeoutAlpha);
					}
				}
			}
			else
			{
				EditorGUILayout.PropertyField(mUseFadeoutScale);
				if (_baseScript.mUseFadeoutScale)
				{
					EditorGUILayout.Slider(mRatioStartFadeoutScale, 0, 1);
					EditorGUILayout.PropertyField(mFadeoutScale);
				}
				EditorGUILayout.PropertyField(mUseFadeoutAlpha);
				if (_baseScript.mUseFadeoutAlpha)
				{
					EditorGUILayout.Slider(mRatioStartFadeoutAlpha, 0, 1);
					EditorGUILayout.PropertyField(mFadeoutAlpha);
				}
			}


			serializedObject.ApplyModifiedProperties();



			if(prevDisplaySpriteId != mDisplaySprite.objectReferenceInstanceIDValue)
			{
				//sprite updated
				_baseScript.UpdateDisplay();
			}
		}
	}
}
