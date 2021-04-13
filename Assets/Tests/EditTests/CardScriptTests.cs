using System.Collections.Generic;
using NUnit.Framework;

namespace Tests    
{
    public class CardScriptTests
    {
        [Test]
        public void PokerValue_ReturnsFourteen()
        {
            Card card = new Card(CardRank.Ace, CardSuit.Hearts);
            var value = card.PokerValue();
            Assert.AreEqual(value, 14);
        }

        [Test]
        public void PokerValue_ReturnsTwo()
        {
            Card card = new Card(CardRank.Two, CardSuit.Hearts);
            var value = card.PokerValue();
            Assert.AreEqual(value, 2);
        }

        [Test]
        public void GetStringValue_ReturnsCardValuesAsString()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Two, CardSuit.Hearts));
            hand.Add(new Card(CardRank.King, CardSuit.Diamonds));
            var stringValue = hand.GetStringValue();
            Assert.AreEqual(stringValue, "Two of Hearts, King of Diamonds");
        }

        [Test]
        public void SortHandByValue_ReturnsSortedHand()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Two, CardSuit.Hearts));
            hand.Add(new Card(CardRank.King, CardSuit.Diamonds));
            hand.Add(new Card(CardRank.Four, CardSuit.Spades));
            hand.Add(new Card(CardRank.Queen, CardSuit.Clubs));
            hand.Add(new Card(CardRank.Nine, CardSuit.Diamonds));

            var sortedHand = hand.SortHandByValue();

            Assert.AreEqual(sortedHand[0].Rank, CardRank.Two);
            Assert.AreEqual(sortedHand[1].Rank, CardRank.Four);
            Assert.AreEqual(sortedHand[2].Rank, CardRank.Nine);
            Assert.AreEqual(sortedHand[3].Rank, CardRank.Queen);
            Assert.AreEqual(sortedHand[4].Rank, CardRank.King);
        }

        [Test]
        public void HasPair_ReturnsTrue()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Two, CardSuit.Hearts));
            hand.Add(new Card(CardRank.King, CardSuit.Diamonds));
            hand.Add(new Card(CardRank.Four, CardSuit.Spades));
            hand.Add(new Card(CardRank.King, CardSuit.Clubs));
            hand.Add(new Card(CardRank.Nine, CardSuit.Diamonds));

            var hasPair = hand.HasPair();

            Assert.True(hasPair);
        }

        [Test]
        public void HasPair_ReturnsFalse()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Two, CardSuit.Hearts));
            hand.Add(new Card(CardRank.King, CardSuit.Diamonds));
            hand.Add(new Card(CardRank.Four, CardSuit.Spades));
            hand.Add(new Card(CardRank.Jack, CardSuit.Clubs));
            hand.Add(new Card(CardRank.Nine, CardSuit.Diamonds));

            var isPair = hand.IsPair();

            Assert.False(isPair);
        }

        [Test]
        public void IsTwoPair_ReturnsFalse()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Two, CardSuit.Hearts));
            hand.Add(new Card(CardRank.King, CardSuit.Diamonds));
            hand.Add(new Card(CardRank.Two, CardSuit.Spades));
            hand.Add(new Card(CardRank.Jack, CardSuit.Clubs));
            hand.Add(new Card(CardRank.Nine, CardSuit.Diamonds));

            var isTwoPair = hand.IsTwoPair();

            Assert.False(isTwoPair);
        }

        [Test]
        public void IsTwoPair_ReturnsTrue()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Two, CardSuit.Hearts));
            hand.Add(new Card(CardRank.King, CardSuit.Diamonds));
            hand.Add(new Card(CardRank.Two, CardSuit.Spades));
            hand.Add(new Card(CardRank.King, CardSuit.Clubs));
            hand.Add(new Card(CardRank.Nine, CardSuit.Diamonds));

            var isTwoPair = hand.IsTwoPair();

            Assert.True(isTwoPair);
        }

        [Test]
        public void IsStraight_ReturnsTrue()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Two, CardSuit.Hearts));
            hand.Add(new Card(CardRank.Five, CardSuit.Diamonds));
            hand.Add(new Card(CardRank.Three, CardSuit.Spades));
            hand.Add(new Card(CardRank.Six, CardSuit.Clubs));
            hand.Add(new Card(CardRank.Four, CardSuit.Diamonds));

            var isStraight = hand.IsStraight();

            Assert.True(isStraight);
        }

        [Test]
        public void IsStraight_ReturnsFalse()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Two, CardSuit.Hearts));
            hand.Add(new Card(CardRank.King, CardSuit.Diamonds));
            hand.Add(new Card(CardRank.Four, CardSuit.Spades));
            hand.Add(new Card(CardRank.Queen, CardSuit.Clubs));
            hand.Add(new Card(CardRank.Nine, CardSuit.Diamonds));

            var isStraight = hand.IsStraight();

            Assert.False(isStraight);
        }

        [Test]
        public void IsThreeOfKind_ReturnsTrue()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Two, CardSuit.Hearts));
            hand.Add(new Card(CardRank.King, CardSuit.Diamonds));
            hand.Add(new Card(CardRank.Four, CardSuit.Spades));
            hand.Add(new Card(CardRank.Two, CardSuit.Clubs));
            hand.Add(new Card(CardRank.Two, CardSuit.Diamonds));

            var isThreeOfKind = hand.IsThreeOfKind();

            Assert.True(isThreeOfKind);
        }

        [Test]
        public void IsThreeOfKind_ReturnsFalse()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Two, CardSuit.Hearts));
            hand.Add(new Card(CardRank.King, CardSuit.Diamonds));
            hand.Add(new Card(CardRank.Two, CardSuit.Spades));
            hand.Add(new Card(CardRank.Queen, CardSuit.Clubs));
            hand.Add(new Card(CardRank.Nine, CardSuit.Diamonds));

            var isThreeOfKind = hand.IsThreeOfKind();

            Assert.False(isThreeOfKind);
        }

        [Test]
        public void IsFlush_ReturnsFalse()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Two, CardSuit.Diamonds));
            hand.Add(new Card(CardRank.King, CardSuit.Diamonds));
            hand.Add(new Card(CardRank.Two, CardSuit.Diamonds));
            hand.Add(new Card(CardRank.Queen, CardSuit.Clubs));
            hand.Add(new Card(CardRank.Nine, CardSuit.Diamonds));

            var isFlush = hand.IsFlush();

            Assert.False(isFlush);
        }

        [Test]
        public void IsFlush_ReturnsTrue()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Two, CardSuit.Hearts));
            hand.Add(new Card(CardRank.King, CardSuit.Hearts));
            hand.Add(new Card(CardRank.Three, CardSuit.Hearts));
            hand.Add(new Card(CardRank.Queen, CardSuit.Hearts));
            hand.Add(new Card(CardRank.Nine, CardSuit.Hearts));

            var isFlush = hand.IsFlush();

            Assert.True(isFlush);
        }

        [Test]
        public void IsFourOfKind_ReturnsFalse()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Two, CardSuit.Hearts));
            hand.Add(new Card(CardRank.King, CardSuit.Diamonds));
            hand.Add(new Card(CardRank.Two, CardSuit.Spades));
            hand.Add(new Card(CardRank.Queen, CardSuit.Clubs));
            hand.Add(new Card(CardRank.Two, CardSuit.Diamonds));

            var isFourOfKind = hand.IsFourOfKind();

            Assert.False(isFourOfKind);
        }

        [Test]
        public void IsFourOfKind_ReturnsTrue()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Nine, CardSuit.Clubs));
            hand.Add(new Card(CardRank.King, CardSuit.Hearts));
            hand.Add(new Card(CardRank.Nine, CardSuit.Diamonds));
            hand.Add(new Card(CardRank.Nine, CardSuit.Hearts));
            hand.Add(new Card(CardRank.Nine, CardSuit.Spades));

            var isFourOfKind = hand.IsFourOfKind();

            Assert.True(isFourOfKind);
        }

        [Test]
        public void IsFullHouse_ReturnsTrue()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Nine, CardSuit.Clubs));
            hand.Add(new Card(CardRank.King, CardSuit.Hearts));
            hand.Add(new Card(CardRank.Nine, CardSuit.Diamonds));
            hand.Add(new Card(CardRank.Nine, CardSuit.Hearts));
            hand.Add(new Card(CardRank.King, CardSuit.Spades));

            var isFullHouse = hand.IsFullHouse();

            Assert.True(isFullHouse);
        }

        [Test]
        public void IsFullHouse_ReturnsFalse()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Nine, CardSuit.Clubs));
            hand.Add(new Card(CardRank.King, CardSuit.Hearts));
            hand.Add(new Card(CardRank.Four, CardSuit.Diamonds));
            hand.Add(new Card(CardRank.Nine, CardSuit.Hearts));
            hand.Add(new Card(CardRank.King, CardSuit.Spades));

            var isFullHouse = hand.IsFullHouse();

            Assert.False(isFullHouse);
        }

        [Test]
        public void IsRoyalFlush_ReturnsFalse()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Nine, CardSuit.Clubs));
            hand.Add(new Card(CardRank.King, CardSuit.Hearts));
            hand.Add(new Card(CardRank.Four, CardSuit.Diamonds));
            hand.Add(new Card(CardRank.Nine, CardSuit.Hearts));
            hand.Add(new Card(CardRank.King, CardSuit.Spades));

            var isRoyalFlush = hand.IsRoyalFlush();

            Assert.False(isRoyalFlush);
        }

        [Test]
        public void IsRoyalFlush_ReturnsTrue()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Jack, CardSuit.Clubs));
            hand.Add(new Card(CardRank.King, CardSuit.Clubs));
            hand.Add(new Card(CardRank.Ten, CardSuit.Clubs));
            hand.Add(new Card(CardRank.Queen, CardSuit.Clubs));
            hand.Add(new Card(CardRank.Ace, CardSuit.Clubs));

            var isRoyalFlush = hand.IsRoyalFlush();

            Assert.False(isRoyalFlush);
        }

        [Test]
        public void IsStraightFlush_ReturnsTrue()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Jack, CardSuit.Hearts));
            hand.Add(new Card(CardRank.Eight, CardSuit.Hearts));
            hand.Add(new Card(CardRank.Ten, CardSuit.Hearts));
            hand.Add(new Card(CardRank.Seven, CardSuit.Hearts));
            hand.Add(new Card(CardRank.Nine, CardSuit.Hearts));

            var isStraightFlush = hand.IsStraightFlush();

            Assert.True(isStraightFlush);
        }

        [Test]
        public void IsStraightFlush_ReturnsFalse()
        {
            List<Card> hand = new List<Card>();
            hand.Add(new Card(CardRank.Jack, CardSuit.Spades));
            hand.Add(new Card(CardRank.Eight, CardSuit.Hearts));
            hand.Add(new Card(CardRank.Ten, CardSuit.Hearts));
            hand.Add(new Card(CardRank.Seven, CardSuit.Hearts));
            hand.Add(new Card(CardRank.Nine, CardSuit.Hearts));

            var isStraightFlush = hand.IsStraightFlush();

            Assert.False(isStraightFlush);
        }

        [Test]
        public void GetEvaluatedHand_ReturnsHandValue()
        {
            List<Card> cards = new List<Card>();
            cards.Add(new Card(CardRank.Ten, CardSuit.Spades));
            cards.Add(new Card(CardRank.Eight, CardSuit.Hearts));
            cards.Add(new Card(CardRank.Four, CardSuit.Hearts));
            cards.Add(new Card(CardRank.Seven, CardSuit.Diamonds));
            cards.Add(new Card(CardRank.Four, CardSuit.Clubs));

            Hand hand = cards.GetEvaluatedHand();

            Assert.AreEqual(hand.PokerHand, PokerHand.OnePair);
            Assert.AreEqual(hand.SortedHand[4].Rank, CardRank.Ten);
        }

        [Test]
        public void CompareHands_ReturnsMinusOne()
        {
            List<Card> first = new List<Card>();
            first.Add(new Card(CardRank.Ten, CardSuit.Spades));
            first.Add(new Card(CardRank.Eight, CardSuit.Hearts));
            first.Add(new Card(CardRank.Four, CardSuit.Hearts));
            first.Add(new Card(CardRank.Seven, CardSuit.Diamonds));
            first.Add(new Card(CardRank.Four, CardSuit.Clubs));

            var firstHand = first.GetEvaluatedHand();

            List<Card> second = new List<Card>();
            second.Add(new Card(CardRank.Ten, CardSuit.Spades));
            second.Add(new Card(CardRank.Seven, CardSuit.Hearts));
            second.Add(new Card(CardRank.Four, CardSuit.Hearts));
            second.Add(new Card(CardRank.Seven, CardSuit.Diamonds));
            second.Add(new Card(CardRank.Four, CardSuit.Clubs));

            var secondHand = second.GetEvaluatedHand();

            var result = Deck.CompareHands(firstHand, secondHand);

            Assert.AreEqual(result, -1);
        }

        [Test]
        public void CompareHands_ReturnsOne()
        {
            // high pair, queen
            List<Card> first = new List<Card>();
            first.Add(new Card(CardRank.Ten, CardSuit.Spades));
            first.Add(new Card(CardRank.Eight, CardSuit.Hearts));
            first.Add(new Card(CardRank.Queen, CardSuit.Hearts));
            first.Add(new Card(CardRank.Seven, CardSuit.Diamonds));
            first.Add(new Card(CardRank.Queen, CardSuit.Clubs));

            var firstHand = first.GetEvaluatedHand();

            List<Card> second = new List<Card>();
            second.Add(new Card(CardRank.Ten, CardSuit.Spades));
            second.Add(new Card(CardRank.Seven, CardSuit.Hearts));
            second.Add(new Card(CardRank.Jack, CardSuit.Hearts));
            second.Add(new Card(CardRank.Seven, CardSuit.Diamonds));
            second.Add(new Card(CardRank.Two, CardSuit.Clubs));

            var secondHand = second.GetEvaluatedHand();

            var result = Deck.CompareHands(firstHand, secondHand);

            Assert.AreEqual(result, 1);
        }

        [Test]
        public void CompareHands_ReturnsZero()
        {
            List<Card> first = new List<Card>();
            first.Add(new Card(CardRank.Ten, CardSuit.Spades));
            first.Add(new Card(CardRank.Eight, CardSuit.Hearts));
            first.Add(new Card(CardRank.Queen, CardSuit.Hearts));
            first.Add(new Card(CardRank.Seven, CardSuit.Diamonds));
            first.Add(new Card(CardRank.Ace, CardSuit.Clubs));

            var firstHand = first.GetEvaluatedHand();

            List<Card> second = new List<Card>();
            second.Add(new Card(CardRank.Eight, CardSuit.Hearts));
            second.Add(new Card(CardRank.Queen, CardSuit.Hearts));
            second.Add(new Card(CardRank.Ace, CardSuit.Hearts));
            second.Add(new Card(CardRank.Ten, CardSuit.Diamonds));
            second.Add(new Card(CardRank.Seven, CardSuit.Clubs));

            var secondHand = second.GetEvaluatedHand();

            var result = Deck.CompareHands(firstHand, secondHand);

            Assert.AreEqual(result, 0);
        }
    }
}