using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests    
{
    public class DataManagerTests
    {
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
    }
}