using UnityEngine;
using System.Collections;

public class Map06: IMapData
{
    GameObject[] block = new GameObject[3];
    public Map06()
    {
        Vector3[] blockPos = new Vector3[] {
        new Vector3(-5,2.6f,0),
        new Vector3(-4,10.6f,-11),
        new Vector3(6,7.6f,6)
        };
        for (int i = 0; i < blockPos.Length; i++)
        {
            block[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block[i].transform.localScale = new Vector3(0.5f, 0.2f, 0.5f);
            Vector3 pos = blockPos[i];
           //  block[i].tag = "Floor";
            block[i].name = "B" + pos.x + pos.y + pos.z;
            block[i].transform.position = pos;
            block[i].GetComponent<Renderer>().material.color = new Color(0,1,0);
            //block[i].GetComponent<BoxCollider>().isTrigger = true;
        }
        block[0].AddComponent<Block00Behave>();
        block[1].AddComponent<Block01Behave>();
        block[2].AddComponent<Block02Behave>();
    }

    Vector3[] cubePos = new Vector3[]{
        new Vector3(0,2,3),
        new Vector3(0,2,4),
        new Vector3(-3,2,0),
        new Vector3(-4,2,0),

        new Vector3(3,2,0),
        new Vector3(0,7,4),
        new Vector3(0,7,3),
        new Vector3(3,7,0),

        new Vector3(4,7,0),
        new Vector3(-4,11,-7),
        new Vector3(-4,11,-8),
        new Vector3(-4,11,-9),
        new Vector3(4,2,0),
       

        new Vector3(0,2,0),
        new Vector3(0,2,1),
        new Vector3(0,2,2),
        new Vector3(-1,2,0),
        new Vector3(-2,2,0),


        new Vector3(-5,2,0),
        new Vector3(-4,11,-10),
        new Vector3(-4,10,-11),
        new Vector3(5,7,0),
        new Vector3(6,7,0),
        new Vector3(6,7,1),
        new Vector3(6,7,2),
        new Vector3(6,7,3),
        new Vector3(6,7,4),
        new Vector3(6,7,5),
        new Vector3(0,7,5),
        new Vector3(0,7,6),
        new Vector3(1,7,6),
        new Vector3(2,7,6),
        new Vector3(3,7,6),
        new Vector3(4,7,6),
        new Vector3(5,7,6),
        new Vector3(6,7,6),

    };

    Vector3[] slopePos = new Vector3[]{
         new Vector3(-5,11,-9),
    };

    

    Quaternion[] slopeRot = new Quaternion[]{
        Quaternion.Euler(0,0,90),
    };

    RotatingObject[] rotObj = new RotatingObject[]{
        new RotatingObject("C020", new Vector3(0,2,0), new Vector3(0,1,0), false),
        new RotatingObject("C021", new Vector3(0,2,0), new Vector3(0,1,0), false),
        new RotatingObject("C022", new Vector3(0,2,0), new Vector3(0,1,0), false),
        new RotatingObject("C-120", new Vector3(0,2,0), new Vector3(0,1,0), false),
        new RotatingObject("C-220", new Vector3(0,2,0), new Vector3(0,1,0), false),
    };

    Vector3 playerStart = new Vector3(0, 2, 4);

    public Vector3[] CubePos { get { return cubePos; } }
    public Vector3[] SlopePos { get { return slopePos; } }
    public Quaternion[] SlopeRot { get { return slopeRot; } }
    public RotatingObject[] RotObj { get { return rotObj; } }
    public Vector3 PlayerStart { get { return playerStart; } }

}
