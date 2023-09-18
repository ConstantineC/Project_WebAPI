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
                        "ID int NOT NULL," +
                        "CommonName varchar(255) NOT NULL," +
                        "Capital varchar(255)," +
                        "Borders varchar(255)," +
                        "PRIMARY KEY (ID));";
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
            
            public void InsertRow(string databaseName,SqlCommand cmd,int id,string country,string capital,string borders)
            {
                try
                {
                    if (capital == "")
                        capital = "None";
                    if (borders == "")
                        borders = "None";

                    country = country.Replace("'", "''");
                    capital = capital.Replace("'", "''");

                    //insert new row
                    cmd.CommandText = "INSERT INTO " + databaseName + ".dbo.Countries(ID,CommonName,Capital,Borders)" +
                        "VALUES('"+id+"','" + country + "' , '" + capital + "' , '" + borders + "');";

                    //Console.WriteLine(cmd.CommandText);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return;
                }
            }
        }
    }
}
