using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileInfo;

public class Tile : MonoBehaviour
{

	public TileType TileType = TileType.Corridor;

	public int wing = -1;
	public int sidewing = -1;

	protected Color inactiveGizmo = new Color(1f, 1f, 1f, 0.25f);

	protected void OnDrawGizmosSelected()
	{
		if (!transform.Find("AnchorPoints"))
			return;

		if (transform.Find("AnchorPoints/Walls").childCount > 0) {
			foreach (Transform t in transform.Find("AnchorPoints/Walls")) {

				WallTypeTag tag = t.GetComponent<WallTypeTag>();

				Gizmos.color = Color.red;

				if (tag) {

					switch (tag.WallType) {
						case WallType.None:
							break;
						case WallType.Railing:
							Gizmos.color = Color.magenta;
							break;
						case WallType.ClosedDoor:
							Gizmos.color = Color.gray;
							break;
						default:
							break;
					}

				}

				if (!t.gameObject.activeInHierarchy)
					Gizmos.color *= inactiveGizmo;

				Gizmos.DrawCube(t.position, new Vector3(.4f, .4f, .4f));
				Gizmos.DrawLine(t.position, t.position + t.forward + t.up);
			}
		}

		if (transform.Find("AnchorPoints/Entries").childCount > 0) {
			Gizmos.color = Color.green;
			foreach (Transform t in transform.Find("AnchorPoints/Entries")) {
				if (!t.gameObject.activeInHierarchy)
					Gizmos.color *= inactiveGizmo;
				Gizmos.DrawLine(t.position, t.position + t.forward + t.up * .6f);
			}
		}

		if (transform.Find("AnchorPoints/Exits").childCount > 0) {
			Gizmos.color = Color.blue;
			foreach (Transform t in transform.Find("AnchorPoints/Exits")) {

				if (!t.gameObject.activeInHierarchy)
					Gizmos.color *= inactiveGizmo;

				Gizmos.DrawLine(t.position, t.position + t.forward + t.up);
			}
		}
	}


}
