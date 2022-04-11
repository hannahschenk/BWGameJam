using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TileInfo;

public class ApartmentBuilder : MonoBehaviour
{
	//Build Parameters
	[Tooltip("Represents how closely tiles can be placed to prevent overlap")]
	public const float tileGridSnap = 3.0f;
	public int maxFloors = 1;

	public Vector2 roomsPerFloorRange = new Vector2(4.0f, 8.0f);

	//public Vector2 closedDoorsPerFloorRange = new Vector2(6.0f, 10.0f);
	public float closedDoorChance = 0.5f;

	//Need to find existing assets
	public bool usePrebuiltFloor = false;
	public GameObject Floor; //Will dynamically assign this in the future, once it's part of template selection

	// Tiles
	public Tile startRoomType;
	public Tile endRoomType;

	// Wing blocks
	public Tile[] corridorTypes; //10x2
	public Tile[] rightTurnTypes; //4x2, exit is 3 units from entry (e.g. 2x2 square + 1/2 of 2x2 square)
	public Tile[] teeLeftTypes;  //10x2 with Fork in middle?
	public Tile[] forkTypes;

	// Side Path Blocks
	public Tile[] roomTypes;
	public Tile[] smallCorridorTypes; //6x2
	public Tile[] leftTurnTypes;

	// Walls
	public GameObject[] wallTypes;
	public GameObject[] railingTypes;
	public GameObject[] closedDoorTypes;
	public GameObject[] exteriorPanels;

	// Rooms
	public Tile[] roomComponentTypes;

	//Build data
	protected Transform currentFloor; //empty wing gameobject for organization purposes
	protected float elevation = 0f;
	protected float floorHeight = 2.7f;
	protected int floorsBuilt = 0;

	List<Vector3> tileCoords = new List<Vector3>();
	List<Tile> placedTiles = new List<Tile>();

	//List<Transform> availableEntries = new List<Transform>();
	List<Transform> availableExits = new List<Transform>();
	List<Transform> availableWalls = new List<Transform>();
	//if we stored these in a linked array or a dictionary, we could be able to check for availableWalls by looking up specific positions...

	List<Transform> closedDoors = new List<Transform>();

	Dictionary<Tile, Transform> rootRoomTiles = new Dictionary<Tile, Transform>(); //to prevent putting rooms to the same tile.
	Dictionary<Tile, int> roomComponentsArea = new Dictionary<Tile, int>();

	public bool buildRailings = true;
	public int currentSeed = 0;

	[Range(0, 3)]
	public int debugForksPerSegment = 0;

	//State
	//protected bool makeLevel = false;
	//protected int wingsBuilt = 0; //every floor will be 3 main wings (with start/end as additional, small wings) to a total of 5
	//protected int forksPerWing = 0;
	//protected int corrsPerSegment = 1;

	// Start is called before the first frame update
	void Start()
	{
		CheckRoomComponents();

		GenerateApartmentParams();

		NewLevel();
	}

	// Check size of Room Component tiles so we can store their area, for choosing which room components to use later on.
	protected void CheckRoomComponents()
	{
		for (int i = 0; i < roomComponentTypes.Length; i++) {
			Tile tile = roomComponentTypes[i];

			Renderer renderer = tile.GetComponentInChildren<Renderer>();
			//Collider renderer = tile.GetComponentInChildren<Collider>();
			//MeshCollider renderer = tile.GetComponentInChildren<MeshCollider>();
			

			Bounds bounds = renderer.bounds;

			roomComponentsArea.Add(tile, Mathf.RoundToInt(bounds.size.x * bounds.size.z));
			//Debug.LogFormat("Tile {0} has an area of {1}", roomComponentTypes[i], Mathf.RoundToInt(bounds.size.x * bounds.size.z));
		}
	}

	protected struct FloorParams
	{
		public int maxRoomsPerFloor;
	}

	FloorParams[] floorParams;

	protected void GenerateApartmentParams()
	{
		floorParams = new FloorParams[maxFloors + 1];
		for (int i = 1; i < maxFloors + 1; i++) {
			floorParams[i].maxRoomsPerFloor = Mathf.RoundToInt(Random.Range(roomsPerFloorRange.x, roomsPerFloorRange.y));
		}

	}

	protected void EmptyLevel()
	{
		foreach (Transform t in transform) {
			Destroy(t.gameObject);
		}
		//tileCoords = new List<Vector3>(); //Why is this here, and not in ClearVars()?
		ClearVars();
	}

	protected void ClearVars()
	{
		//tileCoords = new List<Vector3>();
		//availableExits.RemoveAll(i => i == null);
		//placedTiles.RemoveAll(i => i == null);

		tileCoords.Clear();
		availableExits.Clear();
		availableWalls.Clear();
		placedTiles.Clear();

		rootRoomTiles.Clear();

	}

	protected void NewLevel()
	{
		if (currentSeed == 0)
			currentSeed = (int)System.DateTime.Now.Ticks;

		Random.InitState(currentSeed);

		EmptyLevel();

		if (debugRoomTesting)
			return;

		MakeLevel();
	}

	protected void Update()
	{
		if (debugRoomTesting)
			BuildBetweenTwoPoints();
	}

	// Update is called once per frame
	//  protected void Update()
	//  {
	//      if (makeLevel) {
	//	MakeLevel();
	//	return;
	//}
	//  }

	protected void MakeLevel()
	{
		if (usePrebuiltFloor) {
			CheckExistingFloor();
			BuildWalls();
		} else {

			elevation = 0 - (maxFloors / 2) * floorHeight; //test elevation, start at middle floor
														   //elevation = 0 - (floorHeight * 2.0f); //test elevation, start at floor 2

			while (floorsBuilt < maxFloors) {
				floorsBuilt++;

				BuildFloor();

				//Debug.LogFormat("Built floor, {0} available exits left",availableExits.Count);

				TagCourtyardRailings();

				BuildSideWings();

				//Debug.LogFormat("Built side wings, {0} available exits left", availableExits.Count);

				FindSuitableRoomPositions();

				//Debug.LogFormat("Built rooms, {0} available exits left", availableExits.Count);

				BuildWalls();

				// Build Exterior Panel
				Vector3 panelPos = new Vector3(3f, elevation, -3f);
				//Quaternion.FromToRotation(Vector3.forward, Vector3.right);
				Instantiate(exteriorPanels[Random.Range(0, exteriorPanels.Length)], panelPos, Quaternion.FromToRotation(Vector3.forward, Vector3.right), currentFloor);

				elevation += floorHeight;
				ClearVars();
			}
		}


		//		BuildWalls(); //Probably the last step, would also pave over unused exits, etc.

		//makeLevel = false;
	}

	protected void CheckExistingFloor()
	{
		for (int i = 0; i < Floor.transform.childCount; i++) {

			Transform currentTile = Floor.transform.GetChild(i);
			if (!currentTile)
				return;

			CheckAllTileFeatures(currentTile);
		}
	}

	protected void BuildFloor()
	{

		currentFloor = new GameObject("Floor " + floorsBuilt).transform;
		currentFloor.position = Vector3.up * elevation;
		currentFloor.SetParent(transform);

		Tile startTile = Instantiate(startRoomType, new Vector3(0f, elevation, 0f), Quaternion.identity, currentFloor);
		RecordTile(startTile);

		//startRoomType.transform.size

		//BuildTile(startRoomType, currentFloor, out Tile startTile);

		BuildWing(2, startTile); //starts with the final parts of a wing (corridor + right turn), and continues to build 3 full wings
	}


	//Before we build the side paths, mark the right-hand walls of all transitory spaces as Railings
	protected void TagCourtyardRailings()
	{

		for (int i = 0; i < placedTiles.Count; i++) {

			if (placedTiles[i].TileType == TileType.Room)
				continue;

			Transform entry = GetEntry(placedTiles[i]);
			Vector3 right = entry.right;

			foreach (Transform t in placedTiles[i].transform.Find("AnchorPoints/Walls")) {
				Vector3 wallDirection = t.forward;
				bool isFacingCourtyard = false;

				isFacingCourtyard = ap_Utility.IsFacing(wallDirection, right);

				//Additional logic for RightTurn tiles, their courtyard-facing wall also includes the south wall after the turn
				if (!isFacingCourtyard && placedTiles[i].TileType == TileType.TurnRight)
					isFacingCourtyard = ap_Utility.IsFacing(wallDirection, -entry.forward);

				if (isFacingCourtyard) {
					WallTypeTag tag = t.gameObject.AddComponent<WallTypeTag>();
					tag.WallType = WallType.Railing;
				}
			}
		}
	}

	/// <summary>
	/// Builds an apartment wing such that the final tile is always a Right Turn, and the penultimate tile is always a corridor
	/// </summary>
	protected void BuildWing(int _remainingRooms, Tile _lastTile)
	{

		int forksPerWing = Random.Range(1, 4); //1-3 forks
		int forksBuilt = 0;

		if (debugForksPerSegment > 0)
			forksPerWing = debugForksPerSegment;

		Vector3 forkSlot;
		switch (forksPerWing) {
			case 3:
				forkSlot = new Vector3(1f, 3f, 5f);
				break;
			case 2:
				forkSlot = Random.Range(0, 2) == 0 ? new Vector3(1f, 5f) : new Vector3(2f, 4f); //2 forks can either be in slots 1+5, or slots 2+4
				break;
			case 1:
				forkSlot = new Vector3(3f, 0f, 0f);
				break;
			default:
				forkSlot = Vector3.zero;
				break;
		}

		//Debug.LogFormat("Floor {0} should have {1} forks", floorsBuilt, forksPerWing);
		//int segmentSpacer = 0;

		//int corrsPerSegment = Random.Range(1, 2);
		//int corrsPerSegment = 2;

		//RULES ------
		//int _corrsBuilt = 0;

		//int segmentSpacer = Random.Range(0, 1);
		//Tile segmentSpacerType = Random.Range(0, 1) > 0 ? corridorTypes[0] : teeLeftTypes[0];


		//int segments = 2;

		//int totalRoomsPerWing = (corrsPerSegment * segments + forksPerWing + segmentSpacer * segments) + 1; //+1 because the final hallway is always a Right Turn
		//int totalRoomsPerWing = (corrsPerSegment * segments + 1) + 1;
		int totalRoomsPerWing = 5 + 1;

		//e.g., 2 corrs * 2 segments + 1 fork + 1 = 6 rooms [corr, corr + fork + corr, corr + turn]
		//int totalRoomsPerWing = 6;

		int maxWings = 5;
		int wingsBuilt = 1;
		int currentTileSlot = 0;
		while (_remainingRooms > 0 && wingsBuilt <= maxWings) {

			Transform joiningPoint = null;

			List<Transform> validExits = GetAvailableExits(_lastTile);

			//e.g., what if no exits?
			if (validExits.Count <= 0) {
				//Debug.Log("No valid exits remaining!");
				return;
			}

			//joiningPoint = availableExits[Random.Range(0, availableExits.Count)];
			//joiningPoint = validExits[Random.Range(0, validExits.Count)];

			if (validExits.Count > 1) {
				joiningPoint = validExits[Random.Range(0, validExits.Count)];
				if (_lastTile.TileType == TileType.TeeLeft) {
					Vector3 forward = GetEntry(_lastTile).forward;
					foreach (Transform exit in validExits) {
						if (ap_Utility.IsFacing(exit.forward, forward)) { //try to use the forward path for the main wing
																		   //Debug.LogFormat("{0} appears to be forwards in the {1} tee", exit, _lastTile);
							joiningPoint = exit;
							break;
						}
					}
				}
			} else {
				joiningPoint = validExits[0];
			}

			if (!joiningPoint) {
				//Debug.Log("Error, couldn't find an exit to build from!");
				return;
			}

			//Debug.LogFormat("Next tile should join to exit at {0}", joiningPoint.position);

			currentTileSlot++;

			//Determine tile type to add
			Tile newTileType = null;
			if (_remainingRooms > 1) {
				//corridors, tees, etc

				if (forksPerWing > 0f && currentTileSlot == forkSlot[forksBuilt]) {
					newTileType = teeLeftTypes[Random.Range(0, teeLeftTypes.Length)];
					forksBuilt++;
				} else {
					newTileType = corridorTypes[Random.Range(0, corridorTypes.Length)];
				}

				//OLD LOGIC
				//if (_corrsBuilt < corrsPerSegment || forksPerWing < 1) {
				//	//Debug.Log("Building Corridor...");
				//	newTileType = corridorTypes[Random.Range(0, corridorTypes.Length)];
				//	_corrsBuilt++;
				//} else {
				//	//Debug.Log("Built segment, now building Tee Left...");
				//	newTileType = teeLeftTypes[Random.Range(0, teeLeftTypes.Length)];
				//	_corrsBuilt = 0;
				//}

			} else {
				//Debug.Log("Ending Wing, building Right Turn...");
				newTileType = rightTurnTypes[Random.Range(0, rightTurnTypes.Length)];
			}

			if (wingsBuilt == maxWings && currentTileSlot == 2) { //e.g., we're on the last wing, and have built 1 tile, early exit
				newTileType = endRoomType;
				_remainingRooms = 0;
			}

			if (!newTileType) {
				//Debug.Log("Error, couldn't find a new tile to build!");
				return;
			}

			BuildTile(newTileType, joiningPoint, out _lastTile);

			_remainingRooms--;
			if (_remainingRooms == 0) {
				wingsBuilt++;
				forksBuilt = 0;
				currentTileSlot = 0;
				//_corrsBuilt = 0;
				_remainingRooms = totalRoomsPerWing;
			}
		}
	}

	/// <summary>
	/// Calculates the new position for a tile, lines up its Entry to the joiningPoint, checks for collision, builds the tile and returns a reference to it.
	/// </summary>
	/// <param name="newTileType">The type of tile to build</param>
	/// <param name="joiningPoint">The transform reference whose position and facing will be matched</param>
	/// <param name="newTile">The built tile</param>
	protected void BuildTile(Tile newTileType, Transform joiningPoint, out Tile newTile, float snap = tileGridSnap)
	{
		Vector3 tilePos = GetNewTilePosition(newTileType, joiningPoint);
		BuildTile(newTileType, joiningPoint, tilePos, out newTile, snap);
	}


	protected void BuildTile(Tile newTileType, Transform joiningPoint, Vector3 tilePos, out Tile newTile, float snap = tileGridSnap)
	{
		Vector3 blockCoordinates = GetBlockCoordinates(tilePos, snap);

		if (!tileCoords.Contains(blockCoordinates)) {
			newTile = Instantiate(newTileType, tilePos, joiningPoint.rotation, currentFloor);
			RecordTile(newTile);
		} else {
			newTile = null;
			Debug.LogFormat("Couldn't place {0} tile at coords, already occupied", newTileType);
		}

		//availableExits.Remove(joiningPoint);
		if (!availableExits.Remove(joiningPoint))
			availableWalls.Remove(joiningPoint);
	}

	protected void BuildSideWings()
	{
		
		Transform[] wingExits = availableExits.ToArray();
		//Debug.LogFormat("Building Sidewings for Floor {0}, starting with {1} available exits (before adding paths)", currentFloor, wingExits.Length);

		//availableExits.CopyTo(wingExits);

		for (int i = 0; i < wingExits.Length; i++) {

			Transform joiningPoint = wingExits[i];
			Tile exitParentTile = joiningPoint.GetComponentInParent<Tile>();
			//Tile.TileType sourceForkType;
			//if (exitParentTile)
			//	sourceForkType = exitParentTile.TileType;
			//else {
			//	Debug.Log("ERROR: Couldn't find exit {0}'s parent tile type", joiningPoint);
			//	return;
			//}

			//Tile.TileType sourceForkType = joiningPoint.GetComponentInParent<Tile>().TileType;

			Tile newTileType = null;
			int segment = 0;

			for (int j = 0; j < 3; j++) {

				if (j < 2) {
					newTileType = corridorTypes[Random.Range(0, corridorTypes.Length)];
				} else {

					//if (sourceForkType == Tile.TileType.TeeLeft)
					//	newTileType = forkTypes[Random.Range(0, forkTypes.Length)];
					//else
					//	newTileType = roomTypes[Random.Range(0, roomTypes.Length)];

					if (segment < 1) {
						//Debug.Log("Building Fork");
						newTileType = forkTypes[Random.Range(0, forkTypes.Length)];
						//j = 0;
						segment++;
					} /*else {
						Debug.Log("End of Sidepath, building Room");
						newTileType = roomTypes[Random.Range(0, roomTypes.Length)];
						//continue;
					}*/
				}

				if (newTileType) {

					BuildTile(newTileType, joiningPoint, out Tile newTile);

					if (!newTile) {
						Debug.Log("Couldn't build sidepath segment at {0}", exitParentTile.gameObject);
						break;
					}

					if (newTile.TileType == TileType.Fork) {
						//Debug.LogFormat("Storing New Fork, it has {0} exits available", GetAvailableExits(newTile).Count);
						exitParentTile = newTile; //remember this tile for return
					}

					joiningPoint = GetRandomExit(newTile);

				}

				if (exitParentTile.TileType == TileType.Fork && j > 1 && GetAvailableExits(exitParentTile).Count > 0) {
					//Debug.Log("Built Sidepaths A from Fork, Attempting to return to Fork");
					joiningPoint = GetRandomExit(exitParentTile);
					j = 0;
				}


				//List<Transform> availableExits

				//joiningPoint = GetAvailableExits(newTile)[0];

			}

		}

		//Debug.LogFormat("Finished building side paths, remaining wing exits is {0}, available exits is {1}", wingExits.Length, availableExits.Count);


		//while (building) {


		//	building = false;
		//}

		//BuildBetweenTwoPoints();

	}

	protected void FindSuitableRoomPositions()
	{
		// All possible room positions (any wall or exit can become a room)
		Transform[] possibleRoomPositions = new Transform[availableExits.Count + availableWalls.Count];
		availableExits.CopyTo(possibleRoomPositions);
		availableWalls.CopyTo(possibleRoomPositions, availableExits.Count);


		int maxRoomsPerFloor = floorParams[floorsBuilt].maxRoomsPerFloor;
		//int maxRoomsPerFloor = Mathf.RoundToInt(Random.Range(roomsPerFloorRange.x, roomsPerFloorRange.y));

		// List of actual rooms to build, after rolling for amount to build
		List<Transform> roomPositions = GetRandomRoomNodesAndRemoveFromArray(maxRoomsPerFloor, possibleRoomPositions);

		// Decorate the remaning room positions
		AddClosedDoors(possibleRoomPositions);

		// Assemble method -- messy, builds on itself or the hallway, hard to control
		//AssembleRoomsFromBlocks(roomPositions);
		AssembleRoomsFromBlocks2(roomPositions);

		// Block out method
		// CreateRoomBoundaries(roomPositions);

		// Prebuilt method
		//PlacePrebuiltRooms(roomPositions);

	}

	protected void AddClosedDoors(Transform[] possibleRoomPositions)
	{
		foreach (Transform currentAnchor in possibleRoomPositions) {
			if (currentAnchor == null)
				continue;

			float closedDoor = Random.Range(0f, 1.0f);

			if (closedDoor < closedDoorChance)
				continue;

			Tile parentTile = currentAnchor.GetComponentInParent<Tile>();
			if (ShouldBuildDoorHere(currentAnchor, parentTile, true)) {
				currentAnchor.gameObject.AddComponent<WallTypeTag>().WallType = WallType.ClosedDoor;
			}
		}
	}

	protected int maxRoomArea = 144; //96? //Could be a range...
	protected float returnToRootRoomProbability = 0.5f; //Chance the room builder will return to fork from a root room (medium) rather than continue building

	public bool debugRoomAssembly = false;

	protected void AssembleRoomsFromBlocks2(List<Transform> roomPositions)
	{
		int failedRooms = 0;
		int totalRoomsBuilt = 0;
		int totalAreaLeft = 0;

		//foreach (Transform roomStartNode in roomPositions) {
		for (int i = 0; i < roomPositions.Count; i++) {

			Tile currentTile = null;
			List<Tile> roomsToBuild = new List<Tile>(); //component blocks in the rooms to build
			List<Tile> candidateTiles = new List<Tile>(roomComponentTypes); //Candidate tiles that we can build
			Dictionary<Tile, Vector3> candidateTilesAtPositions = new Dictionary<Tile, Vector3>();

			int _remainingArea = Random.Range(96, maxRoomArea);

			//Transform joiningPoint = roomPositions[i];
			List<Transform> joiningPoints = new List<Transform>();

			joiningPoints.Add(roomPositions[i]);

			int roomsBuilt = 0;
			//int rewinds = 0;

			if (debugRoomAssembly)
				Debug.LogFormat("-------------------Room {0} started!-------------------------", i);

			// Try to place room tiles, ensuring they fit our area budget, and can be placed without any collisions.
			while (_remainingArea > 0) {

				candidateTiles = CheckTilesArea(candidateTiles, _remainingArea);

				if (debugRoomAssembly)
					Debug.LogFormat("Room {0}: {1} tiles can fit in the room area", i, candidateTiles.Count);

				bool fitTile = false;
				int currentJoiningPoint = Random.Range(0, joiningPoints.Count);
				int attempts = 0;
				Transform joiningPoint = null;
				while (!fitTile && attempts < joiningPoints.Count) {
					fitTile = CanFitTile(candidateTiles, joiningPoints[currentJoiningPoint], out candidateTilesAtPositions);

					if (fitTile) {
						joiningPoint = joiningPoints[currentJoiningPoint];
						break;
					}

					currentJoiningPoint = (currentJoiningPoint + 1) % joiningPoints.Count;
					attempts++;
				}

				//if (attempts > 0) {
					//Debug.LogFormat("Floor {0} Room {1}: Fit a room at an alternate exit", floorsBuilt, i);
				//}

				// NEED TO SOMEHOW BE ABLE TO GO /BACK/ TO TRY ANOTHER ROOM STARTING NODE (e.g., more attached to the selection process for nodes)
				// Might make more sense to 'build' rooms as we select them, instead of being so separated.
				if (!fitTile && roomsBuilt == 0) {
					Debug.LogFormat("ERROR: Floor {0} Room {0} - No rooms built, and couldn't fit any tiles here at all", floorsBuilt, i);
					//currentFloor = new GameObject("Floor " + floorsBuilt).transform;
					GameObject error = new GameObject("ERROR: Room " + i);
					error.transform.SetParent(joiningPoints[0]);
				}


				if (!fitTile && roomsBuilt > 0) {
					if (debugRoomAssembly)
						Debug.LogFormat("Room {0}: Couldn't fit any more tiles to {1}, can't backtrack any further, remaining area: {2}", i, currentTile, _remainingArea);
				}



				//if (!fitTile && roomsBuilt > 0) { //e.g., not the first tile

				//	// No exits at our current tile, go back a tile
				//	int lastTileIndex = placedTiles.IndexOf(currentTile) - 1;

				//	if (roomsBuilt <= rewinds) {
				//		//if (debugRoomAssembly)
				//			Debug.LogFormat("Room {0}: Couldn't fit any more tiles to {1}, can't backtrack any further, remaining area: {2}", i, currentTile, _remainingArea);
				//		break;
				//	}

				//	Debug.LogFormat("Room {0}: Couldn't fit any more tiles to {1}, rewinding...", i, currentTile);

				//	rewinds++;
				//	joiningPoints.Clear();
				//	joiningPoints = GetAvailableExits(placedTiles[lastTileIndex]);
				//	//joiningPoints.AddRange(GetAvailableExits(placedTiles[lastTileIndex]));
				//	continue;

				//}


				/*while (!fitTile) {

					List<Transform> entries = GetEntries()


					Tile parentTile = joiningPoint.GetComponentInParent<Tile>();



				}*/
				// Tricky edge case -- what if the tile can't fit, and it's the 


				if (candidateTilesAtPositions.Count <= 0) {
					//e.g., restart, report this failure somewhere....
					failedRooms++;
					break;
				}

				// Success! We can build from our list

				List<Tile> selectedTiles = Enumerable.ToList(candidateTilesAtPositions.Keys);
				Tile newTileType = selectedTiles[Random.Range(0, candidateTilesAtPositions.Count)];
				candidateTilesAtPositions.TryGetValue(newTileType, out Vector3 pos);

				// We can even be picky here, if it's not building enough medium rooms!


				roomComponentsArea.TryGetValue(newTileType, out int area);
				_remainingArea -= area;

				if (debugRoomAssembly)
					Debug.LogFormat("Room {0}: Success! Building tile {1}, remaining area: {2}", i, newTileType, _remainingArea);


				BuildTile(newTileType, joiningPoint, pos, out currentTile, 1f);
				roomsBuilt++;

				joiningPoints.Remove(joiningPoint);
				joiningPoints.AddRange(GetAvailableExits(currentTile));
				//joiningPoints.Clear();
				//joiningPoints = GetAvailableExits(currentTile);

				//joiningPoint = GetRandomExit(newTile);

				//int _tilesTypesTried = 0;

				//int _randomTileSlot = Random.Range(0, roomComponentTypes.Length);

				//while (_remainingArea > 0 /*&& _validRemainingSpots > 0 ?? */) {

				//	Tile newTile = roomComponentTypes[_randomTileSlot];

				//	roomComponentsArea.TryGetValue(newTile, out int currentRoomArea);

				//	// if the tile we've selected is too big, try again
				//	if (_remainingArea < currentRoomArea) {
				//		if (_tilesTypesTried < roomComponentTypes.Length) { // try the previous tile, gambling on it being smaller
				//			_randomTileSlot = (_randomTileSlot--) % roomComponentTypes.Length;
				//			_tilesTypesTried++;
				//			continue;
				//		} else {
				//			break;
				//		}
				//	}

				//	// Sufficient area, now check other rules
				//	if (!CanBuildRoomComponent(newTile, roomStartNode)) {
				//		continue;
				//	}


				//	_remainingArea -= currentRoomArea;

				//}

			}
			totalRoomsBuilt += roomsBuilt;
			totalAreaLeft += _remainingArea;
			if (debugRoomAssembly)
				Debug.LogFormat("-------------------Room {0} finished!-------------------------", i);
			//break;
		}
		Debug.LogFormat("Floor {0}: Built {1} rooms, with a total unrealized area of {2}", floorsBuilt, totalRoomsBuilt, totalAreaLeft);
	}

	protected List<Tile> CheckTilesArea(List<Tile> candidateTiles, int remainingArea)
	{
		List<Tile> fittingTiles = new List<Tile>();

		for (int i = 0; i < candidateTiles.Count; i++) {

			roomComponentsArea.TryGetValue(candidateTiles[i], out int currentRoomArea);
			if (currentRoomArea <= remainingArea)
				fittingTiles.Add(candidateTiles[i]);
		}
		return fittingTiles;
	}

	protected bool CanFitTile(List<Tile> candidateTiles, Transform joiningPoint, out Dictionary<Tile, Vector3> tilesAtPos)
	{
		if (debugRoomAssembly)
			Debug.LogFormat("--Checking if {0} candidate tiles can fit", candidateTiles.Count);

		//fittingTiles = new List<Tile>();
		tilesAtPos = new Dictionary<Tile, Vector3>();

		for (int i = 0; i < candidateTiles.Count; i++) {

			Tile tile = candidateTiles[i];

			bool canFit = false;

			List<Transform> entries = GetEntries(tile);

			//int currentEntry = 0;
			int currentEntry = Random.Range(0, entries.Count);
			int attempts = 0;

			Vector3 pos = Vector3.zero;

			while (!canFit && attempts < entries.Count) {
				pos = GetNewTilePosition(tile, joiningPoint, entries[currentEntry].localPosition);
				Bounds bounds = tile.GetComponentInChildren<Renderer>().bounds;

				Collider[] cols = Physics.OverlapBox(pos, bounds.extents, joiningPoint.rotation);

				// TODO: WE COULD INSTEAD OUTPUT ALL VALID ENTRY POSITIONS OF ROOMS
				// Unsure if that'd be meaningfully different than starting from a random pick and walking...
				if (cols.Length <= 0) {
					canFit = true;
					break;
				}

				if (debugRoomAssembly)
					Debug.LogFormat("--Collision! {0} tile cannot fit!", tile);

				currentEntry = (currentEntry + 1) % entries.Count;
				attempts++;

				//if (attempts > entries.Count)
				//	break;
			}


			if (canFit) {
				if (debugRoomAssembly)
					Debug.LogFormat("--{0} tile can fit! Adding...", tile);

				//fittingTiles.Add(tile);
				tilesAtPos.Add(tile, pos);
			}
		}
		return tilesAtPos.Count > 0;
		//return fittingTiles.Count > 0;
	}

	protected bool CanBuildRoomComponent(Tile tile, Transform joiningPoint)
	{
		Vector3 pos = GetNewTilePosition(tile, joiningPoint);

		Bounds bounds = tile.GetComponent<Renderer>().bounds;

		Collider[] cols = Physics.OverlapBox(pos, bounds.extents, joiningPoint.rotation);
		if (cols.Length > 0) {
			Debug.Log("Collision!");
			return false;
		}
		return true;
	}

	protected bool ShouldBuildDoorHere(Transform currentAnchor, Tile parentTile, bool preventNeighbors)
	{

		switch (parentTile.TileType) {
			case TileType.Corridor:
				break;
			case TileType.TurnRight:
				break;
			case TileType.TeeLeft: // Rooms here tend to clip badly
				return false;
			case TileType.Fork: // Could allow here, only IF it's opposite the tee...
				return false;
			case TileType.Room: // Don't add rooms to the start or end rooms
				return false;
			case TileType.CorridorSmall:
				break;
			default:
				break;
		}

		//Don't build a room here if we have built a room from this tile already //TBD -- might want to be able to build across...
		if (preventNeighbors && rootRoomTiles.ContainsKey(parentTile))
			return false;

		WallTypeTag tileTag = currentAnchor.GetComponent<WallTypeTag>();

		//Don't convert railings to rooms
		if (tileTag && tileTag.WallType == WallType.Railing)
			return false;

		if (preventNeighbors)
			rootRoomTiles.Add(parentTile, currentAnchor);
		return true;
	}


	//protected List<Transform> GetRandomRoomNodes(int maxRange, List<Transform> sourceTransforms, bool preventNeighbors = true)
	protected List<Transform> GetRandomRoomNodesAndRemoveFromArray(int maxRange, Transform[] sourceTransforms, bool preventNeighbors = true)
	{
		List<Transform> roomNodes = new List<Transform>();
		int i = 0;
		while (i < maxRange) {

			int randomRoomSlot = Random.Range(0, sourceTransforms.Length);

			Transform roomNode = sourceTransforms[randomRoomSlot];

			if (roomNode == null)
				continue;

			Tile parentTile = roomNode.GetComponentInParent<Tile>();

			if (!ShouldBuildDoorHere(roomNode, parentTile, preventNeighbors))
				continue;

			roomNodes.Add(roomNode);
			//sourceTransforms.Remove(roomNode);
			sourceTransforms[randomRoomSlot] = null;
			i++;
		}
		return roomNodes;
	}

	public bool debugRoomTesting = true;
	public Transform pointA;
	public Transform pointB;

	protected void BuildBetweenTwoPoints()
	{
		if (!pointA || !pointB) {
			debugRoomTesting = false;
			return;
		}

		//Debug.LogFormat("is Vector3.Right to the Right or Left of Forward? {0}", Vector3.Dot(Vector3.forward, Vector3.right) );
		//Debug.LogFormat("is Vector3.Left to the Right or Left of Forward? {0}", Vector3.Dot(Vector3.left, Vector3.forward));
		//Debug.LogFormat("is Vector3.Left to the Right or Left of Forward? {0}", ap_Utility.AngleDir(Vector3.forward, Vector3.left));


		//Vector3 totalOffset = 


		//I guess, we need to check that the points work. So don't actually build until it's all good?
		//Build until pointA.y = pointB.y
		//Repeat until pointA.x = pointB.x
		//Vector3 currentPoint = pointA.position;
		Vector3 currentDirection = pointA.forward;

		//Vector3 currentOffset = pointB.position - Vector3.one * 3.0f - currentPoint;
		//Vector3 currentOffset = pointB.position - currentPoint;
		Vector3 currentOffset = pointB.position - pointA.position;
		//GetBlockCoordinates(currentOffset);
		//List<Tile> tiles = new List<Tile>();
		int tilesBuilt = 0;
		int maxTiles = 10;

		Transform joiningPoint = pointA;

		while (GetBlockCoordinates(currentOffset) != Vector3.zero && tilesBuilt < maxTiles) {

			//Vector3.Project
			Vector3 buildDirection = Vector3.Project(currentOffset, currentDirection);
			float buildDistance = buildDirection.sqrMagnitude;


			Tile newTileType;

			if (buildDistance >= 10f * 10f) {
				newTileType = corridorTypes[Random.Range(0, corridorTypes.Length)];
			} else if (buildDistance >= 6f * 6f) {
				//Debug.Log("Building Corridor...");
				newTileType = smallCorridorTypes[Random.Range(0, smallCorridorTypes.Length)];
			} else {
				//This direction is now sufficient along this axis, 
				//right
				//float facing = ap_Utility.AngleDir(currentDirection, currentOffset);
				float facing = ap_Utility.AngleDir(currentDirection, currentOffset, Vector3.up);
				//Debug.LogFormat("ExitDirection: {0}, OffsetToGoal: {1}", currentDirection, currentOffset);
				//Debug.LogFormat("Facing: {0}, Dot: {1}", facing, Vector3.Dot(currentDirection, currentOffset));
				//Debug.LogFormat("BuildDirection < 6*6; trying to turn, facing is {0}, direction is {1}, offset is {2}", facing, currentDirection, currentOffset);

				if (facing > 0.1f) { //direction towards our right
									 //Debug.LogFormat("Facing is {0}, Building to our right", facing);
					newTileType = rightTurnTypes[Random.Range(0, rightTurnTypes.Length)];
				} else if (facing < -0.1f) { //direction towards our left
											 //Debug.LogFormat("Facing is {0}, Building to our left", facing);
					newTileType = leftTurnTypes[Random.Range(0, leftTurnTypes.Length)];
				} else { //we're facing the goal, but we're short...
					Debug.Log("ERROR: We're going to need a shorter tile...");
					return;
				}

			}

			BuildTile(newTileType, joiningPoint, out Tile newTile);
			if (!newTile)
				break;

			joiningPoint = GetAvailableExits(newTile)[0];

			currentDirection = joiningPoint.forward;
			currentOffset = pointB.position - joiningPoint.position;
			tilesBuilt++;

			//if (currentOffset.z >= 6f) {
			//	newTile = smallCorridorTypes[Random.Range(0, smallCorridorTypes.Length)];
			//} else {

			//}

		}

		debugRoomTesting = false;

		//HOWEVER, we want there to be an offset between the tiles at about the width/length of the L turn...

	}

	//protected Tile GetRandomTile(List<Tile> list)
	//{
	//	return list[Random.Range(0, list.Count)];
	//}

	/// <summary>
	/// Return the position to place a NewTile such that its EntryPoint lines up with JoiningPoint
	/// </summary>
	/// <returns></returns>
	protected Vector3 GetNewTilePosition(Tile newTileType, Transform joiningPoint)
	{
		//Vector3 localEntryPos = GetEntry(newTileType).localPosition;
		Vector3 localEntryPos = GetRandomEntry(newTileType).localPosition;
		return GetNewTilePosition(newTileType, joiningPoint, localEntryPos);
	}

	/// <summary>
	/// Intermediate tile helper -- for when we want to try specific entry positions (in local space)
	/// </summary>
	/// <returns></returns>
	protected Vector3 GetNewTilePosition(Tile newTileType, Transform joiningPoint, Vector3 localEntryPos)
	{
		Vector3 offset = joiningPoint.rotation * (Vector3.zero - localEntryPos); //Get offset between Tile's Centre (v3.zero) and its Entry, rotate based on joining point's facing
		Vector3 worldPos = joiningPoint.position + offset;
		return new Vector3(Mathf.Round(worldPos.x), worldPos.y, Mathf.Round(worldPos.z));
	}

	/// <summary>
	/// Return coordinates rounded to gridsnap (typically used for tile coordinates, to prevent being placed too closely together)
	/// </summary>
	protected Vector3 GetBlockCoordinates(Vector3 coords, float snap = tileGridSnap)
	{
		//return ap_Utility.RoundTo(coords, tileGridSnap);
		return new Vector3(ap_Utility.RoundTo(coords.x, snap), coords.y, ap_Utility.RoundTo(coords.z, snap));
	}

	protected void BuildWalls()
	{
		//List<Transform> 
		List<Vector3> placedWallCoords = new List<Vector3>();

		List<Transform> wallsToBuild = availableWalls;
		//List<Transform> wallsToBuild = new List<Transform>();

		//wallsToBuild.AddRange(availableWalls);
		wallsToBuild.AddRange(availableExits);

		//wallsToBuild.AddRange(availableExits.AsReadOnly());

		//string debug = "";
		//if (availableWalls)
			//debug += "";


		//Debug.LogFormat("{0} available walls, {1} available exits, should build {2} walls", availableWalls.Count, availableExits.Count, wallsToBuild.Count );

		for (int i = 0; i < wallsToBuild.Count; i++) {

			Transform joiningPoint = wallsToBuild[i];

			GameObject newWallType = wallTypes[0];
			if (buildRailings) {
				WallTypeTag tag = joiningPoint.GetComponent<WallTypeTag>();

				if (tag) {

					if (tag.WallType == WallType.Railing)
						newWallType = railingTypes[0];

					if (tag.WallType == WallType.ClosedDoor)
						newWallType = closedDoorTypes[0];
				}
			}

			//TODO: REMOVE THIS COORDINATE BLOCKING IF WE'RE USING WALLS WITH CULLED BACKFACES (e.g., interior/exterior walls would be placed very close together)
			//Vector3 blockCoords = GetBlockCoordinates(joiningPoint.position, 1.0f);
			//if (placedWallCoords.Contains(blockCoords))
			//	continue;
			//placedWallCoords.Add(blockCoords);

			GameObject newWall = Instantiate(newWallType, joiningPoint.position, joiningPoint.rotation, joiningPoint.parent);
			//Debug.Log("Built wall");
		}
	}

	/// <summary>
	/// Records all the features of the tile (exits, entrances, walls), adds the block coordinates to our grid for collision checking, and adds the tile to our list of placed tiles.
	/// </summary>
	/// <param name="_tile"></param>
	protected void RecordTile(Tile _tile)
	{
		CheckAllTileFeatures(_tile.transform); //record new features of our tile
		AddTileCoords(_tile.transform.position); //record the grid position of our tile
		placedTiles.Add(_tile);
	}

	protected void AddTileCoords(Vector3 coords)
	{
		tileCoords.Add(GetBlockCoordinates(coords));
		//tileCoords.Add(ap_Utility.RoundTo(coords, tileGridSnap));
	}

	/// <summary>
	/// Returns the unused exits of a tile
	/// </summary>
	protected List<Transform> GetAvailableExits(Tile _tile)
	{
		List<Transform> validExits = new List<Transform>();
		foreach (Transform t in _tile.transform.Find("AnchorPoints/Exits")) {
			if (availableExits.Contains(t)) {
				validExits.Add(t);
			}
		}
		return validExits;
	}

	protected Transform GetRandomExit(Tile _tile)
	{
		List<Transform> validExits = new List<Transform>();
		foreach (Transform t in _tile.transform.Find("AnchorPoints/Exits")) {
			if (availableExits.Contains(t)) {
				validExits.Add(t);
			}
		}

		if (validExits.Count > 0)
			return validExits[Random.Range(0, validExits.Count)];
		else
			return null;
	}

	/// <summary>
	/// Returns all of the exits of a tile
	/// </summary>
	//protected List<Transform> GetExits(Tile _tile)
	//{
	//	List<Transform> exits = new List<Transform>();
	//	foreach (Transform t in _tile.transform.Find("AnchorPoints/Exits")) {
	//			exits.Add(t);
	//	}
	//	return exits;
	//}


	/// <summary>
	/// Returns the entry of a tile (typically to line up placement)
	/// </summary>
	protected Transform GetEntry(Tile _tile)
	{
		foreach (Transform t in _tile.transform.Find("AnchorPoints/Entries")) {
			if (t.gameObject.activeSelf)
				return t;
		}
		return null;
	}

	protected List<Transform> GetEntries(Tile _tile)
	{
		List<Transform> entries = new List<Transform>();
		foreach (Transform t in _tile.transform.Find("AnchorPoints/Entries")) {
			if (t.gameObject.activeSelf)
				entries.Add(t);
		}
		return entries;
	}

	protected Transform GetRandomEntry(Tile _tile)
	{
		List<Transform> entries = GetEntries(_tile);
		return entries[Random.Range(0, entries.Count)];

		//if (entries.Count > 0)
		//	return validExits[Random.Range(0, validExits.Count)];
		//else
		//	return null;
	}

	protected void CheckAllTileFeatures(Transform _tile)
	{
		CheckTileExits(_tile);
		//CheckTileEntries(_tile);
		CheckTileWalls(_tile);
	}

	protected void CheckTileExits(Transform _tile)
	{
		foreach (Transform t in _tile.Find("AnchorPoints/Exits")) {
			if (t.gameObject.activeSelf && !availableExits.Contains(t))
				availableExits.Add(t);
		}
	}

	//protected void CheckTileEntries(Transform _tile)
	//{
	//	foreach (Transform t in _tile.Find("AnchorPoints/Entries")) {
	//		if (t.gameObject.activeSelf)
	//			availableEntries.Add(t);
	//	}
	//}

	protected void CheckTileWalls(Transform _tile)
	{
		foreach (Transform t in _tile.Find("AnchorPoints/Walls")) {
			if (t.gameObject.activeSelf)
				availableWalls.Add(t);
		}
	}
}
