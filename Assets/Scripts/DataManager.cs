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
                return true;
            }

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
                return player;
            }

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
}