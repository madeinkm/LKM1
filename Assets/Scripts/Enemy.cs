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

    [Header("적 스텟")]
    [SerializeField] private float Hp = 5f;
    private float maxHp = 0;
    [SerializeField] private float Speed = 1f;

    [SerializeField] Sprite[] arrSprite;//0 기본상태 1 공격받았을때
    private SpriteRenderer Sr;

    [SerializeField] private GameObject objExplosion;
    private bool haveItem = false;
    private bool isAlive = true;

    [Header("보스")]
    private bool isStartingMove = false;//보스가 출동했을때 기본 이동을 했는지
    private float startPointY;//보스가 생성된 최초 위치
    private float startPointX;//보스가 생성된 최초 위치
    private float ratioY = 0.0f;//어디까지 이동 했는지 비율
    private bool isSwayRight = false;//어디로 이동해야하는지

    [Header("보스 패턴")]
    //앞으로 발사, 1
    [SerializeField] private int pattern1Count = 10;//몇발을 쏘는지
    [SerializeField] private float pattern1Reload = 1f;//리로드시간
    [SerializeField] private GameObject pattern1Bullet;
    [SerializeField] private float pattern1Speed = 8f;
    [Space]
    //샷건, 2
    [SerializeField] private int pattern2Count = 5;//몇발을 쏘는지
    [SerializeField] private float pattern2Reload = 1f;//리로드시간
    [SerializeField] private GameObject pattern2Bullet;
    [SerializeField] private float pattern2Speed = 5f;
    [Space]
    //조준 개틀링, 3
    [SerializeField] private int pattern3Count = 30;//몇발을 쏘는지
    [SerializeField] private float pattern3Reload = 0.2f;//리로드시간
    [SerializeField] private GameObject pattern3Bullet;
    [SerializeField] private float pattern3Speed = 10f;

    private int curPattern = 1;//현재 패턴
    private int curPatternShootCount = 0;//현재 패턴 카운트
    private float curPatternTimer = 0.0f;//현재 패턴 타이머
    [SerializeField] private float patternChangeTime = 0.5f;
    private bool patternChange = false;//패턴을 바꿔야 하는지

    private Transform trsPlayer;

    private void OnDestroy()//진짜 삭제될때 한번만 동작하는 함수
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
        if (enemyType != EnemyType.EnemyBoss)//보스가 아닐때
        {
            transform.position += Vector3.down * Speed * Time.deltaTime;
        }
        else//보스일때
        {
            if (isStartingMove == false)
            {
                //스타팅 무브를 해줌
                bossStartMove();
            }
            else
            {
                //좌우로 왔다 스웨이 무브
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

        //패턴이 변경되며 잠시동안 플레이어가 공격할 시간을 만들어줌
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

        switch (curPattern)//현재 패턴 값
        {
            case 1: //전방으로 3발을 발사 하는 패턴
                {
                    if (curPatternTimer >= pattern1Reload)
                    {
                        curPatternTimer = 0.0f;
                        //총알을 발사하는 함수
                        shootStraight(pattern1Speed);
                        if (pattern1Count <= curPatternShootCount)//패턴을 교체
                        {
                            curPattern++;// 1->2, 
                            patternChange = true;
                            curPatternShootCount = 0;
                        }
                    }
                }
                break;
            case 2: //샷건 패턴
                {
                    if (curPatternTimer >= pattern2Reload)
                    {
                        curPatternTimer = 0.0f;
                        //샷건 총알을 발사하는 함수
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
            case 3: //조준 개틀링 패턴
                {
                    if (curPatternTimer >= pattern3Reload)
                    {
                        curPatternTimer = 0.0f;
                        //개틀링 총알을 발사하는 함수
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

    private void bossStartMove()//정해진 위치로 이동
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
        if (isSwayRight == true)//우측이동
        {
            transform.position += Vector3.right * Time.deltaTime * Speed;
            checkBossMoveLimit();
        }
        else//좌측이동
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
