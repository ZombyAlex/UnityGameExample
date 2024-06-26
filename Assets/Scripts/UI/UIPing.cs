using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPing : MonoBehaviour
{
    public TextMeshProUGUI text;
    private float time = 0;
    
    void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0)
        {
            time = 5.0f;
            //CheckPing("188.120.232.19");
            CheckPing("127.0.0.1");
        }
    }

    public void CheckPing(string ip)
    {
        StartCoroutine(StartPing(ip));
    }

    IEnumerator StartPing(string ip)
    {
        WaitForSeconds f = new WaitForSeconds(0.05f);
        Ping p = new Ping(ip);
        while (p.isDone == false)
        {
            yield return f;
        }
        PingFinished(p);
    }
    
    public void PingFinished(Ping p)
    {
        text.text = "Ping: " + p.time;
    }

}
