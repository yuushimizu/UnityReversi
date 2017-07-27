using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour, Tile.Callback {
	public GameObject tilePrefab;
	public GameObject stonePrefab;
	public int xTileCount = 8;
	public int yTileCount = 8;

	private Vector3 tilePositionOffset;
	private Vector3 tileSize;

	private GameObject[,] tiles;
	private GameObject[,] stones;

	private bool currentIsBlack;

	private Vector3 IndexToPosition(int x, int y) {
		return tilePositionOffset + new Vector3(x * tileSize.x, y * tileSize.y);
	}

	private bool IsValidIndex(int x, int y) {
		return x >= 0 && y >= 0 && x < xTileCount && y < yTileCount;
	}

	private GameObject StoneAt(int x, int y) {
		return IsValidIndex(x, y) ? stones[x, y] : null;
	}

	private void InitializeTiles() {
		tiles = new GameObject[xTileCount, yTileCount];
		for (int y = 0; y < yTileCount; ++y) {
			for (int x = 0; x < xTileCount; ++x) {
				var tile = Instantiate(tilePrefab, IndexToPosition(x, y), Quaternion.identity, transform);
				tile.GetComponent<Tile>().Initialize(x, y, this);
				tiles[x, y] = tile;
			}
		}
	}

	private void InitializeStones() {
		stones = new GameObject[xTileCount, yTileCount];
		var baseX = xTileCount / 2 - 1;
		var baseY = yTileCount / 2 - 1;
		for (int yOffset = 0; yOffset < 2; ++yOffset) {
			for (int xOffset = 0; xOffset < 2; ++xOffset) {
				int x = baseX + xOffset;
				int y = baseY + yOffset;
				var stone = Instantiate(stonePrefab, IndexToPosition(x, y), Quaternion.identity, transform);
				stone.GetComponent<Stone>().Put((xOffset + yOffset) % 2 == 0);
				stones[x, y] = stone;
			}
		}
	}

	// Use this for initialization
	void Start() {
		tileSize = tilePrefab.GetComponent<SpriteRenderer>().bounds.size;
		tilePositionOffset = -Vector3.Scale(tileSize, new Vector3(xTileCount - 1, yTileCount - 1, 1)) / 2;
		InitializeTiles();
		InitializeStones();
		currentIsBlack = true;
	}
	
	// Update is called once per frame
	void Update() {
		
	}

	private class Direction {
		public int x;
		public int y;

		public Direction(int x, int y) {
			this.x = x;
			this.y = y;
		}
	}

	private static Direction[] directions = {
		new Direction(-1, 1),
		new Direction(0, 1),
		new Direction(1, 1),
		new Direction(-1, 0),
		new Direction(1, 0),
		new Direction(-1, -1),
		new Direction(0, -1),
		new Direction(1, -1)
	};

	private bool ReversedStoneExists(int startX, int startY, bool isBlack, Direction direction) {
		int x = startX;
		int y = startY;
		bool exists = false;
		while (true) {
			x += direction.x;
			y += direction.y;
			var stone = StoneAt(x, y);
			if (stone == null) {
				return false;
			}
			if (stone.GetComponent<Stone>().isBlack == isBlack) {
				return exists;
			} else {
				exists = true;
			}
		}
	}

	private bool ReversedStoneExists(int x, int y, bool isBlack) {
		foreach (var direction in directions) {
			if (ReversedStoneExists(x, y, isBlack, direction)) {
				return true;
			}
		}
		return false;
	}

	private bool CanPut(int x, int y, bool isBlack) {
		if (stones[x, y] != null) {
			return false;
		}
		return ReversedStoneExists(x, y, isBlack);
	}

	private void ReverseFrom(int startX, int startY, bool isBlack, Direction direction) {
		int endX = startX;
		int endY = startY;
		while (true) {
			endX += direction.x;
			endY += direction.y;
			var stone = StoneAt(endX, endY);
			if (stone == null) {
				return;
			}
			if (stone.GetComponent<Stone>().isBlack == isBlack) {
				break;
			}
		}
		int x = startX + direction.x;
		int y = startY + direction.y;
		while (!(x == endX && y == endY)) {
			StoneAt(x, y).GetComponent<Stone>().Reverse();
			x += direction.x;
			y += direction.y;
		}
	}

	private void ReverseFrom(int x, int y, bool isBlack) {
		foreach (var direction in directions) {
			ReverseFrom(x, y, isBlack, direction);
		}
	}

	private bool PuttableTileExists(bool isBlack) {
		for (int x = 0; x < xTileCount; ++x) {
			for (int y = 0; y < yTileCount; ++y) {
				if (CanPut(x, y, isBlack)) {
					return true;
				}
			}
		}
		return false;
	}

	private void Put(int x, int y) {
		if (!CanPut(x, y, currentIsBlack)) {
			return;
		}
		var stone = Instantiate(stonePrefab, IndexToPosition(x, y), Quaternion.identity, transform);
		stone.GetComponent<Stone>().Put(currentIsBlack);
		stones[x, y] = stone;
		ReverseFrom(x, y, currentIsBlack);
		if (PuttableTileExists(!currentIsBlack)) {
			currentIsBlack = !currentIsBlack;
		}
	}

	public void OnClick(int x, int y) {
		Put(x, y);
	}
}
