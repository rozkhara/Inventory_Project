using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenRobotCleaner : MonsterClass
{
    float playerSpeed = 5f;
    float AttackSpeed = 2.5f;
    bool contacted_wall = true;
    int ChooseDir = 0;

    public bool Contact_rWall = false;
    public bool Contact_lWall = false;
    public bool Contact_uWall = false;
    public bool Contact_dWall = false;

    public bool Attack = false;
    bool AttackDelay = false;
    bool AfterAttackDelay = false;

    GameObject player;
    Rigidbody2D rigid;
    public Sprite[] sprites = new Sprite[3];
    SpriteRenderer sr;

    protected override void Start()
    {
        base.Start();
        MaxHealth = Health = 20f;
        AttackDamage = 30f;
        MovementSpeed = 0.5f;
        Size = 1f;
        player = GameObject.FindObjectOfType<Player>().gameObject;
        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }
    public override void GetDamaged(float damage)
    {
        base.GetDamaged(damage);
    }
    protected override Queue<IEnumerator> DecideNextRoutine()
    {
        Queue<IEnumerator> nextRoutines = new Queue<IEnumerator>();

        if (FindObjectOfType<Player>() != null) // 플레이어가 살아있는지 확인해야함 !!
        {
            if (contacted_wall)
            {
                Attack = false;
                if (AfterAttackDelay)
                {
                    nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1f)));
                    AfterAttackDelay = false;
                }
                nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(0.5f)));
                for (int i = 0; i <= 50f; i++)
                    nextRoutines.Enqueue(NewActionRoutine(Distancing(ChooseDir)));
                nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(0.5f)));
                do
                {
                    ChooseDir = Random.Range(0, 4);
                } while ((Contact_uWall && ChooseDir == 0) || (Contact_dWall && ChooseDir == 1) || (Contact_rWall && ChooseDir == 2) || (Contact_lWall && ChooseDir == 3));
                contacted_wall = false;
            }
            else if (!contacted_wall)
            {
                if (!Attack)
                    nextRoutines.Enqueue(NewActionRoutine(MoveTowardOneDir(MovementSpeed, ChooseDir)));
                else
                {
                    if (AttackDelay)
                    {
                        nextRoutines.Enqueue(NewActionRoutine(WaitRoutine(1f)));
                        AttackDelay = false;
                    }
                    nextRoutines.Enqueue(NewActionRoutine(MoveTowardOneDir(AttackSpeed, ChooseDir)));
                }
            }
        }
        else nextRoutines.Enqueue(NewActionRoutine(NewActionRoutine(WaitRoutine(1f))));

        return nextRoutines;
    }
    private IEnumerator MoveTowardOneDir(float Speed, int dir)
    {
        switch (dir)
        {
            case 0:
                sr.sprite = sprites[0];
                sr.flipX = false;
                rigid.MovePosition(rigid.position + Vector2.up * Time.fixedDeltaTime * Speed * playerSpeed);
                break;
            case 1:
                sr.sprite = sprites[1];
                sr.flipX = false;
                rigid.MovePosition(rigid.position + Vector2.down * Time.fixedDeltaTime * Speed * playerSpeed);
                break;
            case 2:
                sr.sprite = sprites[2];
                sr.flipX = true;
                rigid.MovePosition(rigid.position + Vector2.right * Time.fixedDeltaTime * Speed * playerSpeed);
                break;
            case 3:
                sr.sprite = sprites[2];
                sr.flipX = false;
                rigid.MovePosition(rigid.position + Vector2.left * Time.fixedDeltaTime * Speed * playerSpeed);
                break;
        }

        if (!Attack)
            CheckPlayer();
        CheckWall();
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
            contacted_wall = true;
        if ((collision.gameObject == player) && Attack)
        {
            player.GetComponent<Player>().GetDamaged(AttackDamage);
        }
    }
    private void CheckWall()
    {
        RaycastHit2D hit_r = Physics2D.Raycast(transform.position, Vector2.right, 1f, LayerMask.GetMask("Default"));
        RaycastHit2D hit_l = Physics2D.Raycast(transform.position, Vector2.left, 1f, LayerMask.GetMask("Default"));
        RaycastHit2D hit_u = Physics2D.Raycast(transform.position, Vector2.up, 1.5f, LayerMask.GetMask("Default"));
        RaycastHit2D hit_d = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Default"));

        Debug.DrawRay(transform.position, 1 * Vector2.right, Color.black);
        Debug.DrawRay(transform.position, 1 * Vector2.left, Color.black);
        Debug.DrawRay(transform.position, 1 * Vector2.up, Color.black);
        Debug.DrawRay(transform.position, 1 * Vector2.down, Color.black);

        if (hit_r.collider != null)
        {
            if (hit_r.transform.tag == "Wall")
            {
                Contact_rWall = true;
            }
        }
        else Contact_rWall = false;

        if (hit_l.collider != null)
        {
            if (hit_l.transform.tag == "Wall")
            {
                Contact_lWall = true;
            }
        }
        else Contact_lWall = false;

        if (hit_u.collider != null)
        {
            if (hit_u.transform.tag == "Wall")
            {
                Contact_uWall = true;
            }
        }
        else Contact_uWall = false;

        if (hit_d.collider != null)
        {
            if (hit_d.transform.tag == "Wall")
            {
                Contact_dWall = true;
            }
        }
        else Contact_dWall = false;
    }
    private void CheckPlayer()
    {
        RaycastHit2D hit_r = Physics2D.Raycast(transform.position, Vector2.right, 1000f, LayerMask.GetMask("Default"));
        RaycastHit2D hit_l = Physics2D.Raycast(transform.position, Vector2.left, 1000f, LayerMask.GetMask("Default"));
        RaycastHit2D hit_u = Physics2D.Raycast(transform.position, Vector2.up, 1000f, LayerMask.GetMask("Default"));
        RaycastHit2D hit_d = Physics2D.Raycast(transform.position, Vector2.down, 1000f, LayerMask.GetMask("Default"));
        if (hit_r.collider != null)
        {
            if (hit_r.transform.name == "Player")
            {
                AttackDelay = true;
                AfterAttackDelay = true;
                Attack = true;
                ChooseDir = 2;
            }
        }
        if (hit_l.collider != null)
        {
            if (hit_l.transform.name == "Player")
            {
                AttackDelay = true;
                AfterAttackDelay = true;
                Attack = true;
                ChooseDir = 3;
            }
        }
        if (hit_u.collider != null)
        {
            if (hit_u.transform.name == "Player")
            {
                AttackDelay = true;
                AfterAttackDelay = true;
                Attack = true;
                ChooseDir = 0;
            }
        }
        if (hit_d.collider != null)
        {
            if (hit_d.transform.name == "Player")
            {
                AttackDelay = true;
                AfterAttackDelay = true;
                Attack = true;
                ChooseDir = 1;
            }
        }
    }
    private IEnumerator Distancing(int dir)
    {
        if (dir == 0)
        {
            rigid.MovePosition(rigid.position + Vector2.down * Time.fixedDeltaTime);
        }
        else if (dir == 1)
        {
            rigid.MovePosition(rigid.position + Vector2.up * Time.fixedDeltaTime);
        }
        else if (dir == 2)
        {
            rigid.MovePosition(rigid.position + Vector2.left * Time.fixedDeltaTime);
        }
        else if (dir == 3)
        {
            rigid.MovePosition(rigid.position + Vector2.right * Time.fixedDeltaTime);
        }

        yield return null;
    }
}
