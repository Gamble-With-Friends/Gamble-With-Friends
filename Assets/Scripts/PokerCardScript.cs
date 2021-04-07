using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokerCardScript : MonoBehaviour
{
    public List<Material> materials;
    public Material zoneMaterial;
    public MeshRenderer cardMeshRender;
    public int position;
    public bool changeRequested;
    public bool disabled;

    private void OnEnable()
    {
        EventManager.OnClick += OnClick;
    }

    private void OnDisable()
    {
        EventManager.OnClick -= OnClick;
    }

    public void SetCardValue(string value)
    {
        var strChucks = value.Split('|');
        var rank = (CardRank) Enum.Parse(typeof(CardRank), strChucks[0]);
        var suit = (CardSuit) Enum.Parse(typeof(CardSuit), strChucks[1]);
        var card = new Card(rank,suit);

        var matName = "Black_PlayingCards_" + card.GetImageName() + "_00";

        var found = materials.FirstOrDefault(material => matName == material.name);

        GetComponent<Renderer>().materials = new[]{found};
    }

    private void OnClick(int instanceId)
    {
        if(instanceId != transform.GetInstanceID()) return;
        
        if(disabled) return;

        changeRequested = !changeRequested;
        
        if (changeRequested)
        {
            GetComponent<Renderer>().materials = new[]{GetComponent<Renderer>().materials[0], zoneMaterial};
        }
        else
        {
            GetComponent<Renderer>().materials = new[] {GetComponent<Renderer>().materials[0]};
        }
    }
}