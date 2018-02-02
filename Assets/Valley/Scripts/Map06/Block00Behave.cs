using UnityEngine;
using System.Collections;

public class Block00Behave : MonoBehaviour {

    private GameObject cubeMove;
    private GameObject block01Move;
	// Use this for initialization
	void Start () {
        this.gameObject.tag = "Floor";
        cubeMove = GameObject.Find("C-410-11");
        block01Move = GameObject.Find("B-410.6-11");
	}
	
    void OnCollisionEnter(Collision col)
    {
        if(col.collider.tag == "Player")
        {
            //Debug.Log("fddsajdfjdsafjdsa");
            iTween.MoveTo(this.gameObject,new Vector3(-5,2.45f,0),2f);
            iTween.MoveTo(cubeMove,new Vector3(-4,11,-11),2f);
            iTween.MoveTo(block01Move, new Vector3(-4, 11.6f, -11),2f);
        }
    }
}
