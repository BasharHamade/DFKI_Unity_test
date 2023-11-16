using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class clientTest : MonoBehaviour
{
    [SerializeField] private int superSize = 1; // Screenshot resolution multiplier
    private TcpClient client;
    private NetworkStream stream;
    private byte[] buffer = new byte[1024];
    private RenderTexture renderTexture;
    bool send = false;

    private void Start()
    {
        client = new TcpClient("localhost", 12345);  // Replace with the server's IP and port
        stream = client.GetStream();
        renderTexture = new RenderTexture(Screen.width * superSize, Screen.height * superSize, 24);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            send = true;
            //SendData("Hello from Unity");
            // ReceiveData();
        }
        if (send)
        {
            SendData("Hello");
            send = false;
            ReceiveData();
        }

        
    }

    private void SendData(string message)
    {
        Debug.Log("send now");
        Texture2D screenshot = CaptureScreenshot();


        //Debug.Log(screenshot);
        Texture2D texture = screenshot;
        byte[] pngData = texture.EncodeToPNG();
        string base64String = Convert.ToBase64String(pngData);
        Debug.Log(base64String);
        Debug.Log(base64String.Length);
        byte[] data = Encoding.UTF8.GetBytes(base64String);
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

    Texture2D CaptureScreenshot()
    {
        // Set the camera's target texture to the render texture
        Camera.main.targetTexture = renderTexture;

        // Render the camera's view to the render texture
        Camera.main.Render();

        // Create a new Texture2D to store the screenshot
        Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        // Read the screen into the Texture2D
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        screenshot.Apply();
        RenderTexture.active = null;

        // Reset the camera's target texture
        Camera.main.targetTexture = null;

        return screenshot;
    }
}
