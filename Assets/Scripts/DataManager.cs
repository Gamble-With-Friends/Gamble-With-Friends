using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.Data.SqlClient;
using System;

public class DataManager
{
    private const string SERVER_IP = "216.58.1.35";
    private const string DATABASE = "GambleWithFriends";
    private const string USERNAME = "Anthony";
    private const string PASSWORD = "password123";

    private const string ConnectionString = @"Data Source = " + SERVER_IP + ";Database=" + DATABASE + ";User Id=" +
                                            USERNAME + ";Password=" + PASSWORD + ";";
    
    

    public static void AddUser(string email, string displayName, string password)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("INSERT Users VALUES (@userId, @email, @displayName, @password, 1000.00)", db);

            var userIdParam = new SqlParameter();
            userIdParam.ParameterName = "@userId";
            userIdParam.Value = Guid.NewGuid().ToString();
            cmd.Parameters.Add(userIdParam);

            var emailParam = new SqlParameter();
            emailParam.ParameterName = "@email";
            emailParam.Value = email;
            cmd.Parameters.Add(emailParam);

            var userNameParam = new SqlParameter();
            userNameParam.ParameterName = "@displayName";
            userNameParam.Value = displayName;
            cmd.Parameters.Add(userNameParam);

            var passwordParam = new SqlParameter();
            passwordParam.ParameterName = "@password";
            passwordParam.Value = password;
            cmd.Parameters.Add(passwordParam);

            db.Open();
            cmd.ExecuteNonQuery();
            db.Close();
        }
    }

    public static bool DisplayNameExists(string displayName)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand(
                "SELECT * FROM Users WHERE displayName=@displayName COLLATE SQL_Latin1_General_CP1_CS_AS", db);

            var param = new SqlParameter {ParameterName = "@displayName", Value = displayName};
            cmd.Parameters.Add(param);

            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                reader.Close();
                return true;
            }
            reader.Close();
            return false;
        }
    }

    public static PlayerModelScript GetUser(string displayName, string password)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand(
                "SELECT userId, displayName, coins FROM Users WHERE displayName=@displayName COLLATE SQL_Latin1_General_CP1_CS_AS AND password=@password COLLATE SQL_Latin1_General_CP1_CS_AS",
                db);

            var userNameParam = new SqlParameter();
            userNameParam.ParameterName = "@displayName";
            userNameParam.Value = displayName;
            cmd.Parameters.Add(userNameParam);

            var passwordParam = new SqlParameter();
            passwordParam.ParameterName = "@password";
            passwordParam.Value = password;
            cmd.Parameters.Add(passwordParam);

            db.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var userId = reader.GetString(0);
                var player = new PlayerModelScript
                {
                    UserId = userId,
                    UserName = reader.GetString(1),
                    Coins = reader.GetDecimal(2)
                };
                reader.Close();
                return player;
            }
            reader.Close();
            return null;
        }
    }

    public static void ChangeCoinValue(string userId, decimal amount)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("SELECT coins FROM Users WHERE userId=@userId", db);
            cmd.Parameters.AddWithValue("@userId", userId);
            db.Open();
            var reader = cmd.ExecuteReader();
            decimal currentAmount = 0;
            
            while (reader.Read())
            { 
                currentAmount = reader.GetDecimal(0);
                if (currentAmount + amount < 0) throw new ArgumentException("Funds not available");
            }
            reader.Close();
            
            cmd = new SqlCommand("UPDATE Users SET coins = @coins Where userId=@userId", db);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@coins", currentAmount + amount);
            cmd.ExecuteNonQuery();
        }
    }

    public static void Buyitem(string userId, string itemName)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("INSERT Inventory VALUES (@userId, @itemId, @equipped, @purchaseDate, @payouts)", db);

            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@itemId", GameItems.GetItems()[itemName].ItemId);
            cmd.Parameters.AddWithValue("@equipped", 0);
            cmd.Parameters.AddWithValue("@purchaseDate", DateTime.Now);
            cmd.Parameters.AddWithValue("@payouts", 0);

            db.Open();
            cmd.ExecuteNonQuery();
        }

        InventoryItems.UpdateItems();
    }

    public static void SellItem(string userId, string itemId)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("Delete From Inventory where playerId = @userId and itemId = @itemId", db);

            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@itemId", itemId);

            db.Open();
            cmd.ExecuteNonQuery();
        }
        InventoryItems.UpdateItems();
    }

    public static void EquiptItem(string userId, string itemName)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("UPDATE Inventory set equipped = 1 where playerId = @userId and itemId = @itemId", db);

            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@itemId", GameItems.GetItems()[itemName].ItemId);

            db.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public static void UnequiptItem(string userId, string itemName)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("UPDATE Inventory set equipped = 0 where playerId = @userId and itemId = @itemId", db);

            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@itemId", GameItems.GetItems()[itemName].ItemId);

            db.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public static void GetItems()
    {

        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("SELECT itemId, itemTitle, itemType, coinValue, incomeAmount FROM Item", db);

            db.Open();
            var reader = cmd.ExecuteReader();

            GameItems.itemNameToRecord = new Dictionary<string, GameItems.Item>();

            while (reader.Read())
            {
                GameItems.itemNameToRecord.Add(reader.GetString(1),new GameItems.Item {
                    ItemId =  reader.GetString(0),
                    ItemTitle = reader.GetString(1),
                    ItemType = reader.GetInt32(2),
                    CoinValue = reader.GetDecimal(3),
                    IncomeAmount = reader.GetDecimal(4),
                });
            }
            reader.Close();
        }
    }

    public static Dictionary<string, InventoryItems.InventoryItem> GetInventoryItems(string userId)
    {

        var itemIdToInventoryItem = new Dictionary<string, InventoryItems.InventoryItem>();
            
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("SELECT itemId, equipped, purchaseDate, payouts FROM Inventory where playerId = @userId", db);

            cmd.Parameters.AddWithValue("@userId", userId);

            db.Open();
            var reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                itemIdToInventoryItem.Add(reader.GetString(0), new InventoryItems.InventoryItem
                {
                    ItemId = reader.GetString(0),
                    Equipped = reader.GetBoolean(1),
                    PurchaseDate = reader.GetDateTime(2),
                    Payouts = reader.GetInt32(3)
                });
            }
            reader.Close();
        }

        return itemIdToInventoryItem;
    }

    public static List<string> GetFriends(string playerId)
    {
        var friendList = new List<string>();

        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand(
                "SELECT Users.displayName FROM Friends JOIN Users ON Friends.friendID = Users.userId WHERE Friends.userId = @userId AND Friends.status=1;", db);

            var param = new SqlParameter { ParameterName = "@userId", Value = playerId };
            cmd.Parameters.Add(param);

            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                friendList.Add(reader.GetString(0));
            }
            reader.Close();
        }

        return friendList;
    }

    public static void UnfriendUser(string localUserId, string friendUserId)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("DELETE FROM Friends WHERE (userId=@userId AND friendId=@friendUserId) OR (userId=@friendUserId AND friendId=@userId)", db);
            cmd.Parameters.AddWithValue("@userId", localUserId);
            cmd.Parameters.AddWithValue("@friendUserId", friendUserId);

            db.Open();
            cmd.ExecuteNonQuery();

            db.Close();
        }
    }

    public static bool GetUserFunds(string playerId, out decimal playerFunds)
    {
        playerFunds = 0M;
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("SELECT coins FROM Users WHERE userId = @userId", db);

            var param = new SqlParameter { ParameterName = "@userId", Value = playerId };
            cmd.Parameters.Add(param);

            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                playerFunds = reader.GetDecimal(0);
                reader.Close();
                db.Close();
                return true;
            }
            reader.Close();
            db.Close();
            return false;
        }
    }

    public static void AddCoinsByDisplayName(string displayName, decimal amountToAdd)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("SELECT userId, coins FROM Users WHERE displayName = @displayName", db);

            var param = new SqlParameter { ParameterName = "@displayName", Value = displayName };
            cmd.Parameters.Add(param);

            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            string userId = string.Empty;
            decimal currentAmount = 0M;
            while (reader.Read())
            {
                userId = reader.GetString(0);
                currentAmount = reader.GetDecimal(1);
                reader.Close();
                break;
            }

            cmd = new SqlCommand("UPDATE Users SET coins = @coins Where userId=@userId", db);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@coins", currentAmount + amountToAdd);
            cmd.ExecuteNonQuery();

            db.Close();
        }
    }

    public static List<string> FindBySearchString(string searchString, string currentUser)
    {
        List<string> foundUsers = new List<string>();

        using (var db = new SqlConnection(ConnectionString))
        {
            string strSQL = "SELECT displayName FROM Users WHERE displayName LIKE '%" + searchString + "%'";
            var cmd = new SqlCommand(strSQL, db);

            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                foundUsers.Add(reader.GetString(0));
            }

            db.Close();
        }
        if (foundUsers.Contains(currentUser))
        {
            foundUsers.Remove(currentUser);
        }

        return foundUsers;
    }
    
    public static bool IsFriend(string userId, string friendName)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand(
                "SELECT f.userId FROM Friends f JOIN Users u ON f.userId = u.userId WHERE friendId=@userId AND u.displayName=@friendName AND f.status=1", db);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@friendName", friendName);

            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                return true;
            }

            db.Close();
        }
        return false;
    }

    public static bool FriendRequestAlreadySent(string userId, string friendName)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand(
                "SELECT f.userId FROM Friends f JOIN Users u ON f.userId = u.userId WHERE friendId=@userId AND u.displayName=@friendName AND f.status=0", db);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@friendName", friendName);

            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                return true;
            }

            db.Close();
        }
        return false;
    }

    public static void SendFriendRequest(string senderId, string recipientUsername)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            string recipientId = string.Empty;
            var cmd = new SqlCommand("SELECT userId FROM Users WHERE displayName=@recipientUsername", db);
            cmd.Parameters.AddWithValue("@recipientUsername", recipientUsername);

            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                recipientId = reader.GetString(0);
                break;
            }
            reader.Close();

            cmd = new SqlCommand("INSERT Friends VALUES (@recipientId, @senderId, 0)", db);            
            cmd.Parameters.AddWithValue("@senderId", senderId);
            cmd.Parameters.AddWithValue("@recipientId", recipientId);
            cmd.ExecuteNonQuery();

            db.Close();
        }
    }

    public static List<string> GetPendingFriendRequests(string userId)
    {
        List<string> pendingRequest = new List<string>();

        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("SELECT displayName FROM Users u JOIN Friends f ON u.userId = f.friendId WHERE f.userId=@userId AND f.status=0;", db);
            cmd.Parameters.AddWithValue("@userId", userId);

            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                pendingRequest.Add(reader.GetString(0));
                break;
            }
            reader.Close();
            db.Close();
        }
        return pendingRequest;
    }

    public static void AcceptFriendRequest(string currentUserId, string otherUserDisplayName)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            string otherUserId = string.Empty;
            var cmd = new SqlCommand("SELECT userId FROM Users WHERE displayName=@otherUserDisplayName", db);
            cmd.Parameters.AddWithValue("@otherUserDisplayName", otherUserDisplayName);

            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                otherUserId = reader.GetString(0);
                break;
            }
            reader.Close();

            cmd = new SqlCommand("UPDATE Friends SET status = 1 WHERE userId=@userId AND friendId=@otherUserId", db);
            cmd.Parameters.AddWithValue("@userId", currentUserId);
            cmd.Parameters.AddWithValue("@otherUserId", otherUserId);
            cmd.ExecuteNonQuery();

            cmd = new SqlCommand("INSERT Friends VALUES (@userId, @friendId, 1)", db);
            cmd.Parameters.AddWithValue("@userId", otherUserId);
            cmd.Parameters.AddWithValue("@friendId", currentUserId);
            cmd.ExecuteNonQuery();

            db.Close();
        }
    }

    public static void RemoveFriendRequest(string currentUserId, string otherUserDisplayName)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            string otherUserId = string.Empty;
            var cmd = new SqlCommand("SELECT userId FROM Users WHERE displayName=@otherUserDisplayName", db);
            cmd.Parameters.AddWithValue("@otherUserDisplayName", otherUserDisplayName);

            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                otherUserId = reader.GetString(0);
                break;
            }
            reader.Close();

            cmd = new SqlCommand("DELETE FROM Friends WHERE userId=@userId AND friendId=@otherUserId", db);
            cmd.Parameters.AddWithValue("@userId", currentUserId);
            cmd.Parameters.AddWithValue("@otherUserId", otherUserId);
            cmd.ExecuteNonQuery();

            db.Close();
        }
    }

    public static void CreateServer(string serverId)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("INSERT Server VALUES (@serverId)", db);
            cmd.Parameters.AddWithValue("@serverId",serverId);
            db.Open();
            cmd.ExecuteNonQuery();
            db.Close();
        }
    }
    
    public static void CreateLoginSession(int connectionId, string serverId, string userId)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("INSERT LoginSession (loginSessionId, playerId, loginDateTime, serverId) VALUES (@loginSessionId, @playerId, @loginDateTime, @serverId)", db);
            cmd.Parameters.AddWithValue("@serverId",serverId);
            cmd.Parameters.AddWithValue("@loginSessionId", connectionId);
            cmd.Parameters.AddWithValue("@playerId",userId);
            cmd.Parameters.AddWithValue("@loginDateTime", DateTime.Now);
            db.Open();
            cmd.ExecuteNonQuery();
            db.Close();
        }
    }
    
    public static void UpdateLogoutTime(int connectionId, string serverId)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("UPDATE LoginSession SET logoutDateTime = @logoutDateTime WHERE serverId = @serverId AND loginSessionId = @loginSessionId", db);
            cmd.Parameters.AddWithValue("@logoutDateTime",DateTime.Now);
            cmd.Parameters.AddWithValue("@loginSessionId", connectionId);
            cmd.Parameters.AddWithValue("@serverId",serverId);
            db.Open();
            cmd.ExecuteNonQuery();
            db.Close();
        }
    }

    public static bool IsPlayerLoggedIn(string userId, string serverId)
    {
        var isLoggedIn = false;
        
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("SELECT loginSessionId FROM LoginSession WHERE serverId = @serverId AND playerId = @playerId AND logoutDateTime IS NULL", db);
            cmd.Parameters.AddWithValue("@playerId", userId);
            cmd.Parameters.AddWithValue("@serverId", serverId);
            db.Open();
            var reader = cmd.ExecuteReader();
            isLoggedIn = reader.HasRows;
            reader.Close();
            db.Close();
        }

        return isLoggedIn;
    }

    public static void CreateGameSession(int connectionId, string serverId, string userId, string gameId)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("INSERT GameSession (gameSessionId, playerId, startTime, serverId, gameId, connectionId, result) VALUES (@gameSessionId, @playerId, @startTime, @serverId, @gameId, @connectionId, @result)", db);
            cmd.Parameters.AddWithValue("@serverId", serverId);
            cmd.Parameters.AddWithValue("@gameSessionId", Guid.NewGuid().ToString());
            cmd.Parameters.AddWithValue("@playerId", userId);
            cmd.Parameters.AddWithValue("@startTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@gameId", gameId);
            cmd.Parameters.AddWithValue("@result", UserInfo.GetInstance().TotalCoins);
            cmd.Parameters.AddWithValue("@connectionId", connectionId);
            db.Open();
            cmd.ExecuteNonQuery();
            db.Close();
        }
    }

    private static decimal GetGameSessionResult(int connectionId, string serverId, string gameId)
    {
        decimal result = 0.00m;

        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("SELECT Top 1 result FROM GameSession WHERE serverId = @serverId AND connectionId = @connectionId AND gameId=@gameId AND endTime IS NULL ORDER BY startTime Desc", db);
            cmd.Parameters.AddWithValue("@connectionId", connectionId);
            cmd.Parameters.AddWithValue("@serverId", serverId);
            cmd.Parameters.AddWithValue("@gameId", gameId);
            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result = reader.GetDecimal(0);
                break;
            }
            reader.Close();
            db.Close();
        }

        return result;
    }

    public static void UpdateGameSessionTime(int connectionId, string serverId, string gameId)
    {
        var totalCoins = GetGameSessionResult(connectionId, serverId, gameId);

        using (var db = new SqlConnection(ConnectionString))
        {
            string gameSessionId = "";
            var cmd = new SqlCommand("SELECT Top 1 gameSessionId from GameSession WHERE serverId = @serverId AND connectionId = @connectionId AND gameId=@gameId AND endTime IS NULL ORDER BY startTime Desc", db);

            cmd.Parameters.AddWithValue("@connectionId", connectionId);
            cmd.Parameters.AddWithValue("@serverId", serverId);
            cmd.Parameters.AddWithValue("@gameId", gameId);

            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                gameSessionId = reader.GetString(0);
                break;
            }
            reader.Close();

            cmd = new SqlCommand("UPDATE GameSession SET endTime = @endTime, result = @result WHERE gameSessionId = @gameSessionId", db);
            cmd.Parameters.AddWithValue("@endTime", DateTime.Now);
            cmd.Parameters.AddWithValue("@result", UserInfo.GetInstance().TotalCoins - totalCoins);
            cmd.Parameters.AddWithValue("@gameSessionId", gameSessionId);
            cmd.ExecuteNonQuery();
            db.Close();
        }
    }

    public static void UpdateInvestmentPayout(string userId, string itemId, int payouts)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("UPDATE Inventory SET payouts = @payouts WHERE playerId = @userId AND itemId = @itemId", db);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@itemId", itemId);
            cmd.Parameters.AddWithValue("@payouts", payouts);
            db.Open();
            cmd.ExecuteNonQuery();
            db.Close();
        }
    }

    public static void RegisterSendCoinsTransaction(string senderId, string recieverDisplayName, decimal amount)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            string receiverId = string.Empty;
            var cmd = new SqlCommand("SELECT userId FROM Users WHERE displayName=@recieverDisplayName", db);
            cmd.Parameters.AddWithValue("@recieverDisplayName", recieverDisplayName);

            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                receiverId = reader.GetString(0);
                break;
            }
            reader.Close();

            cmd = new SqlCommand("INSERT INTO TransactionHistory VALUES (@transactionId, @senderId, @receiverId, @amount, @transactionInitiated)", db);
            cmd.Parameters.AddWithValue("@transactionId", Guid.NewGuid());
            cmd.Parameters.AddWithValue("@senderId", senderId);
            cmd.Parameters.AddWithValue("@receiverId", receiverId);
            cmd.Parameters.AddWithValue("@amount", amount);
            cmd.Parameters.AddWithValue("@transactionInitiated", DateTime.Now);
            cmd.ExecuteNonQuery();

            db.Close();
        }
    }

    public static void SaveMessage(string currentUserId, string friendDisplayName, string messageContent)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            string receiverId = string.Empty;
            var cmd = new SqlCommand("SELECT userId FROM Users WHERE displayName=@friendDisplayName", db);
            cmd.Parameters.AddWithValue("@friendDisplayName", friendDisplayName);

            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                receiverId = reader.GetString(0);
                break;
            }
            reader.Close();

            cmd = new SqlCommand("INSERT INTO ChatMessages VALUES (@messageId, @senderId, @receiverId, @content, @created, @updated)", db);
            cmd.Parameters.AddWithValue("@messageId", Guid.NewGuid().ToString());
            cmd.Parameters.AddWithValue("@senderId", currentUserId);
            cmd.Parameters.AddWithValue("@receiverId", receiverId);
            cmd.Parameters.AddWithValue("@content", messageContent);
            cmd.Parameters.AddWithValue("@created", DateTime.Now);
            cmd.Parameters.AddWithValue("@updated", DateTime.Now);
            cmd.ExecuteNonQuery();

            db.Close();
        }
    }
    public static string GetUserId(string displayName)
    {
        string userId = string.Empty;
        using (var db = new SqlConnection(ConnectionString))
        {            
            var cmd = new SqlCommand("SELECT userId FROM Users WHERE displayName=@displayName", db);
            cmd.Parameters.AddWithValue("@displayName", displayName);

            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                userId = reader.GetString(0);
                break;
            }
            
            reader.Close();
            db.Close();
        }
        return userId;
    }

    public static List<ChatMessage> GetLastTenMessages(string currentUserId, string friendUserId)
    {
        List<ChatMessage> lastMessages = new List<ChatMessage>();

        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("SELECT TOP 10 messageId, senderId, messageContent, DateTimeCreated, DateTimeEdited FROM ChatMessages WHERE (senderId=@currentUserId AND receiverId=@friendUserId) OR (senderId=@friendUserId AND receiverId=@currentUserId) ORDER BY DateTimeCreated DESC", db);
            cmd.Parameters.AddWithValue("@currentUserId", currentUserId);
            cmd.Parameters.AddWithValue("@friendUserId", friendUserId);
            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lastMessages.Add(new ChatMessage(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetDateTime(4)));
            }

            reader.Close();
            db.Close();
        }
        return lastMessages;
    }

    public static string GetItemId(string itemName)
    {
        string ItemId = string.Empty;
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("SELECT itemId FROM item WHERE itemTitle=@itemName", db);
            cmd.Parameters.AddWithValue("@itemName", itemName);

            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ItemId = reader.GetString(0);
                break;
            }

            reader.Close();
            db.Close();
        }
        return ItemId;
    }

    public static void DeleteMessage(string messageId)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("DELETE FROM ChatMessages WHERE messageId=@messageId", db);
            cmd.Parameters.AddWithValue("@messageId", messageId);
            db.Open();           
            cmd.ExecuteNonQuery();
            db.Close();
        }
    }

    public static string GetMessage(string messageId)
    {
        var content = string.Empty;
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("SELECT messageContent FROM ChatMessages WHERE messageId=@messageId", db);
            cmd.Parameters.AddWithValue("@messageId", messageId);
            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                content = reader.GetString(0);
                break;
            }

            reader.Close();
            db.Close();
        }
        return content;
    }

    public static void UpdateMessage(string messageId, string content)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("UPDATE ChatMessages SET messageContent = @content WHERE messageId = @messageId", db);
            cmd.Parameters.AddWithValue("@content", content);
            cmd.Parameters.AddWithValue("@messageId", messageId);
            db.Open();
            cmd.ExecuteNonQuery();
            db.Close();
        }
    }
}