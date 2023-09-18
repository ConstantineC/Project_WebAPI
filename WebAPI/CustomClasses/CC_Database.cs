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

            public void CreateDatabase(string directoryPath, string databaseName)
            {  
                SqlConnection conn = new SqlConnection("Server=localhost;Integrated security=SSPI;TrustServerCertificate=True");
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;

                //if db doesnt exist
                if (DatabaseExists(conn,cmd,databaseName))
                    return;
                
                //Console.WriteLine(cmd.CommandText);
                try
                {
                    if(conn.State==ConnectionState.Closed)
                        conn.Open();

                    //database creation command
                    cmd.CommandText = "CREATE DATABASE " + databaseName + " ON PRIMARY " +
                        "(NAME = " + databaseName + "_Data, " +
                         "FILENAME ='" + directoryPath + databaseName + ".mdf')";
                    cmd.ExecuteNonQuery();

                    //table creation command
                    cmd.CommandText = "CREATE TABLE " + databaseName + ".dbo.Countries (" +
                        "CommonName varchar(255)" +
                        "Capital varchar(255)" +
                        "Borders varchar(255)" +
                        ");";
                    cmd.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    //close connection
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        Console.WriteLine("Closed");
                    }
                }
            }

            private bool DatabaseExists(SqlConnection conn,SqlCommand cmd,string databaseName)
            {
                bool exists = false;
                try
                {
                    if (conn.State == ConnectionState.Closed)
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
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
                return exists;
            }
            
            public void InsertRow(string new_country,string capital,string borders,string databaseName)
            {
                // Creating instance of SqlConnection  
                SqlConnection conn = new SqlConnection("Server=localhost;Integrated security=SSPI;TrustServerCertificate=True");
                SqlCommand cmd = new SqlCommand();// Creating instance of SqlCommand  
                cmd.Connection = conn; // set the connection to instance of SqlCommand  
                try
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Open();

                    //insert new row
                    cmd.CommandText = "INSERT INTO " + databaseName + ".dbo.Countries(CommonName,Capital,Borders)" +
                        "VALUES('" + new_country + "','" + capital + "','" + borders + "'); ";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return;
                }
                finally
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Close();
                }
            }
        }
    }
}
