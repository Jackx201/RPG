using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolLog : log
{    

    public Transform[] path;
    public int currentPoint;
    public Transform currentGoal;
    public float roundingDistance;

    public override void CheckDistance(){
        if(Vector3.Distance(target.position, transform.position) <= chaseRadious
        && Vector3.Distance(target.position, transform.position) > attackRadious){

            if(currentState == EnemyState.idle || currentState == EnemyState.walk && currentState != EnemyState.stagger){
            Vector3 temp = Vector3.MoveTowards(transform.position, target.position, enemySpeed * Time.deltaTime);
            changeAnim(temp - transform.position);
            myRigidBody2D.MovePosition(temp);
            //ChangeState(EnemyState.walk);
            anim.SetBool("WakeUp", true);
            }

        } else if(Vector3.Distance(target.position, transform.position) > chaseRadious){
            
            if(Vector3.Distance(transform.position, path[currentPoint].position) > roundingDistance)
            {
            Vector3 temp = Vector3.MoveTowards(transform.position, path[currentPoint].position, enemySpeed * Time.deltaTime);                        
            changeAnim(temp - transform.position);
            myRigidBody2D.MovePosition(temp);
            } else {
                ChangeGoal();
            }
            

        }
    }

    private void ChangeGoal()
    {
        if(currentPoint == path.Length - 1)
        {
            currentPoint = 0;
            currentGoal = path[0];
        } else {
            currentPoint++;
            currentGoal = path[currentPoint];
        }
    } 
} 
