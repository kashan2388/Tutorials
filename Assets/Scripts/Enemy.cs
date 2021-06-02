using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
	public enum State
	{
		Idle, //아무것도 하지 않는 상태
		Chasing, //플레이어를 추격하는 상태
		Attacking  //공격하는 도중의 상태
	};
	State currentState;

	NavMeshAgent pathfinder;
	Transform target;
	LivingEntity targetEntity;
	Material skinMaterial;

	Color originalColour;

	float attackDistanceThreshold = .5f;//공격거리 임계값, 공격 한계 거리
	float timeBetweenAttacks = 1; //공격 사이 시간
	float damage = 1;

	float nextAttackTime; //공격 가능 시간
	float myCollisionRadius; //적 그 자신의 것
	float targetCollisionRadius; //목표의 충돌범위를 위한 것 

	bool hasTarget;

	protected override void Start()
	{
		base.Start();
		pathfinder = GetComponent<NavMeshAgent>();
		skinMaterial = GetComponent<Renderer>().material;
		originalColour = skinMaterial.color;


		if(GameObject.FindGameObjectWithTag("Player") != null) //플레이어 오브젝트가 존재하고 있을 때만 구문 실행
        {
			currentState = State.Chasing; //추격 상태 
			hasTarget = true; //목표가 존재 

			target = GameObject.FindGameObjectWithTag("Player").transform;
			targetEntity = target.GetComponent<LivingEntity>();
			targetEntity.OnDeath += OnTargetDeath;


			myCollisionRadius = GetComponent<CapsuleCollider>().radius;//캡슐 콜라이더의 반지름 값
			targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

			StartCoroutine(UpdatePath());
		}
		
	}

	//목표가 죽었을 때
	void OnTargetDeath()
    {
		hasTarget = false;
		currentState = State.Idle;

    }

	void Update()
	{
		if (hasTarget)
		{
			if (Time.time > nextAttackTime) //현재의 시간이 공격 가능 시간보다 이후
			{
				float sqrDstToTarget = (target.position - transform.position).sqrMagnitude; //목표까지의 제곱거리  (Vector3).sqrMagnitude //목표의 위치와 자신의 위치의 차에 제곱을 한 수를 가져온다
				if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2)) //2만큼 제곱, 참일 때 공격 
				{
					nextAttackTime = Time.time + timeBetweenAttacks; //다음 공격 시간은 현재시간 + 공격 간격

					StartCoroutine(Attack());
				}

			}

		}
	}

	IEnumerator Attack() //찌르기를 애니메이트 하고 싶다..? 시작점과 목표점 저장 시작점 ->목표점 이동->플레이어 보면 시작점 복귀
	{

		currentState = State.Attacking;
		pathfinder.enabled = false;

		Vector3 originalPosition = transform.position; //현재 위치 저장
		Vector3 dirToTarget = (target.position - transform.position).normalized; //목표로의 방향벡터 얻어옴 
		Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

		float attackSpeed = 3; //값이 높을 수록 공격 애니메가 빨라진다
		float percent = 0;

		skinMaterial.color = Color.yellow;
		bool hasAppliedDamage = false;

		while (percent <= 1)//찌르기 동작을 함 코루틴이라 yield return null사용 이는 while루프의 각 처리 사이에서 프레임 스킵 
		{//yield return 지점에서 작업을 멈추고 update 메소드 작업 완전 수행 이후 다음 프레임으로 넘어갔을 때 yield 키워드 아래의 코드 혹은 다음 루프 실행

			if(percent >= 0.5f && !hasAppliedDamage)
            {
				hasAppliedDamage = true;
				targetEntity.TakeDamage(damage);
            }
			percent += Time.deltaTime * attackSpeed;
			float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;//transform.position값이 originalP에서 출발해서 attackP로 가며, interp을 참조! 
			transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
			

			yield return null;
		}

		skinMaterial.color = originalColour;
		currentState = State.Chasing;
		pathfinder.enabled = true;
	}

	IEnumerator UpdatePath()//코루틴 한번 호출시 루프 실행. refresh정의 대로 1초마다 루프 반복
	{
		float refreshRate = .25f;

		while (hasTarget)//타겟 존재 동안
		{
			if (currentState == State.Chasing)
			{
				Vector3 dirToTarget = (target.position - transform.position).normalized;//목표로의 방향 벡터를 얻었다! 
				Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
				if (!dead)
				{
					pathfinder.SetDestination(targetPosition);
				}
			}
			yield return new WaitForSeconds(refreshRate);
		}
	}
}