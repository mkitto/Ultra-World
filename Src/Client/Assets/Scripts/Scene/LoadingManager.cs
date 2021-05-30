using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

using SkillBridge.Message;
using ProtoBuf;
using Services;

public class LoadingManager : MonoBehaviour {

    public GameObject UITips;
    public GameObject UILoading;
    public GameObject UILogin;
    //public float loadTime = 5f;
    //private float Timer;

    public Slider progressBar;
    public UnityEngine.UI.Text progressText;
    public UnityEngine.UI.Text progressNumbrer;


    /*初始化
    void Start()
    {
        //加载配置文件
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager start");

        UILoading.gameObject.SetActive(true);
        UILogin.gameObject.SetActive(false);
        UITips.gameObject.SetActive(true);
        //yield return new WaitForSeconds(2f);
        //UILoading.SetActive(true);
        //yield return new WaitForSeconds(1f);
        //UITips.SetActive(false);

        //yield return DataManager.Instance.LoadData();

        //Init basic services
        //MapService.Instance.Init();
        //UserService.Instance.Init();


        //加载模拟
        /*for (float i = 50; i < 100; )
        {
            i += Random.Range(0.1f, 1.5f);
            progressBar.value = i;
            yield return new WaitForEndOfFrame();
        }

        UILoading.SetActive(false);
        UILogin.SetActive(true);
        yield return null;*/

    /*void Update()
    {
        Loading();
        Finishedloading();
    }

    void Loading()
    {
        Timer += Time.deltaTime;
        if (Timer >= loadTime)
        {
            Timer = loadTime;

        }
        double percentage = Timer / loadTime;

        progressBar.value = (float)percentage;
        progressNumbrer.text = "已加载" + percentage.ToString("P");

    }
    void Finishedloading()
    {
        if (progressBar.value == 1)
        {
            progressBar.gameObject.SetActive(false);
            UILogin.gameObject.SetActive(true);

        }
    }*/

    // Use this for initialization
    IEnumerator Start()
    {
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager start");

        UITips.SetActive(true);
        UILoading.SetActive(false);
        UILogin.SetActive(false);
        yield return new WaitForSeconds(2f);
        UILoading.SetActive(true);
        yield return new WaitForSeconds(1f);
        UITips.SetActive(false);

        yield return DataManager.Instance.LoadData();

        //Init basic services
        MapService.Instance.Init();
        UserService.Instance.Init();


        // Fake Loading Simulate
        for (float i = 0; i < 100;)
        {
            i += Random.Range(0.1f,0.5f);
            progressBar.value = i;
            progressNumbrer.text = (int) i + "%";
            yield return new WaitForEndOfFrame();
        }

        UILoading.SetActive(false);
        UILogin.SetActive(true);
        yield return null;
    }


    // Update is called once per frame
    void Update()
    {

    }
}


