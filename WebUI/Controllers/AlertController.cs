using BL;
using Domain.Account;
using Domain.Entiteit;
using Domain.Enum;
using Microsoft.AspNet.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebUI.Models;

namespace WebUI.Controllers
{


    public class AlertController : Controller
    {


        public ActionResult Notifications()
        {
            IAccountManager accountManager = new AccountManager();
            List<Alert> allAlerts = accountManager.getAlleAlerts();
            List <Alert> webalerts = new List<Alert>();
            List<Alert> mailalerts = new List<Alert>();
            List<Alert> androidalerts = new List<Alert>();

             allAlerts = allAlerts.Where(x => x.Triggered == true).ToList();
             webalerts = allAlerts.Where(x =>  x.PlatformType == PlatformType.WEB ).ToList();
             mailalerts = allAlerts.Where(x =>  x.PlatformType == PlatformType.EMAIL).ToList();
            androidalerts = allAlerts.Where(x => x.PlatformType == PlatformType.ANDROID).ToList();



            if (mailalerts.Count() > 0)
            {
                sendMail(mailalerts);
            }
            if (androidalerts.Count() > 0)
            {
                sendAndroid(androidalerts);
            }
         
            return PartialView("~/Views/Shared/Dashboard/_DashboardAlerts.cshtml", webalerts.AsEnumerable());
        }

        public ActionResult ReadAlert(int id)
        {
            AccountManager mgr = new AccountManager();
           Alert alert =  mgr.GetAlert(id);
            alert.Triggered = false;
            mgr.UpdateAlert(alert);
            AccountManager acm = new AccountManager();
            List<Alert> alerts = acm.getAlleAlerts();

            IEnumerable<Alert> newalerts = alerts.Where(x => x.Account.IdentityId == User.Identity.GetUserId());

            return RedirectToAction("UpdateAlerts", "Manage");

        }
        public void sendMail(List<Alert> mailalerts)
        {
            List<Alert> tempAlerts = mailalerts;

            AccountManager acm = new AccountManager();

            var apiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
            string Mail = ConfigurationManager.AppSettings["mailAccount"];

            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(Mail, "Politieke Barometer"),
                Subject = "U heeft nieuwe alerts op Politieke Barometer",



            };
            int i = 1;

            int e = 0;
            mailalerts.GroupBy(x => x.Entiteit.EntiteitId);
            foreach (Alert alert in tempAlerts.ToList())
            {
                msg.HtmlContent = "<h1> Politieke Barometer</h1> <h3> U heeft nieuwe alerts: </h3> ";
                foreach (Alert a in tempAlerts.Where(x => x.Account.AccountId == alert.Account.AccountId).ToList())
                {
                    msg.HtmlContent += "<h5><b>" + a.AlertNaam + ": </b> </h5><p>" + a.TrendType + " " + alert.Voorwaarde + " <p>";
                    tempAlerts.Remove(a);


                }
                msg.AddTo(new EmailAddress(alert.Account.Email));
                var response = client.SendEmailAsync(msg);


            }


        }
        public void sendAndroid(List<Alert> alerts)
        {
            foreach (Alert alert in alerts) {


                try { 


                string deviceId = "f7mDZC1LF5Y:APA91bHoKrY5BmyfMF-m1csc49mwjLyp7Ioq3jRM70LGI5RU_dFucqkSlQF-qxONJGYm6ykDom0YFyC0qlYNrHW67MJbgNQCm8m3L1qROVoTDSNWhoCYZLG7WLPO-RmPhMp7-5IEFfdt";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Headers.Add("Authorization", "key=AAAAqR7gPVE:APA91bE_doWC0ah6uYH2KnM3djCI8E0rp4QJ4T6P5X1hL5KVCgofzr_c39psDcACiNYCrpy1TU5fIk8YpXQ_VqOHnfFRANR7uaHmKDtodm9iIa0fPczE4dED0G0zzYP7N4UUvm_qwtWB");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    notification = new
                    {
                        body = alert.TrendType + " " + alert.Voorwaarde,
                        title = alert.AlertNaam,
                        sound = "Enabled"
                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);

                Byte[] byteArray = Encoding.UTF8.GetBytes(json);

                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                            }
                        }
                    }
                }
                }
                catch (Exception ex)
                {
                    string str = ex.Message;
                }
            }
     }

    }
    }
