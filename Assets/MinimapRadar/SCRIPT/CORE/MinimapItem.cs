using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MinimapRadar
{
	public class MinimapItem : MonoBehaviour
	{
		public float mRotationOffset = 0;


		public Sprite mDisplaySprite;
		public Color mColor = Color.green;
		public float mScaleRatio = 1f;
		public bool mOrientDisplay = true;
		public bool mDisplayOnMap = true;
		
		
		public bool mUseFadeoutAlpha = true;
		public float mRatioStartFadeoutAlpha = .8f;
		public AnimationCurve mFadeoutAlpha = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0) });
		public bool mUseFadeoutScale = true;
		public float mRatioStartFadeoutScale = .8f;
		public AnimationCurve mFadeoutScale = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0) });

		public bool mStayOnMapBorder = false;
		public bool mAlwaysStayOnMapBorder = true;
		public float mDistanceHideFromBorder = 5;
		public bool mUseBorderFadeoutScale = false;
		public AnimationCurve mBorderFadeoutScale = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0) });
		public bool mUseBorderFadeoutAlpha = false;
		public AnimationCurve mBorderFadeoutAlpha = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 0) });

		public MinimapItemDisplay mDisplayItem;
		public Vector2 mPosDiffFromPlayer;
		public float mDistFromPlayer;

		public void SetPosDiffFromPlayer(Vector2 pos)
		{
			mPosDiffFromPlayer = pos;
			mDistFromPlayer = mPosDiffFromPlayer.magnitude;
		}

		protected Transform mTrans;
		public Transform trans
		{
			get { return mTrans; }
		}

		protected virtual void CustomOnEnable()
		{
			MinimapRadar.RegisterItem(this);
		}

		protected virtual void CustomOnDisable()
		{
			MinimapRadar.UnregisterItem(this);
		}

		protected void OnEnable()
		{
			CustomOnEnable();
		}

		protected void OnDisable()
		{
			CustomOnDisable();
		}

		private void Awake()
		{
			mTrans = transform;

			//in case of duplicate scene item
			mDisplayItem = null;
		}

		public virtual void UpdateDisplay()
		{
			if (mDisplayItem != null)
				mDisplayItem.img.sprite = mDisplaySprite;
		}

		public virtual void SetSprite(Sprite sprite)
		{
			mDisplaySprite = sprite;
			if(mDisplayItem != null)
				mDisplayItem.img.sprite = sprite;
		}

		public virtual void SetupDisplay(MinimapItemDisplay display, MinimapRadar minimap)
		{
			mDisplayItem = display;
			mDisplayItem.mCurLinkedItem = this;
			if (display.img.sprite != mDisplaySprite)
				display.img.sprite = mDisplaySprite;
			display.img.color = mColor;
			display.rectTrans.localPosition = Vector3.zero;
			display.rectTrans.localScale = new Vector3(minimap.mIconsScale * mScaleRatio, minimap.mIconsScale * mScaleRatio, minimap.mIconsScale * mScaleRatio);
			display.rectTrans.localRotation = Quaternion.Euler(Vector3.zero);
			display.gameObject.SetActive(true);
		}

		public bool ManageFadeoutDisplay(float ratio, MinimapRadar minimap)
		{
			if (mStayOnMapBorder || ratio > 1)
				return false;
			bool _scaleModified = false;
			if (mUseFadeoutScale)
			{
				float _trueRatio = Mathf.InverseLerp(mRatioStartFadeoutScale, 1, ratio);
				float _newScale = mFadeoutScale.Evaluate(_trueRatio) * minimap.mIconsScale * mScaleRatio;
				mDisplayItem.rectTrans.localScale = new Vector3(_newScale, _newScale, 1);
				_scaleModified = true;
			}
			
			if (mUseFadeoutAlpha)
			{
				float _trueRatio = Mathf.InverseLerp(mRatioStartFadeoutAlpha, 1, ratio);
				float _newAlpha = mFadeoutAlpha.Evaluate(_trueRatio);
				Color _col = mDisplayItem.img.color;
				_col.a = _newAlpha;
				mDisplayItem.img.color = _col;
			}
			return _scaleModified;
		}

		public bool ManageOvershootDisplay(float ratio, MinimapRadar minimap)
		{
			if (!mStayOnMapBorder || mAlwaysStayOnMapBorder)
				return false;
			bool _scaleModified = false;

			if (mUseBorderFadeoutScale)
			{
				float _newScale = mBorderFadeoutScale.Evaluate(ratio) * minimap.mIconsScale * mScaleRatio;
				mDisplayItem.rectTrans.localScale = new Vector3(_newScale, _newScale,1);
				_scaleModified = true;
			}
			
			if (mUseBorderFadeoutAlpha)
			{
				float _newAlpha = mBorderFadeoutAlpha.Evaluate(ratio);
				Color _col = mDisplayItem.img.color;
				_col.a = _newAlpha;
				mDisplayItem.img.color = _col;
			}
			return _scaleModified;
		}

		public bool IsVisible(MinimapRadar minimap)
		{
			if (!mDisplayOnMap)
				return false;
			if (mDistFromPlayer <= minimap.mDistanceRadius)
				return true;
			if (mStayOnMapBorder && (mAlwaysStayOnMapBorder || mDistFromPlayer <= minimap.mDistanceRadius + mDistanceHideFromBorder))
				return true;
			return false;
		}
	}
}


