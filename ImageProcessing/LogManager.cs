using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace ImageProcessing
{

    public static class LogManager
    {
        static SQLiteConnection connection = new SQLiteConnection("Data Source=Logs.db;Version=3;");
        public static void Log(LogData logData)
        {
            bool Complated = false;
            do
            {
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
                    sqCommand.CommandText = "INSERT INTO system_logs(date,time,type,message) Values('" +
                        logData.LogTime.ToString("yyyy-MM-dd") + "','" +
                        logData.LogTime.ToString("HH:mm:ss") + "'," +
                        (int)logData.LogType + ",'" +
                        logData.LogMessage + "')";
                    sqCommand.ExecuteNonQuery();
                    myTrans.Commit();
                    Complated = true;
                    string _logMessage = string.Format("[{0}] [{1}] {2}" + Environment.NewLine,
                    EnumHelper.GetEnumDescription(logData.LogType),
                    logData.LogTime.ToString("dd-MM-yyyy HH-mm-ss"),
                    logData.LogMessage);
                    System.Diagnostics.Debug.Write(_logMessage);
                }
                catch (Exception e)
                {
                    myTrans.Rollback();
                }
                finally
                {
                    connection.Close();
                }
            } while (!Complated);

            //ProcessWrite(string.Format("logs/{0}.log", logData.LogTime.ToString("dd-MM-yyyy")), _logMessage).Wait();
        }
        /*static async System.Threading.Tasks.Task WriteTextAsync(string filePath, string text)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);

            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Append, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
            };
        }
        static System.Threading.Tasks.Task ProcessWrite(string filePath,string text)
        {
            return WriteTextAsync(filePath, text);
        }*/
    }
}
