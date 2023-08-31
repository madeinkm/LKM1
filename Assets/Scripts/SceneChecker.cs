using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Checker
{
    public static bool isLoadedToStartScene = false;
}

public class SceneChecker : MonoBehaviour
{
    public bool statrtScene = false;

    private void Awake()
    {
        if (statrtScene == true)
        {
            Checker.isLoadedToStartScene = true;
        }

        if (Checker.isLoadedToStartScene == false)
        {
            SceneManager.LoadScene((int)SceneIndex.mainScene);
        }
    }
}
