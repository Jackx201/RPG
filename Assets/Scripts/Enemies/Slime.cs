using UnityEngine;
using System.Collections;

public class Slime : log
{
    [Header("Dash Settings")]
    public float dashForce = 4f;
    public float dashDuration = 0.3f;
    public float chargeTime = 0.5f;
    public float cooldownTime = 1f;

    private DamageOnContact[] damageOnContacts;
    private bool dashInterrupted;

    private void OnEnable()
    {
        damageOnContacts = GetComponentsInChildren<DamageOnContact>();

        foreach (DamageOnContact damageOnContact in damageOnContacts)
        {
            damageOnContact.AddPlayerDamagedListener(StopDash);
        }
    }

    private void OnDisable()
    {
        if (damageOnContacts == null)
        {
            return;
        }

        foreach (DamageOnContact damageOnContact in damageOnContacts)
        {
            damageOnContact.RemovePlayerDamagedListener(StopDash);
        }
    }

    public override void CheckDistance()
    {
        float distanceToTarget = Vector3.Distance(target.position, transform.position);

        if (distanceToTarget <= chaseRadious && distanceToTarget > attackRadious)
        {
            if (currentState == EnemyState.idle || currentState == EnemyState.walk && currentState != EnemyState.stagger)
            {
                // Moving towards the player
                Vector3 temp = Vector3.MoveTowards(transform.position, target.position, enemySpeed * Time.deltaTime);
                changeAnim(temp - transform.position);
                myRigidBody2D.MovePosition(temp);
                ChangeState(EnemyState.walk);
                anim.SetBool("WakeUp", true);
            }
        }
        else if (distanceToTarget <= chaseRadious && distanceToTarget <= attackRadious)
        {
            if ((currentState == EnemyState.walk || currentState == EnemyState.idle) && currentState != EnemyState.stagger)
            {
                StartCoroutine(DashAttackCo());
            }
        }
        else if (distanceToTarget > chaseRadious)
        {
            anim.SetBool("WakeUp", false);
            ChangeState(EnemyState.idle);
        }
    }

    public IEnumerator DashAttackCo()
    {
        dashInterrupted = false;
        currentState = EnemyState.attack;
        myRigidBody2D.linearVelocity = Vector2.zero; // Stop moving
        
        // 1. Charge a dash
        anim.SetBool("Attacking", true);
        yield return new WaitForSeconds(chargeTime);

        if (!dashInterrupted)
        {
            // 2. Dash towards the player
            Vector3 dashDirection = (target.position - transform.position).normalized;
            float dashSpeed = dashForce / dashDuration;

            myRigidBody2D.linearVelocity = dashDirection * dashSpeed;
        }

        // Wait for dash to finish
        yield return new WaitForSeconds(dashDuration);

        // 3. Cooldown
        myRigidBody2D.linearVelocity = Vector2.zero;
        anim.SetBool("Attacking", false);
        yield return new WaitForSeconds(cooldownTime);

        // 4. Reset state
        currentState = EnemyState.idle;
    }

    private void StopDash()
    {
        if (currentState != EnemyState.attack)
        {
            return;
        }

        dashInterrupted = true;
        myRigidBody2D.linearVelocity = Vector2.zero;
        anim.SetBool("Attacking", false);
    }
}
