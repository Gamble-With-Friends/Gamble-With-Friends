using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests    
{
    public class DataManagerTests
    {
        [SetUp]
        public void Init()
        {
            DataManager.isTest = true;
        }
        
        [Test]
        public void DisplayNameExists_True()
        {
            string displayName = "test1234";
            bool exists = DataManager.DisplayNameExists(displayName);
            Assert.False(exists);
        }

        public void DisplayNameExists_False()
        {
            string displayName = "test123456789";
            displayName = "testuser";
            bool exists = DataManager.DisplayNameExists(displayName);
            Assert.True(exists);
        }

        [Test]
        public void LoginUser_IncorrectPassword()
        {
            string displayName = "testuser";
            string password = "1234";
            var loggedIn = DataManager.GetUser(displayName, password);
            Assert.IsNull(loggedIn);
        }

        [Test]
        public void LoginUser_UserNameNotExists()
        {
            string displayName = "testuser123456789";
            string password = "testing123!";
            var loggedIn = DataManager.GetUser(displayName, password);
            Assert.IsNull(loggedIn);
        }

        [Test]
        public void LoginUser_Successful()
        {
            string displayName = "testuser";
            string password = "testing123";
            var loggedIn = DataManager.GetUser(displayName, password);
            Assert.IsNotNull(loggedIn);
        }
        
        [Test]
        public void AddUser_UsernameExists()
        {
            string displayName = "testuser";
            string password = "testing123";
            DataManager.AddUser("test@user.com", displayName, password);
        }

        [Test]
        public void AddUser_UsernameDoesntExists()
        {
            string displayName = "testuser232";
            string password = "testing123";
            DataManager.AddUser("test@user.com", displayName, password);
        }

        [Test]
        public void ChangeCoinValue_Test()
        {
            var script = DataManager.GetUser("testuser", "testing123");
            var originalAmount = script.Coins;
            DataManager.ChangeCoinValue(script.UserId, 1000);
            script = DataManager.GetUser("testuser", "testing123");
            Assert.AreNotEqual(originalAmount,script.Coins);
        }

        [Test]
        public void BuySellItem_Test()
        {
            var script = DataManager.GetUser("testuser", "testing123");
            UserInfo.GetInstance().UserId = script.UserId;
            DataManager.GetItems();
            InventoryItems.itemIdToRecord = DataManager.GetInventoryItems(script.UserId);
            DataManager.Buyitem(script.UserId, "Hat");
            DataManager.GetItems();
            InventoryItems.itemIdToRecord = DataManager.GetInventoryItems(script.UserId);
            DataManager.SellItem(script.UserId, InventoryItems.itemIdToRecord["a0a76ca5-7e32-480e-8d81-45ab47abfe2c"].ItemId);
        }

        [Test]
        public void EquipItem_Test()
        {
            var script = DataManager.GetUser("testuser", "testing123");
            UserInfo.GetInstance().UserId = script.UserId;
            DataManager.GetItems();
            InventoryItems.itemIdToRecord = DataManager.GetInventoryItems(script.UserId);
            DataManager.Buyitem(script.UserId, "Hat");
            DataManager.EquiptItem(script.UserId,"Hat");
            DataManager.GetItems();
            InventoryItems.itemIdToRecord = DataManager.GetInventoryItems(script.UserId);
            DataManager.SellItem(script.UserId, InventoryItems.itemIdToRecord["a0a76ca5-7e32-480e-8d81-45ab47abfe2c"].ItemId);
        }
        
        [Test]
        public void UnequipItem_Test()
        {
            var script = DataManager.GetUser("testuser", "testing123");
            UserInfo.GetInstance().UserId = script.UserId;
            DataManager.GetItems();
            InventoryItems.itemIdToRecord = DataManager.GetInventoryItems(script.UserId);
            DataManager.Buyitem(script.UserId, "Hat");
            DataManager.UnequiptItem(script.UserId,"Hat");
            DataManager.GetItems();
            InventoryItems.itemIdToRecord = DataManager.GetInventoryItems(script.UserId);
            DataManager.SellItem(script.UserId, InventoryItems.itemIdToRecord["a0a76ca5-7e32-480e-8d81-45ab47abfe2c"].ItemId);
        }
        
        [Test]
        public void GetItem_Test()
        {
            DataManager.GetItems();
        }
        
        [Test]
        public void GetInventoryItems_Test()
        {
            var script = DataManager.GetUser("testuser", "testing123");
            DataManager.GetInventoryItems(script.UserId);
        }
        
               
        [Test]
        public void GetFriends_Test()
        {
            var script = DataManager.GetUser("testuser", "testing123");
            DataManager.GetFriends(script.UserId);
        }

        [Test]
        public void GetFunds_Test()
        {
            var script = DataManager.GetUser("testuser", "testing123");
            DataManager.GetUserFunds(script.UserId, out decimal d);
            Assert.NotZero(d);
        }

        [Test]
        public void AddCoinsByDisplayName_Test()
        {
            var script = DataManager.GetUser("testuser", "testing123");
            DataManager.AddCoinsByDisplayName("testuser",1000);
            var newScript = DataManager.GetUser("testuser", "testing123");
            Assert.AreNotEqual(script.Coins,newScript.Coins);
        }
        
        [Test]
        public void FindBySearchString_Test()
        {
            var script = DataManager.GetUser("testuser", "testing123");
            DataManager.AddCoinsByDisplayName("testuser",1000);
            var users = DataManager.FindBySearchString("testuser", script.UserId);
            Assert.NotZero(users.Count);
        }
        
        [Test]
        public void IsFriend_Test()
        {
            var script = DataManager.GetUser("testuser", "testing123");
            DataManager.AddCoinsByDisplayName("testuser",1000);
            var isFriend = DataManager.IsFriend(script.UserId, "harout");
            Assert.False(isFriend);
        }
        
        [Test]
        public void FriendRequestAlreadySent_Test()
        {
            var script = DataManager.GetUser("testuser", "testing123");
            DataManager.AddCoinsByDisplayName("testuser",1000);
            var isFriend = DataManager.FriendRequestAlreadySent(script.UserId, "harout");
            Assert.False(isFriend);
        }
        
        [Test]
        public void SendFriendRequest_Test()
        {
            var script = DataManager.GetUser("testuser", "testing123");
            DataManager.SendFriendRequest(script.UserId,"tony");
        }
        
        [Test]
        public void GetPendingFriendRequests_Test()
        {
            var script = DataManager.GetUser("testuser", "testing123");
            DataManager.SendFriendRequest(script.UserId,"tony");
        }
        
        [Test]
        public void AcceptFriendRequest_Test()
        {
            var script = DataManager.GetUser("testuser", "testing123");
            DataManager.AcceptFriendRequest(script.UserId,"tony");
        }
        
        [Test]
        public void RemoveFriendRequest_Test()
        {
            var script = DataManager.GetUser("testuser", "testing123");
            DataManager.RemoveFriendRequest(script.UserId,"tony");
        }
        
        [Test]
        public void CreateLoginSession_Test()
        {
            var script = DataManager.GetUser("testuser", "testing123");
            DataManager.CreateLoginSession(1234,"TEST",script.UserId);
        }
        
        [Test]
        public void UpdateLogoutTime_Test()
        {
            var script = DataManager.GetUser("testuser", "testing123");
            DataManager.UpdateLogoutTime(1234,"TEST");
        }
    }
}