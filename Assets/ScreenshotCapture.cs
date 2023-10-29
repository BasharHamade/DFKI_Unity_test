using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class ScreenshotCapture : MonoBehaviour
{
    [SerializeField] private int superSize = 1; // Screenshot resolution multiplier

    private bool captureScreenshot = false;
    private RenderTexture renderTexture;
    private int img_count = 0;
    private Texture2D receivedImage = null;

    private void Start()
    {
        renderTexture = new RenderTexture(Screen.width * superSize, Screen.height * superSize, 24);
    }

    private void Update()
    {
        // Check for input to capture a screenshot
        if (Input.GetKeyDown(KeyCode.S))
        {
            
            captureScreenshot = true;
            Texture2D screenshot = CaptureScreenshot();

            captureScreenshot = false;
            Debug.Log(screenshot);
            Texture2D texture = screenshot;
            byte[] pngData = texture.EncodeToPNG();
            // Specify the path within the "Assets" directory
            string filePath = Application.dataPath + "/" + img_count + ".png";

            // Write the JPG data to the specified file
            File.WriteAllBytes(filePath, pngData);
            SendPNGDataToServer(pngData);
            img_count++;
            // Receive and process the image from the server in a separate thread
            //Thread receiveThread = new Thread(ReceiveImageFromServer);
            //receiveThread.Start();


        }

        if (receivedImage != null)
        {
            // Process the received image as needed
            Debug.Log("Received and processed image");
            receivedImage = null; // Reset the received image
        }

        // Rest of your code...
    }

    // The rest of your methods...

    // The SendPNGDataToServer method remains the same as in your previous code. 
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
        try
        {
            string serverIP = "127.0.0.1"; // Replace with your server's IP address
            int serverPort = 12345; // Replace with your server's port

            // Create a TCP client and connect to the server
            using (TcpClient client = new TcpClient(serverIP, serverPort))
            {
                using (NetworkStream stream = client.GetStream())
                {
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

    void ReceiveImageFromServer()
    {
        Debug.Log("reciever thread");
        try
        {
            string serverIP = "127.0.0.1"; // Replace with your server's IP address
            int serverPort = 12345; // Replace with your server's port

            // Create a TCP client and connect to the server
            using (TcpClient client = new TcpClient(serverIP, serverPort))
            {
                using (NetworkStream stream = client.GetStream())
                {
                    // Read the image data from the server
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;
                        Debug.Log("time to read");
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            memoryStream.Write(buffer, 0, bytesRead);
                        }

                        // Convert the received image data to a Texture2D 
                        Debug.Log("we should recieve");
                        Texture2D receivedImage = new Texture2D(2, 2);
                        receivedImage.LoadImage(memoryStream.ToArray());

                        lock (this)
                        {
                            this.receivedImage = receivedImage;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error receiving image from the server: {e.Message}");
        }
    }
}

