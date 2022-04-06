using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ap_ContextMenuHelpers : Editor
{

	//private static List<GameObject> selection = new List<GameObject>();

	[MenuItem("GameObject/Snap Positions and Rotations", false, 0)]
	private static void SnapPositionsAndRotations()
	{
		Transform[] targets = Selection.transforms;

		for (int i = 0; i < targets.Length; i++) {

			//targets[i].localPosition

			Undo.RecordObject(targets[i], "Snap Positions and Rotations " + targets[i].name);
			targets[i].localPosition = ap_Utility.RoundTo(targets[i].localPosition, 3.0f);
			targets[i].eulerAngles = ap_Utility.RoundTo(targets[i].eulerAngles, 90.0f);

			//Undo.RecordObject(targets[i], "Paste Transform Values " + targets[i].name);
			//targets[i].localPosition = values[j].localPosition;
			//targets[i].localRotation = values[j].localRotation;
			//targets[i].localScale = values[j].localScale;
		}
	}

	[MenuItem("GameObject/Select Walls/+X", false, 1)]
	private static void SelectWallsRight()
	{
		SelectWalls(Vector3.right);
	}
	[MenuItem("GameObject/Select Walls/-X", false, 1)]
	private static void SelectWallsLeft()
	{
		SelectWalls(Vector3.left);
	}
	[MenuItem("GameObject/Select Walls/+Z", false, 1)]
	private static void SelectWallsForward()
	{
		SelectWalls(Vector3.forward);
	}
	[MenuItem("GameObject/Select Walls/-Z", false, 1)]
	private static void SelectWallsBackward()
	{
		SelectWalls(Vector3.back);
	}

	//[MenuItem("GameObject/Select Walls/Left", false, 1)]
	//private static void SelectWallsLeft()
	//{

	//}



	private static void SelectWalls(Vector3 goalDirection)
	{

		Transform[] targets = Selection.transforms;
		//List<Transform> walls = new List<Transform>();
		List<GameObject> walls = new List<GameObject>();

		foreach (Transform target in targets) {
			if (!(target.childCount > 0)) {
				//Debug.LogFormat("{0} has no children", target);
				continue;
			}

			foreach (Transform t in target.Find("AnchorPoints/Walls")) {

				//Vector3 direction = t.position - t.parent.position;
				Vector3 direction = t.forward;

				if (ap_Utility.IsFacing(direction, goalDirection))
					walls.Add(t.gameObject);
				//walls.Add(t);
			}
		}

		if (walls.Count == 0)
			return;
		//Debug.LogFormat("Selecting {0} objects", walls.Count);
		Selection.objects = walls.ToArray();


		//if (transform.Find("AnchorPoints/Walls").childCount > 0) {
		//	foreach (Transform t in transform.Find("AnchorPoints/Walls")) {

		//		TileTag tag = t.GetComponent<TileTag>();

		//		if (tag && tag.tileTag == TileTag.TileInfoTag.Railing)
		//			Gizmos.color = Color.magenta;
		//		else
		//			Gizmos.color = Color.red;

		//		Gizmos.DrawCube(t.position, new Vector3(.4f, .4f, .4f));
		//		Gizmos.DrawLine(t.position, t.position + t.forward + t.up);
		//	}
		//}

	}


}
