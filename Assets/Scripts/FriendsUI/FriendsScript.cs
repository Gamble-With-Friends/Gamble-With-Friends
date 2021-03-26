using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendsScript : MonoBehaviour
{
    public GameObject friendCardPrefab;
    public GameObject scrollViewContent;

    
    // Start is called before the first frame update
    void Start()
    {
        InstantiateFriendCards(transform.position.x, transform.position.y, transform.position.z); // transform.position - position of the parent objectof the script
    }    

    private void InstantiateFriendCards(float posX, float posY, float posZ)
    {
        var cardHeight = friendCardPrefab.transform.localScale.y;
        var offset = -30f;

        var localUserId = UserInfo.GetInstance().UserId;
        
        if (localUserId == null)
        {
            return;
        }

        var friends = DataManager.GetFriends(localUserId);

        foreach (var friend in friends)
        {            
            var card = Instantiate(friendCardPrefab, scrollViewContent.transform); // instantiate prefab
            card.GetComponent<FriendCardPrefabScript>().DisplayNameText.text = friend;
            var localPosition = card.transform.localPosition;
            card.transform.localPosition = new Vector3(localPosition.x, cardHeight + offset, localPosition.z);
            offset -= 40;
        }        
    }
}
