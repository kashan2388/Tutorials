using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 100;
    protected float health;
    protected bool dead;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        health = startingHealth;
    }
    public void TakeHit(float damage, RaycastHit hit) //raycasthit변수 사용해서 발사체가 적을 맞춤 지점을 감지할 수 있고, 파티클을 바로 생성... 
    {
        TakeDamage(damage);
    }
    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && dead != true) //0이하 & 아직 안 죽었을 때 
        {
            Die();
        }
        print("플레이어 현재 Hp: " + health);
    }

    protected void Die()
    {
        dead = true;
        if(OnDeath != null)//오브젝트 파괴전 OnDeath 가 null이 아니라면 
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }
}

