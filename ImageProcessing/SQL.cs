using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
namespace ImageProcessing
{
    public class DBImageData
    {
        public string ImageID { get; set; }
        public string ImageType { get; set; }
        public DBImageData(string imageID, string imageType)
        {
            ImageID = imageID;
            ImageType = imageType;
        }
    }
    public class SQL
    {
        SQLiteConnection connection;
        public SQL()
        {
            connection = new SQLiteConnection("Data Source=ImageList.db;Version=3;");
        }
        public bool AddImage(string imageID,string imageType)
        {
            bool Complated = false;
            if (connection.State == System.Data.ConnectionState.Closed) ;
                connection.Open();

            SQLiteCommand sqCommand = new SQLiteCommand();
            sqCommand.Connection = connection;
            SQLiteTransaction myTrans;

            // Start a local transaction
            myTrans = connection.BeginTransaction();
            // Assign transaction object for a pending local transaction
            sqCommand.Transaction = myTrans;

            try
            {
                sqCommand.CommandText = "INSERT INTO Images(imageID,imageType) Values('"+imageID+"','"+imageType+"')";
                sqCommand.ExecuteNonQuery();
                myTrans.Commit();
                Complated = true;
            }
            catch (Exception e)
            {
                myTrans.Rollback();
            }
            finally
            {
                connection.Close();
            }
            return Complated;
        }
        public bool SetSend(string imageID)
        {
            bool Complated = false;
            if (connection.State == System.Data.ConnectionState.Closed) ;
            connection.Open();

            SQLiteCommand sqCommand = new SQLiteCommand();
            sqCommand.Connection = connection;
            SQLiteTransaction myTrans;

            // Start a local transaction
            myTrans = connection.BeginTransaction();
            // Assign transaction object for a pending local transaction
            sqCommand.Transaction = myTrans;

            try
            {
                sqCommand.CommandText = "UPDATE Images set isSend=1 where imageID='"+imageID+"'";
                sqCommand.ExecuteNonQuery();
                myTrans.Commit();
                Complated = true;
            }
            catch (Exception e)
            {
                myTrans.Rollback();
            }
            finally
            {
                connection.Close();
            }
            return Complated;
        }
        public List<DBImageData> GetUnsendImages()
        {
            List<DBImageData> list = new List<DBImageData>();
            string sql = "select * from Images where isSend=0";
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
            SQLiteCommand command = new SQLiteCommand(sql, connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
                list.Add(new DBImageData(reader["imageID"].ToString(),reader["imageType"].ToString()));
            connection.Close();
            return list;
        }


    }
}
