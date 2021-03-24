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
    private const string PASSWORD = "Password123";

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

    public static PlayerModelScript LoginUser(string displayName, string password)
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
                var player = new PlayerModelScript
                {
                    PlayerId = reader.GetString(0),
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
            var cmd = new SqlCommand("INSERT Inventory VALUES (@userId, @itemId, @equipped)", db);

            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@itemId", GameItems.GetItems()[itemName].ItemId);
            cmd.Parameters.AddWithValue("@equipped", 0);

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

    public static void GetInventoryItems(string userId)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("SELECT itemId, equipped FROM Inventory where playerId = @userId", db);

            cmd.Parameters.AddWithValue("@userId", userId);

            db.Open();
            var reader = cmd.ExecuteReader();

            
            InventoryItems.itemIdToRecord = new Dictionary<string, InventoryItems.InventoryItem>();

            while (reader.Read())
            {
                InventoryItems.itemIdToRecord.Add(reader.GetString(0), new InventoryItems.InventoryItem
                {
                    ItemId = reader.GetString(0),
                    Equipped = reader.GetBoolean(1)
                });
            }
            reader.Close();
        }
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

    public static void UnfriendUser(string localUserId, string friendDisplayName)
    {
        using (var db = new SqlConnection(ConnectionString))
        {
            var cmd = new SqlCommand("DELETE f FROM Friends f JOIN Users u ON f.friendId=u.userId WHERE f.userId=@userId AND u.displayName=@friendDisplayName", db);

            var userIdParam = new SqlParameter();
            userIdParam.ParameterName = "@userId";
            userIdParam.Value = localUserId;
            cmd.Parameters.Add(userIdParam);

            var friendIdParam = new SqlParameter();
            friendIdParam.ParameterName = "@friendDisplayName";
            friendIdParam.Value = friendDisplayName;
            cmd.Parameters.Add(friendIdParam);

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
}