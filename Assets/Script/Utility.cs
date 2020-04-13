using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
	/// <summary>
	/// 要素を近い整数値に丸める
	/// </summary>
	/// <param name="vec"></param>
	/// <returns></returns>
	static public Vector3 roundVector(Vector3 vec)
	{
		return new Vector3(Mathf.Round(vec.x), Mathf.Round(vec.y));
	}
}
