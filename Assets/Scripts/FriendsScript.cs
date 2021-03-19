using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendsScript : MonoBehaviour
{
    public GameObject friendCardPrefab;
    public GameObject scrollViewContent;

    List<string> friends = new List<string>
        {
            "Harout",
            "Tony",
            "Sean",
            "Mark",
            "Docker"
        };
    // Start is called before the first frame update
    void Start()
    {
        InstantiateFriendCards(friends, transform.position.x, transform.position.y, transform.position.z); // transform.position - position of the parent objectof the script
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenFriendListTab()
    {
        
    }

    private void InstantiateFriendCards(List<string> friends, float posX, float posY, float posZ)
    {
        var cardHeight = friendCardPrefab.transform.localScale.y;
        var offset = -100f;

        foreach (var friend in friends)
        {            
            var card = Instantiate(friendCardPrefab, scrollViewContent.transform); // instantiate prefab
            var localPosition = card.transform.localPosition;
            card.transform.localPosition = new Vector3(localPosition.x, cardHeight + offset, localPosition.z);
            offset -= 100;
        }        
    }
}
