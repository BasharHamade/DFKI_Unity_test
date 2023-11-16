using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ColorReceiver : MonoBehaviour
{
    private const int Port = 8080;
    private TcpListener listener;
    private bool isListening;

    private void Start()
    {
        StartListening();
    }

    private void OnDestroy()
    {
        StopListening();
    }

    private void StartListening()
    {
        try
        {
            listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            isListening = true;
            Debug.Log("Server started. Listening for incoming connections...");
            AcceptClients();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error starting server: {e.Message}");
        }
    }

    private void StopListening()
    {
        if (isListening)
        {
            listener.Stop();
            isListening = false;
            Debug.Log("Server stopped.");
        }
    }

    private async void AcceptClients()
    {
        while (isListening)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();
            ProcessClient(client);
        }
    }

    private async void ProcessClient(TcpClient client)
    {
        using (NetworkStream stream = client.GetStream())
        {
            try
            {
                byte[] data = new byte[client.ReceiveBufferSize];
                int bytesRead = await stream.ReadAsync(data, 0, data.Length);
                string json = System.Text.Encoding.ASCII.GetString(data, 0, bytesRead);
                List<List<float>> colors = DeserializeColors(json);
                LogColors(colors);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error processing client data: {e.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }

    private List<List<float>> DeserializeColors(string json)
    {
        List<List<float>> colors = new List<List<float>>();
        string[] colorStrings = json.Split(';');
        foreach (string colorString in colorStrings)
        {
            string[] colorValues = colorString.Split(',');
            if (colorValues.Length == 3)
            {
                List<float> color = new List<float>();
                foreach (string value in colorValues)
                {
                    if (float.TryParse(value, out float floatValue))
                    {
                        color.Add(floatValue);
                    }
                }
                colors.Add(color);
            }
        }
        return colors;
    }

    private void LogColors(List<List<float>> colors)
    {
        foreach (List<float> color in colors)
        {
            Debug.Log($"Received color: R={color[0]}, G={color[1]}, B={color[2]}");
        }
    }
}