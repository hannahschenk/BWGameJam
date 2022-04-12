using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ap_Utility
{

	public static int layer_floor = 6;

	/// <summary>
	/// Rounds each component of a vector to the nearest integer
	/// </summary>
	/// <param name="v"></param>
	/// <returns></returns>
	public static Vector3 Round(Vector3 v)
	{
		return new Vector3(
			Mathf.Round(v.x),
			Mathf.Round(v.y),
			Mathf.Round(v.z)
			);
	}

	/// <summary>
	/// Rounds each component of a vector to a multiple of another number (e.g., every 3 units, etc)
	/// </summary>
	/// <param name="value"></param>
	/// <param name="multipleOf"></param>
	/// <returns></returns>
	public static Vector3 RoundTo(Vector3 value, float multipleOf)
	{
		//Vector3 v = Round(value / multipleOf) * multipleOf;
		//Debug.LogFormat("ROUNDING TO MULTIPLE OF 3. INPUT: {0}, OUTPUT: {1}", value, v);
		//return v;
		return Round(value / multipleOf) * multipleOf;
	}

	/// <summary>
	/// Rounds a float to a multiple of another number (e.g., every 3 units, etc)
	/// </summary>
	/// <param name="value"></param>
	/// <param name="multipleOf"></param>
	/// <returns></returns>
	public static float RoundTo(float value, float multipleOf)
	{
		return Mathf.Round(value / multipleOf) * multipleOf;
	}

	/// <summary>
	/// Is direction facing goalDirection? (e.g., where -1 is opposite, 0 is perpendicular, >0 facing)
	/// </summary>
	/// <returns></returns>
	public static bool IsFacing(Vector3 direction, Vector3 goalDirection)
	{
		return Vector3.Dot(direction, goalDirection) > 0.1;
	}

	/// <summary>
	/// Returns value describing if targetDir is to the left (negative) or right (positive) of forward
	/// </summary>
	/// <param name="fwd"></param>
	/// <param name="targetDir"></param>
	/// <param name="up"></param>
	/// <returns></returns>
	public static float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
	{
		Vector3 perp = Vector3.Cross(fwd, targetDir);
		return Vector3.Dot(perp, up);
	}

	/// <summary>
	/// Returns a list of walls who are offset (e.g., facing) along a world direction axis
	/// </summary>
	/// <param name="tile"></param>
	/// <param name="goalDirection"></param>
	/// <returns></returns>
	public static List<Transform> GetWallsAlongDirection(Tile tile, Vector3 goalDirection)
	{

		//Transform[] targets = Selection.transforms;
		List<Transform> walls = new List<Transform>();

		//foreach (Transform target in targets) {
		if ((tile.transform.childCount > 0)) {

			foreach (Transform t in tile.transform.Find("AnchorPoints/Walls")) {

				Vector3 direction = t.position - t.parent.position;
				if (ap_Utility.IsFacing(direction, goalDirection))
					walls.Add(t);

			}
		}
		//}

		return walls;
	}

}
