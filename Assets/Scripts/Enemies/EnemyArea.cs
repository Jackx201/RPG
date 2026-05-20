using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArea : log
{
    public Collider2D boundary;

    public override void CheckDistance()
    {
        if (Vector3.Distance(target.position, transform.position) <= chaseRadious
        && Vector3.Distance(target.position, transform.position) > attackRadious
        && boundary.bounds.Contains(target.transform.position))
        {

            if ((currentState == EnemyState.idle && playerState.myState != GenericState.dead) || (currentState == EnemyState.walk && currentState != EnemyState.stagger))
            {
                Vector3 temp = Vector3.MoveTowards(transform.position, target.position, enemySpeed * Time.deltaTime);
                changeAnim(temp - transform.position);
                myRigidBody2D.MovePosition(temp);
                ChangeState(EnemyState.walk);
                anim.SetBool("WakeUp", true);
            }

            if(playerState.myState == GenericState.dead){
            anim.SetBool("WakeUp", false);
            ChangeState(EnemyState.idle);
            }

        }
        else if (Vector3.Distance(target.position, transform.position) > chaseRadious
            || !boundary.bounds.Contains(target.transform.position))
        {
            anim.SetBool("WakeUp", false);
            ChangeState(EnemyState.idle);
        }
    }
}
