using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NFTstorage;
using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using TMPro;

public class VLink : M2MqttUnityClient
{
    [Header("Wallet Connect Only")]
    [SerializeField]
    TextMeshProUGUI codeObj;
    [SerializeField]
    bool transitionOnWalletConnect = true;

    string channel;

    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }

    public void Upload(RenderTexture renderTexture, Texture2D texture2D, byte[] bytes)
    {
        Debug.Log("converting");
        if (renderTexture != null)
        {
            texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBAFloat, false);
            RenderTexture.active = renderTexture;
        }
        if (texture2D != null)
        {
            texture2D.ReadPixels(new Rect(0, 0, texture2D.width, texture2D.height), 0, 0);
            texture2D.Apply();
            bytes = texture2D.EncodeToPNG();
        }

        Debug.Log("uploading");
        StartCoroutine(NetworkManager.UploadObject(sendToMQTT, bytes));
    }

    public void sendToMQTT(DataResponse dataResponse)
    {
        Debug.Log("uploaded");
        if (!dataResponse.Success)
        {
            Debug.LogError(message: "Upload fail");
            return;
        }

        if (dataResponse.Values != null)
        {
            var path = Helper.GenerateGatewayPath(dataResponse.Values[0].cid, Constants.GatewaysSubdomain[0], isSubdomain: true);
            Debug.Log(message: "Image found at " + path);

            Publish(path);
        }
    }






    //using C# Property GET/SET and event listener to reduce Update overhead in the controlled objects
    private string m_msg;

    public string msg
    {
        get
        {
            return m_msg;
        }
        set
        {
            if (m_msg == value) return;
            m_msg = value;
            if (OnMessageArrived != null)
            {
                OnMessageArrived(m_msg);
            }
        }
    }

    public event OnMessageArrivedDelegate OnMessageArrived;
    public delegate void OnMessageArrivedDelegate(string newMsg);

    //using C# Property GET/SET and event listener to expose the connection status
    private bool m_isConnected;

    public bool isConnected
    {
        get
        {
            return m_isConnected;
        }
        set
        {
            if (m_isConnected == value) return;
            m_isConnected = value;
            if (OnConnectionSucceeded != null)
            {
                OnConnectionSucceeded(isConnected);
            }
        }
    }
    public event OnConnectionSucceededDelegate OnConnectionSucceeded;
    public delegate void OnConnectionSucceededDelegate(bool isConnected);

    // a list to store the messages
    private List<string> eventMessages = new List<string>();

    public void Publish(string message)
    {
        client.Publish("VLink" + channel, System.Text.Encoding.UTF8.GetBytes(message), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
        Debug.Log("Test message published at" + channel);
    }
    public void SetEncrypted(bool isEncrypted)
    {
        this.isEncrypted = isEncrypted;
    }

    protected override void OnConnecting()
    {
        base.OnConnecting();
    }

    protected override void OnConnected()
    {
        Debug.Log("connected");
        base.OnConnected();
        isConnected = true;

        channel = Random.Range(10000, 99999).ToString();
        codeObj.text = channel;

        SubscribeTopics();
    }

    protected override void OnConnectionFailed(string errorMessage)
    {
        Debug.Log("CONNECTION FAILED! " + errorMessage);
    }

    protected override void OnDisconnected()
    {
        Debug.Log("Disconnected.");
        isConnected = false;
    }

    protected override void OnConnectionLost()
    {
        Debug.Log("CONNECTION LOST!");
    }

    protected override void SubscribeTopics()
    {
        client.Subscribe(new string[] { "VLink" + channel }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
    }

    protected override void UnsubscribeTopics()
    {
        client.Unsubscribe(new string[] { "VLink" + channel });
    }

    protected override void Start()
    {
        Debug.Log("starting");
        base.Start();
    }

    protected override void DecodeMessage(string topic, byte[] message)
    {
        //The message is decoded
        msg = System.Text.Encoding.UTF8.GetString(message);

        Debug.Log("Received: " + msg);

        StoreMessage(msg);
    }

    private void StoreMessage(string eventMsg)
    {
        if (eventMessages.Count > 50)
            eventMessages.RemoveAt(0);
        eventMessages.Add(eventMsg);

        string[] words = eventMsg.Split(' ');
        if (words[0] == "Wallet:")
        {
            PlayerPrefs.SetString("walletAddress", words[1]);
            Debug.Log("Wallet Saved");

            if (transitionOnWalletConnect)
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    protected override void Update()
    {
        base.Update(); // call ProcessMqttEvents()

    }

    private void OnDestroy()
    {
        Disconnect();
    }
}
