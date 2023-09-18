using Microsoft.Data.SqlClient;
using System.Data;

namespace WebAPI.CustomClasses
{
    public class CC_Database
    {
        public class DatabaseHandler
        {
            public SqlCommand EstablishConnection(SqlConnection conn)
            {
                SqlCommand sqlCommand = new SqlCommand();
                sqlCommand.Connection = conn;
                try
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                        Console.WriteLine("Opened Connection");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return null;
                }
                return sqlCommand;
            }

            public void CloseConnection(SqlConnection conn)
            {
                try
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        Console.WriteLine("Closed Connection");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            public string GetDatabaseDirectory()
            {
                string databaseDirectory = Environment.CurrentDirectory + "\\Databases\\";
                //Console.WriteLine(databaseDirectory);
                return databaseDirectory;
            }

            public void CreateDatabase(string databaseName,SqlCommand cmd)
            {
                string directoryPath = GetDatabaseDirectory();
                try
                {
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
            }

            public bool DatabaseExists(SqlCommand cmd,string databaseName)
            {
                bool exists = false;
                try
                {
                    //Check if db exists
                    cmd.CommandText = "Select name From dbo.sysdatabases where  name='" + databaseName + "'";
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        exists = true;
                        Console.WriteLine("Database exists");
                    }
                    reader.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                return exists;
            }
            
            public string GetAllRows(SqlCommand cmd, string databaseName)
            {
                try
                {
                    //Check if db exists
                    cmd.CommandText = "Select * From " + databaseName + ".dbo.Countries";
                    SqlDataReader reader = cmd.ExecuteReader();
                    string return_string = "";
                    while (reader.Read())
                    {

                        //add to the string
                        return_string += reader["ID"] + " Country: " + reader["CommonName"] + " / " +
                            "Capital(s): " + reader["Capital"] + " / " +
                            "Bordering Countries: " + reader["Borders"] + "\n";
                    }
                    reader.Close();
                    return return_string;
                }
                catch(Exception e) {
                    Console.WriteLine(e.ToString());
                    return "Failed to read from database";
                }
                
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

            public string DeleteDatabase(SqlCommand cmd, string databaseName)
            {
                try
                {
                    cmd.CommandText = "ALTER DATABASE " + databaseName + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" +
                        "DROP DATABASE[" + databaseName + "]";

                    //Console.WriteLine(cmd.CommandText);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return "FAILED Database Deletion";
                }
                return "Deleted Successfully";
            }
        }


    }
}
