using Microsoft.Data.SqlClient;
using System.Data;

namespace WebAPI.CustomClasses
{
    public class CC_Database
    {
        public class DatabaseHandler
        {
            public string GetDatabaseDirectory()
            {
                string databaseDirectory = Environment.CurrentDirectory + "\\Databases\\";
                //Console.WriteLine(databaseDirectory);
                return databaseDirectory;
            }

            public void CreateDatabase(string directoryPath)
            {
                string databaseName = "MyDB";
                // Creating instance of SqlConnection  
                SqlConnection conn = new SqlConnection("Server=localhost;Integrated security=SSPI;TrustServerCertificate=True");
                SqlCommand cmd = new SqlCommand();// Creating instance of SqlCommand  
                cmd.Connection = conn; // set the connection to instance of SqlCommand  

                bool exists = false;
                try
                {
                    conn.Open();
                    //Check if db exists
                    cmd.CommandText = "Select name From dbo.sysdatabases where  name='" + databaseName + "'";
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        exists = true;
                        Console.WriteLine("Database already exists");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return;
                }
                finally
                {
                    conn.Close();
                }

                if (exists)
                    return;
                //if db doesnt exist
                cmd.CommandText = "CREATE DATABASE " + databaseName + " ON PRIMARY " +
                    "(NAME = " + databaseName + "_Data, " +
                     "FILENAME ='" + directoryPath + databaseName + ".mdf')";

                //Console.WriteLine(cmd.CommandText);
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "CREATE TABLE " + databaseName + ".dbo.Countries (" +
                        "CommonName varchar(255)" +
                        ");";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        Console.WriteLine("Closed");
                    }
                }
            }

            public void InsertRow(string new_country)
            {
                string databaseName = "MyDB";
                // Creating instance of SqlConnection  
                SqlConnection conn = new SqlConnection("Server=localhost;Integrated security=SSPI;TrustServerCertificate=True");
                SqlCommand cmd = new SqlCommand();// Creating instance of SqlCommand  
                cmd.Connection = conn; // set the connection to instance of SqlCommand  
                try
                {
                    conn.Open();
                    //Check if db exists
                    cmd.CommandText = "INSERT INTO " + databaseName + ".dbo.Countries(CommonName)" +
                        "VALUES('" + new_country + "'); ";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }
}
