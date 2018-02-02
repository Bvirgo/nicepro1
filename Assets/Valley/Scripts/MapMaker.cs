using UnityEngine;
using System.Collections;

public class MapMaker : MonoBehaviour {
    
    public static MapMaker _instance;

    

    private IMapData currentMap;
	Vector3[] slopeVertices = new Vector3[]{
		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f),

		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),

		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),

		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f),

		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
	};

	int[] slopeTriangles = new int[]{
		0, 1, 2,
		0, 2, 3,
		4, 6, 5,
		8, 7, 9,
		10, 12, 11,
		10, 13, 12,
		14, 17, 15,
		15, 17, 16,
	};
	
	GameObject makeSlope () {
		GameObject obj = GameObject.CreatePrimitive (PrimitiveType.Cube);
		obj.name = "Slope";
		Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
		mesh.Clear ();
		mesh.vertices = slopeVertices;
		mesh.triangles = slopeTriangles;
		mesh.RecalculateNormals ();

		return obj;
	}

   
    void Awake()
    {
        _instance = this;
    }
	// Use this for initialization
	void Start () {
        IMapData mp7 = new Map07();
        currentMap = mp7;
		MakeMap (mp7);
	}

	void MakeMap(IMapData mapData) {
		GameObject[] objs = GameObject.FindGameObjectsWithTag("Floor");
		foreach (GameObject obj in objs)
			GameObject.DestroyImmediate (obj);

		foreach (Vector3 pos in mapData.CubePos) {
			GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
			cube.tag = "Floor";
			cube.name = "C"+pos.x+pos.y+pos.z;	// 'C'ube
			cube.transform.position = pos;
		}
		for (int i = 0; i < mapData.SlopePos.Length; i++) {
			GameObject slope = makeSlope ();
			Vector3 pos = mapData.SlopePos[i];
			slope.tag = "Floor";
			slope.name = "S"+pos.x+pos.y+pos.z;	// 'S'lope
			slope.transform.position = pos;
			slope.transform.rotation = mapData.SlopeRot[i];
		}

		foreach (RotatingObject data in mapData.RotObj) {
			GameObject obj = GameObject.Find(data.name);
			if (obj == null) continue;
			Rotating script = obj.AddComponent<Rotating>() as Rotating;
			script.point = data.point;
			script.axis = data.axis;
			script.PlayerRot = data.playerRot;
		}
		{
			GameObject player = GameObject.Find ("Player");
			player.transform.position = new Vector3(mapData.PlayerStart.x, mapData.PlayerStart.y + 1, mapData.PlayerStart.z);
			player.GetComponent<MoveCharacter>().Init();
		}
	}

	public void Load (int stage_id) {
		switch (stage_id) {
		case 1:
            IMapData mp01 = new Map01();
            currentMap = mp01;
            MakeMap (mp01);
			break;
		case 2:
            IMapData mp02 = new Map02();
            currentMap = mp02;
            MakeMap (mp02);
			break;
		case 3:
            IMapData mp03 = new Map03();
            currentMap = mp03;
            MakeMap (mp03);
			break;
		case 4:
            IMapData mp04 = new Map04();
            currentMap = mp04;
            MakeMap (mp04);
			break;
		case 5:
            IMapData mp05 = new Map05();
            currentMap = mp05;
            MakeMap (mp05);
			break;
        case 6:
            IMapData mp06 = new Map06();
            currentMap = mp06;
            MakeMap(mp06);
            break;
            case 7:
                IMapData mp07 = new Map07();
                currentMap = mp07;
                MakeMap(mp07);
                break;
		}
	}
	
    //public void ChangeRotateAxis(Vector3 newAxis)
    //{
    //    for(int i=0;i<currentMap.RotObj.Length;i++)
    //    {
    //        Debug.Log("b????"+currentMap.RotObj[i].point);
    //        currentMap.RotObj[i].point = newAxis;
    //        Debug.Log("a????"+currentMap.RotObj[i].point);
    //    }
    //}

	// Update is called once per frame
	void Update () {

    	
	}
}
