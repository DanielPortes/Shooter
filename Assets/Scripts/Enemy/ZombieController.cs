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
    public float health = 50f;
    private bool dying = false;

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

            if(distance < sightRange && !dying)
                pursue = true;
            else
                pursue = false;

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
                agent.updatePosition = false;
                animator.SetTrigger("Dead");
                dying = true;
                StartCoroutine(Die());
            }
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }
}
