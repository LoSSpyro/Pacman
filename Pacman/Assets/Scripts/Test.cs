using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	public GameObject deadEnd1;
	public GameObject deadEnd2;
	// Use this for initialization
	void Start () {
		deadEnd1.GetComponent<WayPoint> ().leftWaypoint = deadEnd2.GetComponent<WayPoint> ();
		deadEnd2.GetComponent<WayPoint> ().rightWaypoint = deadEnd1.GetComponent<WayPoint> ();
		Debug.Log ((int)deadEnd1.transform.rotation.eulerAngles.y);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
