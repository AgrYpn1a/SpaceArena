using GameSparks.RT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    [SerializeField]
    private GameObject messageInput;
    [SerializeField]
    private GameObject messageBox;
    [SerializeField]
    private GameObject messagesContentPane;
    [SerializeField]
    private Text message;
    [SerializeField]
    private Text chatOutput;
    [SerializeField]
    private GameObject messagesPanelHolder;
    [SerializeField]
    private Scrollbar messagesScrollbar;
    [SerializeField]
    private Button btnSend;

    [SerializeField]
    private float minPosY, maxPosY;

    [SerializeField]
    private int elementsInChatLog = 7;

    private Queue<string> chatLog = new Queue<string>();

    private bool isHidden;
    private Vector2 defaultPanelPosition;

    private bool sending;

    void Start()
    {
        defaultPanelPosition = this.GetComponent<RectTransform>().anchoredPosition;
        btnSend.onClick.AddListener(SendMessage);

        // hide chat panel
        EventSystem.current.SetSelectedGameObject(null);
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(defaultPanelPosition.x, minPosY);
        messagesPanelHolder.GetComponent<Image>().color += new Color(0, 0, 0, -0.2f);
        message.text = string.Empty;
        isHidden = true;

        // scrollbar update
        messagesScrollbar.onValueChanged.AddListener((f) =>
        {
            if(sending)
                messagesScrollbar.value = 0;
        });

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && EventSystem.current.currentSelectedGameObject != messageInput)
        {
            messageInput.GetComponent<InputField>().text = string.Empty;

            if (isHidden)
            {
                // show
                EventSystem.current.SetSelectedGameObject(messageInput);
                this.GetComponent<RectTransform>().anchoredPosition = new Vector2(defaultPanelPosition.x, maxPosY);
                messagesPanelHolder.GetComponent<Image>().color += new Color(0, 0, 0, 0.2f);
                isHidden = false;
            }
            else
            {
                // hide
                EventSystem.current.SetSelectedGameObject(null);
                this.GetComponent<RectTransform>().anchoredPosition = new Vector2(defaultPanelPosition.x, minPosY);
                messagesPanelHolder.GetComponent<Image>().color += new Color(0, 0, 0, -0.2f);
                isHidden = true;
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape) && !isHidden)
        {
            // hide
            EventSystem.current.SetSelectedGameObject(null);
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(defaultPanelPosition.x, minPosY);
            messagesPanelHolder.GetComponent<Image>().color += new Color(0, 0, 0, -0.2f);
            isHidden = true;
        }

        if (Input.GetKeyDown(KeyCode.Return))
            SendMessage();
    }

    private void SendMessage()
    {
        string msg = messageInput.GetComponent<InputField>().text;
        //Debug.Log(EventSystem.current.currentSelectedGameObject.ToString());
        if (EventSystem.current.currentSelectedGameObject != messageInput.gameObject)
            return;

        if (msg != string.Empty)
        {
            using (RTData data = RTData.Get())
            {
                data.SetString(1, msg);

                // show my message
                UpdateChatLog("<color=orange>Me</color>", msg);

                // clear message input
                messageInput.GetComponent<InputField>().text = string.Empty;

                if(GameSparksManager.Instance() != null)
                {
                    GameSparksManager.Instance().GetRTSession().SendData(
                        1,
                        GameSparks.RT.GameSparksRT.DeliveryIntent.RELIABLE,
                        data
                    );
                }
            }
        }
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(messageInput);

        messagesScrollbar.value = 0.0f;


    }

    public void OnMessageRecieved(RTPacket _packet)
    {
        Debug.Log("Message received...\n" + _packet.Data.GetString(1));

        foreach (RTSessionInfo.RTPlayer player in GameSparksManager.Instance().GetSessionInfo().GetPlayerList())
        {
            if (player.peerId == _packet.Sender)
            {
                UpdateChatLog(player.displayName, _packet.Data.GetString(1));
            }
        }
    }

    private void UpdateChatLog(string _sender, string _message)
    {
        sending = true;
        chatLog.Enqueue("<color=orange><b>" + _sender + "</b></color>\n" + _message);

        if (chatLog.Count > elementsInChatLog)
            chatLog.Dequeue();

        GameObject tempMsg = Instantiate(messageBox, messagesContentPane.transform);
        tempMsg.GetComponentInChildren<Text>().text = "[" + _sender + "]: " + _message;
        Rect tmpRect = tempMsg.GetComponent<RectTransform>().rect;

        tempMsg.GetComponent<RectTransform>().localScale = Vector3.one;

        tempMsg.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 
            tempMsg.GetComponentInChildren<Text>().preferredHeight + 4);

        Debug.Log(tempMsg.GetComponentInChildren<Text>().preferredHeight);


        /*
        chatOutput.text = string.Empty;
        foreach (string logEntry in chatLog.ToArray())
            chatOutput.text += logEntry + "\n";
        */
    }

    public void ScrollbarOnDrag()
    {
        sending = false;
    }
}
