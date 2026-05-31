using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class log : Enemmy //Herencia del Script Enemy
{
    public Rigidbody2D myRigidBody2D;

    [Header("Target Variables")]
    public Transform target;
    public float chaseRadious;
    public float attackRadious;
    public StateMachine playerState;


    [Header("animator")]
    public Animator anim;


    void Start()
    {
        currentState = EnemyState.idle;
        myRigidBody2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        if (target == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
                playerState = playerObj.GetComponentInChildren<StateMachine>();
            }
        }
        
        anim.SetBool("WakeUp", true);
    }

    
    void FixedUpdate()
    {
        CheckDistance();
    }
    public virtual void CheckDistance(){
        if(Vector3.Distance(target.position, transform.position) <= chaseRadious
        && Vector3.Distance(target.position, transform.position) > attackRadious){

            if(currentState == EnemyState.idle || currentState == EnemyState.walk && currentState != EnemyState.stagger){
            Vector3 temp = Vector3.MoveTowards(transform.position, target.position, enemySpeed * Time.deltaTime);
            changeAnim(temp - transform.position);
            myRigidBody2D.MovePosition(temp);
            ChangeState(EnemyState.walk);
            anim.SetBool("WakeUp", true);
            }

        } else if(Vector3.Distance(target.position, transform.position) > chaseRadious){
            anim.SetBool("WakeUp", false);
        }
    }

    public void changeAnim(Vector2 direction){
        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y)){
            if(direction.x > 0){
            SetAnimFloat(Vector2.right);
            } else if (direction.x < 0){
            SetAnimFloat(Vector2.left);
            }
        } else if (Mathf.Abs(direction.x) < Mathf.Abs(direction.y)){
            if(direction.y > 0){
            SetAnimFloat(Vector2.up);
            } else if (direction.y < 0){
            SetAnimFloat(Vector2.down);
            }
        }
    }

    public void SetAnimFloat(Vector2 setVector){
        anim.SetFloat("LogmoveX", setVector.x);
        anim.SetFloat("LogmoveY", setVector.y);

    }

    public void ChangeState(EnemyState newState){
        if (currentState != newState){
            currentState = newState;
        }
    }

}
