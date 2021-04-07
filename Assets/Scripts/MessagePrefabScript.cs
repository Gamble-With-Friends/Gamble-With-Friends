using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessagePrefabScript : MonoBehaviour
{
    public string MessageId;
    public GameObject ContentInput;

    public void EditMessage()
    {
        EventManager.FireFriendRawActionClick(FriendRawAction.EditMessage, MessageId);
    }
    public void DeleteMessage()
    {
        EventManager.FireFriendRawActionClick(FriendRawAction.DeleteMessage, MessageId);
    }
}
