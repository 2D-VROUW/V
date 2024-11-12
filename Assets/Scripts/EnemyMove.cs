using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private SpriteRenderer render;

    [SerializeField] private float speed = 2.5f;
    [SerializeField] private float followDistance = 5f;    // 추격 시작 거리
    [SerializeField] private float stopChaseRange = 2f;    // 추적 멈출 거리 (공격 준비 거리)

    private Transform player;
    private bool isPlayerOnSamePlatform;
    private bool isChasing;
    private int nextMove;

    NER EA;
    void Start()
    {
        EA = GetComponentInChildren<NER>();
    }
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").transform;
        Think(); // 초기 이동 방향 설정
    }

    void FixedUpdate()
    {
        CheckPlatform(); // 플레이어와 같은 플랫폼에 있는지 확인

        if (isPlayerOnSamePlatform && Vector2.Distance(transform.position, player.position) <= followDistance)
        {
            // 플레이어가 추적 범위 내에 있으면
            if (Vector2.Distance(transform.position, player.position) > stopChaseRange)
            {
                // 공격 거리 이상일 때 추격
                ChasePlayer();
            }
            else
            {
                // 공격 범위 내에 도달하면 정지
                StopAndPrepareAttack();
            }
        }
        else if (isChasing && Vector2.Distance(transform.position, player.position) > followDistance)
        {
            // 추적 중지
            StopChasing();
        }
        else if (!isChasing)
        {
            // 정찰 상태
            Patrol();
        }
    }

    private void Patrol()
    {
        rigid.velocity = new Vector2(nextMove * speed, rigid.velocity.y);
        Vector2 frontVector = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y);
        Debug.DrawRay(frontVector, Vector3.down, Color.green);

        RaycastHit2D rayHit = Physics2D.Raycast(frontVector, Vector3.down, 2f, LayerMask.GetMask("Ground"));

        if (rayHit.collider == null)
        {
            Turn();
        }
    }

    private void ChasePlayer()
    {
        isChasing = true;
        Vector2 direction = (player.position - transform.position).normalized;
        rigid.velocity = new Vector2(direction.x * speed, rigid.velocity.y);
        render.flipX = direction.x < 0;
    }

    private void StopChasing()
    {
        isChasing = false;
        Think(); // 정찰 상태로 전환
    }

    private void StopAndPrepareAttack()
    {
        rigid.velocity = Vector2.zero;
        EA.Start();
    }

    private void Think()
    {
        nextMove = Random.Range(-1, 2);
        render.flipX = nextMove == -1;
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime); // 일정 시간 후 방향 변경
    }

    private void Turn()
    {
        nextMove *= -1;
        render.flipX = nextMove == -1;
    }

    private void CheckPlatform()
    {
        isPlayerOnSamePlatform = Mathf.Abs(player.position.y - transform.position.y) < 0.5f;
    }
}