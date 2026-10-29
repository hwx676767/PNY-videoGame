using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MinimapRadar
{
	public class MinimapItemDisplay : MonoBehaviour
	{
		public MinimapItem mCurLinkedItem;

		protected Image mImg;
		protected RectTransform mRectTransform;
		public RectTransform rectTrans
		{
			get { return mRectTransform; }
		}
		public Image img
		{
			get { return mImg; }
		}

		protected virtual void CustomAwake()
		{
			mImg = GetComponent<Image>();
			mRectTransform = GetComponent<RectTransform>();
		}

		private void Awake()
		{
			CustomAwake();
		}
	}
}


