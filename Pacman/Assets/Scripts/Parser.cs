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

	private List<string> lines = new List<string> ();
	private List<GameObject> gameObjects = new List<GameObject> ();

	private int endLine;
	private int lineLength;

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
				string c = ""+line [j];
				switch (c) {
				case ("|"):
					GameObject temp0 = (GameObject)Instantiate (corridor, getPosition (i, j), getRotation ("CorridorV"));
					temp0.name = "Corridor " + i + "" + j;
					gameObjects.Add (temp0);
					break;
				case "-":
					GameObject temp1 = (GameObject)Instantiate (corridor, getPosition (i, j), getRotation ("CorridorH"));
					temp1.name = "Corridor " + i + "" + j;
					gameObjects.Add (temp1);
					break;
				case "+":
					switch (Openings (i, j)) {
					case 4:
						GameObject temp2 = (GameObject)Instantiate (cross, getPosition (i, j), getRotation ("Cross"));
						temp2.name = "Cross " + i + "" + j;
						gameObjects.Add (temp2);
						break;
					case 3:
						GameObject temp3 = (GameObject)Instantiate (tCross, getPosition (i, j), getRotation (GetDirection (i, j, "TCross")));
						temp3.name = "TCross " + i + "" + j;
						gameObjects.Add (temp3);
						break;
					case 2:
						GameObject temp4 = (GameObject)Instantiate (corner, getPosition (i, j), getRotation (GetDirection (i, j, "Corner")));
						temp4.name = "Corner " + i + "" + j;
						gameObjects.Add (temp4);
						break;
					case 1:
						GameObject temp5 = (GameObject)Instantiate (deadEnd, getPosition (i, j), getRotation (GetDirection (i, j, "DeadEnd")));
						temp5.name = "DeadEnd " + i + "" + j;
						gameObjects.Add (temp5);
						break;
					default:
						break;
					}
					break;
				default:
					break;
				}
			}
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
			break;
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
			} else if (zPos == 0) {
				if (("" + lines [zPos + 1] [xPos]).Equals ("|") || ("" + lines [zPos + 1] [xPos]).Equals ("+"))
					return type + "down";
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
			} else if (zPos == endLine) {
				if (("" + lines [zPos - 1] [xPos]).Equals ("|") || ("" + lines [zPos - 1] [xPos]).Equals ("+"))
					return type + "up";
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
			} else if (xPos == 0) {
				if (("" + lines [zPos] [xPos + 1]).Equals ("-") || ("" + lines [zPos] [xPos + 1]).Equals ("+"))
					return type + "right";
				if (zPos > 0 && zPos < endLine) {
					if (("" + lines [zPos - 1] [xPos]).Equals ("|") || ("" + lines [zPos - 1] [xPos]).Equals ("+"))
						return type + "up";
					else
						return type + "down";
				}
			} else if (xPos == lineLength) {
				if (("" + lines [zPos] [xPos - 1]).Equals ("-") || ("" + lines [zPos] [xPos - 1]).Equals ("+"))
					return type + "left";
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




}
