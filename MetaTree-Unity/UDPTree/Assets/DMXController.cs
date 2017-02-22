//
// DMX機器制御
//
// Copyright(C) 2015 Team Hashilus.
//

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

/// <summary>
/// uDMXを用いて、DMX機器をコントロールするスクリプト
/// </summary>
public class DMXController : MonoBehaviour {
	public bool ShowConfig;
	public bool UpdateChannel = false;

    class DMXValue
    {
        public DMXValue()
        {
            channel = 0;
            value = 0;
        }
        public DMXValue(int ch, int val)
        {
            SetVal(ch, val);
        }

        public void SetVal(int ch,int val)
        {
            channel = ch;
            value = val;
        }
        public int GetCh()
        {
            return channel;
        }
        public int GetValue()
        {
            return value;
        }

        int channel;
        int value;
    }

    [SerializeField]
    List<int> usedChannels= new List<int>();

    List<DMXValue> dmxvalues= new List<DMXValue>();
	
    float timeElapsed;

	public Text logtext;
    
	/// <summary>
	/// DMX機器が制御可能かどうかを調べる
	/// </summary>
	/// <returns>制御可能な場合は<c>true</c>、それ以外は<c>false</c>。</returns>
	public bool IsReady() {
		return uDMX.Connected();
	}



	/// <summary>
	/// スクリプトが開始された時に呼ばれる
	/// </summary>
	void Start() {
		Debug.Log("Is DMX device ready? " + IsReady());
		logtext.text = "Is DMX device ready? " + IsReady ();
        /*
        if (string.IsNullOrEmpty(SequenceFile))
        {
            SequenceFile = Application.dataPath + "/../DMXSequence.txt";
        }
        else
        {
            SequenceFile = Application.dataPath + "/" + SequenceFile;
        }*/
	}


    public void ForceStopAll()
    {
        foreach (var item in usedChannels)
        {
            Debug.Log("update channel value ch" + item + " val:" + 0);
            uDMX.ChannelSet(item, 0);
        }
    }

	/// <summary>
	/// スクリプトが終了する時に呼ばれる
	/// </summary>
	void OnApplicationQuit() {
        ForceStopAll();
    }

	/// <summary>
	/// フレームが更新される時に呼ばれる
	/// </summary>
	void Update() {
        timeElapsed += Time.deltaTime;
	     if (Time.frameCount % 10 == 0) {
			OnShowConfig();
			OnUpdateChannel();
		}
	}

    /// <summary>
    /// 文字列をパースする
    /// 例文「channel,1,value,255」みたいなやつ
    /// </summary>
    /// <param name="input"></param>
    /// <param name="ch"></param>
    /// <param name="val"></param>
    /// <returns></returns>
    bool ParseSingleMessage( string input, out int ch, out int val)
    {
        ch = -1;
        val = 0;
    
            string[] values = input.Split(',', ':');
            int channelNo;
            int value;
            if (values.Length >= 4)
            {
                if (int.TryParse(values[1], out channelNo))
                {

                    //変換出来たら、
                    ch = channelNo;
                }
                if (int.TryParse(values[3], out value))
                {

                    //変換出来たら、
                    val = value;
                }
            }

        
        return true;
    }

    public void AddNewValue(string message)
    {

        string nl = Environment.NewLine;
        string[] result = message.Split(new string[] { nl }, StringSplitOptions.None);
        
        foreach (string word in result)
        {
            int channel=-1;
            int value=-1;

            ParseSingleMessage(word, out channel, out value);
            if (channel >= 0)
            {
                AddNewValue(channel, value);
            }


        }
    }

    public void AddNewValue(int targetChannel, int targetValue)
    {
        if (usedChannels.Contains(targetChannel) == false)
        {
            usedChannels.Add(targetChannel);
        }
        DMXValue val = new DMXValue(targetChannel, targetValue);
        dmxvalues.Add(val);
        UpdateChannel = true;
    }


    /// <summary>
    /// ShowConfigフラグがtrueにセットされた時に、uDMXの設定ダイアログを表示する
    /// </summary>
    void OnShowConfig() {
		if (ShowConfig) {
			ShowConfig = false;
			uDMX.Configure();
		}
	}

	/// <summary>
	/// UpdateChannelフラグがtrueにセットされた時に、DMXチャンネル値を更新する
	/// </summary>
	void OnUpdateChannel() {
		if (UpdateChannel) {
			UpdateChannel = false;
            foreach (var item in dmxvalues)
            {
                Debug.Log("update channel value ch" + item.GetCh() + " val:" + item.GetValue());

                uDMX.ChannelSet(item.GetCh(), item.GetValue());
            }
            dmxvalues.Clear();
        }
    }
    
}
