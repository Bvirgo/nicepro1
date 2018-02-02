using UnityEngine;
using System.Collections;

public class Map07 : IMapData {

    Vector3[] cubePos = new Vector3[]{
        new Vector3(0,0,0),
        new Vector3(0,0,-1),
        new Vector3(0,0,-2),
        new Vector3(0,6,-2),
        new Vector3(0,6,-1),
        new Vector3(0,6,0),
       
         new Vector3(0,6,3),
        new Vector3(0,6,2),
        new Vector3(0,6,1),
        new Vector3(0,6,4),
        new Vector3(0,6,5),
        new Vector3(0,6,6),
        new Vector3(0,6,7),

        new Vector3(2,6,8),
        new Vector3(1,6,8),
        new Vector3(0,6,8),
        new Vector3(3,6,8),
        new Vector3(4,6,8),
        new Vector3(4,6,7),
        new Vector3(4,6,6),
        new Vector3(4,6,5),
        new Vector3(4,6,4),

        new Vector3(-3,0,4),
        new Vector3(-2,0,4),
        new Vector3(-1,0,4),
        new Vector3(0,0,4),
        new Vector3(3,0,4),
        new Vector3(2,0,4),
        new Vector3(1,0,4),

        new Vector3(0,0,8),
        new Vector3(-8,9,1),
        new Vector3(-9,9,1),
       // new Vector3(-9,9,0),
        new Vector3(-4,6,4),
        new Vector3(0,0,9)

    };

    Vector3[] slopePos = new Vector3[]{
        new Vector3(-9,9,0),
    };

    Quaternion[] slopeRot = new Quaternion[]{
        Quaternion.Euler(0,270,90),
    };

    RotatingObject[] rotObj = new RotatingObject[]{
        new RotatingObject("C063", new Vector3(0,6,4), new Vector3(0,1,0), false),
        new RotatingObject("C062", new Vector3(0,6,4), new Vector3(0,1,0), false),
        new RotatingObject("C061", new Vector3(0,6,4), new Vector3(0,1,0), false),
        new RotatingObject("C064", new Vector3(0,6,4), new Vector3(0,1,0), false),
        new RotatingObject("C065", new Vector3(0,6,4), new Vector3(0,1,0), false),
        new RotatingObject("C066", new Vector3(0,6,4), new Vector3(0,1,0), false),
        new RotatingObject("C067", new Vector3(0,6,4), new Vector3(0,1,0), false),

        new RotatingObject("C-304", new Vector3(0,0,4), new Vector3(0,1,0), false),
        new RotatingObject("C-204", new Vector3(0,0,4), new Vector3(0,1,0), false),
        new RotatingObject("C-104", new Vector3(0,0,4), new Vector3(0,1,0), false),
        new RotatingObject("C004", new Vector3(0,0, 4), new Vector3(0,1,0), false),
        new RotatingObject("C304", new Vector3(0,0, 4), new Vector3(0,1,0), false),
        new RotatingObject("C204", new Vector3(0,0, 4), new Vector3(0,1,0), false),
        new RotatingObject("C104", new Vector3(0,0, 4), new Vector3(0,1,0), false),
    };

    Vector3 playerStart = new Vector3(0, 6f, -1);


    public Vector3[] CubePos { get { return cubePos; } }
    public Vector3[] SlopePos { get { return slopePos; } }
    public Quaternion[] SlopeRot { get { return slopeRot; } }
    public RotatingObject[] RotObj { get { return rotObj; } }
    public Vector3 PlayerStart { get { return playerStart; } }
}
