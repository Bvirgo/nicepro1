using UnityEngine;
using System.Collections;

public class MonoSingletion<T> : MonoBehaviour where T : MonoBehaviour 
{
	protected static bool s_bEnableAutoCreate = true;
	protected static T s_pInstance;
	//public static T Instance
	//{
	//	get
	//	{
	//		return s_pInstance;
	//	}
	//}

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// must invoke when gameobject actived
    /// </summary>
    public void Init()
    {

    }

	public static T Instance
	{
		get
		{
			if (s_pInstance == null)
			{
				s_pInstance = GameObject.FindObjectOfType<T>();
				if (s_pInstance == null && s_bEnableAutoCreate)
				{
					GameObject singleGO = GameObject.Find("Singletion");
					if(singleGO == null)
					{
						singleGO = new GameObject("Singletion");						
					}

					GameObject instanceObject = new GameObject(typeof(T).Name);
					s_pInstance = instanceObject.AddComponent<T>();
					instanceObject.transform.SetParent(singleGO.transform);
				}
				else if (s_pInstance == null)
				{
					//VCity.Logger.error("empty refrenced in this scene : " + typeof(T).Name);
				}
			}
			return s_pInstance;
		}
	}  
}
