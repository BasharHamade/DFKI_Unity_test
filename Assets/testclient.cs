using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class testclient : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private byte[] buffer = new byte[1024];
    bool send = false;

    private void Start()
    {
        client = new TcpClient("localhost", 12345);  // Replace with the server's IP and port
        stream = client.GetStream();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            send = true;
            //SendData("Hello from Unity");
            // ReceiveData();
        } 
        if(send)
        {
            SendData("Hello");
            send = false;
            ReceiveData();
        } 
        

       
    }

    private void SendData(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }

    private void ReceiveData()
    {
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        if (bytesRead > 0)
        {
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Debug.Log("Received: " + receivedMessage);
        }
    }

    private void OnApplicationQuit()
    {
        stream.Close();
        client.Close();
    }
}
