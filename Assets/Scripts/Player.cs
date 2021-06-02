//모든 플레이어의 입력이 오는 곳

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]

public class Player : LivingEntity //LivingEntitiy 를 상속 받는 이유: 이미 LivingEntitiy 에서 Mono와 IDamage를 상속 받고 있다.  
{

	public float moveSpeed = 5;

	Camera viewCamera;
	PlayerController controller;
	GunController gunController;

	protected override void Start()
	{
		base.Start();
		gunController = GetComponent<GunController>(); //스크립트가 오브젝트에 미리 붙어 있는 상황 가정 시 GetComponent가 젤 좋은 방법
		controller = GetComponent<PlayerController>();
		viewCamera = Camera.main;
	}

	void Update()
	{
		//이동 입력받는 곳
		Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		Vector3 moveVelocity = moveInput.normalized * moveSpeed;
		controller.Move(moveVelocity);

		//바라보는 방향 입력받는 
		Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
		Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
		float rayDistance;

		//바라보는 방향 표시하는 
		if (groundPlane.Raycast(ray, out rayDistance))
		{
			Vector3 point = ray.GetPoint(rayDistance);
			Debug.DrawLine(ray.origin, point, Color.red);
			//Debug.DrawRay(ray.origin,ray.direction * 100,Color.red);
			controller.LookAt(point);
		}

		//무기 조작입력
		if(Input.GetMouseButtonDown(0))
        {
			gunController.Shoot();

		}

		
		
	}
}