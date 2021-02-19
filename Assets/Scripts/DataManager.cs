using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.Data.SqlClient;


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
        Debug.Log("Creating User");
        SendSQL();
    }

    public void SendSQL()
    {
        using (SqlConnection db = new SqlConnection(connectionString))
        {
            SqlCommand cmd = new SqlCommand("INSERT Users VALUES ('TEST_USER','test@test.com','testuser','testing123','1991/12/18','100.00','Online')", db);
            db.Open();
            cmd.ExecuteNonQuery();
        }
    }
}