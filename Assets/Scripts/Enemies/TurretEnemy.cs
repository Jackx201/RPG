using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretEnemy : log
{
    public GameObject projectile;
    public float fireDelay;
    private float fireDelaySeconds;
    public bool canFire;

    private void Update()
    {
        fireDelaySeconds -= Time.deltaTime;
        if(fireDelaySeconds <= 0)
        {
            canFire = true;
            fireDelaySeconds = fireDelay;
        }
    }

    public override void CheckDistance()
    {
        if(Vector3.Distance(target.position, transform.position) <= chaseRadious
        && Vector3.Distance(target.position, transform.position) > attackRadious){

       // Debug.Log("In chase radious");
        
            if(currentState == EnemyState.idle || currentState == EnemyState.walk && currentState != EnemyState.stagger){
                //Debug.Log("Can attack");
                if(canFire)
                {
                    //Debug.Log("Attacking");
                    Vector3 tempVector = target.transform.position - transform.position;
                    GameObject current = Instantiate(projectile, transform.position, Quaternion.identity);
                    current.GetComponent<Projectile>().Launch(tempVector);
                    canFire = false;
                    ChangeState(EnemyState.walk);
                    anim.SetBool("WakeUp", true);
                }
            }

        } else if(Vector3.Distance(target.position, transform.position) > chaseRadious){
            anim.SetBool("WakeUp", false);
        }
    }
}
