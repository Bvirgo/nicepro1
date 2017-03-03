using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildMesh{

	// Use this for initialization
	public static GameObject Create(List<Vector3> verticesList,Texture tex = null,float uv_tile = 1){

		if (verticesList.Count < 3) {
			return null;
        }
        List<Vector3> listWithTop = new List<Vector3>();
        for (int i = 0, length = verticesList.Count; i < length; i++)
        {
            listWithTop.Add(verticesList[i]);
        }

		//有多少个顶点就有多少个三角面
        int[] triangles = new int[listWithTop.Count * 3];

		GameObject go = new GameObject ();
		MeshFilter meshFilter = go.AddComponent<MeshFilter>();
		Mesh mesh = meshFilter.mesh;
		//三角形顶点的坐标数组
        Vector3 centerVertice = getCenterVertice(listWithTop) + new Vector3(0, 0.8f, 0);
        go.transform.position = centerVertice;
        
        listWithTop.Add(centerVertice);
        for (int i = 0, length = listWithTop.Count; i < length; i++)
        {
            listWithTop[i] = listWithTop[i] - go.transform.position;//偏移
        }


        Vector3[] vertices = listWithTop.ToArray();
		mesh.vertices = vertices;

        //按三角面遍历
        for (int i = 0, len = triangles.Length / 3; i < len; i++)
        {
            triangles[i * 3 + 0] = i;
            triangles[i * 3 + 1] = (i + 1) % len;
            triangles[i * 3 + 2] = len;
        }

		mesh.triangles = triangles;

		mesh.uv = getUVs (mesh,uv_tile);

		MeshRenderer meshRender = go.AddComponent<MeshRenderer> ();
        Shader shader = Shader.Find("MyShader/Normal");  
		meshRender.sharedMaterial = new Material (shader);  
		meshRender.sharedMaterial.mainTexture = tex;
        return go;
	}

	private static Vector3 getCenterVertice(List<Vector3> verticesList){
		float x = 0;
		float y = 0;
		float z = 0;
		foreach (Vector3 vertice in verticesList) {
			x += vertice.x;
			y += vertice.y;
			z += vertice.z;
		}
		return new Vector3 (x / verticesList.Count, y / verticesList.Count,z / verticesList.Count);
	}

	private static Vector2[] getUVs(Mesh mesh,float uv_tile){
		if (uv_tile == 0) {
			uv_tile = 1;
		}
		Bounds bounds = mesh.bounds;
		Vector2 min = new Vector2 (mesh.bounds.min.x, mesh.bounds.min.z);
		Vector2 size = new Vector2 (mesh.bounds.size.x, mesh.bounds.size.z);
		List<Vector2> uvList = new List<Vector2> ();
		Vector3[] vertices = mesh.vertices;
		foreach (Vector3 v in vertices) {
			Vector2 tuv = new Vector2();
			tuv.x = (v.x - min.x)/size.x*uv_tile;
			tuv.y = (v.z - min.y)/size.y*uv_tile;
            //Debug.Log(v + "," + tuv);
			uvList.Add(tuv);
		}
		return uvList.ToArray();
	}
}
