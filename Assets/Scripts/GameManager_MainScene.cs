using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager_MainScene : MonoBehaviour
{
    private RankingManager rankingManager;

    private void Awake()
    {
        float targetRatio = 9.0f / 16.0f;//FHD 1920 1080 16:9
        float ratio = (float)Screen.width / (float)Screen.height;
        float scaleHeight = ratio / targetRatio;
        float fixedWidth = (float)Screen.width / scaleHeight;
        Screen.SetResolution((int)fixedWidth, Screen.height, true); // 화면비율 및 크기 조정하는 코드, 화면이 깜빡이게하는기능도 있음

        //Application.targetFrameRate = 60; // 프레임을 제한해주는 코드

        Button[] arrayBtn = GetComponentsInChildren<Button>(true);
        arrayBtn[0].onClick.AddListener(startGame);
        arrayBtn[1].onClick.AddListener(ranking);
        arrayBtn[2].onClick.AddListener(exitGame);

        rankingManager = Transform.FindObjectOfType<RankingManager>();
    }

    private void startGame()
    {
        SceneManager.LoadScene((int)SceneIndex.playScene);
    }
    private void ranking()
    {
        rankingManager.ShowRank();
    }
    private void exitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
