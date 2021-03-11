using System;
using System.Collections.Generic;
using System.Linq;

public enum CardSuit
{
    Diamonds,
    Clubs,
    Hearts,
    Spades
}

public enum CardRank
{
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13
}

public class Card
{
    public CardRank Rank { get; set; }
    public CardSuit Suit { get; set; }

    public Card(CardRank rank, CardSuit suit)
    {
        Rank = rank;
        Suit = suit;
    }

    public int BlackjackValue()
    {
        return (int)Rank < 11 ? (int)Rank : (int)Rank == 14 ? 1 : 10;
    }

    public int BaccaratValue()
    {
        return (int)Rank < 10 ? (int)Rank : (int)Rank > 13 ? 1 : 0;
    }

    // Included for testing
    public string Print()
    {
        return Rank.ToString() + " of " + Suit.ToString();
    }
}

public static class CardExtensions
{
    // takes in a deck of cards, removes the first, and returns the removed card
    public static Card DealCard(this List<Card> deck)
    {
        Card card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    // takes in a deck of cards, removes n first cards, and returns the removed cards
    public static List<Card> DealCards(this List<Card> deck, int cardNumber)
    {
        List<Card> cards = deck.Take(cardNumber).ToList();
        deck.RemoveRange(0, cardNumber);
        return cards;
    }
    public static int BaccaratGetHandValue(this List<Card> hand)
    {
        return hand.Sum(c => c.BaccaratValue());
    }

    public static int BlackjackGetHandValue(this List<Card> hand)
    {
        int sum = hand.Sum(c => c.BlackjackValue());
        if (sum > 11 || !hand.Select(c => c.Rank).Contains(CardRank.Ace))
        {
            return sum;
        }
        return sum + 10;
    }

    public static string GetStringValue(this List<Card> hand)
    {
        string val = "";
        foreach (Card c in hand)
        {
            val += c.Rank.ToString() + " of " + c.Suit.ToString() + ", ";
        }
        val = val.Remove(val.Length - 2);

        return val;
    }

    public static List<string> GetImageNames(this List<Card> hand)
    {
        List<string> imageNames = new List<string>();
        foreach (Card c in hand)
        {
            imageNames.Add(SuitToImageName(c.Suit) + RankToImageName(c.Rank));
        }
        return imageNames;
    }

    private static string RankToImageName(CardRank rank)
    {
        string rankString = ((int) rank) + "";
        if (rankString.Length == 1)
        {
            rankString = "0" + rankString;
        }
        return rankString;
    }

    private static string SuitToImageName(CardSuit suit)
    {
        string suitString = "";
        switch (suit)
        {
            case CardSuit.Hearts:
                suitString = "Heart";
                break;
            case CardSuit.Diamonds:
                suitString = "Diamond";
                break;
            case CardSuit.Spades:
                suitString = "Spade";
                break;
            case CardSuit.Clubs:
                suitString = "Club";
                break;
        }
        return suitString;
    }
}

// static class designed to generate a shuffled deck of cards or several shuffled deck of cards
public static class Deck
{
    // static array of ordered cards used to generate chuffled deck(s)
    public static Card[] OrderedDeck = new Card[52]
    {
            new Card(CardRank.Two, CardSuit.Diamonds),
            new Card(CardRank.Two, CardSuit.Clubs),
            new Card(CardRank.Two, CardSuit.Hearts),
            new Card(CardRank.Two, CardSuit.Spades),
            new Card(CardRank.Three, CardSuit.Diamonds),
            new Card(CardRank.Three, CardSuit.Clubs),
            new Card(CardRank.Three, CardSuit.Hearts),
            new Card(CardRank.Three, CardSuit.Spades),
            new Card(CardRank.Four, CardSuit.Diamonds),
            new Card(CardRank.Four, CardSuit.Clubs),
            new Card(CardRank.Four, CardSuit.Hearts),
            new Card(CardRank.Four, CardSuit.Spades),
            new Card(CardRank.Five, CardSuit.Diamonds),
            new Card(CardRank.Five, CardSuit.Clubs),
            new Card(CardRank.Five, CardSuit.Hearts),
            new Card(CardRank.Five, CardSuit.Spades),
            new Card(CardRank.Six, CardSuit.Diamonds),
            new Card(CardRank.Six, CardSuit.Clubs),
            new Card(CardRank.Six, CardSuit.Hearts),
            new Card(CardRank.Six, CardSuit.Spades),
            new Card(CardRank.Seven, CardSuit.Diamonds),
            new Card(CardRank.Seven, CardSuit.Clubs),
            new Card(CardRank.Seven, CardSuit.Hearts),
            new Card(CardRank.Seven, CardSuit.Spades),
            new Card(CardRank.Eight, CardSuit.Diamonds),
            new Card(CardRank.Eight, CardSuit.Clubs),
            new Card(CardRank.Eight, CardSuit.Hearts),
            new Card(CardRank.Eight, CardSuit.Spades),
            new Card(CardRank.Nine, CardSuit.Diamonds),
            new Card(CardRank.Nine, CardSuit.Clubs),
            new Card(CardRank.Nine, CardSuit.Hearts),
            new Card(CardRank.Nine, CardSuit.Spades),
            new Card(CardRank.Ten, CardSuit.Diamonds),
            new Card(CardRank.Ten, CardSuit.Clubs),
            new Card(CardRank.Ten, CardSuit.Hearts),
            new Card(CardRank.Ten, CardSuit.Spades),
            new Card(CardRank.Jack, CardSuit.Diamonds),
            new Card(CardRank.Jack, CardSuit.Clubs),
            new Card(CardRank.Jack, CardSuit.Hearts),
            new Card(CardRank.Jack, CardSuit.Spades),
            new Card(CardRank.Queen, CardSuit.Diamonds),
            new Card(CardRank.Queen, CardSuit.Clubs),
            new Card(CardRank.Queen, CardSuit.Hearts),
            new Card(CardRank.Queen, CardSuit.Spades),
            new Card(CardRank.King, CardSuit.Diamonds),
            new Card(CardRank.King, CardSuit.Clubs),
            new Card(CardRank.King, CardSuit.Hearts),
            new Card(CardRank.King, CardSuit.Spades),
            new Card(CardRank.Ace, CardSuit.Diamonds),
            new Card(CardRank.Ace, CardSuit.Clubs),
            new Card(CardRank.Ace, CardSuit.Hearts),
            new Card(CardRank.Ace, CardSuit.Spades)
    };


    private static int[] ShuffleIntArray(int numberDecks)
    {
        // intialize an ordered array of indices
        int[] array = new int[numberDecks * 52];
        for (int i = 0; i < numberDecks * 52; i++)
        {
            array[i] = i;
        };

        // seed the Random with current current time millisecond value
        var rnd = new Random(DateTime.Now.Millisecond);

        // randomly shuffle array
        for (int i = array.Length; i > 0; i--)
        {
            int j = rnd.Next(i);
            int k = array[j];
            array[j] = array[i - 1];
            array[i - 1] = k;
        }
        return array;
    }


    public static List<Card> GetShuffledDeck(int deckNumber)
    {
        if (deckNumber == 0)
        {
            return null;
        }
        List<Card> deck = new List<Card>();

        int[] randomIndexes = ShuffleIntArray(deckNumber);

        for (int i = 0; i < randomIndexes.Length; i++)
        {
            deck.Add(OrderedDeck[randomIndexes[i] % 52]);
        }
        return deck;
    }

    public static List<Card> GetShuffledDeck()
    {
        return GetShuffledDeck(1);
    }
}
