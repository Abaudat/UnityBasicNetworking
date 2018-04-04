using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendMessage : MonoBehaviour {

    public ClientSendAPI sendAPI;

    public void SendContentAsMessage()
    {
        sendAPI.SendDebugMessage(GetComponent<InputField>().text);
        GetComponent<InputField>().text = "";
    }
}
