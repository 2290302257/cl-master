using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Alurax
{
	[RequireComponent(typeof(CanvasRenderer))]
	[RequireComponent(typeof(Button))]
	public class UIEmptySelectable : MaskableGraphic
	{
		protected UIEmptySelectable()
		{
			useLegacyMeshGeneration = false;
		}

		public override void SetMaterialDirty()
		{
		}

		public override void SetVerticesDirty()
		{
		}

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
		}
		
#if  UNITY_EDITOR
		protected override void OnValidate()
		{
			Reset();
		}

		protected override void Reset()
		{
			base.Reset();
			
			gameObject.GetComponent<Button>().transition = Selectable.Transition.None;
		}
#endif
	}
}
