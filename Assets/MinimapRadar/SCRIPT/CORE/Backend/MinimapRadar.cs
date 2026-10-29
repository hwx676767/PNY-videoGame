using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

namespace MinimapRadar
{
	public class MinimapRadar : MonoBehaviour
	{
		public static Stack<MinimapItemDisplay> sDisplayPool = new Stack<MinimapItemDisplay>();
		public static List<MinimapItem> sItems = new List<MinimapItem>();
		public static MRPlayer sCenterItem;
		public static MRPlayerVisionAngleDisplay sPlayerVisionAngle;


		public MinimapItemDisplay mDisplayPrefab;
		public float mDistanceRadius = 10f;
		RectTransform mItemContainer;
		public bool mRotateMapFromPlayerOrient = false;
		public float mIconsScale = 1;   //todo : icone scale



		

		private void Awake()
		{
			mItemContainer = transform.GetChild(0).Find("Container").GetComponent<RectTransform>();
		}

		private void OnDestroy()
		{
			sDisplayPool.Clear();
			sItems.Clear();
			sCenterItem = null;
			sPlayerVisionAngle = null;
		}

		public ViewAxis mViewAvis = ViewAxis.X;
		public enum ViewAxis
		{
			X,Y,Z
		}

		public static void RegisterCenterItem(MRPlayer item)
		{
			sCenterItem = item;
		}

		public static void UnregisterCenterItem(MRPlayer item)
		{
			if (sCenterItem == item)
				sCenterItem = null;
		}

		public static void RegisterItem(MinimapItem item)
		{
			if (!sItems.Contains(item))
				sItems.Add(item);
		}

		public static void UnregisterItem(MinimapItem item)
		{

			RemoveDisplay(item);
			sItems.Remove(item);
		}

		public static void RemoveDisplay(MinimapItem item)
		{


			if (item.mDisplayItem != null)
			{
				item.mDisplayItem.gameObject.SetActive(false);
				AddDisplayToPool(item.mDisplayItem);
				item.mDisplayItem.mCurLinkedItem = null;
				item.mDisplayItem = null;
			}
		}

		public static void AddDisplayToPool(MinimapItemDisplay item)
		{
			sDisplayPool.Push(item);
		}

		public MinimapItemDisplay GetAvailableDisplay()
		{
			if(sDisplayPool.Count > 0)
				return sDisplayPool.Pop();
			MinimapItemDisplay _toReturn = Instantiate(mDisplayPrefab.gameObject).GetComponent<MinimapItemDisplay>();
			_toReturn.transform.SetParent(mItemContainer);
			return _toReturn;
		}

		private Vector2 Get2DPos(Vector3 pos)
		{
			Vector2 _pos2D = Vector2.zero;
			switch (mViewAvis)
			{
				case ViewAxis.X:
					_pos2D.x = pos.z;
					_pos2D.y = pos.y;
					break;
				case ViewAxis.Y:
					_pos2D.x = pos.x;
					_pos2D.y = pos.z;
					break;
				case ViewAxis.Z:
					_pos2D.x = pos.x;
					_pos2D.y = pos.y;
					break;
			}
			return _pos2D;
		}

		void OrientItem(MinimapItem item, bool reverse = false)
		{
			if (item == null || item.mDisplayItem == null)
				return;
			Vector3 _rotEuleur = item.mDisplayItem.rectTrans.localRotation.eulerAngles;
			switch (mViewAvis)
			{
				case ViewAxis.X:
					if(reverse)
						_rotEuleur.z = -item.trans.rotation.eulerAngles.x;
					else
						_rotEuleur.z = item.trans.rotation.eulerAngles.x;
					break;
				case ViewAxis.Y:
					if (reverse)
						_rotEuleur.z = item.trans.rotation.eulerAngles.y;
					else
						_rotEuleur.z = -item.trans.rotation.eulerAngles.y;

					break;
				case ViewAxis.Z:
					if (reverse)
						_rotEuleur.z = -item.trans.rotation.eulerAngles.z;
					else
						_rotEuleur.z = item.trans.rotation.eulerAngles.z;
					break;
			}
			item.mDisplayItem.rectTrans.localRotation = Quaternion.Euler(_rotEuleur);
		}

		void ManageCenterItemVisionAngle()
		{
			if (sPlayerVisionAngle == null || sCenterItem == null)
				return;
			if (!sCenterItem.mUseVisionAngleDisplay && sPlayerVisionAngle.img.enabled)
			{
				sPlayerVisionAngle.img.enabled = false;
				return;
			}else if(sCenterItem.mUseVisionAngleDisplay && !sPlayerVisionAngle.img.enabled)
			{
				sPlayerVisionAngle.img.enabled = true;
			}

			Vector3 _rotEuleur = sPlayerVisionAngle.rectTrans.localRotation.eulerAngles;
			switch (mViewAvis)
			{
				case ViewAxis.X:
					_rotEuleur.z = sCenterItem.trans.rotation.eulerAngles.x;
					break;
				case ViewAxis.Y:
					_rotEuleur.z = -sCenterItem.trans.rotation.eulerAngles.y;
					break;
				case ViewAxis.Z:
					_rotEuleur.z = sCenterItem.trans.rotation.eulerAngles.z;
					break;
			}

			sPlayerVisionAngle.img.fillAmount = sCenterItem.mVisionAngleDegree / 360f;
			float _RotZ = 180f + (sCenterItem.mVisionAngleDegree / 2f);

			if(!mRotateMapFromPlayerOrient)
				sPlayerVisionAngle.rectTrans.localRotation = Quaternion.Euler(_rotEuleur + new Vector3(0,0, _RotZ));
			else
				sPlayerVisionAngle.rectTrans.localRotation = Quaternion.Euler(new Vector3(0, 0, _RotZ));

		}

		private void Update()
		{
			if (sCenterItem == null)
				return;
			if(sCenterItem.mDisplayItem == null)
				sCenterItem.SetupDisplay(GetAvailableDisplay(), this);

			if (mRotateMapFromPlayerOrient)
			{
				Vector3 _rotEuleur = mItemContainer.localRotation.eulerAngles;
				switch (mViewAvis)
				{
					case ViewAxis.X:
						_rotEuleur.z = -sCenterItem.trans.rotation.eulerAngles.x;
						break;
					case ViewAxis.Y:
						_rotEuleur.z = sCenterItem.trans.rotation.eulerAngles.y;
						break;
					case ViewAxis.Z:
						_rotEuleur.z = -sCenterItem.trans.rotation.eulerAngles.z;
						break;
				}
				mItemContainer.localRotation = Quaternion.Euler(_rotEuleur);
			}

			if (sCenterItem.mOrientDisplay)
			{
				OrientItem(sCenterItem);
			}
			ManageCenterItemVisionAngle();
			sCenterItem.mDisplayItem.rectTrans.localScale = new Vector3(sCenterItem.mScaleRatio * mIconsScale, sCenterItem.mScaleRatio * mIconsScale, 1);

			Vector2 _PlayerPos = Get2DPos(sCenterItem.trans.position);
			Vector2 _ItemPos;
			Vector2 _ItemRadarPos;
			Vector2 _PosMult = new Vector2(mItemContainer.rect.width / 2f, mItemContainer.rect.height / 2f);
			float _DistRatio;
			float _OvershootRatio = 0;
			bool _scaleModifiedByFadeout = false;
			bool _scaleModifiedByOvershoot = false;

			for (int i = 0; i < sItems.Count; i++)
			{
				_ItemPos = Get2DPos(sItems[i].trans.position);
				sItems[i].SetPosDiffFromPlayer(_ItemPos - _PlayerPos);
				if (sItems[i].IsVisible(this))
				{
					if (sItems[i].mDisplayItem == null)
					{
						sItems[i].SetupDisplay(GetAvailableDisplay(), this);
					}

					//potential fix for Jason Bushore 
					if (sItems[i].mDisplayItem.mCurLinkedItem != sItems[i])
					{
						sItems[i].SetupDisplay(sItems[i].mDisplayItem, this);
					}



					_DistRatio = sItems[i].mDistFromPlayer / mDistanceRadius;
					_ItemRadarPos = sItems[i].mPosDiffFromPlayer / mDistanceRadius;
					if (_DistRatio > 1)
					{
						_ItemRadarPos = _ItemRadarPos / _DistRatio;
						_OvershootRatio = (sItems[i].mDistFromPlayer - mDistanceRadius) / sItems[i].mDistanceHideFromBorder;
						//_OvershootRatio = (_DistRatio - 1f) * mDistanceRadius;
					}
					else
					{
						_OvershootRatio = 0;
					}
					_scaleModifiedByFadeout = sItems[i].ManageFadeoutDisplay(_DistRatio, this);
					_scaleModifiedByOvershoot = sItems[i].ManageOvershootDisplay(_OvershootRatio, this);
					if(!_scaleModifiedByFadeout && !_scaleModifiedByOvershoot)
					{
						float _newScale = mIconsScale * sItems[i].mScaleRatio;
						sItems[i].mDisplayItem.rectTrans.localScale = new Vector3(_newScale, _newScale, 1);
					}


					_ItemRadarPos *= _PosMult;
					sItems[i].mDisplayItem.rectTrans.anchoredPosition = _ItemRadarPos;
					//Debug.Log(sItems[i].mDisplayItem.rectTrans.localPosition);
					/*
					Vector3 _localPos = sItems[i].mDisplayItem.rectTrans.localPosition;
					_localPos.z = 0;
					sItems[i].mDisplayItem.rectTrans.localPosition = _localPos;
					*/


					if (sItems[i].mOrientDisplay)
					{
						OrientItem(sItems[i]);
					}
				}
				else if(sItems[i].mDisplayItem != null)
				{
					RemoveDisplay(sItems[i]);
				}
			}
		}
	}
}

