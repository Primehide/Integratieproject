using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DAL;
using Domain.Entiteit;
using Domain.Enum;
using Domain.Post;
using Newtonsoft.Json;

namespace BL.Managers
{
    public class EntiteitManager : IEntiteitManager
    {
        private IEntiteitRepository _entiteitRepository = new EntiteitRepository();
        private UnitOfWorkManager _uowManager;

        public EntiteitManager()
        {

        }

        public EntiteitManager(UnitOfWorkManager uofMgr)
        {
            _uowManager = uofMgr;
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
                    _entiteitRepository = new EntiteitRepository(_uowManager.UnitOfWork);
                }
            }
            // Als we niet met UoW willen werken, dan maken we een repo aan als die nog niet bestaat.
            else
            {
                _entiteitRepository = (_entiteitRepository == null) ? new EntiteitRepository() : _entiteitRepository;
            }
        }

        public void CreateTestData()
        {
            InitNonExistingRepo();
            Domain.Entiteit.Organisatie NVA = new Domain.Entiteit.Organisatie()
            {
                Leden = new List<Domain.Entiteit.Persoon>(),
                Naam = "N-VA",
                PlatformId = 1
            };

            Domain.Entiteit.Organisatie OpenVLD = new Domain.Entiteit.Organisatie()
            {
                Leden = new List<Domain.Entiteit.Persoon>(),
                Naam = "Open-VLD",
                PlatformId = 1
            };

            Domain.Entiteit.Persoon BenWeyts = new Domain.Entiteit.Persoon()
            {
                Naam = "Ben Weyts",
                Organisations = new List<Domain.Entiteit.Organisatie>(),
                Trends = new List<Trend>(),
                PlatformId = 1
            };

            Domain.Entiteit.Persoon Maggie = new Domain.Entiteit.Persoon()
            {
                Naam = "Maggie De Block",
                Organisations = new List<Domain.Entiteit.Organisatie>(),
                Trends = new List<Trend>(),
                PlatformId = 1
            };


            Trend trend = new Trend()
            {
                Voorwaarde = Voorwaarde.KEYWORDS
            };
            BenWeyts.Trends.Add(trend);

            Trend trend2 = new Trend()
            {
                Voorwaarde = Voorwaarde.TRENDING
            };

            Domain.Entiteit.Persoon Bartje = new Domain.Entiteit.Persoon()
            {
                Naam = "Bart De Wever",
                Organisations = new List<Domain.Entiteit.Organisatie>(),
                Trends = new List<Trend>(),
                PlatformId = 1
            };
            Bartje.Trends.Add(trend2);

            //legt eveneens relatie van organisatie -> lid (Ben Weyts) en van Ben Weyts kunnen we zijn orginasaties opvragen (in dit geval N-VA)
            BenWeyts.Organisations.Add(NVA);
            Bartje.Organisations.Add(NVA);
            Maggie.Organisations.Add(OpenVLD);


            _entiteitRepository.AddEntiteit(NVA);
            _entiteitRepository.AddEntiteit(OpenVLD);
            _entiteitRepository.AddEntiteit(BenWeyts);
            _entiteitRepository.AddEntiteit(Bartje);
            _entiteitRepository.AddEntiteit(Maggie);
        }

        public List<Entiteit> GetAlleEntiteiten()
        {
            InitNonExistingRepo();
            return _entiteitRepository.getAlleEntiteiten();
        }

        public List<Entiteit> GetAlleEntiteiten(bool includePosts)
        {
            InitNonExistingRepo();
            return _entiteitRepository.getAlleEntiteiten(includePosts);
        }

        public Entiteit GetEntiteit(int id)
        {
            InitNonExistingRepo();
            return _entiteitRepository.ReadEntiteit(id);
        }

        public void UpdateEntiteit(Entiteit entiteit)
        {
            InitNonExistingRepo();
            _entiteitRepository.updateEntiteit(entiteit);
        }


        //deze methode roepen we aan om alle trends terug leeg te maken
        //dit zal na elke data sync moeten gebeuren want we kijken dagelijks of er nieuwe trends zijn en we willen geen gegevens van de vorige dag
        public void ResetTrends()
        {
            InitNonExistingRepo();
            List<Entiteit> alleEntiteiten = _entiteitRepository.getAlleEntiteiten();
            foreach (var e in alleEntiteiten)
            {
                e.Trends = null;
            }
        }

        public bool BerekenTrends(double minVoorwaarde, Entiteit entiteit, TrendType type, Voorwaarde voorwaarde)
        {
            InitNonExistingRepo();
            DateTime vandaag = DateTime.Today;
            DateTime gisteren = DateTime.Today.AddDays(-1);
            //DateTime vandaag = new DateTime(2018, 01, 14);
            //DateTime gisteren = new DateTime(2018, 01, 13);
            List<Post> allePosts = entiteit.Posts;
            List<Post> postsGisteren = allePosts.Where(x => x.Date.Day == gisteren.Day).Where(x => x.Date.Month == gisteren.Month).Where(x => x.Date.Year == gisteren.Year).ToList();
            List<Post> postsVandaag = allePosts.Where(x => x.Date.Day == vandaag.Day).Where(x => x.Date.Month == vandaag.Month).Where(x => x.Date.Year == vandaag.Year).ToList();
            int aantalGisteren = postsGisteren.Count;
            int aantalVandaag = postsVandaag.Count;
            //We MOETEN entiteit even zelf ophalen zodat de context op de hoogte is van welke entiteit we gebruiken
            Entiteit e = GetAlleEntiteiten().Single(x => x.EntiteitId == entiteit.EntiteitId);
            Trend newTrend = new Trend();
            double trendVerandering = 1.3;

            //controle of trend al bestaat, zoja moeten we de berekening niet maken
            foreach (var trend in e.Trends)
            {
                if (trend.Type == type)
                {
                    return true;
                }
            }

            //PRESET voor berekening juist zetten
            switch (type)
            {
                case TrendType.STERKOPWAARTS:
                    trendVerandering = 1.3;
                    newTrend.Type = TrendType.STERKOPWAARTS;
                    break;
                case TrendType.MATIGOPWAARDS:
                    trendVerandering = 1.1;
                    newTrend.Type = TrendType.MATIGOPWAARDS;
                    break;
                case TrendType.MATIGDALEND:
                    trendVerandering = 0.9;
                    newTrend.Type = TrendType.MATIGDALEND;
                    break;
                case TrendType.STERKDALEND:
                    trendVerandering = 0.7;
                    newTrend.Type = TrendType.STERKDALEND;
                    break;
            }


            switch (voorwaarde)
            {
                case Voorwaarde.SENTIMENT:
                    double sentimentGisteren = 0;
                    double sentimentVandaag = 0;

                    foreach (var post in postsGisteren)
                    {
                        sentimentGisteren += (post.Sentiment.polariteit * post.Sentiment.subjectiviteit) / aantalGisteren;
                    }

                    foreach (var post in postsVandaag)
                    {
                        sentimentVandaag += (post.Sentiment.polariteit * post.Sentiment.subjectiviteit) / aantalVandaag;
                    }
                    double sentimentVerschil = 0;

                    if (type == TrendType.STIJGEND)
                    {
                        sentimentVerschil = sentimentVandaag - sentimentGisteren;
                        if (sentimentVerschil >= minVoorwaarde)
                        {
                            newTrend.Type = TrendType.STIJGEND;
                            newTrend.Voorwaarde = Voorwaarde.SENTIMENT;
                            entiteit.Trends.Add(newTrend);
                            UpdateEntiteit(e);
                            return true;
                        }
                    } else if(type == TrendType.DALEND)
                    {
                        sentimentVerschil = sentimentGisteren - sentimentVandaag;
                        newTrend.Type = TrendType.DALEND;
                        newTrend.Voorwaarde = Voorwaarde.SENTIMENT;
                        entiteit.Trends.Add(newTrend);
                        UpdateEntiteit(e);
                        return true;
                    }
                    break;
                case Voorwaarde.AANTALVERMELDINGEN:
                    newTrend.Voorwaarde = Voorwaarde.AANTALVERMELDINGEN;
                    if (type == TrendType.DALEND)
                    {
                        if ((aantalGisteren - aantalVandaag) >= minVoorwaarde)
                        {
                            return true;
                        }
                    }
                    if(type == TrendType.STIJGEND)
                    {
                        if ((aantalVandaag - aantalGisteren) >= minVoorwaarde)
                        {
                            return true;
                        }
                    }
                    if(type == TrendType.STERKOPWAARTS)
                    {
                        if ((aantalVandaag / aantalGisteren) >= trendVerandering)
                        {
                            if (entiteit.Trends == null)
                            {
                                entiteit.Trends = new List<Trend>();
                            }
                            entiteit.Trends.Add(newTrend);
                            UpdateEntiteit(e);
                            return true;
                        }
                    }
                    if(type == TrendType.STERKDALEND)
                    {
                        if ((aantalVandaag / aantalGisteren) <= trendVerandering)
                        {
                            if (entiteit.Trends == null)
                            {
                                entiteit.Trends = new List<Trend>();
                            }
                            entiteit.Trends.Add(newTrend);
                            UpdateEntiteit(e);
                            return true;
                        }
                    }
                    break;


                case Voorwaarde.TRENDING:
                    //de eventuele trend die aanwezig kan zijn juist zetten op de trend waarop we vergelijken
                    newTrend.Voorwaarde = Voorwaarde.TRENDING;
                    //if kijkt na of het aantal posts van gisteren tegenover vandaag met 50% is gestegen en er moet minstens een volume van 10 posts zijn
                    if ((aantalVandaag/aantalGisteren) > 1.5 && aantalGisteren >= 10 && aantalVandaag >= 10)
                    {
                        if (entiteit.Trends == null)
                        {
                            entiteit.Trends = new List<Trend>();
                        }
                        entiteit.Trends.Add(newTrend);
                        UpdateEntiteit(e);
                        return true;
                    }
                    break;
                case Voorwaarde.KEYWORDS:
                    break;
                default:
                    break;
            }
            //als we hier komen is er geen trend aanwezig.
            return false;
        }

        public void AddEntiteit(Entiteit entiteit)
        {
            InitNonExistingRepo();
            _entiteitRepository.AddEntiteit(entiteit);
        }

        #region
        public void AddThema(Thema nieuwThema, List<Sleutelwoord> sleutelwoorden, HttpPostedFileBase imageFile)
        {
            InitNonExistingRepo();
            Thema thema = new Thema()
            {
                Naam = nieuwThema.Naam,
                PlatformId = nieuwThema.PlatformId,
                SleutenWoorden = sleutelwoorden
            
              
            };
            _entiteitRepository.CreateThema(thema, imageFile);
        }


        public void UpdateThema(Thema thema, HttpPostedFileBase imageFile)
        {
            InitNonExistingRepo(false);
            Thema toUpdate = GetThema(thema.EntiteitId);
            if (imageFile != null)
            {
                EntiteitRepository repo = new EntiteitRepository();
                toUpdate.Image = repo.ConvertToBytes(imageFile);

            }
            toUpdate.SleutenWoorden = thema.SleutenWoorden;
            toUpdate.Naam = thema.Naam;
            
            _entiteitRepository.UpdateThema(toUpdate);
        }

        public void DeleteThema(int entiteitsId)
        {
            InitNonExistingRepo();
            _entiteitRepository.DeleteThema(entiteitsId);
        }

        public IEnumerable<Thema> GetThemas(int platId)
        {
            InitNonExistingRepo();
            return _entiteitRepository.ReadThemas().Where(x => x.PlatformId == platId).ToList();
        }

        public Thema GetThema(int entiteitsId)
        {
            InitNonExistingRepo();
            return _entiteitRepository.ReadThema(entiteitsId);
        }

        public void UpdateGrafieken()
        {
          
        }

        public Dictionary<string, double> BerekenGrafiekWaarde(Domain.Enum.GrafiekType grafiekType, List<Entiteit> entiteiten, List<string> cijferOpties, string vergelijkOptie)
        {
            InitNonExistingRepo();
            Dictionary<string, double> grafiekMap = new Dictionary<string, double>();

            switch (grafiekType)
            {
                case Domain.Enum.GrafiekType.CIJFERS:
                    Entiteit e1 = _entiteitRepository.getAlleEntiteiten().Single(x => x.EntiteitId == entiteiten.First().EntiteitId);
                    List<Post> postsEerste = e1.Posts;
                    foreach (var cijferOptie in cijferOpties)
                    {
                        if(cijferOptie.ToLower() == "aantalposts")
                        {
                            int aantalPosts = postsEerste.Count;
                            grafiekMap.Add("Aantal posts", aantalPosts);
                        }
                        if(cijferOptie.ToLower() == "aantalretweets")
                        {
                            int retweets = postsEerste.Where(x => x.retweet == true).Count();
                            grafiekMap.Add("Aantal retweets", retweets);
                        }
                        if(cijferOptie.ToLower() == "aanwezigetrends")
                        {
                            foreach (var trend in e1.Trends)
                            {
                                switch (trend.Voorwaarde)
                                {
                                    case Voorwaarde.SENTIMENT:
                                        grafiekMap.Add("Trend Sentiment", 1);
                                        break;
                                    case Voorwaarde.AANTALVERMELDINGEN:
                                        grafiekMap.Add("Trend Aantal Vermeldingen", 1);
                                        break;
                                    case Voorwaarde.TRENDING:
                                        grafiekMap.Add("Trend trending", 1);
                                        break;
                                    case Voorwaarde.KEYWORDS:
                                        grafiekMap.Add("Trend Keywords", 1);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    break;
                case GrafiekType.VERGELIJKING:
                    if(vergelijkOptie.ToLower() == "populariteit")
                    {
                        foreach (var e in entiteiten)
                        {
                            grafiekMap.Add("Post " + e.Naam, e.Posts.Count);
                        }
                    }
                    if(vergelijkOptie.ToLower() == "postfrequentie")
                    {
                        DateTime today = DateTime.Today;
                        int counter = 0;
                        foreach (var e in entiteiten)
                        {

                            for(int i = 10; i > 0; i--)
                            {
                                List<Post> postsHuidigeDag = e.Posts.Where(x => x.Date.Date == today.AddDays(-i).Date).ToList();
                                grafiekMap.Add("Posts" + i + " " + e.Naam, postsHuidigeDag.Count);
                            }
                            grafiekMap.Add("EndPostFrequentie" + counter,counter);
                            counter++;
                        }
                    }
                    break;
            }
            return grafiekMap;
        }

#endregion
        #region
        public void AddPerson(Persoon p, HttpPostedFileBase imageFile)
        {
            InitNonExistingRepo();
            if (imageFile != null)
            {
                _entiteitRepository.CreatePersonWithPhoto(p, imageFile);
            } else
            {
                _entiteitRepository.CreatePersonWithoutPhoto(p);
            }
        }

        public Persoon ChangePerson(Persoon changedPerson,  HttpPostedFileBase imageFile)
        {
          
            InitNonExistingRepo(false);
            Persoon toUpdate = GetPerson(changedPerson.EntiteitId);
            if (imageFile != null)
            {
                EntiteitRepository repo = new EntiteitRepository();
                toUpdate.Image = repo.ConvertToBytes(imageFile);

            }
            toUpdate.FirstName = changedPerson.FirstName;
            toUpdate.LastName = changedPerson.LastName;
            toUpdate.Naam = changedPerson.FirstName + " " + changedPerson.LastName;
            foreach (Organisatie o in toUpdate.Organisations)
            {
                ChangeOrganisatie(o, imageFile);
            }
            return _entiteitRepository.UpdatePerson(toUpdate);
        }

        public List<Persoon> GetAllPeople(int platId)
        {
            InitNonExistingRepo();
            return _entiteitRepository.ReadAllPeople().Where(x => x.PlatformId == platId).ToList();
        }

        public Persoon GetPerson(int id)
        {
            InitNonExistingRepo(false);
            return _entiteitRepository.ReadPerson(id);
        }

        public void RemovePerson(int id)
        {
            InitNonExistingRepo(false);
            _entiteitRepository.DeletePerson(id);
        }
        #endregion
        #region
        
        public void AddOrganisatie(Organisatie o, HttpPostedFileBase imageFile)
        {
            InitNonExistingRepo(false);
            if (imageFile != null)
            { 
            
                _entiteitRepository.CreateOrganisatieWithPhoto(o, imageFile);
            } else
            {
                _entiteitRepository.CreateOrganisatieWithoutPhoto(o);
            }
        }

        public Organisatie ChangeOrganisatie(Organisatie changedOrganisatie, HttpPostedFileBase imageFile)
        {
            InitNonExistingRepo(false);
      
            Organisatie toUpdate = GetOrganisatie(changedOrganisatie.EntiteitId);
            if (imageFile != null)
            {
                EntiteitRepository repo = new EntiteitRepository();
                toUpdate.Image = repo.ConvertToBytes(imageFile);

            }
            toUpdate.Naam = changedOrganisatie.Naam;
            toUpdate.Gemeente = changedOrganisatie.Gemeente;
            toUpdate.Posts = changedOrganisatie.Posts;
            toUpdate.Trends = changedOrganisatie.Trends;

            return _entiteitRepository.UpdateOrganisatie(changedOrganisatie);
        }

        public List<Organisatie> GetAllOrganisaties(int platId)
        {
            InitNonExistingRepo(false);
            return _entiteitRepository.ReadAllOrganisaties().Where(x => x.PlatformId == platId).ToList();
        }

        public Organisatie GetOrganisatie(int id)
        {
            InitNonExistingRepo(false);
            return _entiteitRepository.ReadOrganisatie(id);
        }

        public void RemoveOrganisatie(int id)
        {
            InitNonExistingRepo(false);
            _entiteitRepository.DeleteOrganisatie(id);
        }

        public Organisatie ChangeOrganisatie(Organisatie changedOrganisatie, IEnumerable<string> selectedPeople, HttpPostedFileBase imageFile)
        {
            InitNonExistingRepo(false);
            Organisatie toUpdate = GetOrganisatie(changedOrganisatie.EntiteitId);


            List<Persoon> newlyAppointedPeople = new List<Persoon>();
            //Bestaande referenties verwijderen
            if (toUpdate.Leden != null)
            {

                foreach (Persoon p in toUpdate.Leden)
                {
                    p.Organisations.Remove(toUpdate);
                }
                toUpdate.Leden = new List<Persoon>();
            }

            //Nieuwe referenties toevoegen
            foreach (string pId in selectedPeople)
            {
                Persoon person = GetPerson(Int32.Parse(pId));
                //person.Organisations.Add(UpdatedOrganisatie);
                toUpdate.Leden.Add(person);
            }

            toUpdate.Naam = changedOrganisatie.Naam;
            toUpdate.Gemeente = changedOrganisatie.Gemeente;
            toUpdate.Posts = changedOrganisatie.Posts;
            toUpdate.Trends = changedOrganisatie.Trends;
            toUpdate.AantalLeden = toUpdate.Leden.Count();
            
            return _entiteitRepository.UpdateOrganisatie(toUpdate);
        }

        public void ChangePerson(Persoon changedPerson, IEnumerable<string> selectedOrganisations, HttpPostedFileBase ImageFile)
        {
            InitNonExistingRepo(false);
            Persoon toUpdated = GetPerson(changedPerson.EntiteitId);

            //Remove all references
            toUpdated.Organisations = new List<Organisatie>();
            if (ImageFile != null)
            {
                EntiteitRepository repo = new EntiteitRepository();
                toUpdated.Image = repo.ConvertToBytes(ImageFile);

            }
            //Add new References
            foreach (string oId in selectedOrganisations)
            {
                toUpdated.Organisations.Add(GetOrganisatie(Int32.Parse(oId)));
            }

            toUpdated.FirstName = changedPerson.FirstName;
            toUpdated.LastName = changedPerson.LastName;

            _entiteitRepository.UpdatePerson(toUpdated);
        }

        public byte[] GetPersonImageFromDataBase(int id)
        {
            InitNonExistingRepo(false);
            return _entiteitRepository.GetPersonImageFromDataBase(id);
        }

        public byte[] GetOrganisationImageFromDataBase(int id)
        {
            InitNonExistingRepo(false);
            return _entiteitRepository.GetOrganisationImageFromDataBase(id);
        }

        public void DeleteSleutelwoord(int sleutelId)
        {
            InitNonExistingRepo();
            _entiteitRepository.DeleteSleutelwoord(sleutelId);
        }

        public Sleutelwoord GetSleutelwoord(int sleutelId)
        {
            InitNonExistingRepo();
            return _entiteitRepository.readSleutelwoord(sleutelId);
        }

        public List<Entiteit> GetEntiteitenVanDeelplatform(int id)
        {
            InitNonExistingRepo();
            return _entiteitRepository.ReadEntiteitenVanDeelplatform(id).ToList();
        }

        public void DeleteEntiteitenVanDeelplatform(int id)
        {
            InitNonExistingRepo();
            _entiteitRepository.DeleteEntiteitenVanDeelplatform(id);
        }

        public List<Entiteit> GetEntiteiten(string naam)
        {
            InitNonExistingRepo();
            return _entiteitRepository.ReadEntiteiten(naam);
        }
        #endregion

        public List<Entiteit> ZoekEntiteiten(string zoek)
        {
            InitNonExistingRepo();
            List<Entiteit> gevondeEntiteiten = new List<Entiteit>();
            foreach (var e in _entiteitRepository.getAlleEntiteiten())
            {
                if (e.Naam.ToLower().Contains(zoek.ToLower()))
                {
                    gevondeEntiteiten.Add(e);
                }
            }
            return gevondeEntiteiten;
        }

        public void ConvertJsonToEntiteit(List<Persoon> jsonEntiteiten)
        {
            foreach (var jsonE in jsonEntiteiten)
            {

                if (GetAllOrganisaties(0).FirstOrDefault(x => x.Naam.ToLower() == jsonE.Organisation.ToLower()) == null)
                {
                    jsonE.Organisations = new List<Organisatie>();
                }

                if (GetAllOrganisaties(jsonE.PlatformId).FirstOrDefault(x => x.Naam.ToLower() == jsonE.Organisation.ToLower()) == null)
                {
                    Organisatie organisatie = new Organisatie()
                    {
                        Naam = jsonE.Organisation,
                        Gemeente = jsonE.Disctrict,
                        PlatformId = jsonE.PlatformId
                    };
                    AddOrganisatie(organisatie, null);
                }

                foreach (var o in GetAllOrganisaties(jsonE.PlatformId))
                {
                    if(String.Equals(o.Naam, jsonE.Organisation, StringComparison.CurrentCultureIgnoreCase))
                    {
                        jsonE.Organisations.Add(o);
                    }
                }
                jsonE.Naam = jsonE.Full_name;
                AddPerson(jsonE, null);
            }
        }


        public void BerekenVasteGrafiekenAlleEntiteiten()
        {
            InitNonExistingRepo();
            List<Entiteit> alleEntiteiten = _entiteitRepository.getAlleEntiteiten();
            DateTime vandaag = new DateTime(2018, 04, 01);
            foreach (var e in alleEntiteiten)
            {
                Grafiek postFrequentie = new Grafiek()
                {
                    Naam = "Post Frequentie - " + e.Naam,
                    Waardes = new List<GrafiekWaarde>()
                };
                vandaag = new DateTime(2018, 04, 01);
                for (int i=0; i < 30; i++)
                {
                    GrafiekWaarde waarde = new GrafiekWaarde();
                    waarde.Naam = vandaag.ToShortDateString();
                    waarde.Waarde = e.Posts.Where(x => x.Date.Date == vandaag.Date).Count();
                    vandaag = vandaag.AddDays(1);
                    postFrequentie.Waardes.Add(waarde);
                }
                e.Grafieken.Clear();
                e.Grafieken.Add(postFrequentie);
                _entiteitRepository.updateEntiteit(e);
            }
        }
     
        public Dictionary<Entiteit, string> FillEntiteiten()
        {
            Dictionary<Entiteit, string> naamType = new Dictionary<Entiteit, string>();
            ArrayList namen = new ArrayList();

            List<Entiteit> entiteits = new List<Entiteit>();

            EntiteitManager mgr = new EntiteitManager();
            entiteits = mgr.GetEntiteitenVanDeelplatform((int)System.Web.HttpContext.Current.Session["PlatformID"]);
            if (naamType.Count == 0)
            {
                foreach (Entiteit entiteit in entiteits)
                {
                    if (entiteit is Persoon)
                    {
                        naamType.Add(entiteit, "Persoon");
                    }
                    if (entiteit is Organisatie)
                    {
                        naamType.Add(entiteit, "Organisatie");
                    }
                    if (entiteit is Thema)
                    {
                        naamType.Add(entiteit, "Thema");
                    }



                }
            }
            naamType.ToList().ForEach(x => namen.Add(x.Key.Naam));
            //ViewBag.Namen = namen;
            return naamType;
        }

        public void Upload(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string str = (new StreamReader(file.InputStream)).ReadToEnd();
                List<Persoon> jsonEntiteiten = JsonConvert.DeserializeObject<List<Persoon>>(str);
                foreach (var p in jsonEntiteiten)
                {
                    p.PlatformId = (int)HttpContext.Current.Session["PlatformID"];
                }
                ConvertJsonToEntiteit(jsonEntiteiten);
            }
        }

        public List<Sleutelwoord> GetSleutelwoorden(string woorden)
        {
            string[] split = woorden.Split(',');
            List<Sleutelwoord> sleutelWoorden = new List<Sleutelwoord>();
            foreach (var woord in split)
            {
                Sleutelwoord sleutelwoord = new Sleutelwoord {woord = woord};
                sleutelWoorden.Add(sleutelwoord);
            }

            return sleutelWoorden;
        }

        public Dictionary<String, int> GetSentimenten(List<Post> posts)
        {
            Dictionary<String, int> result = new Dictionary<string, int>();
            List<Double> polariteitPositief = new List<double>();
            foreach (Post post in posts)
            {
                if (post.Sentiment.polariteit >= 0)
                {
                    double waarde = post.Sentiment.polariteit;
                    polariteitPositief.Add(waarde);
                }

            }

            int totaal = posts.Count;
            int polariteitNegatiefCount = posts.Count - polariteitPositief.Count;
            int polariteitPositiefCount = polariteitPositief.Count;

            result.Add("totaal", totaal);
            result.Add("polariteitNegatiefCount", polariteitNegatiefCount);
            result.Add("polariteitPositiefCount", polariteitPositiefCount);
            
            return result;
        }

        public List<Sleutelwoord> AddSleutelWoordenToLijst(List<Sleutelwoord> sleutelWoorden)
        {
            string woorden = sleutelWoorden[0].woord;
            string[] split = woorden.Split(',');
            List<Sleutelwoord> mijnList = new List<Sleutelwoord>();
            foreach (string woord in split)
            {
                Sleutelwoord sleutelwoord = new Sleutelwoord(woord);
                mijnList.Add(sleutelwoord);
            }

            return mijnList;
        }

        public Thema AddSleutelWoordenToThema(Thema thema, List<Sleutelwoord> sleutelwoorden)
        {
            string woorden = sleutelwoorden[0].woord;
            if (woorden != null)
            {
                string[] split = woorden.Split(',');
                List<Sleutelwoord> mijnList = thema.SleutenWoorden;
                foreach (string woord in split)
                {
                    Sleutelwoord sleutelwoord = new Sleutelwoord(woord);
                    mijnList.Add(sleutelwoord);
                }
                thema.SleutenWoorden = mijnList;
            }

            return thema;
        }
    }
}
