using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    public float speed = 10f;
    public float damage = 1;

    float lifetime = 3;
    float skinWidth = 0.1f;

    private void Start()
    {
        Destroy(gameObject, lifetime);

        Collider[] initialCollisions = Physics.OverlapSphere((transform.position), .1f, collisionMask);
        if(initialCollisions.Length > 0)//총알이 생성되었을 때 어떤 충돌체 오브젝트와 이미 겹친(충돌한) 상태일 때 
        {
            OnHitObject(initialCollisions[0]);
        }
    }

    public void SetSpeed(float newspeed)
    {
        speed = newspeed;
    }
    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward); //새로운 Ray를 할당하고 시작지점과 방향을 주어야 함(발사체 위치, 정면방향)
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance+skinWidth, collisionMask, QueryTriggerInteraction.Collide)) //ray를 넣고 hit을 인자의 초기화 없이 가져오고 거리로 moveDistance , 레이아웃마스크로 collision mask, QueryTriggerInteraction(트리거 콜라이더와의 충돌도 가져올 수 있다)
        {
            //무언가와 충돌한다면 
            OnHitOnject(hit);
        }
    }

    void OnHitOnject(RaycastHit hit) //충돌한 오브젝트 정보를 가져올 RaycastHit을 입력으로
    {
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>(); //hitcollier를 호출해 충돌한 오브젝트를가져온다 
        if(damageableObject != null)
        {
            damageableObject.TakeHit(damage, hit);
        }
        GameObject.Destroy(gameObject);//어딘가 오브젝트에 충돌했을 때 발사체를 파괴

    }
    
    void OnHitObject(Collider c)
    {

        IDamageable damageableObject = c.GetComponent<IDamageable>(); //hitcollier를 호출해 충돌한 오브젝트를가져온다 
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(damage);
        }
        GameObject.Destroy(gameObject);//어딘가 오브젝트에 충돌했을 때 발사체를 파괴
    }
}
