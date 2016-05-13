using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;

public class Parser : MonoBehaviour {

	public GameObject inputFieldObject;
	private InputField inputField;

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

	private List<string> lines = new List<string> ();
	private List<GameObject> gameObjects = new List<GameObject> ();

	private int endLine;
	private int lineLength;

	private int corridorCounter = 0;
	private int crossCounter = 0;
	private int tCrossCounter = 0;
	private int cornerCounter = 0;
	private int deadEndCounter = 0;
	

	void Start () {
		inputField = inputFieldObject.GetComponent<InputField> ();
		inputField.onEndEdit.AddListener (getField);
	}


	public void getField (string text) {
		if (ParseFile (text)) {
			makeField (lines);
			inputFieldObject.SetActive (false);
		}
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

	void makeField (List<string> lines) {
		GetEndLine ();
		for (int i = 0; i < lines.Count; i++) {
			string line = lines [i];
			for (int j = 0; j < line.Length; j++) {
				string c = "" + line [j];
				switch (c) {
				case ("|"):
					GameObject corridorV = (GameObject)Instantiate (corridor, getPosition (i, j), getRotation ("CorridorV"));
					corridorCounter++;
					corridorV.name = "Corridor " + corridorCounter;
					gameObjects.Add (corridorV);
					break;
				case "-":
					GameObject corridorH = (GameObject)Instantiate (corridor, getPosition (i, j), getRotation ("CorridorH"));
					corridorCounter++;
					corridorH.name = "Corridor " + corridorCounter;
					gameObjects.Add (corridorH);
					break;
				case "+":
					switch (Openings (i, j)) {
					case 4:
						GameObject cross0 = (GameObject)Instantiate (cross, getPosition (i, j), getRotation ("Cross"));
						crossCounter++;
						cross0.name = "Cross " + crossCounter;
						gameObjects.Add (cross0);
						break;
					case 3:
						GameObject tCross0 = (GameObject)Instantiate (tCross, getPosition (i, j), getRotation (GetDirection (i, j, "TCross")));
						tCrossCounter++;
						tCross0.name = "TCross " + tCrossCounter;
						gameObjects.Add (tCross0);
						break;
					case 2:
						GameObject corner0 = (GameObject)Instantiate (corner, getPosition (i, j), getRotation (GetDirection (i, j, "Corner")));
						cornerCounter++;
						corner0.name = "Corner " + cornerCounter;
						gameObjects.Add (corner0);
						break;
					case 1:
						GameObject deadEnd0 = (GameObject)Instantiate (deadEnd, getPosition (i, j), getRotation (GetDirection (i, j, "DeadEnd")));
						deadEndCounter++;
						deadEnd0.name = "DeadEnd " + deadEndCounter;
						gameObjects.Add (deadEnd0);
						break;
					default:
						break;
					}
					break;
				default:
					break;
				}
			}
			float xPos;
			float zPos;
			if (line.Contains ("Pacman")) {
				xPos = float.Parse ("" + line [7]);
				zPos = float.Parse ("" + line [9]);
				GameObject pacman0 = (GameObject)Instantiate (pacman, new Vector3 (xPos, 0.5f, -zPos), Quaternion.identity);
				pacman0.transform.localScale = Vector3.one * 0.45f;
				gameObjects.Add (pacman0);
			}
			if (line.Contains ("Blinky")) {
				xPos = float.Parse ("" + line [7]);
				zPos = float.Parse ("" + line [9]);
				GameObject blinky0 = (GameObject)Instantiate (blinky, new Vector3 (xPos, 0.5f, -zPos), Quaternion.identity);
				blinky0.transform.localScale = Vector3.one * 0.45f;
				gameObjects.Add (blinky0);
			}
			if (line.Contains ("Inky")) {
				xPos = float.Parse ("" + line [5]);
				zPos = float.Parse ("" + line [7]);
				GameObject inky0 = (GameObject)Instantiate (inky, new Vector3 (xPos, 0.5f, -zPos), Quaternion.identity);
				inky0.transform.localScale = Vector3.one * 0.45f;
				gameObjects.Add (inky0);
			}
			if (line.Contains ("Pinky")) {
				xPos = float.Parse ("" + line [6]);
				zPos = float.Parse ("" + line [8]);
				GameObject pinky0 = (GameObject)Instantiate (pinky, new Vector3 (xPos, 0.5f, -zPos), Quaternion.identity);
				pinky0.transform.localScale = Vector3.one * 0.45f;
				gameObjects.Add (pinky0);
			}
			if (line.Contains ("Clyde")) {
				xPos = float.Parse ("" + line [6]);
				zPos = float.Parse ("" + line [8]);
				GameObject clyde0 = (GameObject)Instantiate (clyde, new Vector3 (xPos, 0.5f, -zPos), Quaternion.identity);
				clyde0.transform.localScale = Vector3.one * 0.45f;
				gameObjects.Add (clyde0);
			}
			SetWayPoints ();
		}
	}

	void GetEndLine () {
		for (int i = 0; i < lines.Count; i++) {
			string line = lines [i];
			if (!(line.Contains ("+") || line.Contains ("|") || line.Contains ("-"))) {
				endLine = i-1;
				lineLength = line.Length - 1;
				break;
			}
		}
	}

	Vector3 getPosition(float z, float x) {
		return new Vector3 (x, 0f, -z);
	}

	Quaternion getRotation(string type) {
		Quaternion rot;
		switch (type) {
		case ("CorridorH"):
			rot = Quaternion.Euler (0, 90, 0);
			break;
		case ("TCrossup"):
			rot = Quaternion.Euler (0, 90, 0);
			break;
		case ("Cornerdownright"):
			rot = Quaternion.Euler (0, 90, 0);
			break;
		case ("DeadEndleft"):
			rot = Quaternion.Euler (0, 90, 0);
			break;
		case "TCrossright":
			rot = Quaternion.Euler (0, 180, 0);
			break;
		case "Cornerdownleft":
			rot = Quaternion.Euler (0, 180, 0);
			break;
		case "DeadEndup":
			rot = Quaternion.Euler (0, 180, 0);
			break;
		case "TCrossdown":
			rot = Quaternion.Euler (0, 270, 0);
			break;
		case "Cornerupleft":
			rot = Quaternion.Euler (0, 270, 0);
			break;
		case "DeadEndright":
			rot = Quaternion.Euler (0, 270, 0);
			break;
		default:
			rot = Quaternion.identity;
			break;
		}
		return rot;
	}

	int Openings(int zPos, int xPos) {
		Debug.Log (zPos + " " + xPos + " " + endLine + " " + lineLength);
		int ports = 4;
		if (zPos == 0) {
			ports--;
		} else if (("" + lines [zPos - 1] [xPos]).Equals ("-")) {
			ports--;
		}
		if (zPos == endLine) {
			ports--;
		} else if (zPos < endLine) {
			if (("" + lines [zPos + 1] [xPos]).Equals ("-")) {
				ports--;
			}
		}
		if (xPos == 0) {
			ports--;
		} else if (("" + lines [zPos] [xPos - 1]).Equals ("|")) {
			ports--;
		}
		if (xPos == lineLength) {
			ports--;
		} else if (("" + lines [zPos] [xPos + 1]).Equals ("|")) {
			ports--;
		}

		return ports;
	}

	string GetDirection(int zPos, int xPos, string type) {
		string endstring = type;
		switch(type) {
		case "TCross":
			if (zPos == 0)
				return type + "up";
			else if (zPos == endLine)
				return type + "down";
			else if (xPos == 0)
				return type + "left";
			else if (xPos == lineLength)
				return type + "right";
			else if (("" + lines [zPos - 1] [xPos]).Equals ("-"))
				return type + "up";
			else if (("" + lines [zPos + 1] [xPos]).Equals ("-"))
				return type + "down";
			else if (("" + lines [zPos] [xPos - 1]).Equals ("|"))
				return type + "left";
			else
				return type + "right";
		case ("Corner"):
			if (zPos == 0)
				endstring += "up" + SecondPort (zPos, xPos);
			else if (("" + lines [zPos - 1] [xPos]).Equals ("-"))
				endstring += "up" + SecondPort (zPos, xPos);
			else if (zPos == endLine)
				endstring += "down" + SecondPort (zPos, xPos);
			else if (("" + lines [zPos + 1] [xPos]).Equals ("-"))
				endstring += "down" + SecondPort (zPos, xPos);
			break;
		case "DeadEnd":
			if (zPos > 0 && zPos < endLine && xPos > 0 && xPos < lineLength) {
				if (("" + lines [zPos + 1] [xPos]).Equals ("|") || ("" + lines [zPos + 1] [xPos]).Equals ("+"))
					return type + "down";
				if (("" + lines [zPos - 1] [xPos]).Equals ("|") || ("" + lines [zPos - 1] [xPos]).Equals ("+"))
					return type + "up";
				if (("" + lines [zPos] [xPos + 1]).Equals ("-") || ("" + lines [zPos] [xPos + 1]).Equals ("+"))
					return type + "right";
				if (("" + lines [zPos] [xPos - 1]).Equals ("-") || ("" + lines [zPos] [xPos - 1]).Equals ("+"))
					return type + "left";
			} else if (zPos == 0 || zPos == endLine) {
				if (zPos == 0) {
					if (("" + lines [zPos + 1] [xPos]).Equals ("|") || ("" + lines [zPos + 1] [xPos]).Equals ("+"))
						return type + "down";
				}
				if (zPos == endLine) {
					if (("" + lines [zPos - 1] [xPos]).Equals ("|") || ("" + lines [zPos - 1] [xPos]).Equals ("+"))
						return type + "up";
				}
				if (xPos == 0)
					return type + "right";
				if (xPos == lineLength)
					return type + "left";
				if (xPos > 0 && xPos < lineLength) {
					if (("" + lines [zPos] [xPos + 1]).Equals ("-") || ("" + lines [zPos] [xPos + 1]).Equals ("+"))
						return type + "right";
					else
						return type + "left";
				}
			} else if (xPos == 0 || xPos == lineLength) {
				if (xPos == 0) {
					if (("" + lines [zPos] [xPos + 1]).Equals ("-") || ("" + lines [zPos] [xPos + 1]).Equals ("+"))
						return type + "right";
				}
				if (xPos == lineLength) {
					if (("" + lines [zPos] [xPos - 1]).Equals ("-") || ("" + lines [zPos] [xPos - 1]).Equals ("+"))
						return type + "left";
				}
				if (zPos > 0 && zPos < endLine) {
					if (("" + lines [zPos - 1] [xPos]).Equals ("|") || ("" + lines [zPos - 1] [xPos]).Equals ("+"))
						return type + "up";
					else
						return type + "down";
				}
			}
			break;
		}
		return endstring;
	}

	string SecondPort(int zPos, int xPos){
		if (xPos == 0)
			return "left";
		else if (xPos == lineLength)
			return "right";
		else if (("" + lines [zPos] [xPos - 1]).Equals ("|"))
			return "left";
		else
			return "right";
		}

	void SetWayPoints () {
		foreach (GameObject tile in gameObjects) {
			if (tile.CompareTag ("Tile")) {
				WayPoint wp = tile.GetComponent<WayPoint> ();
				GameObject up = null;
				GameObject down = null;
				GameObject right = null;
				GameObject left = null;
				string rot = tile.transform.rotation.y.ToString();
				float xPos = tile.transform.position.x;
				float zPos = tile.transform.position.z;
				switch (GetName (tile)) {
				case "Corridor":
					if (rot == "90") {
						foreach (GameObject go in gameObjects) {
							if (FindSide ("left", go, xPos, zPos))
								left = go;
							if (FindSide ("right", go, xPos, zPos))
								right = go;
						}
						wp.leftWaypoint = left.GetComponent<WayPoint> ();
						wp.rightWaypoint = right.GetComponent<WayPoint> ();
       					} else {
						foreach (GameObject go in gameObjects) {
							if (FindSide ("up", go, xPos, zPos))
								up = go;
							if (FindSide ("down", go, xPos, zPos))
								down = go;
						}
						wp.upWaypoint = up.GetComponent<WayPoint> ();
						wp.downWaypoint = down.GetComponent<WayPoint> ();
					}
					break;
				case "Cross":
					foreach (GameObject go in gameObjects) {
						if (FindSide ("up", go, xPos, zPos))
							up = go;
						if (FindSide ("down", go, xPos, zPos))
							down = go;
						if (FindSide ("left", go, xPos, zPos))
							left = go;
						if (FindSide ("right", go, xPos, zPos))
							right = go;
					}
					wp.upWaypoint = up.GetComponent<WayPoint> ();
					wp.downWaypoint = down.GetComponent<WayPoint> ();
					wp.leftWaypoint = left.GetComponent<WayPoint> ();
					wp.rightWaypoint = right.GetComponent<WayPoint> ();
					break;
				case "TCross":
					switch (rot) {
					case "90":
						foreach (GameObject go in gameObjects) {
							if (FindSide ("down", go, xPos, zPos))
								down = go;
							if (FindSide ("left", go, xPos, zPos))
								left = go;
							if (FindSide ("right", go, xPos, zPos))
								right = go;
						}
						wp.downWaypoint = down.GetComponent<WayPoint> ();
						wp.leftWaypoint = left.GetComponent<WayPoint> ();
						wp.rightWaypoint = right.GetComponent<WayPoint> ();
						break;
					case "180":
						foreach (GameObject go in gameObjects) {
							if (FindSide ("up", go, xPos, zPos))
								up = go;
							if (FindSide ("down", go, xPos, zPos))
								down = go;
							if (FindSide ("left", go, xPos, zPos))
								left = go;
						}
						wp.upWaypoint = up.GetComponent<WayPoint> ();
						wp.downWaypoint = down.GetComponent<WayPoint> ();
						wp.leftWaypoint = left.GetComponent<WayPoint> ();
						break;
					case "270":
						foreach (GameObject go in gameObjects) {
							if (FindSide ("up", go, xPos, zPos))
								up = go;
							if (FindSide ("left", go, xPos, zPos))
								left = go;
							if (FindSide ("right", go, xPos, zPos))
								right = go;
						}
						wp.upWaypoint = up.GetComponent<WayPoint> ();
						wp.leftWaypoint = left.GetComponent<WayPoint> ();
						wp.rightWaypoint = right.GetComponent<WayPoint> ();
						break;
					default:
						foreach (GameObject go in gameObjects) {
							if (FindSide ("up", go, xPos, zPos))
								up = go;
							if (FindSide ("down", go, xPos, zPos))
								down = go;
							if (FindSide ("right", go, xPos, zPos))
								right = go;
						}
						wp.upWaypoint = up.GetComponent<WayPoint> ();
						wp.downWaypoint = down.GetComponent<WayPoint> ();
						wp.rightWaypoint = right.GetComponent<WayPoint> ();
						break;

					}
					break;
				case "Corner":
					switch (rot) {
					case "90":
						foreach (GameObject go in gameObjects) {
							if (FindSide ("up", go, xPos, zPos))
								up = go;
							if (FindSide ("left", go, xPos, zPos))
								left = go;
						}
						wp.upWaypoint = up.GetComponent<WayPoint> ();
						wp.leftWaypoint = left.GetComponent<WayPoint> ();
						break;
					case "180":
						foreach (GameObject go in gameObjects) {
							if (FindSide ("up", go, xPos, zPos))
								up = go;
							if (FindSide ("right", go, xPos, zPos))
								right = go;
						}
						wp.upWaypoint = up.GetComponent<WayPoint> ();
						wp.rightWaypoint = right.GetComponent<WayPoint> ();
						break;
					case "270":
						foreach (GameObject go in gameObjects) {
							if (FindSide ("down", go, xPos, zPos))
								down = go;
							if (FindSide ("right", go, xPos, zPos))
								right = go;
						}
						wp.downWaypoint = down.GetComponent<WayPoint> ();
						wp.rightWaypoint = right.GetComponent<WayPoint> ();
						break;
					default:
						foreach (GameObject go in gameObjects) {
							if (FindSide ("down", go, xPos, zPos))
								down = go;
							if (FindSide ("left", go, xPos, zPos))
								left = go;
						}
						wp.downWaypoint = down.GetComponent<WayPoint> ();
						wp.leftWaypoint = left.GetComponent<WayPoint> ();
						break;
					}
					break;
				case "DeadEnd":
					switch (rot) {
					case "90":
						foreach (GameObject go in gameObjects)
							if (FindSide ("left", go, xPos, zPos))
								left = go;
						wp.leftWaypoint = left.GetComponent<WayPoint> ();
						break;
					case "180":
						foreach (GameObject go in gameObjects)
							if (FindSide ("up", go, xPos, zPos))
								up = go;
						wp.upWaypoint = up.GetComponent<WayPoint> ();
						break;
					case "270":
						foreach (GameObject go in gameObjects)
							if (FindSide ("right", go, xPos, zPos))
								right = go;
						wp.rightWaypoint = right.GetComponent<WayPoint> ();
						break;
					default:
						foreach (GameObject go in gameObjects)
							if (FindSide ("down", go, xPos, zPos))
								down = go;
						wp.downWaypoint = down.GetComponent<WayPoint> ();
						break;
					}
					break;
				}
			}
		}
	}

	string GetName (GameObject go) {
		string name = "";
		if (go.name.Contains ("Corridor"))
			name = "Corridor";
		if (go.name.Contains ("Cross"))
			name = "Cross";
		if (go.name.Contains ("TCross"))
			name = "TCross";
		if (go.name.Contains ("Corner"))
			name =  "Corner";
		if (go.name.Contains ("DeadEnd"))
			name = "DeadEnd";
		return name;
		
	}

	bool FindSide(string side, GameObject go, float xPos, float zPos) {
		bool direction = false;
		switch (side) {
		case "up":
			if (go.transform.position.z == zPos + 1)
				direction = true;
			break;
		case "down":
			if (go.transform.position.z == zPos - 1)
				direction = true;
			break;
		case "left":
			if (go.transform.position.x == xPos - 1)
				direction = true;
			break;
		case "right":
			if (go.transform.position.x == xPos + 1)
				direction = true;
			break;
		default:
			break;
		}
		return direction;
	}

}
