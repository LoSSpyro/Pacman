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

	private List<string> lines = new List<string> ();
	private List<List<GameObject>> gameObjects = new List<List<GameObject>> ();

	private int endLine;
	private int lineLength;

	private int blockCounter = 0;
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
			List <GameObject> lineGameObjects = new List<GameObject> ();
			for (int j = 0; j < line.Length; j++) {
				string c = "" + line [j];
				switch (c) {
				case "|":
					switch (Openings ("|", i, j)) {
					case 2:
						GameObject corridorV = (GameObject)Instantiate (corridor, getPosition (i, j), getRotation ("CorridorV"));
						corridorCounter++;
						corridorV.name = "Corridor " + corridorCounter;
						lineGameObjects.Add (corridorV);
						break;
					case 1:
						GameObject deadEnd0 = (GameObject)Instantiate (deadEnd, getPosition (i, j), getRotation (GetDirection (i, j, "DeadEnd|")));
						deadEndCounter++;
						deadEnd0.name = "DeadEnd " + deadEndCounter;
						lineGameObjects.Add (deadEnd0);
						break;
					case 0:
						GameObject block0 = (GameObject)Instantiate (block, getPosition (i, j), Quaternion.identity);
						blockCounter++;
						block0.name = "Block " + blockCounter;
						lineGameObjects.Add (block0);
						break;
					}
					break;
				case "-":
					switch (Openings ("-", i, j)) {
					case 2:
						GameObject corridorH = (GameObject)Instantiate (corridor, getPosition (i, j), getRotation ("CorridorH"));
						corridorCounter++;
						corridorH.name = "Corridor " + corridorCounter;
						lineGameObjects.Add (corridorH);
						break;
					case 1:
						GameObject deadEnd1 = (GameObject)Instantiate (deadEnd, getPosition (i, j), getRotation (GetDirection (i, j, "DeadEnd-")));
						deadEndCounter++;
						deadEnd1.name = "DeadEnd " + deadEndCounter;
						lineGameObjects.Add (deadEnd1);
						break;
					case 0:
						GameObject block1 = (GameObject)Instantiate (block, getPosition (i, j), Quaternion.identity);
						blockCounter++;
						block1.name = "Block " + blockCounter;
						lineGameObjects.Add (block1);
						break;
					}
					break;
				case "+":
					switch (Openings ("+", i, j)) {
					case 4:
						GameObject cross0 = (GameObject)Instantiate (cross, getPosition (i, j), getRotation ("Cross"));
						crossCounter++;
						cross0.name = "Cross " + crossCounter;
						lineGameObjects.Add (cross0);
						break;
					case 3:
						GameObject tCross0 = (GameObject)Instantiate (tCross, getPosition (i, j), getRotation (GetDirection (i, j, "TCross")));
						tCrossCounter++;
						tCross0.name = "TCross " + tCrossCounter;
						lineGameObjects.Add (tCross0);
						break;
					case 2:
						switch (GetKind (i, j)) {
						case "Corner":
							GameObject corner0 = (GameObject)Instantiate (corner, getPosition (i, j), getRotation (GetDirection (i, j, "Corner")));
							cornerCounter++;
							corner0.name = "Corner " + cornerCounter;
							lineGameObjects.Add (corner0);
							break;
						case "CorridorV":
							GameObject corridorV0 = (GameObject)Instantiate (corridor, getPosition (i, j), getRotation ("CorridorV"));
							corridorCounter++;
							corridorV0.name = "Corridor " + corridorCounter;
							lineGameObjects.Add (corridorV0);
							break;
						case "CorridorH":
							GameObject corridorH0 = (GameObject)Instantiate (corridor, getPosition (i, j), getRotation ("CorridorH"));
							corridorCounter++;
							corridorH0.name = "Corridor " + corridorCounter;
							lineGameObjects.Add (corridorH0);
							break;
						}
						break;
					case 1:
						GameObject deadEnd2 = (GameObject)Instantiate (deadEnd, getPosition (i, j), getRotation (GetDirection (i, j, "DeadEnd+")));
						deadEndCounter++;
						deadEnd2.name = "DeadEnd " + deadEndCounter;
						lineGameObjects.Add (deadEnd2);
						break;
					case 0:
						GameObject block0 = (GameObject)Instantiate (block, getPosition (i, j), Quaternion.identity);
						blockCounter++;
						block0.name = "Block " + blockCounter;
						lineGameObjects.Add (block0);
						break;
					}
					break;
				default:
					break;
     				}
			
			}
			gameObjects.Add(lineGameObjects);
		}
		SetWayPoints ();			
		gameObjects.Add (GetEntities());
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

	int Openings(string type, int zPos, int xPos) {
		int ports = 0;
		switch (type) {
		case "+":
			ports = 4;
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
			break;
		case "|":
			ports = 2;
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
			break;
		case "-":
			ports = 2;
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
			break;
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
		case "DeadEnd+":
			if (zPos > 0 && zPos < endLine && xPos > 0 && xPos < lineLength) {
				if (("" + lines [zPos + 1] [xPos]).Equals ("|") || ("" + lines [zPos + 1] [xPos]).Equals ("+"))
					return "DeadEnddown";
				if (("" + lines [zPos - 1] [xPos]).Equals ("|") || ("" + lines [zPos - 1] [xPos]).Equals ("+"))
					return "DeadEndup";
				if (("" + lines [zPos] [xPos + 1]).Equals ("-") || ("" + lines [zPos] [xPos + 1]).Equals ("+"))
					return "DeadEndright";
				if (("" + lines [zPos] [xPos - 1]).Equals ("-") || ("" + lines [zPos] [xPos - 1]).Equals ("+"))
					return "DeadEndleft";
			} else if (zPos == 0 || zPos == endLine) {
				if (zPos == 0) {
					if (("" + lines [zPos + 1] [xPos]).Equals ("|") || ("" + lines [zPos + 1] [xPos]).Equals ("+"))
						return "DeadEnddown";
				}
				if (zPos == endLine) {
					if (("" + lines [zPos - 1] [xPos]).Equals ("|") || ("" + lines [zPos - 1] [xPos]).Equals ("+"))
						return "DeadEndup";
				}
				if (xPos == 0)
					return "DeadEndright";
				if (xPos == lineLength)
					return "DeadEndleft";
				if (xPos > 0 && xPos < lineLength) {
					if (("" + lines [zPos] [xPos + 1]).Equals ("-") || ("" + lines [zPos] [xPos + 1]).Equals ("+")) {
						return "DeadEndright";
					}
					else
						return "DeadEndleft";
				}
			} else if (xPos == 0 || xPos == lineLength) {
				if (xPos == 0) {
					if (("" + lines [zPos] [xPos + 1]).Equals ("-") || ("" + lines [zPos] [xPos + 1]).Equals ("+"))
						return "DeadEndright";
				}
				if (xPos == lineLength) {
					if (("" + lines [zPos] [xPos - 1]).Equals ("-") || ("" + lines [zPos] [xPos - 1]).Equals ("+"))
						return "DeadEndleft";
				}
				if (zPos > 0 && zPos < endLine) {
					if (("" + lines [zPos - 1] [xPos]).Equals ("|") || ("" + lines [zPos - 1] [xPos]).Equals ("+"))
						return "DeadEndup";
					else
						return "DeadEnddown";
				}
			}
			break;
		case "DeadEnd|":
			if (zPos == 0)
				return "DeadEnddown";
			else if (("" + lines [zPos - 1] [xPos]).Equals ("-"))
				return "DeadEnddown";
			else if (zPos == endLine)
				return "DeadEndup";
			else if (("" + lines [zPos + 1] [xPos]).Equals ("-"))
				return "DeadEndup";
			break;
		case "DeadEnd-":
			if (xPos == 0)
				return "DeadEndright";
			else if (("" + lines [zPos] [xPos - 1]).Equals ("|"))
				return "DeadEndright";
			else if (xPos == lineLength)
				return "DeadEndleft";
			else if (("" + lines [zPos] [xPos + 1]).Equals ("|"))
				return "DeadEndleft";
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
		for (int i = 0; i < gameObjects.Count; i++) {
			List<GameObject> line = gameObjects [i];
			for (int j = 0; j < line.Count; j++) {
				GameObject go = line [j];
				switch (GetName (go)) {
				case "Corridor":
					if ((int)go.transform.rotation.eulerAngles.y == 90) {
						SetLeftWayPoint (go, i, j);
						SetRightWayPoint (go, i, j);
					} else {
						SetUpWayPoint (go, i, j);
						SetDownWayPoint(go, i, j);
					}
					break;
				case "Cross":
					SetUpWayPoint (go, i, j);
					SetDownWayPoint (go, i, j);
					SetLeftWayPoint (go, i, j);
					SetRightWayPoint (go, i, j);
					break;
				case "TCross":
					switch ((int)go.transform.rotation.eulerAngles.y) {
					case 90:
						SetDownWayPoint (go, i, j);
						SetLeftWayPoint (go, i, j);
						SetRightWayPoint (go, i, j);
						break;
					case 180:
						SetUpWayPoint (go, i, j);
						SetDownWayPoint (go, i, j);
						SetLeftWayPoint (go, i, j);
						break;
					case 270:
						SetUpWayPoint (go, i, j);
						SetLeftWayPoint (go, i, j);
						SetRightWayPoint (go, i, j);
						break;
					default:
						SetUpWayPoint (go, i, j);
						SetDownWayPoint (go, i, j);
						SetRightWayPoint (go, i, j);
						break;
					}
					break;
				case "Corner":
					switch ((int)go.transform.rotation.eulerAngles.y) {
					case 90:
						SetUpWayPoint (go, i, j);
						SetLeftWayPoint (go, i, j);
						break;
					case 180:
						SetUpWayPoint (go, i, j);
						SetRightWayPoint (go, i, j);
						break;
					case 270:
						SetDownWayPoint (go, i, j);
						SetRightWayPoint (go, i, j);
						break;
					default:
						SetDownWayPoint (go, i, j);
						SetLeftWayPoint (go, i, j);
						break;
					}
					break;
				case "DeadEnd":
					switch ((int)go.transform.rotation.eulerAngles.y) {
					case 90:
						SetLeftWayPoint (go, i, j);
						break;
					case 180:
						SetUpWayPoint (go, i, j);
						break;
					case 270:
						SetRightWayPoint (go, i, j);
						break;
					default:
						SetDownWayPoint (go, i, j);
						break;
					}
					break;
				default:
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

	void SetUpWayPoint(GameObject go, int zPos, int xPos) {
		go.GetComponent<WayPoint> ().upWaypoint = gameObjects [zPos - 1] [xPos].GetComponent<WayPoint> ();
	}

	void SetDownWayPoint(GameObject go, int zPos, int xPos) {
		go.GetComponent<WayPoint> ().downWaypoint = gameObjects [zPos + 1] [xPos].GetComponent<WayPoint> ();
	}
		
	void SetLeftWayPoint(GameObject go, int zPos, int xPos) {
		go.GetComponent<WayPoint> ().leftWaypoint = gameObjects [zPos] [xPos - 1].GetComponent<WayPoint> ();
	}

	void SetRightWayPoint(GameObject go, int zPos, int xPos) {
		go.GetComponent<WayPoint> ().rightWaypoint = gameObjects [zPos] [xPos + 1].GetComponent<WayPoint> ();
	}

	List<GameObject> GetEntities () {
		List<GameObject> entities = new List<GameObject> ();
		for (int i = endLine + 1; i < lines.Count; i++) {
			int xPos;
			int zPos;
			if (lines[i].Contains ("Pacman")) {
				xPos = int.Parse ("" + lines[i] [7]);
				zPos = int.Parse ("" + lines[i] [9]);
				GameObject pacman0 = (GameObject)Instantiate (pacman, new Vector3 (xPos, 0f, -zPos), Quaternion.identity);
				pacman0.name = "Pacman";
				pacman0.transform.localScale = Vector3.one * 0.45f;
				entities.Add (pacman0);
			}
			if (lines[i].Contains ("Blinky")) {
				xPos = int.Parse ("" + lines[i] [7]);
				zPos = int.Parse ("" + lines[i] [9]);
				GameObject blinky0 = (GameObject)Instantiate (blinky, new Vector3 (xPos, 0f, -zPos), Quaternion.identity);
				blinky0.name = "Blinky";
				blinky0.transform.localScale = Vector3.one * 0.45f;
				blinky0.GetComponent<EnemyBehaviourScript> ().currentWaypoint = gameObjects [zPos] [xPos].GetComponent<WayPoint> ();
				entities.Add (blinky0);
			}
			if (lines[i].Contains ("Inky")) {
				xPos = int.Parse ("" + lines[i] [5]);
				zPos = int.Parse ("" + lines[i] [7]);
				GameObject inky0 = (GameObject)Instantiate (inky, new Vector3 (xPos, 0f, -zPos), Quaternion.identity);
				inky0.name = "Inky";
				inky0.transform.localScale = Vector3.one * 0.45f;
				inky0.GetComponent<EnemyBehaviourScript> ().currentWaypoint = gameObjects [zPos] [xPos].GetComponent<WayPoint> ();
				entities.Add (inky0);
			}
			if (lines[i].Contains ("Pinky")) {
				xPos = int.Parse ("" + lines[i] [6]);
				zPos = int.Parse ("" + lines[i] [8]);
				GameObject pinky0 = (GameObject)Instantiate (pinky, new Vector3 (xPos, 0f, -zPos), Quaternion.identity);
				pinky0.name = "Pinky";
				pinky0.transform.localScale = Vector3.one * 0.45f;
				pinky0.GetComponent<EnemyBehaviourScript> ().currentWaypoint = gameObjects [zPos] [xPos].GetComponent<WayPoint> ();
				entities.Add (pinky0);
			}
			if (lines[i].Contains ("Clyde")) {
				xPos = int.Parse ("" + lines[i] [6]);
				zPos = int.Parse ("" + lines[i] [8]);
				GameObject clyde0 = (GameObject)Instantiate (clyde, new Vector3 (xPos, 0f, -zPos), Quaternion.identity);
				clyde0.name = "Clyde";
				clyde0.transform.localScale = Vector3.one * 0.45f;
				clyde0.GetComponent<EnemyBehaviourScript> ().currentWaypoint = gameObjects [zPos] [xPos].GetComponent<WayPoint> ();
				entities.Add (clyde0);
			}
		}
		return entities;
	}

	string GetKind(int zPos, int xPos) {
		bool corridorV = true;
		bool corridorH = true;
		if (xPos == 0 || xPos == lineLength)
			corridorH = false;
		if (zPos == 0 || zPos == endLine)
			corridorV = false;
		if (corridorH && (("" + lines [zPos] [xPos - 1]).Equals ("|") || ("" + lines [zPos] [xPos + 1]).Equals ("|")))
			corridorH = false;
		if (corridorV && (("" + lines [zPos - 1] [xPos]).Equals ("-") || ("" + lines [zPos + 1] [xPos]).Equals ("-")))
			corridorV = false;
		if (corridorH)
			return "CorridorH";
		else if (corridorV)
			return "CorridorV";
		else
			return "Corner";
			
	}

}
