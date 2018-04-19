using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Domain.Entiteit;
using Domain.Enum;
using Domain.Post;

namespace BL
{
    public class EntiteitManager : IEntiteitManager
    {
        private IEntiteitRepository entiteitRepository;
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
            initNonExistingRepo(false);
            Domain.Entiteit.Organisatie NVA = new Domain.Entiteit.Organisatie()
            {
                Leden = new List<Domain.Entiteit.Persoon>(),
                Naam = "N-VA"
            };

            Domain.Entiteit.Persoon BenWeyts = new Domain.Entiteit.Persoon()
            {
                Naam = "Ben Weyts",
                Organisaties = new List<Domain.Entiteit.Organisatie>()
            };

            Domain.Entiteit.Persoon Bartje = new Domain.Entiteit.Persoon()
            {
                Naam = "Bart De Wever",
                Organisaties = new List<Domain.Entiteit.Organisatie>()
            };

            //legt eveneens relatie van organisatie -> lid (Ben Weyts) en van Ben Weyts kunnen we zijn orginasaties opvragen (in dit geval N-VA)
            BenWeyts.Organisaties.Add(NVA);
            Bartje.Organisaties.Add(NVA);


            entiteitRepository.AddEntiteit(NVA);
            entiteitRepository.AddEntiteit(BenWeyts);
            entiteitRepository.AddEntiteit(Bartje);
        }

        public List<Entiteit> getAlleEntiteiten()
        {
            initNonExistingRepo(false);
            return entiteitRepository.getAlleEntiteiten();
        }

        public void updateEntiteit(Entiteit entiteit)
        {
            initNonExistingRepo(false);
            entiteitRepository.updateEntiteit(entiteit);
        }

        //deze methode roepen we aan om alle trends terug leeg te maken
        //dit zal na elke data sync moeten gebeuren want we kijken dagelijks of er nieuwe trends zijn en we willen geen gegevens van de vorige dag
        private void ResetTrends()
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
                case TrendType.STERKOPWAARDS:
                    trendVerandering = 1.3;
                    newTrend.Type = TrendType.STERKOPWAARDS;
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
                    if(type == TrendType.STERKOPWAARDS)
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
    }
}
