using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemmyMele : log
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckDistance();
    }
        public override void CheckDistance(){
        if(Vector3.Distance(target.position, transform.position) <= chaseRadious
        && Vector3.Distance(target.position, transform.position) > attackRadious){

            if(currentState == EnemyState.idle || currentState == EnemyState.walk && currentState != EnemyState.stagger){
            Vector3 temp = Vector3.MoveTowards(transform.position, target.position, enemySpeed * Time.deltaTime);
            changeAnim(temp - transform.position);
            myRigidBody2D.MovePosition(temp);
            ChangeState(EnemyState.walk);
            }
        }   
        else if(Vector3.Distance(target.position, transform.position) <= chaseRadious
        && Vector3.Distance(target.position, transform.position) <= attackRadious){
            if(currentState == EnemyState.walk && currentState != EnemyState.stagger){
            StartCoroutine(AttackCo());
            }
        }
    }

    public IEnumerator AttackCo()
    {
        currentState = EnemyState.attack;
        anim.SetBool("Attacking", true);
        yield return new WaitForSeconds(1f);
        currentState = EnemyState.walk;
        anim.SetBool("Attacking", false);
    }
}
