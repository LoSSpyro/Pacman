using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Text;

public class SyntaxCheck : MonoBehaviour {

	public GameObject inputFieldObject;

	private Parser parser;
	private InputField inputField;

	private List<string> lines = new List<string> ();

	// Use this for initialization
	void Start () {
		parser = GetComponent<Parser> ();
		inputField = inputFieldObject.GetComponent<InputField> ();
		inputField.onEndEdit.AddListener (Check);
	}

	void Check(string text) {
		if (ParseFile (text)) {
			if (CheckSyntax ())
				inputFieldObject.SetActive (false);
				parser.getField (lines);
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

	bool CheckSyntax () {
		int lineLength = lines [0].Length;
		int endLine = GetEndLine ();
		for (int i = 0; i < lines.Count; i++) {
			string line = "" + lines[i];
			if (i < endLine + 1) {
				if (line.Length != lineLength) {
					Debug.LogError ("Line Length: Not all lines have equal length.");
					return false;
				} else {
					for (int j = 0; j < line.Length; j++) {
						string c = "" + line [j];
						if (!(c == "+" || c == "-" || c == "|")) {
							Debug.LogError ("Unknown Character \"" + c + "\"");
							return false;
						}
					}
				}
			} else if (lines.Count < endLine + 6) {
				Debug.LogError ("Missing some Information!");
				return false;
			} else if (lines.Count > endLine + 6) {
				Debug.LogError ("Unknown Information at the end");
				return false;
			}
		}

		if (!lines [endLine + 1].Contains ("Pacman")) {
			Debug.LogError ("Missing Pacman");
			return false;
		} else {
			int emptyCount = 0;
			for (int k = 6; k < lines [endLine + 1].Length; k++) {
				char c = lines [endLine + 1] [k];
				if (Char.IsWhiteSpace (c))
					emptyCount++;
				else if (emptyCount > 2) {
					Debug.LogError ("Unknown Information at the end in Line " + (endLine + 1));
					return false;
				} else if (!char.IsNumber (c)) {
					Debug.LogError ("Position has to be a Number not \"" + c + "\"");
					return false;
				}
			}
			if (emptyCount < 2) {
				Debug.LogError ("Missing position information");
				return false;
			}
		}

		if (!lines [endLine + 2].Contains ("Blinky")) {
			Debug.LogError ("Missing Blinky");
			return false;
		} else {
			int emptyCount = 0;
			for (int k = 6; k < lines [endLine + 2].Length; k++) {
				char c = lines [endLine + 2] [k];
				if (Char.IsWhiteSpace (c))
					emptyCount++;
				else if (emptyCount > 2) {
					Debug.LogError ("Unknown Information at the end in Line " + (endLine + 2));
					return false;
				} else if (!char.IsNumber (c)) {
					Debug.LogError ("Position has to be a Number not \"" + c + "\"");
					return false;
				}
			}
			if (emptyCount < 2) {
				Debug.LogError ("Missing position information");
				return false;
			}
		}

		if (!lines [endLine + 3].Contains ("Inky")) {
			Debug.LogError ("Missing Inky");
			return false;
		} else {
			int emptyCount = 0;
			for (int k = 4; k < lines [endLine + 3].Length; k++) {
				char c = lines [endLine + 3] [k];
				if (Char.IsWhiteSpace (c))
					emptyCount++;
				else if (emptyCount > 2) {
					Debug.LogError ("Unknown Information at the end in Line " + (endLine + 3));
					return false;
				} else if (!char.IsNumber (c)) {
					Debug.LogError ("Position has to be a Number not \"" + c + "\"");
					return false;
				}
			}
			if (emptyCount < 2) {
				Debug.LogError ("Missing position information");
				return false;
			}
		}

		if (!lines [endLine + 4].Contains ("Pinky")) {
			Debug.LogError ("Missing Pinky");
			return false;
		} else {
			int emptyCount = 0;
			for (int k = 5; k < lines [endLine + 4].Length; k++) {
				char c = lines [endLine + 4] [k];
				if (Char.IsWhiteSpace (c))
					emptyCount++;
				else if (emptyCount > 2) {
					Debug.LogError ("Unknown Information at the end in Line " + (endLine + 4));
					return false;
				} else if (!char.IsNumber (c)) {
					Debug.LogError ("Position have to be a Number not \"" + c + "\"");
					return false;
				}
			}
			if (emptyCount < 2) {
				Debug.LogError ("Missing position information");
				return false;
			}
		}


		if (!lines [endLine + 5].Contains ("Clyde")) {
			Debug.LogError ("Missing Clyde");
			return false;
		} else {
			int emptyCount = 0;
			for (int k = 5; k < lines [endLine + 5].Length; k++) {
				char c = lines [endLine + 5] [k];
				if (Char.IsWhiteSpace (c))
					emptyCount++;
				else if (emptyCount > 2) {
					Debug.LogError ("Unknown Information at the end in Line " + (endLine + 5));
					return false;
				} else if (!char.IsNumber (c)) {
					Debug.LogError ("Position has to be a Number not \"" + c + "\"");
					return false;
				}
			}
			if (emptyCount < 2) {
				Debug.LogError ("Missing position information");
				return false;
			}
		}

		return true;
	}

	int GetEndLine() {
		int i = 0;
		foreach (string line in lines) {
			if (!(line.Contains ("+") || line.Contains ("-") || line.Contains ("|")))
				break;
			else
				i++;
		}
		i--;
		return i;
	}
}
