using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fairy : EnemyBase
{
    [SerializeField]
    float playerDistance = 5;
    [SerializeField]
    float keepAwayDistance = 3;
    [SerializeField]
    float chaseDistance = 10;
    bool chasing;

    Transform player;

    List<Vector2> path;

    [SerializeField]
    float speed;

    int pathIndex = 0;

    Rigidbody2D thisRB;
    Vector2 newVelocity;

    [SerializeField]
    float attackDistance = 4;

    BulletPatternScript thisPattern;
    bool attacking = false;

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        pathIndex = 0;
        thisRB = GetComponent<Rigidbody2D>();
        thisPattern = GetComponent<BulletPatternScript>();
        thisPattern.SpawnBullets();
    }


    Vector2 diff;
    Vector2Int pos;
    Vector2Int playerPos;

    Vector2 targetDiff;
    Vector3 target;

    protected override void Update()
    {
        diff = transform.position - player.position;
        if (path != null)
        {

            targetDiff.x =  path[pathIndex-1].x - transform.position.x;
            targetDiff.y = path[pathIndex-1].y - transform.position.y;
        }
        else
        {
            targetDiff = Vector2.zero;
        }
        if ((diff.magnitude > keepAwayDistance && diff.magnitude < playerDistance) )
        {
            Debug.Log("chase");
            chasing = true;

            if(path != null)
            {
                Debug.Log("has Path");
                if(targetDiff.magnitude <= 0.5f || path.Count - 1 <= 0)
                {
                    Debug.Log("Create New Path");
                    pos.x = (int)transform.position.x;
                    pos.y = (int)transform.position.y;
                    playerPos.x = (int)player.position.x;
                    playerPos.y = (int)player.position.y;
                    path = AIPathMaker.Instance.GetPath(pos, playerPos);
                    if(path != null)
                    {
                        pathIndex = path.Count - 1;

                        Debug.Log("NEw PAth created");
                        target.x = path[pathIndex-1].x;
                        target.y = path[pathIndex-1].y;
                    }
                }
            }
            else
            {
                pos.x = (int)transform.position.x;
                pos.y = (int)transform.position.y;
                playerPos.x = (int)player.position.x;
                playerPos.y = (int)player.position.y;

                path = AIPathMaker.Instance.GetPath(pos, playerPos);
                if (path != null)
                {
                    pathIndex = path.Count - 1;
                    Debug.Log("NEw PAth");
                    target.x = path[pathIndex-1].x;
                    target.y = path[pathIndex-1].y;
                }
            }
        }
        else
        {
            chasing = false;
            path = null;
            target = transform.position;
        }

        if(chasing && !attacking)
        {
            newVelocity = targetDiff.normalized * speed; 
            thisRB.velocity = Vector3.Lerp(thisRB.velocity, newVelocity, Time.deltaTime);
        }
        else
        {
            thisRB.velocity = Vector3.Lerp(thisRB.velocity, Vector2.zero, Time.deltaTime);
        }

        if(diff.magnitude < attackDistance&& ! attacking)
        {
            thisPattern.StartPattern();
            attacking = true;
        }

        if(attacking)
        {
            if(thisPattern.CheckPatternComplete())
            {
                thisPattern.DisablePattern();

                attacking = false;
            }
        }

        base.Update();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "PlayerAttack")
        {
            TakeDamage(10);
        }
    }

    Color c = new Color(0, 0, 0, 1);

    private void OnDrawGizmos()
    {
        if(path == null)
        {
            return;
        }
        c.r = 0.0f;
        for (int i = 0; i < path.Count-1; i++)
        {
            c.r += 0.2f;
            Gizmos.color = c;
            Gizmos.DrawLine(new Vector3(path[i].x, path[i].y), new Vector3(path[i + 1].x, path[i + 1].y));
        }

    }
}
