using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserCardPrefabScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Text DisplayNameText;

    public void SendFriendRequest()
    {
        EventManager.FireFriendRawActionClick(FriendRawAction.SendFriendRequest, this.GetComponent<UserCardPrefabScript>().DisplayNameText.text);
    }
}
