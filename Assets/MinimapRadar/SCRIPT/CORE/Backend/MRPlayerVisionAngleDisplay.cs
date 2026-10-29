using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MinimapRadar
{
	public class MRPlayerVisionAngleDisplay : MinimapItemDisplay
	{
		protected override void CustomAwake()
		{
			base.CustomAwake();
			MinimapRadar.sPlayerVisionAngle = this;
		}
	}
}
