using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerCardScript : MonoBehaviour
{
    public List<Material> materials;
    public MeshRenderer cardMeshRender;
    
    public void SetCardValue(string value)
    {
        var strChucks = value.Split('|');
        var rank = (CardRank) Enum.Parse(typeof(CardRank), strChucks[0]);
        var suit = (CardSuit) Enum.Parse(typeof(CardSuit), strChucks[1]);
        var card = new Card(rank,suit);

        var matName = "Black_PlayingCards_" + card.GetImageName() + "_00";

        Material found = null;
        foreach (var material in materials)
        {
            if (matName == material.name)
            {
                found = material;
                break;
            }
        }

        GetComponent<Renderer>().materials = new[]{found};
    }
    
}