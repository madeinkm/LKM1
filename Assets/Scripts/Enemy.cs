using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum EnemyType
    {
        EnemyA,
        EnemyB,
        EnemyC,
        EnemyBoss
    }

    [SerializeField] private EnemyType enemyType;

    [Header("�� ����")]
    [SerializeField] private float Hp = 5f;
    private float maxHp = 0;
    [SerializeField] private float Speed = 1f;

    [SerializeField] Sprite[] arrSprite;//0 �⺻���� 1 ���ݹ޾�����
    private SpriteRenderer Sr;

    [SerializeField] private GameObject objExplosion;
    private bool haveItem = false;
    private bool isAlive = true;

    [Header("����")]
    private bool isStartingMove = false;//������ �⵿������ �⺻ �̵��� �ߴ���
    private float startPointY;//������ ������ ���� ��ġ
    private float startPointX;//������ ������ ���� ��ġ
    private float ratioY = 0.0f;//������ �̵� �ߴ��� ����
    private bool isSwayRight = false;//���� �̵��ؾ��ϴ���

    [Header("���� ����")]
    //������ �߻�, 1
    [SerializeField] private int pattern1Count = 10;//����� �����
    [SerializeField] private float pattern1Reload = 1f;//���ε�ð�
    [SerializeField] private GameObject pattern1Bullet;
    [SerializeField] private float pattern1Speed = 8f;
    [Space]
    //����, 2
    [SerializeField] private int pattern2Count = 5;//����� �����
    [SerializeField] private float pattern2Reload = 1f;//���ε�ð�
    [SerializeField] private GameObject pattern2Bullet;
    [SerializeField] private float pattern2Speed = 5f;
    [Space]
    //���� ��Ʋ��, 3
    [SerializeField] private int pattern3Count = 30;//����� �����
    [SerializeField] private float pattern3Reload = 0.2f;//���ε�ð�
    [SerializeField] private GameObject pattern3Bullet;
    [SerializeField] private float pattern3Speed = 10f;

    private int curPattern = 1;//���� ����
    private int curPatternShootCount = 0;//���� ���� ī��Ʈ
    private float curPatternTimer = 0.0f;//���� ���� Ÿ�̸�
    [SerializeField] private float patternChangeTime = 0.5f;
    private bool patternChange = false;//������ �ٲ�� �ϴ���

    private Transform trsPlayer;

    private void OnDestroy()//��¥ �����ɶ� �ѹ��� �����ϴ� �Լ�
    {
        GameManager.Instance.RemoveEnemy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        Sr = GetComponent<SpriteRenderer>();
        startPointY = transform.position.y;
        startPointX = transform.position.x;
        maxHp = Hp;
    }

    private void Start()
    {
        trsPlayer = GameManager.Instance.GetPlayerTransform();
        GameManager.Instance.CheckBossHP(Hp, maxHp);
    }

    void Update()
    {
        moving();
        doPattern();
    }

    private void moving()
    {
        if (enemyType != EnemyType.EnemyBoss)//������ �ƴҶ�
        {
            transform.position += Vector3.down * Speed * Time.deltaTime;
        }
        else//�����϶�
        {
            if (isStartingMove == false)
            {
                //��Ÿ�� ���긦 ����
                bossStartMove();
            }
            else
            {
                //�¿�� �Դ� ������ ����
                bossSwayMove();
            }
        }
    }

    private void doPattern()
    {
        if (enemyType != EnemyType.EnemyBoss || isStartingMove == false)
        {
            return;
        }

        //������ ����Ǹ� ��õ��� �÷��̾ ������ �ð��� �������
        curPatternTimer += Time.deltaTime;
        if (patternChange == true)
        {
            if (curPatternTimer >= patternChangeTime)
            {
                curPatternTimer = 0.0f;
                patternChange = false;
            }
            return;
        }

        switch (curPattern)//���� ���� ��
        {
            case 1: //�������� 3���� �߻� �ϴ� ����
                {
                    if (curPatternTimer >= pattern1Reload)
                    {
                        curPatternTimer = 0.0f;
                        //�Ѿ��� �߻��ϴ� �Լ�
                        shootStraight(pattern1Speed);
                        if (pattern1Count <= curPatternShootCount)//������ ��ü
                        {
                            curPattern++;// 1->2, 
                            patternChange = true;
                            curPatternShootCount = 0;
                        }
                    }
                }
                break;
            case 2: //���� ����
                {
                    if (curPatternTimer >= pattern2Reload)
                    {
                        curPatternTimer = 0.0f;
                        //���� �Ѿ��� �߻��ϴ� �Լ�
                        shootShotgun(pattern2Speed);
                        if (pattern2Count <= curPatternShootCount)
                        {
                            curPattern++;// 2->3
                            patternChange = true;
                            curPatternShootCount = 0;
                        }
                    }
                }
                break;
            case 3: //���� ��Ʋ�� ����
                {
                    if (curPatternTimer >= pattern3Reload)
                    {
                        curPatternTimer = 0.0f;
                        //��Ʋ�� �Ѿ��� �߻��ϴ� �Լ�
                        shootGatling(pattern3Speed);
                        if (pattern3Count <= curPatternShootCount)
                        {
                            curPattern = 1;
                            patternChange = true;
                            curPatternShootCount = 0;
                        }
                    }
                }
                break;
        }

    }

    private void shootStraight(float _bSpeed)
    {
        createBullet(pattern1Bullet, transform.position, new Vector3(0, 0, 180f), _bSpeed);
        createBullet(pattern1Bullet, transform.position + new Vector3(-1f, 0, 0), new Vector3(0, 0, 180f), _bSpeed);
        createBullet(pattern1Bullet, transform.position + new Vector3(1f, 0, 0), new Vector3(0, 0, 180f), _bSpeed);
        curPatternShootCount++;
    }

    private void shootShotgun(float _bSpeed)
    {
        createBullet(pattern2Bullet, transform.position, new Vector3(0, 0, 180f), _bSpeed);
        createBullet(pattern2Bullet, transform.position, new Vector3(0, 0, 180f - 15f), _bSpeed);
        createBullet(pattern2Bullet, transform.position, new Vector3(0, 0, 180f + 15f), _bSpeed);
        createBullet(pattern2Bullet, transform.position, new Vector3(0, 0, 180f - 30f), _bSpeed);
        createBullet(pattern2Bullet, transform.position, new Vector3(0, 0, 180f + 30f), _bSpeed);
        createBullet(pattern2Bullet, transform.position, new Vector3(0, 0, 180f - 45f), _bSpeed);
        createBullet(pattern2Bullet, transform.position, new Vector3(0, 0, 180f + 45f), _bSpeed);
        createBullet(pattern2Bullet, transform.position, new Vector3(0, 0, 180f - 60f), _bSpeed);
        createBullet(pattern2Bullet, transform.position, new Vector3(0, 0, 180f + 60f), _bSpeed);
        curPatternShootCount++;
    }

    private void shootGatling(float _bSpeed)
    {
        if (trsPlayer == null)
        {
            return;
        }

        Vector3 playerPos = trsPlayer.position;
        float zAngle = Quaternion.FromToRotation(Vector3.up, playerPos - transform.position).
            eulerAngles.z;
        createBullet(pattern3Bullet, transform.position, new Vector3(0,0, zAngle), _bSpeed);
        curPatternShootCount++;
    }

    private void createBullet(GameObject _obj, Vector3 _pos, Vector3 _rot, float _bSpeed)
    {
        GameObject obj = Instantiate(_obj, _pos, Quaternion.Euler(_rot), transform.root);
        Bullet sc = obj.GetComponent<Bullet>();
        sc.SetBullet(_bSpeed, 1);
    }

    private void bossStartMove()//������ ��ġ�� �̵�
    {
        ratioY += Time.deltaTime * 0.5f;
        if (ratioY >= 1.0f)
        {
            isStartingMove = true;
        }

        Vector3 vecPos = transform.position;
        vecPos.y = Mathf.SmoothStep(startPointY, 3.0f, ratioY);
        vecPos.x = Mathf.SmoothStep(startPointX, 0.0f, ratioY);
        transform.position = vecPos;
    }

    private void bossSwayMove()
    {
        if (isSwayRight == true)//�����̵�
        {
            transform.position += Vector3.right * Time.deltaTime * Speed;
            checkBossMoveLimit();
        }
        else//�����̵�
        {
            transform.position += Vector3.left * Time.deltaTime * Speed;
            checkBossMoveLimit();
        }
    }

    private void checkBossMoveLimit()
    {
        Vector3 currPos = Camera.main.WorldToViewportPoint(transform.position);

        if (isSwayRight == true && currPos.x > 0.95f)
        {
            isSwayRight = false;
        }
        else if (isSwayRight == false && currPos.x < 0.05f)
        {
            isSwayRight = true;
        }
    }

    public void Hit(float _damege)
    {
        if (isAlive == false)
        {
            return;
        }

        Hp -= _damege;
        GameManager.Instance.CheckBossHP(Hp, maxHp);

        if (Hp <= 0)
        {
            if (enemyType == EnemyType.EnemyBoss)
            {
                GameManager.Instance.DestroyBoss();
            }

            isAlive = false;
            ExplosionEnemy();

            GameManager.Instance.AddDestroyCount(enemyType);

            if (haveItem == true)
            {
                GameManager.Instance.CreatItem(transform.position);
            }
        }
        else
        {
            Sr.sprite = arrSprite[1];
            Invoke("defaultSprite", 0.1f);
        }
    }

    public void ExplosionEnemy()
    {
        Destroy(gameObject);

        GameObject obj = Instantiate(objExplosion, transform.position,
            Quaternion.identity, transform.parent);

        Explosion sc = obj.GetComponent<Explosion>();
        sc.SetAnimationSize(Sr.sprite.rect.width);
    }

    private void defaultSprite()
    {
        Sr.sprite = arrSprite[0];
    }

    public void SetHaveItem()
    {
        haveItem = true;
        Sr.color = new Color(0.5f, 0.5f, 1);
    }

    public EnemyType GetEnemyType()
    {
        return enemyType;
    }
}
