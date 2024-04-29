using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private PlayerMovement player;
    private float helth = 5;
    [SerializeField] private NavMeshAgent agent;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }


    private void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        }
        else
        {
            agent.SetDestination(player.transform.position);


            if(Vector3.Distance(player.transform.position, transform.position) < 2)
            {
                player.Fall();
            }
        }
    }


    public void Hit(float damage)
    {
        helth -= damage;

        if(helth <= 0)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
