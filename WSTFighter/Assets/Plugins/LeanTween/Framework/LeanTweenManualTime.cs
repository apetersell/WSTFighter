using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeanTweenManualTime : MonoBehaviour
{
    static public float deltaTime = 0.1f;

    bool alreadyHaveInstance => instance != null;

    IEnumerator updatedCo;

    static public LeanTweenManualTime instance;
    
    void Awake()
    {
        if (alreadyHaveInstance) 
        {
            Destroy(this);
            return;
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        StartCoroutine("UpdateTime");
    }

    IEnumerator UpdateTime()
    {
        while (true)
        {
            LeanTween.dtManual = 0f;
            yield return new WaitForSecondsRealtime(deltaTime);
            LeanTween.dtManual = deltaTime;
            yield return new WaitUntil(LeanTween.HasFinishedThisFrame);
        }
    }

    static public void LazyInit()
    {
        if (instance == null)
        {
            var newGO = new GameObject();
            newGO.name = "~LeanTweenManualTime";
            newGO.AddComponent(typeof(LeanTweenManualTime));
            newGO.isStatic = true;
            DontDestroyOnLoad(newGO);
        }
    }
}
