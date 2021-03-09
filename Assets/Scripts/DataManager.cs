using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.Data.SqlClient;
using System;


// unsecure implementation for testing connection to DB, need API in future
public class DataManager : MonoBehaviour
{
    private const string SERVER_IP = "216.58.1.35";
    private const string DATABASE = "GambleWithFriends";
    private const string USERNAME = "Anthony";
    private const string PASSWORD = "Password123";

    private string connectionString = @"Data Source = " + SERVER_IP +
                                    ";Database=" + DATABASE +
                                    ";User Id=" + USERNAME +
                                    ";Password=" + PASSWORD + ";";

    public void Start()
    {

    }

    public void SendSQL()
    {
        using (SqlConnection db = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("INSERT Users VALUES ('5d931a30-dff0-44b2-aa0b-079529e43756','test@test.com','testuser','testing123!', 1000.00)", db);
            db.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public void AddUser(string email, string displayName, string password)
    {
        using (SqlConnection db = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("INSERT Users VALUES (@userId, @email, @displayName, @password, 1000.00)", db);

            SqlParameter userIdParam = new SqlParameter();
            userIdParam.ParameterName = "@userId";
            userIdParam.Value = Guid.NewGuid().ToString();
            cmd.Parameters.Add(userIdParam);

            SqlParameter emailParam = new SqlParameter();
            emailParam.ParameterName = "@email";
            emailParam.Value = email;
            cmd.Parameters.Add(emailParam);

            SqlParameter userNameParam = new SqlParameter();
            userNameParam.ParameterName = "@displayName";
            userNameParam.Value = displayName;
            cmd.Parameters.Add(userNameParam);

            SqlParameter passwordParam = new SqlParameter();
            passwordParam.ParameterName = "@password";
            passwordParam.Value = password;
            cmd.Parameters.Add(passwordParam);

            db.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public bool DisplayNameExists(string displayName)
    {
        using (SqlConnection db = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE displayName=@displayName", db);

            SqlParameter param = new SqlParameter();
            param.ParameterName = "@displayName";
            param.Value = displayName;
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

    public bool LoginUser(string displayName, string password)
    {
        using (SqlConnection db = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE displayName=@displayName AND password=@password", db);

            SqlParameter userNameParam = new SqlParameter();
            userNameParam.ParameterName = "@displayName";
            userNameParam.Value = displayName;
            cmd.Parameters.Add(userNameParam);

            SqlParameter passwordParam = new SqlParameter();
            passwordParam.ParameterName = "@password";
            passwordParam.Value = password;
            cmd.Parameters.Add(passwordParam);

            db.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                return true;
            }
            return false;
        }
    }
}