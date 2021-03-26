using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendCardPrefabScript : MonoBehaviour
{
    public Text DisplayNameText;

    public void UnfriendUser()
    {
        var localUserId = UserInfo.GetInstance().UserId;
        var friendDisplayName = this.GetComponent<FriendCardPrefabScript>().DisplayNameText.text;

        if (localUserId != null && friendDisplayName != null)
        {
            DataManager.UnfriendUser(localUserId, friendDisplayName);
            this.gameObject.SetActive(false);
        }        
    }

    public void SendFriendCoins()
    {
        EventManager.FireFriendRawActionClick(FriendRawAction.SendCoins, this.GetComponent<FriendCardPrefabScript>().DisplayNameText.text);
    }
}
