using UnityEngine;
using WebSocketSharp;
using System.Collections.Generic;

public class SofiCommand
{
    public string name;
    public string command;
}

public class WebSocketClient : MonoBehaviour {
    WebSocket ws;
    List<string> commands = new List<string>();

	void Start() {
        // Initiatlize the socket

        ws = new WebSocket("ws://127.0.0.1:9000");
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Server says: " + e.Data);
            commands.Add(e.Data);
        };

        ws.Connect();
        ws.Send("{\"event\":\"init\"}");
    }

	void Update ()
    {
        // Send update events as needed

        if (commands.Count > 0)
        {
            HandleCommand(commands[0]);
            commands.Remove(commands[0]);
        }
    }

    void OnDestroy()
    {
        // Close the socket

        ws.Close();
    }

    void HandleCommand(string data)
    {
        SofiCommand cmd = JsonUtility.FromJson<SofiCommand>(data);
        Debug.Log("Command " + cmd.name + ": " + cmd.command);

        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.transform.position = GameObject.Find("SpawnPoint").transform.position;
    }
}
