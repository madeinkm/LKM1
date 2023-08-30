using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RankData
{
    public int Score;
    public string Name;
}

public enum SceneIndex
{
    mainScene,
    playScene,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;//�̱���

    [SerializeField] private List<GameObject> listEnemy;
    [SerializeField] private GameObject enemyBoss;

    private float spawnTimer = 0.0f;//Ÿ�̸�
    [SerializeField] private float spawnTime;//���Ͱ� ����Ǵ� �ð�

    private Transform spawnPoint;//���� ������ġ
    [SerializeField] private Transform trsDynamicObject;//�������� �����̴� ������Ʈ�� �����Ǵ� ����

    [Header("������")]
    [SerializeField, Range(0.0f, 100.0f)] private float itemDropRate = 30.0f;
    [SerializeField] private List<GameObject> listItem;

    [Header("���� �⵿ ������")]
    [SerializeField] private Slider sliderTimer;
    [SerializeField] private TMP_Text textTimer;
    private float timer = 0.0f;//Ÿ�̸�
    [SerializeField] private float appearBossTime = 30.0f;//���� �⵿ �ð�

    [SerializeField] private Slider sliderDestroy;
    [SerializeField] private TMP_Text textDestroy;
    private int DestCount = 0;
    private int appearBossDestroy = 10;

    private bool appearBoss = false;//������ �⵿ �ߴ���

    [Header("����")]
    [SerializeField] private TMP_Text textScore;
    private int score = 0;

    private List<GameObject> listAppearEnemy = new List<GameObject>();

    private Transform trsPlayer;

    [Header("���ӿ��� ��ǲ�ʵ�")]
    [SerializeField] private TMP_Text textGameoverRank;
    [SerializeField] private TMP_InputField ipfGameoverName;
    [SerializeField] private Button btnGameover;

    private List<RankData> listRank = new List<RankData>();
    private string keyRank = "keyRank";
    private int rankCount = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
        trsPlayer = FindObjectOfType<Player>().transform;
        ipfGameoverName.gameObject.SetActive(false);
        getRankDatas();
    }

    private void getRankDatas()//����Ƽ�� ����� ��ũ�����͸� �ҷ����ݴϴ�. listRank��
    {
        string value = PlayerPrefs.GetString(keyRank, string.Empty);//""
        if (value == string.Empty)//"", "[]"
        {
            for (int iNum = 0; iNum < rankCount; ++iNum)
            {
                
                RankData data = new RankData();
                data.Name = string.Empty;
                data.Score = 0;
                listRank.Add(data);
            }
            
            value = JsonConvert.SerializeObject(listRank);
            PlayerPrefs.SetString(keyRank, value);
        }
        else
        {
            listRank = JsonConvert.DeserializeObject<List<RankData>>(value);
            if (listRank.Count != rankCount)
            {
                for (int iNum = 0; iNum < rankCount; ++iNum)
                {
                    RankData data = new RankData();
                    data.Name = string.Empty;
                    data.Score = 0;
                    listRank.Add(data);
                }

                value = JsonConvert.SerializeObject(listRank);
                PlayerPrefs.SetString(keyRank, value);
                listRank = JsonConvert.DeserializeObject<List<RankData>>(value);
            }
        }
    }

    void Start()
    {
        //spawnPoint = transform.Find("SpawnPoint");
        spawnPoint = transform.GetChild(0);
    }

    void Update()
    {
        checkSpawn();
        checkTimer();
        //checkDestroyCount();
    }

    private void checkSpawn()
    {
        if (appearBoss == true)
        {
            return;
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnTime)
        {
            spawnTimer = 0.0f;
            spawnEnemy();
        }
    }

    private void checkTimer()
    {
        if (appearBoss == false)
        { 
            if (timer < appearBossTime)
            {
                timer += Time.deltaTime;

                if (timer > appearBossTime)
                {
                    timer = appearBossTime;
                }

                sliderTimer.value = timer / appearBossTime;
                textTimer.text = $"{(int)timer} / {(int)appearBossTime}";
            }

            if (appearBoss == false && timer >= appearBossTime)
            {
                appearBoss = true;
                destroyAllEnemy();//������� ��� ����
                //������ �⵿
                Instantiate(enemyBoss, spawnPoint.position, 
                    Quaternion.identity, trsDynamicObject);
            }
        }
    }

    public void CheckBossHP(float _currHp, float _maxHp)
    {
        if (appearBoss == false)
        {
            return;
        }

        sliderTimer.value = _currHp / _maxHp;
        textTimer.text = $"{(int)_currHp} / {(int)_maxHp}";
    }

    private void destroyAllEnemy()
    {
        int count = listAppearEnemy.Count;
        for (int iNum = count - 1; iNum > -1; --iNum)
        {
            Destroy(listAppearEnemy[iNum]);
        }
    }

    private void checkDestroyCount()
    {
        sliderDestroy.value = (float)DestCount / appearBossDestroy;
        textDestroy.text = $"{DestCount} / {appearBossDestroy}";
    }

    public void AddDestroyCount(Enemy.EnemyType _type)
    {
        //if (DestCount < appearBossDestroy)
        //{ 
        //    DestCount++;
        //}

        switch (_type)
        {
            case Enemy.EnemyType.EnemyA:
                score += 10;
                break;
            case Enemy.EnemyType.EnemyB:
                score += 30;
                break;
            case Enemy.EnemyType.EnemyC:
                score += 100;
                break;
            case Enemy.EnemyType.EnemyBoss:
                score += 200;
                break;
        }

        textScore.text = score.ToString("D8");
    }

    private void spawnEnemy()//���� ����
    {
        int iRand = Random.Range(0, listEnemy.Count);//0 ~ 2;���� �ѹ���
        GameObject objEnemy = listEnemy[iRand];//������ ���� ���ʹ�
        float fRand = Random.Range(-2.5f, 2.5f);//-2.0 ~ 2.0;���� ���� �¿� ����Ʈ
        Vector3 spawnPostion = spawnPoint.position;
        spawnPostion.x += fRand;

        GameObject obj = Instantiate(objEnemy, spawnPostion, Quaternion.identity, trsDynamicObject);
        Enemy enemy = obj.GetComponent<Enemy>();

        float rate = Random.Range(0.0f, 100.0f);
        if (rate <= itemDropRate)
        {
            enemy.SetHaveItem();
        }

        listAppearEnemy.Add(obj);
    }

    public void RemoveEnemy(GameObject _obj)
    {
        listAppearEnemy.Remove(_obj);
    }

    public void CreatItem(Vector3 _pos)
    {
        int rand = Random.Range(0, listItem.Count);//0, 1;
        GameObject instObj = listItem[rand];
        Instantiate(instObj, _pos, Quaternion.Euler(Vector3.zero), trsDynamicObject);
    }

    public Transform GetPlayerTransform()
    {
        return trsPlayer;
    }

    public void GameOver()
    {
        //int, float, string

        int rank = getMyRank(score);//������� üũ�ϴ� �ý��۸�������
        if (rank == -1)
        {
            textGameoverRank.text = $"����� ���� ���߽��ϴ�";
        }
        else
        {
            textGameoverRank.text = $"����� ��ũ�� {rank} �� �̾����ϴ�";
        }

        ipfGameoverName.gameObject.SetActive(true);

        btnGameover.onClick.RemoveAllListeners();
        btnGameover.onClick.AddListener(() =>
        {
            onClickGameoverFunction();
        });
    }

    private void onClickGameoverFunction()
    {
        string ipfText = ipfGameoverName.text;
        if (ipfText == string.Empty || ipfText.Length < 3)
        {
            textGameoverRank.text = "�̸����� �����̳�,\n3���ڹ̸��� �̸��� ������ �����ϴ�.";
            return;
        }
        int rank = getMyRank(score);//1~10, -1
        if (rank != -1)//����� ���� ���ߴ�
        { 
            setMyRank(score, ipfText, rank);
            string value = JsonConvert.SerializeObject(listRank);
            PlayerPrefs.SetString(keyRank, value);
        }

        //�ٸ������� �̵�
        SceneManager.LoadScene((int)SceneIndex.mainScene);
    }

    private int getMyRank(int _score)
    {
        for (int iNum = 0; iNum < rankCount; ++iNum)
        {
            if (listRank[iNum].Score < _score)
            {
                return iNum + 1;//����� �� �� �־��ٸ�
            }
        }
        return -1;//����� �� �� �����ٸ�
    }

    private void setMyRank(int _score, string _name, int _rank)
    {
        _rank = _rank - 1;

        listRank.Insert(_rank, new RankData() { Name = _name, Score = _score });
        listRank.RemoveAt(rankCount);
    }

    public void DestroyBoss()
    {
        //�ٽ� ���Ⱑ �⵿�ϰ� �ð��� �帣���� �������
        timer = 0.0f;
        appearBoss = false;
    }
}
