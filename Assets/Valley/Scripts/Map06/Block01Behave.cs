using UnityEngine;
using System.Collections;

public class Block01Behave : MonoBehaviour {

    private GameObject[] cubeMove = new GameObject[5];
    // Use this for initialization
    void Start()
    {
        this.gameObject.tag = "Floor";
        cubeMove[0] = GameObject.Find("C022");
        cubeMove[1] = GameObject.Find("C021");
        cubeMove[2] = GameObject.Find("C020");
        cubeMove[3] = GameObject.Find("C-120");
        cubeMove[4] = GameObject.Find("C-220");
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Player")
        {
            this.GetComponent<BoxCollider>().enabled = false;
            //Debug.Log("fddsajdfjdsafjdsa");
            iTween.MoveTo(this.gameObject, new Vector3(-4, 11.45f,-11), 2f);
            
            for (int i =0;i<cubeMove.Length;i++)
            {
                iTween.MoveTo(cubeMove[i],iTween.Hash("y",7,"time",1));
            }
           

        }
    }
}
