using Domain.Account;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using WebUI.Hubs;

namespace WebUI.Models
{
    public static class NotificaionService
    {
        static readonly string connString = @"Server=localhost\SQLEXPRESS;Database=Integratieproject;Trusted_Connection=True;";

        internal static SqlCommand Command = null;
        internal static SqlDependency Dependency = null;


        /// <summary>
        /// Gets the notifications.
        /// </summary>
        /// <returns></returns>
        public static string GetNotification()
        {

            try
            {

                var messages = new List<Alert>();
                using (var connection = new SqlConnection(connString))
                {

                    connection.Open();
                    //// Sanjay : Alwasys use "dbo" prefix of database to trigger change event
                    using (Command = new SqlCommand(@"SELECT [AlertId], [Alertnaam], [Triggered] FROM [Integratieproject].[dbo].[Alerts]", connection))
                    {
                        Command.Notification = null;

                        if (Dependency == null)
                        {
                            Dependency = new SqlDependency(Command);
                            Dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                        }

                        if (connection.State == ConnectionState.Closed)
                            connection.Open();

                        var reader = Command.ExecuteReader();

                        while (reader.Read())
                        {
                            messages.Add(item: new Alert
                            {
                                AlertId = (int)reader["AlertId"],
                                AlertNaam = reader["Alertnaam"] != DBNull.Value ? (string)reader["Alertnaam"] : "Zonder naam",
                                Triggered = (bool)reader["Triggered"]

                            });
                        }
                    }


                }
                /*
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(messages); */

                var json = JsonConvert.SerializeObject(messages);
                return json;

            }
            catch (Exception ex)
            {

                Exception test = ex;
                return null;
            }


        }

        private static void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (Dependency != null)
            {
                Dependency.OnChange -= dependency_OnChange;
                Dependency = null;
            }
            if (e.Type == SqlNotificationType.Change)
            {
                MyHub.Send();
            }
        }

    }
}