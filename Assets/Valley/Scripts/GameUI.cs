using UnityEngine;
using System.Collections;

public class GameUI : MonoBehaviour {

	MapMaker stage;

	// Use this for initialization
	void Start () {
        stage = MapMaker._instance;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

	void OnGUI () {
		GUI.Box(new Rect(10,10,100,240), "Stages");
		
		if(GUI.Button(new Rect(20,40,80,20), "Stage 1")) {
			stage.Load (1);
		}
		
		if(GUI.Button(new Rect(20,70,80,20), "Stage 2")) {
			stage.Load (2);
		}

		if(GUI.Button(new Rect(20,100,80,20), "Stage 3")) {
			stage.Load (3);
		}

		if(GUI.Button(new Rect(20,130,80,20), "Stage 4")) {
			stage.Load (4);
		}

        if (GUI.Button(new Rect(20, 160, 80, 20), "Stage 5"))
        {
            stage.Load(5);
        }
        if (GUI.Button(new Rect(20, 190, 80, 20), "Stage 6"))
        {
            stage.Load(6);
        }
        if (GUI.Button(new Rect(20, 220, 80, 20), "Stage 7"))
        {
            stage.Load(7);
        }
    }
}
