using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentScript : MonoBehaviour
{
    public bool isRed;  //this shows us what team this agent is on

    public int hp=100;

    public int speed;   //how fastthis agent moves
    public int viewRange; //the range in which this agent will see an anemy ang begin to fight
    public int dps; //how much damage this agent deals per second

    GameControl GC; //GC has lists of every alive agent
    NavMeshAgent Nav; //we will use AI navigation for easy movement

    public GameObject Target; //which enemy this agent targets now

    LineRenderer shotLine;  //a visual indicator of attack

    bool isAttacking; 

    void Start()
    {
        speed = Random.Range(2, 4);
        viewRange = Random.Range(100, 50);
        dps = Random.Range(5, 10);
        //stats will be randomised each time to make it more fun

        isAttacking = false;
        GC = GameObject.Find("Floor").GetComponent<GameControl>();
        Nav = GetComponent<NavMeshAgent>();
        shotLine = GetComponent<LineRenderer>();
        Nav.speed = speed; //we take AI nav component speed from stats
        FindNewTarget();
    }

    void Update()
    {
        if (hp > 0)
        {
            if (Target != null)
            { //if we have a target - we move to it
                Nav.SetDestination(Target.transform.position);
                if((Target.transform.position - transform.position).sqrMagnitude < viewRange && !isAttacking)
                { //if the target in the attack range and we are not already attacking
                    isAttacking = true; //we begin the attack
                    StartCoroutine(AttackTarget());
                }
            }
            else  //and we find one if we don't have a target
                FindNewTarget();
        }
    }

    void FindNewTarget()
    {
        List<GameObject> EnemiesList = isRed ? GC.blueTeam : GC.redTeam; //we take alive anemy team agents list from GC
        GameObject ChosenEnemy = null;
        float distance = 10000f; //just a really big number for our arena size
        foreach (GameObject Enemy in EnemiesList)
        {   //we check distance to every enemy on the list
            float newDistance = (Enemy.transform.position - transform.position).sqrMagnitude;
            if (newDistance < distance)
            {
                distance = newDistance;
                ChosenEnemy = Enemy;
            }
        }   //and the closest become our target
        Target = ChosenEnemy;
    }

    IEnumerator AttackTarget()
       {
        bool targetDead = false;
        while (Target != null && (Target.transform.position - transform.position).sqrMagnitude < viewRange&&
            !targetDead&&hp>0)
            { //attacks if agent has a target, is in range, target and agent are both not dead 
             Target.GetComponent<AgentScript>().GetShot(dps,out targetDead); //we send the damage to our target

            //This is all to show a line of attack
            shotLine.enabled = true;
            shotLine.SetPosition(0, transform.position);
            shotLine.SetPosition(1, Target.transform.position);
            shotLine.startWidth = 0.1f;
            shotLine.endWidth = 0.1f;
            yield return new WaitForSeconds(0.1f);
            shotLine.startWidth = 0;
            shotLine.endWidth = 0;

            yield return new WaitForSeconds(0.9f); //and this is to make an attack per second
            }
        //if agent cant attack
        shotLine.enabled = false; //we make line for a shot invisible 
        isAttacking = false; //we stop attack mode for update loop to work again
        if (targetDead)
            FindNewTarget(); //find new target immediately after kill so not to wait until target destroyes
    }

    public void GetShot (int damage, out bool targetDead)
    { //this function happens when this agent gets shot by an enemy
        targetDead = false; // specify bool so out works
        hp -= damage;
        if (hp < 1)
        {
            targetDead = true; //we send out tho the shooter that this target is dead so it doesnt waste time shooting dying body
            StartCoroutine(Death());
        }        
    } 

    IEnumerator Death()
    {   //we remove this agent from the list of alive agents
        if (isRed)
            GC.redTeam.Remove(gameObject);
        else
            GC.blueTeam.Remove(gameObject);

        GC.checkTheEnd(); //and check if this one was the last one alive

        //disable components so they won't let us go through the floor
        gameObject.GetComponent<SphereCollider>().enabled = false;
        Nav.enabled = false;

        for(int i = 0; i < 100; i++)
        { // going down
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.01f, transform.position.z);
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
