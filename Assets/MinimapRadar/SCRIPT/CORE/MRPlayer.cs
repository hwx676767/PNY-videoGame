using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MinimapRadar
{
	public class MRPlayer : MinimapItem
	{
		public bool mUseVisionAngleDisplay = false;
		public float mVisionAngleDegree = 45;

		protected override void CustomOnEnable()
		{
			MinimapRadar.RegisterCenterItem(this);
		}

		protected override void CustomOnDisable()
		{
			MinimapRadar.UnregisterCenterItem(this);
		}

		public override void SetupDisplay(MinimapItemDisplay display, MinimapRadar minimap)
		{
			base.SetupDisplay(display, minimap);
			mDisplayItem.rectTrans.anchoredPosition = Vector2.zero;
			mDisplayItem.rectTrans.localPosition = Vector3.zero;
		}
	}
}
