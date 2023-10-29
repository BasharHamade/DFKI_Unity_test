
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Program : MonoBehaviour
{
    [SerializeField] private int superSize = 1; // Screenshot resolution multiplier

    private bool captureScreenshot = false;
    private RenderTexture renderTexture;
    private int img_count = 0;
    private Texture2D receivedImage = null;
    // Define the server's IP address and port
    string serverIP = "127.0.0.1"; // Change this to the IP address of your server
    int serverPort = 8080; // Change this to the port your server is listening on 
    private void Start()
    {
        renderTexture = new RenderTexture(Screen.width * superSize, Screen.height * superSize, 24);
    }
    void Update()
    {
        try
        {

            if (Input.GetKeyDown(KeyCode.S))
            {
                // Create a TCP client socket
                TcpClient client = new TcpClient(serverIP, serverPort);

                // Get a network stream from the client socket
                NetworkStream stream = client.GetStream();

                // Send data to the server

                Texture2D screenshot = CaptureScreenshot();


                Debug.Log(screenshot);
                Texture2D texture = screenshot;
                byte[] pngData = texture.EncodeToPNG();
                // Specify the path within the "Assets" directory
                string filePath = Application.dataPath + "/" + img_count + ".png";
                img_count++;
                // Write the JPG data to the specified file
                File.WriteAllBytes(filePath, pngData);
                SendPNGDataToServer(pngData);





                Debug.Log("Time to recieve");
                //Receive data from the server
                /*byte[] dataReceived = new byte[1024];
                int bytesRead = stream.Read(dataReceived, 0, dataReceived.Length);
                string messageReceived = Encoding.ASCII.GetString(dataReceived, 0, bytesRead);
                */
                

                // Close the client socket and stream
                client.Close();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
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



    void SendPNGDataToServer(byte[] data)
    {
        Debug.Log("data to send " + data);
        try
        {
            // Create a TCP client and connect to the server
            using (TcpClient client = new TcpClient(serverIP, serverPort))
            {
                using (NetworkStream stream = client.GetStream())
                {
                    Debug.Log("sending data from client");
                    // Send the PNG data to the server
                    stream.Write(data, 0, data.Length);
                }


            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error sending PNG data to the server: {e.Message}");
        }

    }

}

