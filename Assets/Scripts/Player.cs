using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("����")]
    [SerializeField] private float m_Speed;
    [SerializeField] private int m_maxHp = 5;
    private int m_curHp;

    private Camera mainCam;
    private Animator anim;

    [SerializeField] private bool IsUserShoot;//true�� �Ǹ� ������ ������ �߻�, false �ڵ����� �߻� �Ǵ� �ý���

    [SerializeField] private GameObject objBullet;
    [SerializeField] private Transform trsShootPoint;
    [SerializeField] private Transform trsObjectDynamic;

    [Header("�ҷ�")]
    [SerializeField][Tooltip("�Ѿ��� ���ݷ��Դϴ�.")] private float bulletDamege;
    [SerializeField, Tooltip("�Ѿ��� �̵��ӵ��Դϴ�.")] private float bulletSpeed;
    private float timer = 0.0f;//�ð�
    [SerializeField] private float m_shootTimer = 0.5f;

    [Header("HP����")]
    [SerializeField] private PlayerHP playerHP;
    [SerializeField] private GameObject objExplosion;
    private SpriteRenderer Sr;

    [Header("���� ����")]
    [SerializeField, Range(1, 5)] private int playerAttackLevel = 1;
    private int playerAttackMaxLevel = 5;

    [SerializeField] private List<GameObject> playerlist;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy sc = collision.gameObject.GetComponent<Enemy>();
            if (sc.GetEnemyType() != Enemy.EnemyType.EnemyBoss)
            { 
                sc.ExplosionEnemy();
            }

            Hit();
        }
        else if (collision.gameObject.tag == "Item")
        {
            Item sc = collision.gameObject.GetComponent<Item>();
            Item.ItemType type = sc.GetItemType();

            switch (type)
            {
                case Item.ItemType.PowerUp:
                    if(playerAttackLevel + 1 <= playerAttackMaxLevel)
                    {
                        playerAttackLevel++;
                    }
                    break;
                case Item.ItemType.HpRecovery:
                    if (m_curHp + 1 <= m_maxHp)
                    {
                        m_curHp++;
                    }
                    playerHP.SetPlayerHp(m_curHp, m_maxHp);
                    break;
            }

            Destroy(collision.gameObject);
        }
    }

    private void Start()
    {
        mainCam = Camera.main;//�̱���, �޸𸮿� �����ϰ� ���
        anim = GetComponent<Animator>();

        m_curHp = m_maxHp;
        playerHP.SetPlayerHp(m_curHp, m_maxHp);

        Sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        moving();
        checkMovePosition();
        doAnimation();
        shoot();
    }

    private void moving()
    {
        float vertical = Input.GetAxisRaw("Vertical") * Time.deltaTime;
        float horizontal = Input.GetAxisRaw("Horizontal") * Time.deltaTime;
        transform.position += new Vector3(horizontal, vertical) * m_Speed;
    }

    private void checkMovePosition()
    {
        //������������ ī�޶��� �� ��Ʈ ����Ʈ�� ����Ǿ� ����
        Vector3 currentPos = mainCam.WorldToViewportPoint(transform.position);

        if (currentPos.x < 0.05f)//�������� �ʹ� ����
        {
            currentPos.x = 0.05f;//0���� ����
        }
        else if (currentPos.x > 0.95f)//���������� �ʹ� ����
        {
            currentPos.x = 0.95f;//1�� ���� 
        }

        if (currentPos.y < 0.05f)
        {
            currentPos.y = 0.05f;
        }
        else if (currentPos.y > 0.95f)
        {
            currentPos.y = 0.95f;
        }

        //����Ʈ �����͸� ���� ���������� �����Ͽ� ����
        transform.position = mainCam.ViewportToWorldPoint(currentPos);
    }

    private void doAnimation()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        anim.SetInteger("horizontal", (int)horizontal);
    }

    private void shoot()
    {
        if (IsUserShoot == true && Input.GetKeyDown(KeyCode.Space))
        {
            shootBullet();
        }
        else if (IsUserShoot == false)
        {
            timer += Time.deltaTime;
            if (timer >= m_shootTimer)
            {
                timer = 0.0f;
                shootBullet();
            }
        }
    }
    private void shootBullet()
    {
        switch (playerAttackLevel)
        {
            case 1:
                createBullet(trsShootPoint.position, Vector3.zero);//1�� �Ѿ�
                break;
            case 2:
                createBullet(trsShootPoint.position + new Vector3(-0.2f, 0f, 0f)//1�� �Ѿ�1
                    , Vector3.zero);
                createBullet(trsShootPoint.position + new Vector3(0.2f, 0f, 0f)//1�� �Ѿ�2
                    , Vector3.zero);
                break;
            case 3:
                createBullet(trsShootPoint.position, Vector3.zero);
                createBullet(trsShootPoint.position + new Vector3(-0.2f, 0f, 0f)//1�� �Ѿ�1
                    , new Vector3(0, 0, 15f));
                createBullet(trsShootPoint.position + new Vector3(0.2f, 0f, 0f)//1�� �Ѿ�2
                    , new Vector3(0, 0, -15f));
                break;
            case 4:
                createBullet(trsShootPoint.position + new Vector3(-0.2f, 0f, 0f)//1�� �Ѿ�1
                    , Vector3.zero);
                createBullet(trsShootPoint.position + new Vector3(0.2f, 0f, 0f)//1�� �Ѿ�2
                    , Vector3.zero);
                createBullet(trsShootPoint.position + new Vector3(-0.2f, 0f, 0f)//1�� �Ѿ�1
                    , new Vector3(0, 0, 15f));
                createBullet(trsShootPoint.position + new Vector3(0.2f, 0f, 0f)//1�� �Ѿ�2
                    , new Vector3(0, 0, -15f));
                break;
            case 5:
                createBullet(trsShootPoint.position, Vector3.zero);
                createBullet(trsShootPoint.position + new Vector3(-0.2f, 0f, 0f)//1�� �Ѿ�1
                    , new Vector3(0, 0, 15f));
                createBullet(trsShootPoint.position + new Vector3(0.2f, 0f, 0f)//1�� �Ѿ�2
                    , new Vector3(0, 0, -15f));
                createBullet(trsShootPoint.position + new Vector3(-0.2f, 0f, 0f)//1�� �Ѿ�1
                    , new Vector3(0, 0, 30f));
                createBullet(trsShootPoint.position + new Vector3(0.2f, 0f, 0f)//1�� �Ѿ�2
                    , new Vector3(0, 0, -30f));
                break;
        }

    }

    private void createBullet(Vector3 _pos, Vector3 _rot)
    {
        GameObject obj = Instantiate(objBullet, _pos, Quaternion.Euler(_rot), trsObjectDynamic);
        Bullet bullet = obj.GetComponent<Bullet>();
        bullet.SetBullet(bulletSpeed, bulletDamege, true);
    }

    public void Hit()
    {
        m_curHp--;
        playerHP.SetPlayerHp(m_curHp, m_maxHp);

        if (playerAttackLevel - 1 > 0)
        {
            playerAttackLevel--;
        }

        if (m_curHp <= 0)
        {
            GameManager.Instance.GameOver();

            Destroy(gameObject);

            GameObject obj = Instantiate(objExplosion, transform.position,
                Quaternion.identity, transform.parent);

            Explosion Esc = obj.GetComponent<Explosion>();
            Esc.SetAnimationSize(Sr.sprite.rect.width);
        }
    }
}
