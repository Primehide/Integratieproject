
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using DAL;
using Domain.Account;
using Domain.Entiteit;
using Domain.Enum;
using Domain.Post;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Web.Script.Serialization;


namespace BL
{
    public class AccountManager : IAccountManager
    {
        private readonly IAccountRepository repo;

        private IAccountRepository accountRepository;
        private UnitOfWorkManager uowManager;

        public AccountManager()
        {
            repo = new AccountRepository();
        }

        public AccountManager(UnitOfWorkManager uofMgr)
        {
            uowManager = uofMgr;

        }

        public void addUser(Account account)
        {
            initNonExistingRepo();
            accountRepository.addUser(account);

            // uowManager.Save();

        }
        public void UpdateUser(Account account)
        {

            initNonExistingRepo();
            Account oldaccount = new Account();

            oldaccount = accountRepository.ReadAccount(account.IdentityId);
            account.AccountId = oldaccount.AccountId;


            accountRepository.updateUser(account);
        }

        public Account getAccount(string ID)
        {
            initNonExistingRepo();
            return repo.ReadAccount(ID);
        }

        public List<Account> GetAccounts(int platId)
        {
            initNonExistingRepo();
            return accountRepository.readAccounts().FindAll(x => x.PlatId == platId); ;
        }
   
        public void genereerAlerts()
        {
            initNonExistingRepo(true);
            EntiteitManager entiteitMgr = new EntiteitManager(uowManager);
            List<Alert> Alerts = getAlleAlerts();
            List<Alert> mailAlerts = new List<Alert>();
            List<Alert> androidalerts = new List<Alert>();
            Entiteit e;
            //1 keer alle trends resetten om vandaag te kunnen kijken of er een trend aanwezig is
            entiteitMgr.ResetTrends();
            foreach (var alert in Alerts)
            {
                e = alert.Entiteit;
                if (entiteitMgr.berekenTrends(alert.MinWaarde, e, alert.TrendType, alert.Voorwaarde))
                {
                    alert.Triggered = true;
                    UpdateAlert(alert);
                    if(alert.PlatformType == Domain.Enum.PlatformType.EMAIL)
                    {
                        mailAlerts.Add(alert);  
                    }
                    if(alert.PlatformType == Domain.Enum.PlatformType.ANDROID)
                    {
                        androidalerts.Add(alert);
                    }
                }
            }

            //Alerts (mail & android) verzenden naar de gebruiker.
            if(mailAlerts.Count > 0)
            {
                sendMailAlerts(mailAlerts);
            }
            if(androidalerts.Count > 0)
            {
                sendAndroidAlerts(androidalerts);
            }
        }

        // Android alerts verzenden
        public async void sendAndroidAlerts(List<Alert> androidalerts)
        {
            AccountManager acm = new AccountManager();
            foreach (Alert alert in androidalerts)
            {

                try
                {
                    AccountManager mgr = new AccountManager();
                
                    string deviceId = alert.Account.DeviceId;
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
                    
                    using (Stream dataStream = await tRequest.GetRequestStreamAsync())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        using (WebResponse tResponse = await tRequest.GetResponseAsync())
                        {
                            using (Stream dataStreamResponse = tResponse.GetResponseStream())
                            {
                                using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                {
                                    String sResponseFromServer = await tReader.ReadToEndAsync();
                                    string str = sResponseFromServer;
                                }
                            }
                        }
                    }
                    alert.Triggered = false;
                    acm.UpdateAlert(alert);
                }
                catch (Exception ex)
                {
                    string str = ex.Message;
                }

                
            }
        }

        // Email alerts verzenden 
       public async void sendMailAlerts(List<Alert> mailalerts)
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
                var response = await client.SendEmailAsync(msg);

                alert.Triggered = false;
                acm.UpdateAlert(alert);
            }


        }


        public void addDeviceId(string userId,string device)
        {
            repo.addDeviceId(userId, device);
        }


        public Alert GetAlert(int alertID)
        {
            return repo.ReadAlert(alertID);
        }
        public void AddAlert(Alert alert, int entiteitId, bool web, bool android, bool mail)
        {
            initNonExistingRepo(true);
        EntiteitManager emg = new EntiteitManager(uowManager);

             alert.Entiteit = emg.getEntiteit(entiteitId);
         
            // var entiteit = emg.getEntiteit(1);
             //entiteit.Alerts.Add(alert);



            if (android == true)
            {
                alert.PlatformType = Domain.Enum.PlatformType.ANDROID;
                repo.AddAlert(alert);

            }

            if (web == true)
            {
                alert.PlatformType = Domain.Enum.PlatformType.WEB;
                repo.AddAlert(alert);

            }

            if (mail == true)
            {
                alert.PlatformType = Domain.Enum.PlatformType.EMAIL;
                repo.AddAlert(alert);

            }



           
             uowManager.Save();
        }
        

        public void UpdateAlert(Alert alert)
        {

            initNonExistingRepo();
            accountRepository.UpdateAlert(alert);
            

        }
        public void DeleteAlert(int alertID)
        {
            initNonExistingRepo();
            accountRepository.DeleteAlert(alertID);
        }
        public void addUser(Alert alert)
        {
            initNonExistingRepo();
            accountRepository.AddAlert(alert);
            uowManager.Save();
        }

        public List<Alert> getAlleAlerts()
        {
            initNonExistingRepo();
            return accountRepository.getAlleAlerts();
        }

        public void initNonExistingRepo(bool withUnitOfWork = false)
        {
            // Als we een repo met UoW willen gebruiken en als er nog geen uowManager bestaat:
            // Dan maken we de uowManager aan en gebruiken we de context daaruit om de repo aan te maken.

            if (withUnitOfWork)
            {
                if (uowManager == null)
                {
                    uowManager = new UnitOfWorkManager();
                    accountRepository = new AccountRepository(uowManager.UnitOfWork);
                }
            }
            // Als we niet met UoW willen werken, dan maken we een repo aan als die nog niet bestaat.
            else
            {
                accountRepository = (accountRepository == null) ? new AccountRepository() : accountRepository;
            }
        }


        public void DeleteUser(string accountId)
        {
            initNonExistingRepo();
            accountRepository.DeleteUser(accountId);
        }

        public void FollowEntity(string identityID, int entiteitID)
        {

            initNonExistingRepo();
            accountRepository.FollowEntiteit(identityID, entiteitID);
        }



        public void UnfollowEntity(string identityID, int entiteitID)
        {
            initNonExistingRepo();
            accountRepository.UnFollowEntiteit(identityID, entiteitID);
        }



        public void grafiekAanGebruikerToevoegen(string IdentityId, Domain.Enum.GrafiekType TypeGrafiek, List<int> entiteitInts, List<string> CijferOpties, string VergelijkOptie, Domain.Enum.GrafiekSoort grafiekSoort)
        {
            initNonExistingRepo(true);
            //IPostManager postManager = new PostManager(uowManager);
            IEntiteitManager entiteitManager = new EntiteitManager(uowManager);
            Domain.Account.Account user = accountRepository.ReadAccount(IdentityId);
            Domain.Post.Grafiek grafiek = new Domain.Post.Grafiek();
            grafiek.Entiteiten = new List<Entiteit>();
            List<Entiteit> entiteiten = new List<Entiteit>();

            foreach (var i in entiteitInts)
            {
                var e = entiteitManager.getAlleEntiteiten().Single(x => x.EntiteitId == i);
                entiteiten.Add(e);
                grafiek.Entiteiten.Add(e);
            }
            Dictionary<string, double> waardes = entiteitManager.BerekenGrafiekWaarde(TypeGrafiek,entiteiten,CijferOpties, VergelijkOptie);
            List<Domain.Post.GrafiekWaarde> grafiekWaardes = new List<Domain.Post.GrafiekWaarde>();
            
            foreach (var item in waardes)
            {
                Domain.Post.GrafiekWaarde w = new Domain.Post.GrafiekWaarde()
                {
                    Naam = item.Key,
                    Waarde = item.Value
                };
                grafiekWaardes.Add(w);
            }
            if (CijferOpties != null)
            {
                grafiek.CijferOpties = new List<Domain.Post.CijferOpties>();
                foreach (var opt in CijferOpties)
                {
                    if (opt.ToLower() == "aantalposts")
                    {
                        grafiek.CijferOpties.Add(new Domain.Post.CijferOpties
                        {
                            optie = opt
                        });
                    }
                    if (opt.ToLower() == "aantalretweets")
                    {
                        grafiek.CijferOpties.Add(new Domain.Post.CijferOpties
                        {
                            optie = opt
                        });
                    }
                    if (opt.ToLower() == "aanwezigetrends")
                    {
                        grafiek.CijferOpties.Add(new Domain.Post.CijferOpties
                        {
                            optie = opt
                        });
                    }
                }
            }
            grafiek.Type = TypeGrafiek;
            grafiek.Waardes = grafiekWaardes;
            grafiek.GrafiekSoort = grafiekSoort;
            if(VergelijkOptie.ToLower() == "populariteit")
            {
                grafiek.soortGegevens = Domain.Enum.SoortGegevens.POPULARITEIT;
            } else if(VergelijkOptie.ToLower() == "postfrequentie")
            {
                grafiek.soortGegevens = Domain.Enum.SoortGegevens.POSTFREQUENTIE;
            }


            //cijfers
            switch (TypeGrafiek)
            {
                case Domain.Enum.GrafiekType.CIJFERS:
                    grafiek.Naam = "Cijfer gegevens - " + entiteiten.First().Naam;
                    break;
                case Domain.Enum.GrafiekType.VERGELIJKING:
                    if(grafiek.soortGegevens == Domain.Enum.SoortGegevens.POSTFREQUENTIE)
                    {
                        grafiek.Naam = "Vergelijking post frequentie";
                    } else if(grafiek.soortGegevens == Domain.Enum.SoortGegevens.POPULARITEIT)
                    {
                        grafiek.Naam = "Vergelijking populariteit";
                    }
                    break;
            }
            foreach (Entiteit e in grafiek.Entiteiten)
            {
                e.Posts = null;
            }
            Domain.Account.DashboardBlok dashboardBlok = new Domain.Account.DashboardBlok()
            {
                Grafiek = grafiek
            };

            if (user.Dashboard.Configuratie.DashboardBlokken == null)
                user.Dashboard.Configuratie.DashboardBlokken = new List<DashboardBlok>();
            user.Dashboard.Configuratie.DashboardBlokken.Add(dashboardBlok);
            accountRepository.updateUser(user);
            uowManager.Save();
        }

        public void updateUser(Account account)
        {
            initNonExistingRepo();
            accountRepository.updateUser(account);
        }

        public void DeleteGrafiekWaardes(int grafiekID)
        {
            accountRepository.DeleteGrafiekWaardes(grafiekID);
        }

        public void addFaq(Faq faq)
        {
            initNonExistingRepo();
            accountRepository.addFaq(faq);

        }
        public void updateFaq(Faq faq)
        {
            initNonExistingRepo();
            repo.UpdateFaq(faq); 

        }
        public void deleteFaq(int faqID)
        {
            initNonExistingRepo();
            accountRepository.DeleteFaq(faqID);

        }
        public List<Faq> getAlleFaqs(int id)
        {
            initNonExistingRepo();
            return accountRepository.getAlleFaqs().Where(x => x.PlatformId == id).ToList();
        }
        public void UpdateAlert(int id)
        {
            initNonExistingRepo();
            Alert alertToUpdate = GetAlert(id);
            alertToUpdate.Triggered = false;
            repo.UpdateAlert(alertToUpdate);
        }

        public void AddUserGrafiek(List<CijferOpties> opties, List<int> entiteitIds, GrafiekType grafiekType, int platId, string IdentityId, string naam, GrafiekSoort grafiekSoort)
        {
            initNonExistingRepo(true);
            EntiteitManager entiteitManager = new EntiteitManager(uowManager);
            IPostManager postManager = new PostManager(uowManager);
            List<Entiteit> entiteiten = new List<Entiteit>();

            Account user = accountRepository.ReadAccount(IdentityId);

            //geselecteerde entiteiten opzoeken
            foreach (var i in entiteitIds)
            {
                entiteiten.Add(entiteitManager.getEntiteit(i));
            }

            //nieuwe grafiek aanmaken
            Grafiek grafiek = new Grafiek()
            {
                CijferOpties = opties,
                Entiteiten = entiteiten,
                Type = grafiekType,
                Waardes = new List<GrafiekWaarde>(),
                Naam = naam,
                GrafiekSoort = grafiekSoort
            };

            if (opties.First().optie.ToLower() == "populariteit")
            {
                grafiek.soortGegevens = Domain.Enum.SoortGegevens.POPULARITEIT;
            }
            else if (opties.First().optie.ToLower() == "postfrequentie")
            {
                grafiek.soortGegevens = Domain.Enum.SoortGegevens.POSTFREQUENTIE;
            }
            else if (opties.First().optie.ToLower() == "sentiment")
            {
                grafiek.soortGegevens = Domain.Enum.SoortGegevens.SENTIMENT;
            }

            //waardes voor de grafiek berekenen
            grafiek.Waardes = postManager.BerekenGrafiekWaardes(opties, entiteiten);

            foreach (var e in entiteiten)
            {
                e.Posts.Clear();
            };

            //kijkt na of de gebruiker al een lijst van blokken heeft om nullpointer te vermijden
            if (user.Dashboard.Configuratie.DashboardBlokken == null)
                user.Dashboard.Configuratie.DashboardBlokken = new List<DashboardBlok>();

            //nieuw blok aanmaken voor de configuratie
            DashboardBlok dashboardBlok = new DashboardBlok()
            {
                Grafiek = grafiek,
            };
            user.Dashboard.Configuratie.DashboardBlokken.Add(dashboardBlok);
            accountRepository.updateUser(user);
            uowManager.Save();
        }

        public void UpdateGrafiek(int grafiekId)
        {
            initNonExistingRepo(true);
            PostManager postManager = new PostManager(uowManager);
            Grafiek grafiekToUpdate = postManager.GetGrafiek(grafiekId);
            grafiekToUpdate.Waardes = postManager.BerekenGrafiekWaardes(grafiekToUpdate.CijferOpties, grafiekToUpdate.Entiteiten);
            //postManager.UpdateGrafiek(grafiekToUpdate);
            uowManager.Save();
        }
    }
}
