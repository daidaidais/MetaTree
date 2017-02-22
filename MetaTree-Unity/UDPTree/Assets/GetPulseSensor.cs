using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GetPulseSensor : MonoBehaviour
{
    //先ほど作成したクラス
    public SerialHandler serialHandler;
    public static int BPM;

    void Start()
    {
        //信号を受信したときに、そのメッセージの処理を行う
        serialHandler.OnDataReceived += OnDataReceived;
    }

    void Update()
    {
        //文字列を送信
        //serialHandler.Write("hogehoge");
    }

    //受信した信号(message)に対する処理
    void OnDataReceived(string message)
    {
        var data = message.Split(
                new string[] { "\t" }, System.StringSplitOptions.None);
        if (data.Length < 2) return;

        try
        {
            BPM = int.Parse(data[0]);
            //Debug.Log(BPM);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}