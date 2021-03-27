using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendCardPrefabScript : MonoBehaviour
{
    public Text DisplayNameText;

    public void UnfriendUser()
    {
        EventManager.FireFriendRawActionClick(FriendRawAction.Unfriend, this.GetComponent<FriendCardPrefabScript>().DisplayNameText.text);
    }

    public void SendFriendCoins()
    {
        EventManager.FireFriendRawActionClick(FriendRawAction.SendCoins, this.GetComponent<FriendCardPrefabScript>().DisplayNameText.text);
    }
}
