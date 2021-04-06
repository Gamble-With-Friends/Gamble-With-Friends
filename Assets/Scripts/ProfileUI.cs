using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    public GameObject profileui;
    public Text playerName;

    public List<Image> images;

    private void OnEnable()
    {
        EventManager.OnFriendRawActionClick += OnFriendRawActionClick;
    }

    private void OnDisable()
    {
        EventManager.OnFriendRawActionClick -= OnFriendRawActionClick;
    }

    private void OnFriendRawActionClick(FriendRawAction action, string displayName)
    {
        if (action == FriendRawAction.ViewProfile)
        {
            Debug.Log("Open ViewProfile");
            OnCheckProfile(displayName);
        }
    }

    private void OnCheckProfile(string displayname)
    {
        //Clear Profile
        ClearProfile();

        //set display name
        playerName.text = displayname;

        //set items alpha to display friends inventory
        FillProfile(displayname);

        //display profileui
        profileui.SetActive(true);
    }

    private void ClearProfile()
    {
        foreach(Image image in images)
        {
            Color shade = image.color;
            shade.a = 0.3f;
            image.color = shade;
            Debug.Log(image.sprite.name); 
        }
    }

    private void FillProfile(string displayName)
    {
        var friendId = DataManager.GetUserId(displayName);
        var friendInventory = DataManager.GetInventoryItems(friendId);

        foreach (Image image in images)
        {
            string itemId = DataManager.GetItemId(image.name);
            if (friendInventory.ContainsKey(itemId))
            {
                Color shade = image.color;
                shade.a = 1f;
                image.color = shade;
                Debug.Log(image.sprite.name);
            }
        }
    }

    public void OnCloseProfileClick()
    {
        profileui.SetActive(false);
    }
}
