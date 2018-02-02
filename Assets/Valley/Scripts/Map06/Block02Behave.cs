using UnityEngine;
using System.Collections;

public class Block02Behave : MonoBehaviour {

    void Start()
    {
        this.gameObject.tag = "Floor";
        this.GetComponent<Renderer>().material.color = new Color(1,0,1);
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Player")
        {
            //Debug.Log("fddsajdfjdsafjdsa");
            iTween.MoveTo(this.gameObject, new Vector3(6, 7.45f, 6), 2f);
           
        }
    }
}
