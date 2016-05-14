using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;

public class Tile {
	public bool UpWall { get; set; }
	public bool DownWall { get; set; }
	public bool LeftWall { get; set; }
	public bool RightWall { get; set; }
	public GameObject type { get; set; }
	public Vector3 position { get; set; }
	public Quaternion rotation { get; set; }

	private int wallCount;

	public Tile() {
		wallCount = 0;
		UpWall = false;
		DownWall = false;
		LeftWall = false;
		RightWall = false;
		type = null;
		position = Vector3.zero;
		rotation = Quaternion.identity;
	}
	public Tile(int zPos, int xPos, bool up, bool down, bool left, bool right, List<GameObject> typeList) {
		wallCount = 0;
		if (up)
			wallCount++;
		if (down)
			wallCount++;
		if (left)
			wallCount++;
		if (right)
			wallCount++;
		if ((up && down) || (left && right))
			wallCount++;
		UpWall = up;
		DownWall = down;
		LeftWall = left;
		RightWall = right;
		type = typeList [wallCount];
		position = new Vector3 (xPos, 0f, -zPos);
		rotation = GetRotation ();
	}
		
	private Quaternion GetRotation () {
		switch (type.name) {
		case "TCross":
			if (LeftWall)
				return Quaternion.identity;
			else if (UpWall)
				return Quaternion.Euler (0f, 90f, 0f);
			else if (RightWall)
				return Quaternion.Euler (0f, 180f, 0f);
			else
				return Quaternion.Euler (0f, 270f, 0f);
		case "Corner":
			if (UpWall && RightWall)
				return Quaternion.identity;
			else if (DownWall && RightWall)
				return Quaternion.Euler (0f, 90f, 0f);
			else if (DownWall && LeftWall)
				return Quaternion.Euler (0f, 180f, 0f);
			else
				return Quaternion.Euler (0f, 270f, 0f);
		case "Corridor":
			if (LeftWall && RightWall)
				return Quaternion.identity;
			else
				return Quaternion.Euler (0f, 90f, 0f);
		case "DeadEnd":
			if (!DownWall)
				return Quaternion.identity;
			else if (!LeftWall)
				return Quaternion.Euler (0f, 90f, 0f);
			else if (!UpWall)
				return Quaternion.Euler (0f, 180f, 0f);
			else
				return Quaternion.Euler (0f, 270f, 0f);
		default:
			return Quaternion.identity;
		}
	}
}

public class Parser : MonoBehaviour {

	public GameObject block;
	public GameObject corner;
	public GameObject corridor;
	public GameObject cross;
	public GameObject deadEnd;
	public GameObject tCross;

	public GameObject pacman;
	public GameObject blinky;
	public GameObject inky;
	public GameObject pinky;
	public GameObject clyde;

	private int blockCounter = 0;
	private int corridorCounter = 0;
	private int crossCounter = 0;
	private int tCrossCounter = 0;
	private int cornerCounter = 0;
	private int deadEndCounter = 0;

	private List<string> lines = new List<string> ();

	private List<List<Tile>> tiles = new List<List<Tile>>();
	private List<GameObject> types = new List<GameObject> ();

	private List<List<GameObject>> gameObjects = new List<List<GameObject>> ();

	private int endLine = 0;
	private int lineLength = 0;

	// Use this for initialization
	void Start () {
		types.Add (cross);
		types.Add (tCross);
		types.Add (corner);
		types.Add (corridor);
		types.Add (deadEnd);
		types.Add (block);
	}
	
	public void getField (List<string> file) {
		lines = file;
		makeField ();
	}

	bool ParseFile (string filename) {
		try {
			string line;
			StreamReader reader = new StreamReader (Application.dataPath + "/" + filename + ".pac", Encoding.Default);
			using (reader) {
				do {
					line = reader.ReadLine ();
					if (line != null) {
						lines.Add (line);
					}
				} while (line != null);
				reader.Close ();
				return true;
			}
		}
		catch (Exception e) {
			Console.WriteLine ("{0}\n", e.Message);
			return false;
		}
	}

	public void makeField() {
		GetLineParameters ();
		for (int i = 0; i < lines.Count; i++) {
			List<Tile> lineTiles = new List<Tile> ();
			string line = lines [i];
			for (int j = 0; j < line.Length; j++) {
				string symbol = "" + line [j];
				switch (symbol) {
				case "|":
					lineTiles.Add(new Tile(i, j, CheckUp(i, j), CheckDown(i, j), true, true, types));
					break;
				case "-":
					lineTiles.Add(new Tile(i, j, true, true, CheckLeft(i, j), CheckRight(i,j), types));
					break;
				case "+":
					lineTiles.Add(new Tile(i, j, CheckUp(i,j), CheckDown(i,j), CheckLeft(i,j), CheckRight(i,j), types));
					break;
				default:
					break;
				}
			}
			tiles.Add (lineTiles);
		}
		CreateGameObjects ();
		SetWayPoints ();
		CreateEntities ();
	}

	bool CheckUp (int zPos, int xPos) {
		if (zPos == 0)
			return true;
		else if ((lines [zPos - 1] [xPos] + "").Equals ("-"))
			return true;
		else
			return false;
	}

	bool CheckDown (int zPos, int xPos) {
		if (zPos == endLine)
			return true;
		else if ((lines [zPos + 1] [xPos] + "").Equals ("-"))
			return true;
		else
			return false;
	}

	bool CheckLeft (int zPos, int xPos) {
		if (xPos == 0)
			return true;
		else if ((lines [zPos] [xPos - 1] + "").Equals ("|"))
			return true;
		else
			return false;
	}

	bool CheckRight (int zPos, int xPos) {
		if (xPos == lineLength)
			return true;
		else if ((lines [zPos] [xPos + 1] + "").Equals ("|"))
			return true;
		else
			return false;
	}

	void GetLineParameters() {
		lineLength = lines [0].Length - 1;
		foreach (string line in lines) {
			if (!(line.Contains ("+") || line.Contains ("-") || line.Contains ("|")))
				break;
			else
				endLine++;
		}
		endLine--;
	}

	void CreateGameObjects () {
		foreach (List<Tile> tileList in tiles) {
			List<GameObject> lineGameObjects = new List<GameObject> ();
			foreach (Tile tile in tileList) {
				GameObject temp = (GameObject)Instantiate (tile.type, tile.position, tile.rotation);
				temp.name = tile.type.name + " " + IncreaseCounter (tile.type.name);
				lineGameObjects.Add (temp);
			}
			gameObjects.Add (lineGameObjects);
		}
	}

	int IncreaseCounter (string name) {
		switch (name) {
		case "Cross":
			return crossCounter++;
		case "TCross":
			return tCrossCounter++;
		case "Corner":
			return cornerCounter++;
		case "Corridor":
			return corridorCounter++;
		case "DeadEnd":
			return deadEndCounter++;
		case "Tile":
			return blockCounter++;
		default:
			return 0;
		}
	}

	void SetWayPoints() {
		for (int i = 0; i < tiles.Count; i++) {
			for (int j = 0; j < tiles [i].Count; j++) {
				GameObject tempGameObject = gameObjects [i] [j];
				Tile tile = tiles [i] [j];
				if (!tile.UpWall)
					tempGameObject.GetComponent<WayPoint>().upWaypoint = gameObjects [i - 1] [j].GetComponent<WayPoint> ();
				if(!tile.DownWall)
					tempGameObject.GetComponent<WayPoint>().downWaypoint = gameObjects [i + 1] [j].GetComponent<WayPoint> ();
				if (!tile.LeftWall)
					tempGameObject.GetComponent<WayPoint>().leftWaypoint = gameObjects [i] [j - 1].GetComponent<WayPoint> ();
				if(!tile.RightWall)
					tempGameObject.GetComponent<WayPoint>().rightWaypoint = gameObjects [i] [j + 1].GetComponent<WayPoint> ();
			}
		}
	}

	void CreateEntities () {
		for (int i = endLine + 1; i < lines.Count; i++) {
			Vector2 spawn = GetSpawnPosition (lines[i]);
			int xPos = (int)spawn.x;
			int zPos = (int)spawn.y;
			if (tiles [zPos] [xPos].type == block) {
				Debug.LogError ("Can not spawn in DeathPath");
				break;
			}
			if (lines[i].Contains ("Pacman")) {
				GameObject tempPacman = (GameObject)Instantiate (pacman, new Vector3 (xPos, 0f, -zPos), Quaternion.identity);
				tempPacman.name = "Pacman";
				tempPacman.transform.localScale = Vector3.one * 0.45f;
			}
			if (lines[i].Contains ("Blinky")) {
				GameObject tempBlinky = (GameObject)Instantiate (blinky, new Vector3 (xPos, 0f, -zPos), Quaternion.identity);
				tempBlinky.name = "Blinky";
				tempBlinky.transform.localScale = Vector3.one * 0.45f;
				tempBlinky.GetComponent<EnemyBehaviourScript> ().currentWaypoint = gameObjects [zPos] [xPos].GetComponent<WayPoint> ();
			}
			if (lines[i].Contains ("Inky")) {
				GameObject tempInky = (GameObject)Instantiate (inky, new Vector3 (xPos, 0f, -zPos), Quaternion.identity);
				tempInky.name = "Inky";
				tempInky.transform.localScale = Vector3.one * 0.45f;
				tempInky.GetComponent<EnemyBehaviourScript> ().currentWaypoint = gameObjects [zPos] [xPos].GetComponent<WayPoint> ();
			}
			if (lines[i].Contains ("Pinky")) {
				GameObject tempPinky = (GameObject)Instantiate (pinky, new Vector3 (xPos, 0f, -zPos), Quaternion.identity);
				tempPinky.name = "Pinky";
				tempPinky.transform.localScale = Vector3.one * 0.45f;
				tempPinky.GetComponent<EnemyBehaviourScript> ().currentWaypoint = gameObjects [zPos] [xPos].GetComponent<WayPoint> ();
			}
			if (lines[i].Contains ("Clyde")) {
				GameObject tempClyde = (GameObject)Instantiate (clyde, new Vector3 (xPos, 0f, -zPos), Quaternion.identity);
				tempClyde.name = "Clyde";
				tempClyde.transform.localScale = Vector3.one * 0.45f;
				tempClyde.GetComponent<EnemyBehaviourScript> ().currentWaypoint = gameObjects [zPos] [xPos].GetComponent<WayPoint> ();
			}
		}
	}

	Vector2 GetSpawnPosition(string line) {
		string[] s = line.Split (' ');
		return (new Vector2 (float.Parse (s [1]), float.Parse (s [2])));
	}

}
