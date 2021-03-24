using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendCoinsTriggerScript : MonoBehaviour
{
    public GameObject SendCoins;
    public string whatever;

    public void OpenSendCoinsUI()
    {
        Cursor.lockState = CursorLockMode.Confined;
        SendCoins.SetActive(true);
        var sendCoinsTitle = GameObject.Find("SendCoinsRawImage/WindowTitle");
        sendCoinsTitle.GetComponent<Text>().text = $"Send coins to {this.GetComponent<FriendCardPrefabScript>().DisplayNameText.text}";
    }
}
