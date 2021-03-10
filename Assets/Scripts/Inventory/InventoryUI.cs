using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Button sellButton;
    public Image icon;
    public GameObject description;

    public void OnItemClick()
    {
        Color shade = icon.color;
        shade.a = 1f;
        icon.color = shade;
        sellButton.interactable = true;
    }

    public void OnSellClick()
    {
        if (sellButton.interactable == true)
        {
            Color shade = icon.color;
            shade.a = 0.3f;
            icon.color = shade;
            sellButton.interactable = false;
        }
    }


    public void OnPointerEnter()
    {
        description.SetActive(true);
    }

    public void OnPointerExit()
    {
        description.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
