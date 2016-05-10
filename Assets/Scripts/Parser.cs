using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Text;

public class Parser : MonoBehaviour {

	public GameObject inputFieldObject;

	private string filename;
	private InputField inputField;

	private ArrayList lines;

	void Start () {
		inputField = inputFieldObject.GetComponent<InputField> ();
		inputField.onEndEdit.AddListener (getFile);
	}


	public void getFile (string text) {
		filename = text;
		Debug.Log (filename);
		inputFieldObject.SetActive (false);

	}

	void ParseFile (string filename) {
		string line;
		StreamReader reader = new StreamReader (filename, Encoding.Default);
		using (reader) {
			do {
				line = reader.ReadLine ();
				if (line != null) {
					lines.Add (line);
				}
			} while (line != null);
			reader.Close ();
		}
	}
}
