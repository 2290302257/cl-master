using UnityEngine;
using System.Collections.Generic;
using Alurax;


public static class TransformExtension
{
	public static void CopyFrom(this Transform self, Transform other, bool localSpace)
	{
		if (localSpace)
		{
			self.localPosition = other.localPosition;
			self.localRotation = other.localRotation;
			self.localScale = other.localScale;
		}
		else
		{
			self.position = other.position;
			self.rotation = other.rotation;
			self.SetWorldScale(other.lossyScale);
		}
	}

	public static Transform RecursivelyFind(this Transform self, string name)
	{
		int len = self.childCount;
		for (int i = 0; i < len; i++)
		{
			Transform transform = self.GetChild(i);
			if (transform.name == name)
			{
				return transform;
			}
			transform = transform.RecursivelyFind(name);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}

	public static void RecursivelyDestroyChildren(this Transform self)
	{
	    Transform[] componentsInChildren = self.GetComponentsInChildren<Transform>(true);
	    for (int i = 0; i < componentsInChildren.Length; i++)
	    {
	        if (componentsInChildren[i] != self)
	        {
	            UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
	        }
	    }
	}

	public static void RecursivelyDestroyChildrenImmediate(this Transform self)
	{
	    Transform[] componentsInChildren = self.GetComponentsInChildren<Transform>(true);
	    for (int i = componentsInChildren.Length - 1; i >= 0; i--)
	    {
	        if (componentsInChildren[i] != self)
	        {
	            UnityEngine.Object.DestroyImmediate(componentsInChildren[i].gameObject);
	        }
	    }
	}
	
	public static bool IsSiblingOf(this Transform self, Transform other)
	{
		return (self.parent == other.parent);
	}

	public static void ScaleBy(this Transform self, float scaleBy)
	{
		self.localScale = (self.localScale * scaleBy);
	}

	public static void ScaleBy(this Transform self, Vector3 scaleBy)
	{
		Vector3 localScale = self.localScale;
		localScale.Scale(scaleBy);
		self.localScale = localScale;
	}

	public static void SetWorldScale(this Transform self, Vector3 scale)
	{
		self.localScale = Vector3.one;
		Vector3 lossyScale = self.lossyScale;
		if (lossyScale.x != 0f)
		{
			scale.x /= lossyScale.x;
		}
		if (lossyScale.y != 0f)
		{
			scale.y /= lossyScale.y;
		}
		if (lossyScale.z != 0f)
		{
			scale.z /= lossyScale.z;
		}
		self.localScale = scale;
	}

	#region transform.position
	public static void SetPositionX(this Transform transform, float x)
	{
		Vector3 vector3 = transform.position;
		vector3.x = x;
		transform.position = vector3;
	}

	public static void SetPositionY(this Transform transform, float y)
	{
		Vector3 vector3 = transform.position;
		vector3.y = y;
		transform.position = vector3;
	}

	public static void SetPositionZ(this Transform transform, float z)
	{
		Vector3 vector3 = transform.position;
		vector3.z = z;
		transform.position = vector3;
	}

	public static void AddPositionX(this Transform transform, float x)
	{
		Vector3 vector3 = transform.position;
		vector3.x += x;
		transform.position = vector3;
	}
	
	public static void AddPositionY(this Transform transform, float y)
	{
		Vector3 vector3 = transform.position;
		vector3.y += y;
		transform.position = vector3;
	}

	public static void AddPositionZ(this Transform transform, float z)
	{
		Vector3 vector3 = transform.position;
		vector3.z += z;
		transform.position = vector3;
	}

	public static void SubPositionX(this Transform transform, float x)
	{
		transform.AddPositionX(-x);
	}

	public static void SubPositionY(this Transform transform, float y)
	{
		transform.AddPositionY(-y);
	}
	
	public static void SubPositionZ(this Transform transform, float z)
	{
		transform.AddPositionZ(-z);
	}

	#endregion transform.position

	#region transform.localPosition
	public static void SetLocalPositionX(this Transform transform, float x)
	{
		Vector3 vector3 = transform.localPosition;
		vector3.x = x;
		transform.localPosition = vector3;
	}

	public static void SetLocalPositionY(this Transform transform, float y)
	{
		Vector3 vector3 = transform.localPosition;
		vector3.y = y;
		transform.localPosition = vector3;
	}

	public static void SetLocalPositionZ(this Transform transform, float z)
	{
		Vector3 vector3 = transform.localPosition;
		vector3.z = z;
		transform.localPosition = vector3;
	}

	public static void AddLocalPositionX(this Transform transform, float x)
	{
		Vector3 vector3 = transform.localPosition;
		vector3.x += x;
		transform.localPosition = vector3;
	}
	
	public static void AddLocalPositionY(this Transform transform, float y)
	{
		Vector3 vector3 = transform.localPosition;
		vector3.y += y;
		transform.localPosition = vector3;
	}

	public static void AddLocalPositionZ(this Transform transform, float z)
	{
		Vector3 vector3 = transform.localPosition;
		vector3.z += z;
		transform.localPosition = vector3;
	}

	public static void SubLocalPositionX(this Transform transform, float x)
	{
		transform.AddLocalPositionX(-x);
	}

	public static void SubLocalPositionY(this Transform transform, float y)
	{
		transform.AddLocalPositionY(-y);
	}
	public static void SubLocalPositionZ(this Transform transform, float z)
	{
		transform.AddLocalPositionZ(-z);
	}

	#endregion transform.localPosition

	#region transform.localScale
	public static void SetLocalScaleX(this Transform transform, float x)
	{
		Vector3 vector3 = transform.localScale;
		vector3.x = x;
		transform.localScale = vector3;
	}

	public static void SetLocalScaleY(this Transform transform, float y)
	{
		Vector3 vector3 = transform.localScale;
		vector3.y = y;
		transform.localScale = vector3;
	}

	public static void SetLocalScaleZ(this Transform transform, float z)
	{
		Vector3 vector3 = transform.localScale;
		vector3.z = z;
		transform.localScale = vector3;
	}

	public static void AddLocalScaleX(this Transform transform, float x)
	{
		Vector3 vector3 = transform.localScale;
		vector3.x += x;
		transform.localScale = vector3;
	}
	
	public static void AddLocalScaleY(this Transform transform, float y)
	{
		Vector3 vector3 = transform.localScale;
		vector3.y += y;
		transform.localScale = vector3;
	}

	public static void AddLocalScaleZ(this Transform transform, float z)
	{
		Vector3 vector3 = transform.localScale;
		vector3.z += z;
		transform.localScale = vector3;
	}

	public static void SubLocalScaleX(this Transform transform, float x)
	{
		transform.AddLocalScaleX(-x);
	}

	public static void SubLocalScaleY(this Transform transform, float y)
	{
		transform.AddLocalScaleY(-y);
	}
	
	public static void SubLocalScaleZ(this Transform transform, float z)
	{
		transform.AddLocalScaleZ(-z);
	}

	#endregion transform.localScale

	#region transform.localEulerAngles
	public static void SetLocalEulerAnglesX(this Transform transform, float x)
	{
		Vector3 vector3 = transform.localEulerAngles;
		vector3.x = x;
		transform.localEulerAngles = vector3;
	}

	public static void SetLocalEulerAnglesY(this Transform transform, float y)
	{
		Vector3 vector3 = transform.localEulerAngles;
		vector3.y = y;
		transform.localEulerAngles = vector3;
	}

	public static void SetLocalEulerAnglesZ(this Transform transform, float z)
	{
		Vector3 vector3 = transform.localEulerAngles;
		vector3.z = z;
		transform.localEulerAngles = vector3;
	}

	public static void AddLocalEulerAnglesX(this Transform transform, float x)
	{
		Vector3 vector3 = transform.localEulerAngles;
		vector3.x += x;
		transform.localEulerAngles = vector3;
	}

	public static void AddLocalEulerAnglesY(this Transform transform, float y)
	{
		Vector3 vector3 = transform.localEulerAngles;
		vector3.y += y;
		transform.localEulerAngles = vector3;
	}

	public static void AddLocalEulerAnglesZ(this Transform transform, float z)
	{
		Vector3 vector3 = transform.localEulerAngles;
		vector3.z += z;
		transform.localEulerAngles = vector3;
	}

	public static void SubLocalEulerAnglesX(this Transform transform, float x)
	{
		transform.AddLocalEulerAnglesX(-x);
	}

	public static void SubLocalEulerAnglesY(this Transform transform, float y)
	{
		transform.AddLocalEulerAnglesY(-y);
	}
	
	public static void SubLocalEulerAnglesZ(this Transform transform, float z)
	{
		transform.AddLocalEulerAnglesZ(-z);
	}

	#endregion transform.localEulerAngles

	#region transform.localRotation
	public static void SetLocalRotationX(this Transform transform, float x)
	{
		Vector3 vector3 = transform.localEulerAngles;
		vector3.x = x;
		transform.localEulerAngles = vector3;
	}

	public static void SetLocalRotationY(this Transform transform, float y)
	{
		Vector3 vector3 = transform.localEulerAngles;
		vector3.y = y;
		transform.localEulerAngles = vector3;
	}

	public static void SetLocalRotationZ(this Transform transform, float z)
	{
		Vector3 vector3 = transform.localEulerAngles;
		vector3.z = z;
		transform.localEulerAngles = vector3;
	}

	public static void AddLocalRotationX(this Transform transform, float x)
	{
		Quaternion vector3 = transform.localRotation;
		vector3 *= Quaternion.AngleAxis(x, Vector3.right);
		transform.localRotation = vector3;
	}
	
	public static void AddLocalRotationY(this Transform transform, float y)
	{
		Quaternion vector3 = transform.localRotation;
		vector3 *= Quaternion.AngleAxis(y, Vector3.up);
		transform.localRotation = vector3;
	}

	public static void AddLocalRotationZ(this Transform transform, float z)
	{
		Quaternion vector3 = transform.localRotation;
		vector3 *= Quaternion.AngleAxis(z, Vector3.forward);
		transform.localRotation = vector3;
	}

	public static void SubLocalRotationX(this Transform transform, float x)
	{
		transform.AddLocalRotationX(-x);
	}

	public static void SubLocalRotationY(this Transform transform, float y)
	{
		transform.AddLocalRotationY(-y);
	}
	
	public static void SubLocalRotationZ(this Transform transform, float z)
	{
		transform.AddLocalRotationZ(-z);
	}

	#endregion transform.localRotation

	#region transform.lossyScale
	public static void SetLossyScale(this Transform trans, Vector3 lossyScale)
	{
		var parent = trans.parent;
		var localScaleOld = trans.localScale;
		if (parent == null)
		{
			trans.localScale = lossyScale;
		}
		else
		{
			//此方法对于parent有旋转时是不正确的, 需要改进//
			if (!Mathf.Approximately(parent.lossyScale.x, 0) && !Mathf.Approximately(parent.lossyScale.y, 0) && !Mathf.Approximately(parent.lossyScale.z, 0))
			{
				trans.localScale = new Vector3(lossyScale.x / parent.lossyScale.x, lossyScale.y / parent.lossyScale.y, lossyScale.z / parent.lossyScale.z);
			}
		}

		if (!Mathf.Approximately(trans.lossyScale.x, lossyScale.x) ||
		    !Mathf.Approximately(trans.lossyScale.y, lossyScale.y) ||
		    !Mathf.Approximately(trans.lossyScale.z, lossyScale.z))
		{
			trans.localScale = localScaleOld;
			Log.E($"SetLossyScale failed:{trans.name}");
		}

	}

	#endregion
	
	#region material.color

	public static void SetAlpha(this Material material, float a)
	{
		material.color = new Color(material.color.r, material.color.g, material.color.b, a);
	}

	public static void SetAlpha(this Material material, string key, float a)
	{
		Color c = material.GetColor(key);
		material.color = new Color(c.r, c.g, c.b, a);
	}

	public static void SetColor(this Renderer renderer, string type, Color color)
	{
		if (renderer != null)
		{
			for (int i = 0; i < renderer.materials.Length; i++)
			{
				renderer.materials[i].SetColor(type, color);
			}
		}
	}

	#endregion

	#region tools

	static List<Component> _hasCompList = new List<Component>(1);
	public static bool HasComponent<T>(this Transform trans) where T : Component
	{
		trans.GetComponents(typeof(T), _hasCompList);
		return _hasCompList.Count > 0;
	}

	public static int GetActiveChildCount(this Transform parent)
	{
		int result = 0;
		for (int i = 0; i < parent.childCount; i++)
		{
			if (parent.GetChild(i).gameObject.activeSelf)
			{
				result++;
			}
		}
		return result;
	}

	public static void ActionOnChildren(this Transform parent, System.Action<Transform> action, bool includeSelf = true)
	{
		if (includeSelf) action(parent);
		foreach (Transform child in parent)
		{
			ActionOnChildren(child, action);
		}
	}

	public static void DestroyChildren(this Transform trans)
	{
		foreach (Transform child in trans)
		{
			GameObject.Destroy(child.gameObject);
		}
	}


	public static void DestroyChildren(this Transform trans, string childName)
	{
		foreach (Transform child in trans)
		{
			if (childName == child.name)
			{
				GameObject.Destroy(child.gameObject);
			}
		}
	}

	public static Transform AddChildFromPrefab(this Transform trans, Transform prefab, string name = null)
	{
		Transform childTrans = GameObject.Instantiate(prefab) as Transform;
		childTrans.SetParent(trans, false);
		if (name != null)
		{
			childTrans.gameObject.name = name;
		}
		return childTrans;
	}

	public static void SetSiblingIndexAfter(this Transform trans, Transform targetTrans)
	{
		trans.SetSiblingIndex(
			Mathf.Max(trans.GetSiblingIndex(), targetTrans.GetSiblingIndex()));
	}

	#endregion
	
	#region animator.speed

	public static void SetAnimatorSpeed(this Transform transform, float speed)
	{
		Animator[] animators = transform.GetComponentsInChildren<Animator>();

		for (int i = 0; i < animators.Length; i++)
		{
			animators[i].speed = speed;
		}
	}

	#endregion

	#region RectTransform.anchoredPosition
	public static void SetAnchoredPositionX(this RectTransform trans, float x)
	{
		Vector2 pos = trans.anchoredPosition;
		pos.x = x;
		trans.anchoredPosition = pos;
	}

	public static void SetAnchoredPositionY(this RectTransform trans, float y)
	{
		Vector2 pos = trans.anchoredPosition;
		pos.y = y;
		trans.anchoredPosition = pos;
	}
	#endregion RectTransform.anchoredPosition
	
	#region transform.child
	public static string GetRelativePath(this Transform parent, Transform child)
	{
		if (child == null)
			return string.Empty;

		string path = child.name;
		while (child.parent != parent && child.parent != null)
		{
			child = child.parent;
			path = child.name + "/" + path;
		}

		return path;
	}
	#endregion
}	


