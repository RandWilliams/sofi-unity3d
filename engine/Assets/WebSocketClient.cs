using UnityEngine;
using WebSocketSharp;
using System.Collections.Generic;

public class SofiCommand
{
    public string command;
    public string name;
    public string obj;

    public string data;

    public string text;
    public string text_anchor;
    public string text_font;
    public int text_font_size;

    public string position_on;
    public float[] position;
    public float[] rotation;
    public float[] scale;

    public float[] color;

    public bool rigidbody;
    public bool collider;

    public string look_at;
}

public class WebSocketClient : MonoBehaviour {
    private WebSocket ws;
    private bool connected = false;

    private List<string> commands = new List<string>();

    private HashSet<string> listenToKeys = new HashSet<string>();

	void Start()
    {
        ws = new WebSocket("ws://127.0.0.1:9000");
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("Connected to server");
            connected = true;
        };

        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Server says: " + e.Data);
            commands.Add(e.Data);
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("Disconnected from server");
            connected = false;
        };

        ConnectToSofi();
    }

	void Update()
    {
        if (connected)
        {
            foreach (string key in listenToKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    ws.SendAsync("{\"event\": \"unity.keyboard\", \"key\":\"" + key + "\"}", null);
                }
            }

            // Send update events as needed
            if (commands.Count > 0)
            {
                HandleCommand(commands[0]);
                commands.Remove(commands[0]);
            }
        }
        else
        {
            ConnectToSofi();
        }

    }

    void OnDestroy()
    {
        // Close the socket
        if (connected)
        {
            ws.Close();
        }
    }

    // void OnGUI()
    // {
    //     Event e = Event.current;
    //
    //     if (listenToKeyboard && e.isKey && e.keyCode != KeyCode.None)
    //     {
    //         ws.SendAsync("{\"event\": \"unity.keyboard\", \"key\":\"" + e.keyCode + "\"}", null);
    //     }
    // }

    void ConnectToSofi()
    {
        ws.Connect();
        ws.SendAsync("{\"event\": \"init\"}", null);
    }

    void HandleCommand(string data)
    {
        SofiCommand cmd = JsonUtility.FromJson<SofiCommand>(data);

        GameObject obj = null;

        if (cmd.command == "unity.spawn")
        {
            if (cmd.obj == "cube")
            {
                obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                if (cmd.collider)
                {
                    obj.AddComponent<BoxCollider>();
                }
            }
            else if (cmd.obj == "sphere")
            {
                obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                if (cmd.collider)
                {
                    obj.AddComponent<SphereCollider>();
                }
            }
            else if (cmd.obj == "cylinder")
            {
                obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                if (cmd.collider)
                {
                    obj.AddComponent<CapsuleCollider>();
                }
            }
            else if (cmd.obj == "capsule")
            {
                obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                if (cmd.collider)
                {
                    obj.AddComponent<CapsuleCollider>();
                }
            }
            else if (cmd.obj == "plane")
            {
                obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
                if (cmd.collider)
                {
                    obj.AddComponent<MeshCollider>();
                }
            }
            else if (cmd.obj == "guitext")
            {
                obj = new GameObject("GUIText");
                obj.AddComponent<GUIText>();
                obj.AddComponent<CanvasRenderer>();
            }
            else if (cmd.obj == "empty")
            {
                obj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            }
            else {
                return;
            }

            if (cmd.name != null)
            {
                obj.name = cmd.name;
            }
        }
        else if (cmd.command == "unity.update")
        {
            obj = GameObject.Find(cmd.name);

            if (obj == null)
            {
                return;
            }
        }
        else if (cmd.command == "unity.remove")
        {
            obj = GameObject.Find(cmd.name);

            if (obj == null)
            {
                return;
            }

            Destroy(obj);
            return;
        }
        else if (cmd.command == "unity.subscribe")
        {
            if (cmd.name == "keyboard")
            {
                foreach (string key in cmd.data.Split(','))
                {
                    Debug.Log("Listening for Key: " + key);
                    listenToKeys.Add(key);
                }
            }

            return;
        }

        if (cmd.obj == "guitext")
        {
            GUIText guiText = obj.GetComponent<GUIText>();
            guiText.text = cmd.text;

            if (cmd.position.Length > 0)
            {
                if (cmd.position[0] == -1)
                {
                    cmd.position[0] = Screen.width / 2;
                }

                if (cmd.position[1] == -1)
                {
                    cmd.position[1] = Screen.height / 2;
                }

                guiText.pixelOffset = new Vector2(cmd.position[0], cmd.position[1]);
            }
            else
            {
                guiText.pixelOffset = new Vector2(Screen.width / 2, Screen.height / 2);
            }

            if (cmd.text_anchor == "upper-left")
            {
                guiText.anchor = TextAnchor.UpperLeft;
            }
            else if (cmd.text_anchor == "upper-center")
            {
                guiText.anchor = TextAnchor.UpperCenter;
            }
            else if (cmd.text_anchor == "upper-right")
            {
                guiText.anchor = TextAnchor.UpperRight;
            }
            else if (cmd.text_anchor == "middle-left")
            {
                guiText.anchor = TextAnchor.MiddleLeft;
            }
            else if (cmd.text_anchor == "middle-right")
            {
                guiText.anchor = TextAnchor.MiddleRight;
            }
            else if (cmd.text_anchor == "lower-left")
            {
                guiText.anchor = TextAnchor.LowerLeft;
            }
            else if (cmd.text_anchor == "lower-center")
            {
                guiText.anchor = TextAnchor.LowerCenter;
            }
            else if (cmd.text_anchor == "lower-right")
            {
                guiText.anchor = TextAnchor.LowerRight;
            }
            else
            {
                guiText.anchor = TextAnchor.MiddleCenter;
            }

            if (cmd.color.Length > 0)
            {
                Color color = guiText.color;

                color.r = cmd.color[0];
                color.g = cmd.color[1];
                color.b = cmd.color[2];
                color.a = cmd.color[3];

                guiText.color = color;
            }

            if (cmd.text_font_size > 0)
            {
                guiText.fontSize = cmd.text_font_size;
            }

            return;
        }

        if (cmd.rigidbody)
        {
            obj.AddComponent<Rigidbody>();
        }

        if (cmd.position_on != null && cmd.position_on != "")
        {
            Transform tform = GameObject.Find(cmd.position_on).transform;
            obj.transform.position = tform.position;
            obj.transform.rotation = tform.rotation;
        }

        if (cmd.position.Length > 0)
        {
            obj.transform.position = new Vector3(cmd.position[0], cmd.position[1], cmd.position[2]);
        }

        if (cmd.rotation.Length > 0)
        {
            obj.transform.rotation = Quaternion.Euler(cmd.rotation[0], cmd.rotation[1], cmd.rotation[2]);
        }

        if (cmd.scale.Length > 0)
        {
            obj.transform.localScale = new Vector3(cmd.scale[0], cmd.scale[1], cmd.scale[2]);
        }

        if (cmd.color.Length > 0)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            rend.material.shader = Shader.Find("Standard");
            Color color = rend.material.color;

            color.r = cmd.color[0];
            color.g = cmd.color[1];
            color.b = cmd.color[2];
            color.a = cmd.color[3];

            rend.material.color = color;
        }
    }
}
