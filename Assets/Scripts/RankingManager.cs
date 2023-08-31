using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    private GameObject fabRanking;
    private GameObject objTitle;
    private GameObject objRanking;

    private List<RankData> listRank;
    private string keyRank = "keyRank";
    private int rankCount = 10;

    private ScrollRect scroll;

    void Awake()
    {
        fabRanking = Resources.Load<GameObject>("Fab_Ranking");

        Transform trsObjParent = GameObject.Find("GameManager_MainScene").transform;
        objTitle = trsObjParent.Find("objTitle").gameObject;
        objRanking = trsObjParent.Find("objRanking").gameObject;

        scroll = objRanking.GetComponentInChildren<ScrollRect>();
        Button btnRankExit = objRanking.GetComponentInChildren<Button>();
        btnRankExit.onClick.AddListener(CloseRank);

        string value = PlayerPrefs.GetString(keyRank);
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
        CloseRank();
    }

    public void ShowRank()
    {
        objTitle.SetActive(false);
        objRanking.SetActive(true);
        setRankingData();
    }

    private void setRankingData()
    {
        if (scroll.content.childCount > 0)
        {
            clearRankingData();
        }

        for (int iNum = 0; iNum < rankCount; ++iNum)
        {
            GameObject obj = Instantiate(fabRanking, scroll.content);
            Fab_Ranking sc = obj.GetComponent<Fab_Ranking>();
            sc.SetRanking((iNum + 1).ToString("D2"), listRank[iNum].Score.ToString("D8"), listRank[iNum].Name);
        }
    }

    private void clearRankingData()
    {
        int count = scroll.content.childCount;//3
        for (int iNum = count - 1; iNum > -1; --iNum)
        {
            Destroy(scroll.content.GetChild(iNum).gameObject);
        }
    }
    private void CloseRank()
    {
        objTitle.SetActive(true);
        objRanking.SetActive(false);
        clearRankingData();
    }
}
