using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Domain.Entiteit;
using Domain.Enum;
using Domain.Post;
using System.Web;

using Domain.Account;

using Domain.TextGain;
using System.Collections;


namespace BL
{
    public class EntiteitManager : IEntiteitManager
    {
        private IEntiteitRepository entiteitRepository = new EntiteitRepository();
        private UnitOfWorkManager uowManager;

        public EntiteitManager()
        {

        }

        public EntiteitManager(UnitOfWorkManager uofMgr)
        {
            uowManager = uofMgr;
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
                    entiteitRepository = new EntiteitRepository(uowManager.UnitOfWork);
                }
            }
            // Als we niet met UoW willen werken, dan maken we een repo aan als die nog niet bestaat.
            else
            {
                entiteitRepository = (entiteitRepository == null) ? new EntiteitRepository() : entiteitRepository;
            }
        }

        public void CreateTestData()
        {
            initNonExistingRepo();
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


            entiteitRepository.AddEntiteit(NVA);
            entiteitRepository.AddEntiteit(OpenVLD);
            entiteitRepository.AddEntiteit(BenWeyts);
            entiteitRepository.AddEntiteit(Bartje);
            entiteitRepository.AddEntiteit(Maggie);
        }

        public List<Entiteit> getAlleEntiteiten()
        {
            initNonExistingRepo();
            return entiteitRepository.getAlleEntiteiten();
        }

        public List<Entiteit> getAlleEntiteiten(bool IncludePosts)
        {
            initNonExistingRepo();
            return entiteitRepository.getAlleEntiteiten(IncludePosts);
        }

        public Entiteit GetEntiteit(int id)
        {
            initNonExistingRepo();
            return entiteitRepository.ReadEntiteit(id);
        }

        public void updateEntiteit(Entiteit entiteit)
        {
            initNonExistingRepo();
            entiteitRepository.updateEntiteit(entiteit);
        }


        //deze methode roepen we aan om alle trends terug leeg te maken
        //dit zal na elke data sync moeten gebeuren want we kijken dagelijks of er nieuwe trends zijn en we willen geen gegevens van de vorige dag
        public void ResetTrends()
        {
            initNonExistingRepo();
            List<Entiteit> AlleEntiteiten = entiteitRepository.getAlleEntiteiten();
            foreach (var e in AlleEntiteiten)
            {
                e.Trends = null;
            }
        }

        public bool berekenTrends(double minVoorwaarde, Entiteit entiteit, TrendType type, Voorwaarde voorwaarde)
        {
            initNonExistingRepo();
            DateTime vandaag = DateTime.Today;
            DateTime gisteren = DateTime.Today.AddDays(-1);
            //DateTime vandaag = new DateTime(2018, 01, 14);
            //DateTime gisteren = new DateTime(2018, 01, 13);
            List<Post> AllePosts = entiteit.Posts;
            List<Post> PostsGisteren = AllePosts.Where(x => x.Date.Day == gisteren.Day).Where(x => x.Date.Month == gisteren.Month).Where(x => x.Date.Year == gisteren.Year).ToList();
            List<Post> PostsVandaag = AllePosts.Where(x => x.Date.Day == vandaag.Day).Where(x => x.Date.Month == vandaag.Month).Where(x => x.Date.Year == vandaag.Year).ToList();
            int AantalGisteren = PostsGisteren.Count;
            int AantalVandaag = PostsVandaag.Count;
            //We MOETEN entiteit even zelf ophalen zodat de context op de hoogte is van welke entiteit we gebruiken
            Entiteit e = getAlleEntiteiten().Single(x => x.EntiteitId == entiteit.EntiteitId);
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

                    foreach (var post in PostsGisteren)
                    {
                        sentimentGisteren += (post.Sentiment.polariteit * post.Sentiment.subjectiviteit) / AantalGisteren;
                    }

                    foreach (var post in PostsVandaag)
                    {
                        sentimentVandaag += (post.Sentiment.polariteit * post.Sentiment.subjectiviteit) / AantalVandaag;
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
                            updateEntiteit(e);
                            return true;
                        }
                    } else if(type == TrendType.DALEND)
                    {
                        sentimentVerschil = sentimentGisteren - sentimentVandaag;
                        newTrend.Type = TrendType.DALEND;
                        newTrend.Voorwaarde = Voorwaarde.SENTIMENT;
                        entiteit.Trends.Add(newTrend);
                        updateEntiteit(e);
                        return true;
                    }
                    break;
                case Voorwaarde.AANTALVERMELDINGEN:
                    newTrend.Voorwaarde = Voorwaarde.AANTALVERMELDINGEN;
                    if (type == TrendType.DALEND)
                    {
                        if ((AantalGisteren - AantalVandaag) >= minVoorwaarde)
                        {
                            return true;
                        }
                    }
                    if(type == TrendType.STIJGEND)
                    {
                        if ((AantalVandaag - AantalGisteren) >= minVoorwaarde)
                        {
                            return true;
                        }
                    }
                    if(type == TrendType.STERKOPWAARTS)
                    {
                        if ((AantalVandaag / AantalGisteren) >= trendVerandering)
                        {
                            if (entiteit.Trends == null)
                            {
                                entiteit.Trends = new List<Trend>();
                            }
                            entiteit.Trends.Add(newTrend);
                            updateEntiteit(e);
                            return true;
                        }
                    }
                    if(type == TrendType.STERKDALEND)
                    {
                        if ((AantalVandaag / AantalGisteren) <= trendVerandering)
                        {
                            if (entiteit.Trends == null)
                            {
                                entiteit.Trends = new List<Trend>();
                            }
                            entiteit.Trends.Add(newTrend);
                            updateEntiteit(e);
                            return true;
                        }
                    }
                    break;


                case Voorwaarde.TRENDING:
                    //de eventuele trend die aanwezig kan zijn juist zetten op de trend waarop we vergelijken
                    newTrend.Voorwaarde = Voorwaarde.TRENDING;
                    //if kijkt na of het aantal posts van gisteren tegenover vandaag met 50% is gestegen en er moet minstens een volume van 10 posts zijn
                    if ((AantalVandaag/AantalGisteren) > 1.5 && AantalGisteren >= 10 && AantalVandaag >= 10)
                    {
                        if (entiteit.Trends == null)
                        {
                            entiteit.Trends = new List<Trend>();
                        }
                        entiteit.Trends.Add(newTrend);
                        updateEntiteit(e);
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
        public Entiteit getEntiteit(int entiteitID)
        {
            initNonExistingRepo();
            return entiteitRepository.ReadEntiteit(entiteitID);
        }

        public void addEntiteit(Entiteit entiteit)
        {
            initNonExistingRepo();
            entiteitRepository.AddEntiteit(entiteit);
        }

        #region
        public void AddThema(Thema nieuwThema, List<Sleutelwoord> sleutelwoorden)
        {
            initNonExistingRepo();
            Thema thema = new Thema()
            {
                Naam = nieuwThema.Naam,
                PlatformId = nieuwThema.PlatformId,
                SleutenWoorden = sleutelwoorden
            };
            entiteitRepository.CreateThema(thema);
        }


        public void UpdateThema(Thema thema)
        {
            initNonExistingRepo();
            entiteitRepository.UpdateThema(thema);
        }

        public void DeleteThema(int entiteitsId)
        {
            initNonExistingRepo();
            entiteitRepository.DeleteThema(entiteitsId);
        }

        public IEnumerable<Thema> GetThemas(int platId)
        {
            initNonExistingRepo();
            return entiteitRepository.ReadThemas().Where(x => x.PlatformId == platId).ToList();
        }

        public Thema GetThema(int entiteitsId)
        {
            initNonExistingRepo();
            return entiteitRepository.ReadThema(entiteitsId);
        }

        public void UpdateGrafieken()
        {
          
        }

        public Dictionary<string, double> BerekenGrafiekWaarde(Domain.Enum.GrafiekType grafiekType, List<Entiteit> entiteiten, List<string> CijferOpties, string VergelijkOptie)
        {
            initNonExistingRepo();
            Dictionary<string, double> grafiekMap = new Dictionary<string, double>();

            switch (grafiekType)
            {
                case Domain.Enum.GrafiekType.CIJFERS:
                    Entiteit e1 = entiteitRepository.getAlleEntiteiten().Single(x => x.EntiteitId == entiteiten.First().EntiteitId);
                    List<Post> postsEerste = e1.Posts;
                    foreach (var cijferOptie in CijferOpties)
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
                    if(VergelijkOptie.ToLower() == "populariteit")
                    {
                        foreach (var e in entiteiten)
                        {
                            grafiekMap.Add("Post " + e.Naam, e.Posts.Count);
                        }
                    }
                    if(VergelijkOptie.ToLower() == "postfrequentie")
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
        public void AddPerson(Persoon p, HttpPostedFileBase ImageFile)
        {
            initNonExistingRepo();
            if (ImageFile != null)
            {
                entiteitRepository.CreatePersonWithPhoto(p, ImageFile);
            } else
            {
                entiteitRepository.CreatePersonWithoutPhoto(p);
            }
        }

        public Persoon ChangePerson(Persoon ChangedPerson)
        {
            initNonExistingRepo(false);
            Persoon toUpdated = GetPerson(ChangedPerson.EntiteitId);

            toUpdated.FirstName = ChangedPerson.FirstName;
            toUpdated.LastName = ChangedPerson.LastName;
            foreach (Organisatie o in toUpdated.Organisations)
            {
                ChangeOrganisatie(o);
            }
            return entiteitRepository.UpdatePerson(toUpdated);
        }

        public List<Persoon> GetAllPeople(int platId)
        {
            initNonExistingRepo();
            return entiteitRepository.ReadAllPeople().Where(x => x.PlatformId == platId).ToList();
        }

        public Persoon GetPerson(int id)
        {
            initNonExistingRepo(false);
            return entiteitRepository.ReadPerson(id);
        }

        public void RemovePerson(int id)
        {
            initNonExistingRepo(false);
            entiteitRepository.DeletePerson(id);
        }
        #endregion
        #region
        
        public void AddOrganisatie(Organisatie o, HttpPostedFileBase ImageFile)
        {
            initNonExistingRepo(false);
            if (ImageFile != null)
            {
                entiteitRepository.CreateOrganisatieWithPhoto(o, ImageFile);
            } else
            {
                entiteitRepository.CreateOrganisatieWithoutPhoto(o);
            }
        }

        public Organisatie ChangeOrganisatie(Organisatie ChangedOrganisatie)
        {
            initNonExistingRepo(false);
            Organisatie toUpdate = GetOrganisatie(ChangedOrganisatie.EntiteitId);
            toUpdate.Naam = ChangedOrganisatie.Naam;
            toUpdate.Gemeente = ChangedOrganisatie.Gemeente;
            toUpdate.Posts = ChangedOrganisatie.Posts;
            toUpdate.Trends = ChangedOrganisatie.Trends;

            return entiteitRepository.UpdateOrganisatie(ChangedOrganisatie);
        }

        public List<Organisatie> GetAllOrganisaties(int platId)
        {
            initNonExistingRepo(false);
            return entiteitRepository.ReadAllOrganisaties().Where(x => x.PlatformId == platId).ToList();
        }

        public Organisatie GetOrganisatie(int id)
        {
            initNonExistingRepo(false);
            return entiteitRepository.ReadOrganisatie(id);
        }

        public void RemoveOrganisatie(int id)
        {
            initNonExistingRepo(false);
            entiteitRepository.DeleteOrganisatie(id);
        }

        public Organisatie ChangeOrganisatie(Organisatie ChangedOrganisatie, IEnumerable<string> selectedPeople)
        {
            initNonExistingRepo(false);
            Organisatie toUpdate = GetOrganisatie(ChangedOrganisatie.EntiteitId);


            List<Persoon> NewlyAppointedPeople = new List<Persoon>();
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

            toUpdate.Naam = ChangedOrganisatie.Naam;
            toUpdate.Gemeente = ChangedOrganisatie.Gemeente;
            toUpdate.Posts = ChangedOrganisatie.Posts;
            toUpdate.Trends = ChangedOrganisatie.Trends;

            toUpdate.AantalLeden = toUpdate.Leden.Count();

            return entiteitRepository.UpdateOrganisatie(toUpdate);
        }

        public void ChangePerson(Persoon changedPerson, IEnumerable<string> selectedOrganisations)
        {
            initNonExistingRepo(false);
            Persoon toUpdated = GetPerson(changedPerson.EntiteitId);

            //Remove all references
            toUpdated.Organisations = new List<Organisatie>();

            //Add new References
            foreach (string oId in selectedOrganisations)
            {
                toUpdated.Organisations.Add(GetOrganisatie(Int32.Parse(oId)));
            }

            toUpdated.FirstName = changedPerson.FirstName;
            toUpdated.LastName = changedPerson.LastName;

            entiteitRepository.UpdatePerson(toUpdated);
        }

        public byte[] GetPersonImageFromDataBase(int id)
        {
            initNonExistingRepo(false);
            return entiteitRepository.GetPersonImageFromDataBase(id);
        }

        public byte[] GetOrganisationImageFromDataBase(int id)
        {
            initNonExistingRepo(false);
            return entiteitRepository.GetOrganisationImageFromDataBase(id);
        }

        public void DeleteSleutelwoord(int sleutelId)
        {
            initNonExistingRepo();
            entiteitRepository.DeleteSleutelwoord(sleutelId);
        }

        public Sleutelwoord GetSleutelwoord(int sleutelId)
        {
            initNonExistingRepo();
            return entiteitRepository.readSleutelwoord(sleutelId);
        }

        public List<Entiteit> GetEntiteitenVanDeelplatform(int id)
        {
            initNonExistingRepo();
            return entiteitRepository.ReadEntiteitenVanDeelplatform(id).ToList();
        }

        public void DeleteEntiteitenVanDeelplatform(int id)
        {
            initNonExistingRepo();
            entiteitRepository.DeleteEntiteitenVanDeelplatform(id);
        }

        public void AddThema(string naam, List<Sleutelwoord> sleutelwoorden)
        {
            throw new NotImplementedException();
        }

        public List<Entiteit> GetEntiteiten(string naam)
        {
            initNonExistingRepo();
            return entiteitRepository.ReadEntiteiten(naam);
        }
        #endregion

        public List<Entiteit> ZoekEntiteiten(string zoek)
        {
            initNonExistingRepo();
            List<Entiteit> gevondeEntiteiten = new List<Entiteit>();
            foreach (var e in entiteitRepository.getAlleEntiteiten())
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
                if(jsonE.Organisations == null)
                {
                    jsonE.Organisations = new List<Organisatie>();
                }
                if(GetAllOrganisaties(jsonE.PlatformId).FirstOrDefault(x => x.Naam.ToLower() == jsonE.Organisation.ToLower()) == null)
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
                    if(o.Naam.ToLower() == jsonE.Organisation.ToLower())
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
            initNonExistingRepo();
            List<Entiteit> alleEntiteiten = entiteitRepository.getAlleEntiteiten();
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
                entiteitRepository.updateEntiteit(e);
            }
        }
      
    }
}
