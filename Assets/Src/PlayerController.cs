using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	
	//移动速度
	public float MoveSpeed=1.5F;
	//奔跑速度
	public float RunSpeed=4.5F;
	//旋转速度
	public float RotateSpeed=30;
	//重力
	public float Gravity=20;
	//动画组件
	private Animator mAnim;
	//声音组件
	private AudioSource mAudio;
	//速度
	private float mSpeed;
	//移动方式,默认为Walk
	public TransportType MoveType=TransportType.Run;
	//角色控制器
	private CharacterController mController;
	

	void Start () 
	{
	   //获取动画组件
	   mAnim=GetComponentInChildren<Animator>();
	   //获取声音组件
	   mAudio=GetComponent<AudioSource>();
	   //获取角色控制器
	   mController=GetComponent<CharacterController>();
	}

	void Update () 
	{
	        MoveManager();
	}
    
	//移动管理
	void MoveManager()
	{
		//移动方向
		Vector3 mDir=Vector3.zero;
		if(mController.isGrounded)
		{
	       if(Input.GetAxis("Vertical")==1)
	       {
		      SetTransportType(MoveType);
			  mDir=Vector3.forward * RunSpeed * Time.deltaTime;
	       }
	       if(Input.GetAxis("Vertical")==-1)
	       {
		      SetTransportType(MoveType);
			  mDir=Vector3.forward * -RunSpeed * Time.deltaTime;
	       }
	       if(Input.GetAxis("Horizontal")==-1)
	       {
		      SetTransportType(MoveType);
		      Vector3 mTarget=new Vector3(0,-RotateSpeed* Time.deltaTime,0);
		      transform.Rotate(mTarget);
	       }
	       if(Input.GetAxis("Horizontal")==1)
	       {
		      SetTransportType(MoveType);
		      Vector3 mTarget=new Vector3(0,RotateSpeed* Time.deltaTime,0);
		      transform.Rotate(mTarget);
	       }
	       if(Input.GetAxis("Vertical")==0 && Input.GetAxis("Horizontal")==0)
	       {
		      mAnim.SetBool("Walk",false);
		      mAnim.SetBool("bRun",false);
	       }
	   }
		//考虑重力因素
		mDir=transform.TransformDirection(mDir);
		float y=mDir.y-Gravity *Time.deltaTime;
		mDir=new Vector3(mDir.x,y,mDir.z);
		mController.Move(mDir);

	   //使用Tab键切换移动方式
	   if(Input.GetKey(KeyCode.Tab))
	   {
		  if(MoveType==TransportType.Walk){
			MoveType=TransportType.Run;
		  }else if(MoveType==TransportType.Run){
			MoveType=TransportType.Walk;
		  }
	   }
	}


    
	//设置角色移动的方式
	public void SetTransportType(TransportType MoveType)
	{
	   switch(MoveType)
	   {
			case TransportType.Walk:
				MoveType=TransportType.Walk;
				mAnim.SetBool("Walk",true);
				mSpeed=MoveSpeed;
				break;
			case TransportType.Run:
				MoveType=TransportType.Run;
				mAnim.SetBool("bRun",true);
				mSpeed=RunSpeed;
				break;
	   }
	}

}

public enum TransportType
{
    Run = 1,
    Walk
}
