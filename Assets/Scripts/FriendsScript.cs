using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsScript : MonoBehaviour
{
    public GameObject friendCardPrefab;

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
        cardHeight += cardHeight * 0.1f; // space after
        //friendCardPrefab.transform.SetParent(transform, false);

        foreach (var friend in friends)
        {            
            //friendCardPrefab.transform.position = new Vector3(posX, posY, posZ); // set position
            var card = Instantiate(friendCardPrefab, transform); // instantiate prefab
            card.transform.localPosition.Set(card.transform.localPosition.x, posY, card.transform.localPosition.z);
            posY += cardHeight;
        }        
    }
}
