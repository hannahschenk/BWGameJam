using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileInfo
{
	public enum TileType
	{
		Corridor,
		TurnRight,
		TeeLeft,
		Fork,
		Room,
		CorridorSmall,
		RoomMed,
		RoomNarrow,
		RoomCloset
	}

	public enum WallType
	{
		None,
		Railing,
		ClosedDoor,
	}

}
