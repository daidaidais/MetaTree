using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UDPReceive : MonoBehaviour
{
	UdpClient objSck;
	public DMXController dmxctrl;
	public bool dmxactive;
	public bool dmxStop;
    public static string text;


    void Start ()
	{
        ConnectToServer();
    }

    void OnEnable() {
        
    }

    public void ConnectToServer()
    {
        byte[] bia = new byte[4];
        bia[0] = 127;
        bia[1] = 0;
        bia[2] = 0;
        bia[3] = 1;

        IPAddress ipa = new IPAddress(bia);
        IPEndPoint ipe = new IPEndPoint(ipa, 7777);
        objSck = new UdpClient(ipe);
        objSck.Connect("127.0.0.1", 8888);
        //objSck.Connect("192.169.1.12", 8888);

        Debug.Log("connect");
        Debug.Log(objSck.Available.ToString());
        objSck.BeginReceive(ReceiveCallback, objSck);

    }

    void Update ()
	{

        if (Input.GetKeyDown(KeyCode.F))
        {
            dmxctrl.AddNewValue("channnel,1,value, 255");

            //dmxctrl.AddNewValue(1, 255);
            //dmxctrl.StartSequence = true;
            dmxactive = true;
        }

        if (Input.GetKeyDown(KeyCode.R)){
            ConnectToServer();
            dmxactive = true;
        }

       

		if (dmxStop) {
			//dmxctrl.StopSequence();
			
		}

        if (Input.GetKeyDown(KeyCode.J))
        {
            Byte[] dat = System.Text.Encoding.ASCII.GetBytes("hello");
            objSck.Send(dat, dat.Length);
        }
    }

	void ReceiveCallback(IAsyncResult AR)
	{
        //Debug.Log("recieve");
		System.Net.IPEndPoint ipAny = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0);
		Byte[] dat = objSck.EndReceive(AR, ref ipAny);
        text = System.Text.Encoding.ASCII.GetString(dat);
        //Debug.Log(text);

        dmxctrl.AddNewValue(text);

		objSck.BeginReceive(ReceiveCallback, objSck);
	}	

}