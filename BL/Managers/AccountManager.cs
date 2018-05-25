using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using BL.Interfaces;
using DAL.Interfaces;
using DAL.Repositories;
using Domain.Account;
using Domain.Entiteit;
using Domain.Enum;
using Domain.Post;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BL.Managers
{
    public class AccountManager : IAccountManager
    {
        private readonly IAccountRepository _repo;

        private IAccountRepository _accountRepository;
        private UnitOfWorkManager _uowManager;

        public AccountManager()
        {
            _repo = new AccountRepository();
        }

        public AccountManager(UnitOfWorkManager uofMgr)
        {
            _uowManager = uofMgr;

        }

        public void AddUser(Account account)
        {
            InitNonExistingRepo();
            _accountRepository.AddUser(account);

            // uowManager.Save();

        }

        public Account GetAccount(string id)
        {
            InitNonExistingRepo();
            return _repo.ReadAccount(id);
        }

        public Account GetAccount(int id)
        {
            InitNonExistingRepo();
            return _repo.ReadAccount(id);
        }

        public List<Account> GetAccounts()
        {
            InitNonExistingRepo();
            return _accountRepository.ReadAccounts();
        }
   
        public void GenereerAlerts()
        {
            InitNonExistingRepo(true);
            EntiteitManager entiteitMgr = new EntiteitManager(_uowManager);
            List<Alert> alerts = GetAlleAlerts();
            List<Alert> mailAlerts = new List<Alert>();
            List<Alert> androidalerts = new List<Alert>();
            //1 keer alle trends resetten om vandaag te kunnen kijken of er een trend aanwezig is
            entiteitMgr.ResetTrends();
            foreach (var alert in alerts)
            {
                var e = alert.Entiteit;
                if (entiteitMgr.BerekenTrends(alert.MinWaarde, e, alert.TrendType, alert.Voorwaarde))
                {
                    alert.Triggered = true;
                    UpdateAlert(alert);
                    if(alert.PlatformType == PlatformType.EMAIL)
                    {
                        mailAlerts.Add(alert);  
                    }
                    if(alert.PlatformType == PlatformType.ANDROID)
                    {
                        androidalerts.Add(alert);
                    }
                }
            }

            //Alerts (mail & android) verzenden naar de gebruiker.
            if(mailAlerts.Count > 0)
            {
                SendMailAlerts(mailAlerts);
            }
            if(androidalerts.Count > 0)
            {
                SendAndroidAlerts(androidalerts);
            }
        }

        // Android alerts verzenden
        public async void SendAndroidAlerts(List<Alert> androidalerts)
        {
            AccountManager acm = new AccountManager();
            foreach (Alert alert in androidalerts)
            {

                try
                {           
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
                                using (StreamReader tReader = new StreamReader(dataStreamResponse ?? throw new InvalidOperationException()))
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
       public async void SendMailAlerts(List<Alert> mailalerts)
        {
            List<Alert> tempAlerts = mailalerts;

            AccountManager acm = new AccountManager();

            var apiKey = ConfigurationManager.AppSettings["SendGridApiKey"];
            string mail = ConfigurationManager.AppSettings["mailAccount"];

            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(mail, "Politieke Barometer"),
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


        public void AddDeviceId(string userId,string device)
        {
            _repo.AddDeviceId(userId, device);
        }


        public Alert GetAlert(int alertId)
        {
            return _repo.ReadAlert(alertId);
        }

        public void AddAlert(Alert alert, int entiteitId, bool web, bool android, bool mail)
        {
            InitNonExistingRepo(true);
            EntiteitManager emg = new EntiteitManager(_uowManager);

             alert.Entiteit = emg.GetEntiteit(entiteitId);
             if (android)
            {
                alert.PlatformType = PlatformType.ANDROID;
                _repo.AddAlert(alert);

            }

            if (web)
            {
                alert.PlatformType = PlatformType.WEB;
                _repo.AddAlert(alert);

            }

            if (mail)
            {
                alert.PlatformType = PlatformType.EMAIL;
                _repo.AddAlert(alert);

            }

             _uowManager.Save();
        }
        

        public void UpdateAlert(Alert alert)
        {

            InitNonExistingRepo();
            _accountRepository.UpdateAlert(alert);
            

        }
        public void DeleteAlert(int alertId)
        {
            InitNonExistingRepo();
            _accountRepository.DeleteAlert(alertId);
        }
        public void AddUser(Alert alert)
        {
            InitNonExistingRepo();
            _accountRepository.AddAlert(alert);
            _uowManager.Save();
        }

        public List<Alert> GetAlleAlerts()
        {
            InitNonExistingRepo();
            return _accountRepository.GetAlleAlerts();
        }
       public List<Alert> GetUserAlerts(string userId)
        {
            return GetAlleAlerts().Where(x => x.Account.IdentityId == userId).ToList();
        }

        public void InitNonExistingRepo(bool withUnitOfWork = false)
        {
            // Als we een repo met UoW willen gebruiken en als er nog geen uowManager bestaat:
            // Dan maken we de uowManager aan en gebruiken we de context daaruit om de repo aan te maken.

            if (withUnitOfWork)
            {
                if (_uowManager == null)
                {
                    _uowManager = new UnitOfWorkManager();
                    _accountRepository = new AccountRepository(_uowManager.UnitOfWork);
                }
            }
            // Als we niet met UoW willen werken, dan maken we een repo aan als die nog niet bestaat.
            else
            {
                _accountRepository = (_accountRepository == null) ? new AccountRepository() : _accountRepository;
            }
        }


        public void DeleteUser(string accountId)
        {
            InitNonExistingRepo();
            _accountRepository.DeleteUser(accountId);
        }

        public void FollowEntity(string identityId, int entiteitId)
        {

            InitNonExistingRepo();
            _accountRepository.FollowEntiteit(identityId, entiteitId);
        }



        public void UnfollowEntity(string identityId, int entiteitId)
        {
            InitNonExistingRepo();
            _accountRepository.UnFollowEntiteit(identityId, entiteitId);
        }



        public void GrafiekAanGebruikerToevoegen(string identityId, GrafiekType typeGrafiek, List<int> entiteitInts, List<string> cijferOpties, string vergelijkOptie, GrafiekSoort grafiekSoort)
        {
            InitNonExistingRepo(true);
            //IPostManager postManager = new PostManager(uowManager);
            IEntiteitManager entiteitManager = new EntiteitManager(_uowManager);
            Account user = _accountRepository.ReadAccount(identityId);
            var grafiek = new Grafiek {Entiteiten = new List<Entiteit>()};
            List<Entiteit> entiteiten = new List<Entiteit>();

            foreach (var i in entiteitInts)
            {
                var e = entiteitManager.GetAlleEntiteiten().Single(x => x.EntiteitId == i);
                entiteiten.Add(e);
                grafiek.Entiteiten.Add(e);
            }
            Dictionary<string, double> waardes = entiteitManager.BerekenGrafiekWaarde(typeGrafiek,entiteiten,cijferOpties, vergelijkOptie);
            List<GrafiekWaarde> grafiekWaardes = new List<GrafiekWaarde>();
            
            foreach (var item in waardes)
            {
                GrafiekWaarde w = new GrafiekWaarde()
                {
                    Naam = item.Key,
                    Waarde = item.Value
                };
                grafiekWaardes.Add(w);
            }
            if (cijferOpties != null)
            {
                grafiek.CijferOpties = new List<CijferOpties>();
                foreach (var opt in cijferOpties)
                {
                    if (opt.ToLower() == "aantalposts")
                    {
                        grafiek.CijferOpties.Add(new CijferOpties
                        {
                            Optie = opt
                        });
                    }
                    if (opt.ToLower() == "aantalretweets")
                    {
                        grafiek.CijferOpties.Add(new CijferOpties
                        {
                            Optie = opt
                        });
                    }
                    if (opt.ToLower() == "aanwezigetrends")
                    {
                        grafiek.CijferOpties.Add(new CijferOpties
                        {
                            Optie = opt
                        });
                    }
                }
            }
            grafiek.Type = typeGrafiek;
            grafiek.Waardes = grafiekWaardes;
            grafiek.GrafiekSoort = grafiekSoort;
            if(vergelijkOptie.ToLower() == "populariteit")
            {
                grafiek.SoortGegevens = SoortGegevens.POPULARITEIT;
            } else if(vergelijkOptie.ToLower() == "postfrequentie")
            {
                grafiek.SoortGegevens = SoortGegevens.POSTFREQUENTIE;
            }


            //cijfers
            switch (typeGrafiek)
            {
                case GrafiekType.CIJFERS:
                    grafiek.Naam = "Cijfer gegevens - " + entiteiten.First().Naam;
                    break;
                case GrafiekType.VERGELIJKING:
                    if(grafiek.SoortGegevens == SoortGegevens.POSTFREQUENTIE)
                    {
                        grafiek.Naam = "Vergelijking post frequentie";
                    } else if(grafiek.SoortGegevens == SoortGegevens.POPULARITEIT)
                    {
                        grafiek.Naam = "Vergelijking populariteit";
                    }
                    break;
            }
            foreach (Entiteit e in grafiek.Entiteiten)
            {
                e.Posts = null;
            }
            DashboardBlok dashboardBlok = new DashboardBlok()
            {
                Grafiek = grafiek
            };

            if (user.Dashboard.Configuratie.DashboardBlokken == null)
                user.Dashboard.Configuratie.DashboardBlokken = new List<DashboardBlok>();
            user.Dashboard.Configuratie.DashboardBlokken.Add(dashboardBlok);
            _accountRepository.UpdateUser(user);
            _uowManager.Save();
        }

        public void UpdateUser(Account account)
        {
            InitNonExistingRepo();
            Account accountToUpdate = _accountRepository.ReadAccount(account.IdentityId);
            accountToUpdate.Achternaam = account.Achternaam;
            accountToUpdate.Voornaam = account.Voornaam;
            accountToUpdate.Email = account.Email;
            if (account.ReviewEntiteiten != null)
            {
                accountToUpdate.ReviewEntiteiten = account.ReviewEntiteiten;
            }
            _accountRepository.UpdateUser(accountToUpdate);
        }

        public void DeleteGrafiekWaardes(int grafiekId)
        {
            _accountRepository.DeleteGrafiekWaardes(grafiekId);
        }

        public void UpdateAlert(int id)
        {
            InitNonExistingRepo();
            Alert alertToUpdate = GetAlert(id);
            alertToUpdate.Triggered = false;
            _repo.UpdateAlert(alertToUpdate);
        }

        public void AddUserGrafiek(List<CijferOpties> opties, List<int> entiteitIds, GrafiekType grafiekType, int platId, string identityId, string naam, GrafiekSoort grafiekSoort)
        {
            InitNonExistingRepo(true);
            EntiteitManager entiteitManager = new EntiteitManager(_uowManager);
            IPostManager postManager = new PostManager(_uowManager);
            List<Entiteit> entiteiten = new List<Entiteit>();

            Account user = _accountRepository.ReadAccount(identityId);

            //geselecteerde entiteiten opzoeken
            foreach (var i in entiteitIds)
            {
                entiteiten.Add(entiteitManager.GetEntiteit(i));
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

            if (opties.First().Optie.ToLower() == "populariteit")
            {
                grafiek.SoortGegevens = SoortGegevens.POPULARITEIT;
            }
            else if (opties.First().Optie.ToLower() == "postfrequentie")
            {
                grafiek.SoortGegevens = SoortGegevens.POSTFREQUENTIE;
            }
            else if (opties.First().Optie.ToLower() == "sentiment")
            {
                grafiek.SoortGegevens = SoortGegevens.SENTIMENT;
            }

            //waardes voor de grafiek berekenen
            grafiek.Waardes = postManager.BerekenGrafiekWaardes(opties, entiteiten);

            foreach (var e in entiteiten)
            {
                e.Posts.Clear();
            }

            //kijkt na of de gebruiker al een lijst van blokken heeft om nullpointer te vermijden
            if (user.Dashboard.Configuratie.DashboardBlokken == null)
                user.Dashboard.Configuratie.DashboardBlokken = new List<DashboardBlok>();

            //nieuw blok aanmaken voor de configuratie
            DashboardBlok dashboardBlok = new DashboardBlok()
            {
                Grafiek = grafiek,
            };
            user.Dashboard.Configuratie.DashboardBlokken.Add(dashboardBlok);
            _accountRepository.UpdateUser(user);
            _uowManager.Save();
        }

        public void UpdateGrafiek(int grafiekId)
        {
            InitNonExistingRepo(true);
            PostManager postManager = new PostManager(_uowManager);
            Grafiek grafiekToUpdate = postManager.GetGrafiek(grafiekId);
            grafiekToUpdate.Waardes = postManager.BerekenGrafiekWaardes(grafiekToUpdate.CijferOpties, grafiekToUpdate.Entiteiten);
            //postManager.UpdateGrafiek(grafiekToUpdate);
            _uowManager.Save();
        }

        public void DeleteDashboardBlok(Account account, int positie)
        {
            InitNonExistingRepo();
            _repo.DeleteDashboardBlok(account, positie);
        }

        public void UpdateLocatie(int blokId, int locatie)
        {
            InitNonExistingRepo();
            _repo.UpdateLocatie(blokId, locatie);
        }

        public void UpdateSize(int blokId, BlokGrootte blokGrootte)
        {
            InitNonExistingRepo();
            _repo.UpdateSize(blokId, blokGrootte);
        }

        public void UpdateTitel(int blokId, String titel)
        {
            InitNonExistingRepo();
            _repo.UpdateTitel(blokId, titel);
        }

        public void UpdateSizeDimensions(int blokId, int x, int y)
        {
            InitNonExistingRepo();
            _repo.UpdateSizeDimensions(blokId, x, y);
        }

        public Dashboard GetPublicDashboard(int id)
        {
            InitNonExistingRepo();
            return _repo.GetPublicDashboard(id);
        }

        public void SetPublic(int dashboardId, bool shared)
        {
            InitNonExistingRepo();
            _repo.SetPublic(dashboardId, shared);
        }

        public void UpdateConfiguratieTitle(int configuratieId, String title)
        {
            InitNonExistingRepo();
            _repo.UpdateConfiguratieTitle(configuratieId, title);
        }

        public List<CijferOpties> CreateCijferOpties(List<string> stringOpties)
        {
            List<CijferOpties> opties = new List<CijferOpties>();
            foreach (var optie in stringOpties)
            {
                CijferOpties o = new CijferOpties()
                {
                    Optie = optie
                };
                opties.Add(o);
            }

            return opties;
        }

        public void CreateDomainUser(string identityId, string email, string voornaam, string achternaam, DateTime geboorteDatum)
        {
            InitNonExistingRepo();
            Account domainAccount = new Account()
            {
                IdentityId = identityId,
                Email = email,
                Voornaam = voornaam,
                Achternaam = achternaam,
                GeboorteDatum = geboorteDatum,
                Dashboard = new Dashboard()
            };
            domainAccount.Dashboard.Configuratie = new DashboardConfiguratie();
            _accountRepository.AddUser(domainAccount);
        }
    }
}
