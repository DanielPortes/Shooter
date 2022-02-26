using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    Transform player;
    NavMeshAgent agent;
    Animator animator;
    private bool pursue = false;
    public float sightRange = 20f;
    public float health = 5;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if(distance < sightRange)
            pursue = true;

        if(pursue && distance > 1.5)
        {
            agent.updatePosition = true;
            agent.SetDestination(player.position);
            animator.SetTrigger("Reset");
        }
        else if(distance <= 1.5)
        {
            agent.updatePosition = false;
            animator.SetTrigger("Attack");
        }

        if(health <= 0)
        {
            animator.SetTrigger("Dead");
            Invoke(nameof(Die), 2f);
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void TakeDamage()
    {
        health -= 1;
    }
}
