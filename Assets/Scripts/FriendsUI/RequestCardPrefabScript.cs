using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestCardPrefabScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Text DisplayNameText;

    public void AcceptFriendRequest()
    {
        EventManager.FireFriendRawActionClick(FriendRawAction.AcceptRequest, this.GetComponent<RequestCardPrefabScript>().DisplayNameText.text);
    }

    public void DenyFriendRequest()
    {
        EventManager.FireFriendRawActionClick(FriendRawAction.DenyRequest, this.GetComponent<RequestCardPrefabScript>().DisplayNameText.text);
    }
}
