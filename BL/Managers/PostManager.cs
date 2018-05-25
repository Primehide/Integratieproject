using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BL.Interfaces;
using DAL.Interfaces;
using DAL.Repositories;
using Domain.Entiteit;
<<<<<<< HEAD
using Domain.Post;
using Domain.TextGain;
using Newtonsoft.Json;
=======
using System.Globalization;
using System.IO;
using BL.Managers;
using DAL.Repositories;
>>>>>>> master

namespace BL.Managers
{
    public class PostManager : IPostManager
    {
        private IPostRepository _postRepository;
        private UnitOfWorkManager _uowManager;

        public PostManager()
        {
            _postRepository = new PostRepository();
        }

        public PostManager(UnitOfWorkManager uofMgr)
        {
            _postRepository = new PostRepository();
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
                    _postRepository = new PostRepository(_uowManager.UnitOfWork);
                }
            }
            // Als we niet met UoW willen werken, dan maken we een repo aan als die nog niet bestaat.
            else
            {
                _postRepository = (_postRepository == null) ? new PostRepository() : _postRepository;
            }
        }

        public void AddPost(Post post)
        {
            InitNonExistingRepo();
            _postRepository.AddPost(post);
        }
        public List<String> GetTopPersonWords(Persoon person)
        {
            EntiteitRepository erepo = new EntiteitRepository();
            Entiteit entiteit = erepo.ReadEntiteit(person.EntiteitId);
            InitNonExistingRepo();
            List<Post> posts = person.Posts;
            List<Word> persoonWords = new List<Word>();
           foreach(Post post in posts)
            {
               persoonWords.AddRange( _postRepository.GetAllWordsFromPost(post));
            }

            Dictionary<string, int> wordCountDic = new Dictionary<string, int>();
            foreach (Word item in persoonWords)
            {
                if (!wordCountDic.ContainsKey(item.word))
                {
                    wordCountDic.Add(item.word, 1);
                }
                else
                {
                    wordCountDic.TryGetValue(item.word, out var count);
                    wordCountDic.Remove(item.word);
                    wordCountDic.Add(item.word, count + 1);
                }
            }


            var sortedDict = wordCountDic.OrderByDescending(entry => entry.Value)
                   .Take(10)
                   .ToDictionary(pair => pair.Key, pair => pair.Value);
            var values = sortedDict.Keys.ToList();

            return values;
        
        }

        public List<Post> GetAllPosts()
        {
            InitNonExistingRepo();
            return _postRepository.GetAllPosts();
        }
        public List<Mention> GetAllMentions()
        {
            InitNonExistingRepo();
            List<Mention> allMentions = new List<Mention>();
            _postRepository.GetAllPosts().ForEach(x => allMentions.AddRange(x.Mentions));
            return allMentions;
        }


        public int GetAantalMentions(Persoon persoon)
        {
            string naam = persoon.Naam;
           naam=  naam.Replace(" ", "");

        
            return GetAllMentions().Count(x => x.mention.ToLower() == naam.ToLower());

        }
        public async Task SyncDataAsync(int platformid)
        {
            InitNonExistingRepo(true);
            EntiteitManager entiteitManager = new EntiteitManager(_uowManager);
            //Sync willen we datum van vandaag en gisteren.
            DateTime vandaag = DateTime.Today.Date;
            DateTime gisteren = DateTime.Today.AddDays(-30).Date;
            List<Persoon> allePersonen = entiteitManager.GetAllPeople(platformid);

            //Enkele test entiteiten, puur voor debug, later vragen we deze op uit onze repository//
            /*
            List<Domain.Entiteit.Persoon> AllePersonen = entiteitManager.GetAllPeople(0);

         /*  PostRequest postRequest1 = new PostRequest()
            {
               since = gisteren,
               until = vandaag
           };

            using (HttpClient http = new HttpClient())
            {
                string uri = "https://kdg.textgain.com/query";
                http.DefaultRequestHeaders.Add("X-API-Key", "aEN3K6VJPEoh3sMp9ZVA73kkr");
               // var myContent = JsonConvert.SerializeObject(postRequest1);
                //var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                //var byteContent = new ByteArrayContent(buffer);
                //byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var result = await http.PostAsync(uri,null).Result.Content.ReadAsStringAsync();
                try
                {
                    var posts = JsonConvert.DeserializeObject<List<TextGainResponse>>(result);
                    if (posts.Count != 0)
                    {
                       // ConvertAndSaveToDb(posts);

                      //  System.IO.File.WriteAllText(@"C:\Users\Zeger\source\repos\Integratieproject\WebUI\Controllers\DataTextGain.json", result);
                    }
                }
                catch (Newtonsoft.Json.JsonReaderException)
                {

                }

            }*/
            //Voor elke entiteit een request maken, momenteel gebruikt het test data, later halen we al onze entiteiten op.
            foreach (var persoon in allePersonen)
            {
                PostRequest postRequest = new PostRequest()
                {
                    name = persoon.Naam,
                    //since = new DateTime(2018, 04, 01),
                    //until = new DateTime(2018, 04, 30)
                    since = gisteren,
                    until = vandaag
                };



                using (HttpClient http = new HttpClient())
                {
                    const string uri = "https://kdg.textgain.com/query";
                    http.DefaultRequestHeaders.Add("X-API-Key", "aEN3K6VJPEoh3sMp9ZVA73kkr");
                    var myContent = JsonConvert.SerializeObject(postRequest);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var result = await http.PostAsync(uri, byteContent).Result.Content.ReadAsStringAsync();
                    try
                    {
                        var posts = JsonConvert.DeserializeObject<List<TextGainResponse>>(result);
                        if (posts.Count != 0)
                        {
                              ConvertAndSaveToDb(posts, persoon.EntiteitId);
                          //  System.IO.File.WriteAllText(@"C:\Users\Zeger\source\repos\Integratieproject\WebUI\controllers\DataTextGain" + Persoon.EntiteitId + ".json", result);
                        }
                    }
                    catch (JsonReaderException)
                    {

                    }
                }
            }
        }

       
        private void ConvertAndSaveToDb(List<TextGainResponse> response, int entiteitId)
        {
            InitNonExistingRepo(true);
            EntiteitManager entiteitManager = new EntiteitManager(_uowManager);
            Entiteit entiteit = entiteitManager.GetAlleEntiteiten().Single(x => x.EntiteitId == entiteitId);
            List<Post> postsToAdd = new List<Post>();
            foreach (var post in response)
            {
                Post newPost = new Post()
                {
                    Profile = new Domain.Post.Profile(),
                    HashTags = new List<HashTag>(),
                    Words = new List<Word>(),
                    Date = post.date,
                    Persons = new List<Person>(),
                    Sentiment = new Sentiment(),
                    Retweet = post.retweet,
                    Source = post.source,
                    Urls = new List<URL>(),
                    Mentions = new List<Mention>(),
                    PostNummer = post.id
                };

                //alle hashtags in ons object steken
                foreach (var hashtag in post.hashtags)
                {
                    HashTag newTag = new HashTag()
                    {
                        hashTag = hashtag
                    };
                    newPost.HashTags.Add(newTag);
                }

                //alle woorden in ons object steken
                foreach (var word in post.Words)
                {
                    Word newWord = new Word()
                    {
                        word = word
                    };
                    newPost.Words.Add(newWord);
                }

                //alle persons in ons object steken
                foreach (var person in post.persons)
                {
                    Person newPerson = new Person()
                    {
                        Naam = person
                    };
                    newPost.Persons.Add(newPerson);
                }

                //alle urls in ons object steken
                foreach (var url in post.urls)
                {
                    URL newUrl = new URL()
                    {
                        Link = url
                    };
                    newPost.Urls.Add(newUrl);
                }

                foreach (var mention in post.mentions)
                {
                    Mention newMention = new Mention()
                    {
                        mention = mention
                    };
                    newPost.Mentions.Add(newMention);
                }

                //sentiment in textgain geeft altijd 2 elementen terug, eerste is polariteit, tweede subjectiviteit
                if (post.sentiment.Count != 0)
                {
                    double polariteit = double.Parse(post.sentiment.ElementAt(0), CultureInfo.InvariantCulture);
                    double subjectiviteit = double.Parse(post.sentiment.ElementAt(1), CultureInfo.InvariantCulture);
                    newPost.Sentiment.Polariteit = polariteit;
                    newPost.Sentiment.Subjectiviteit = subjectiviteit;
                }

                newPost.Retweet = post.retweet;
                newPost.Source = post.source;

                entiteit.Posts.Add(newPost);
                postsToAdd.Add(newPost);
            }

            //linkt de juist entiteit en voegt nieuwe posts toe.
            //postRepository.AddPosts(PostsToAdd);
            entiteitManager.UpdateEntiteit(entiteit);
<<<<<<< HEAD
            _uowManager.Save();
=======
            uowManager.Save();
>>>>>>> master
        }

        public Dictionary<string, double> BerekenGrafiekWaarde(Domain.Enum.GrafiekType grafiekType, List<Entiteit> entiteiten)
        {
            InitNonExistingRepo(true);
            IEntiteitManager entiteitManager = new EntiteitManager(_uowManager);
            Dictionary<string, double> grafiekMap = new Dictionary<string, double>();

            switch (grafiekType)
            {
                case Domain.Enum.GrafiekType.CIJFERS:
                    Entiteit e1 = entiteitManager.GetAlleEntiteiten().Single(x => x.EntiteitId == entiteiten.First().EntiteitId);
                    List<Post> postsEerste = e1.Posts;
                    int aantalPosts = postsEerste.Count;
                    int retweets = postsEerste.Count(x => x.Retweet);
                    //grafiek.Entiteiten.First().Trends;

                    grafiekMap.Add("aantalPosts", aantalPosts);
                    grafiekMap.Add("aantalRetweets", retweets);
                    break;
            }
            return grafiekMap;
        }

        public List<Post> GetRecentePosts()
        {
            return GetAllPosts().Skip(Math.Max(0, GetAllPosts().Count() - 3)).ToList();
        }


        public List<Grafiek> GetAllGrafieken()
        {
            return _postRepository.GetAllGrafieken().ToList();
        }

        public void MaakVasteGrafieken()
        {
            InitNonExistingRepo(true);
            DateTime since = new DateTime(2018, 04, 01);
            DateTime until = new DateTime(2018, 04, 30);
            EntiteitManager entiteitManager = new EntiteitManager(_uowManager);
            AccountManager accountManager = new AccountManager(_uowManager);
            Dictionary<int, double> dictionarySentiment = new Dictionary<int, double>();
            Dictionary<int, int> dictionaryPopulariteit = new Dictionary<int, int>();
            Dictionary<string, int> dictionaryWords = new Dictionary<string, int>();

            foreach (var p in entiteitManager.GetAllPeople(1))
            {
                double sentiment = 0;
                foreach (var post in p.Posts)
                {
                    sentiment += (post.Sentiment.Polariteit * post.Sentiment.Subjectiviteit) / p.Posts.Count();
                }
                dictionarySentiment.Add(p.EntiteitId, sentiment);
                dictionaryPopulariteit.Add(p.EntiteitId, p.Posts.Count);
            }

            Grafiek grafiekSentiment = new Grafiek()
            {
                Type = Domain.Enum.GrafiekType.VASTE,
                Waardes = new List<GrafiekWaarde>(),
                Naam = "Meest Positieve/Negatieve personen"
            };

            Grafiek grafiekPopulair = new Grafiek()
            {
                Type = Domain.Enum.GrafiekType.VASTE,
                Waardes = new List<GrafiekWaarde>(),
                Naam = "Meest Populaire personen"
            };

            Grafiek grafiekPopulairWords = new Grafiek()
            {
                Type = Domain.Enum.GrafiekType.VASTE,
                Waardes = new List<GrafiekWaarde>(),
                Naam = "Meest Populaire Woorden"
            };

            var orderedSentiment = dictionarySentiment.OrderBy(x => x.Value);
            var orderedPopulariteit = dictionaryPopulariteit.OrderByDescending(x => x.Value);
            var frequency = _postRepository.GetAllWords().GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()).OrderByDescending(x => x.Value);

            for (int i = 0; i < 4; i++)
            {
                GrafiekWaarde waarde = new GrafiekWaarde()
                {
                    Naam = entiteitManager.GetEntiteit(orderedSentiment.ElementAt(i).Key).Naam,
                    Waarde = orderedSentiment.ElementAt(i).Value
                };
                GrafiekWaarde waardePop = new GrafiekWaarde()
                {
                    Naam = entiteitManager.GetEntiteit(orderedPopulariteit.ElementAt(i).Key).Naam,
                    Waarde = orderedPopulariteit.ElementAt(i).Value
                };

                GrafiekWaarde waardeWords = new GrafiekWaarde()
                {
                    Naam = frequency.ElementAt(i).Key.word,
                    Waarde = frequency.ElementAt(i).Value
                };

                grafiekSentiment.Waardes.Add(waarde);
                grafiekPopulair.Waardes.Add(waardePop);
                grafiekPopulairWords.Waardes.Add(waardeWords);
            }
            _postRepository.AddGrafiek(grafiekSentiment);
            _postRepository.AddGrafiek(grafiekPopulair);
            _postRepository.AddGrafiek(grafiekPopulairWords);
            _uowManager.Save();
        }

        public void AddGrafiek(Grafiek grafiek)
        {
            InitNonExistingRepo();
            _postRepository.AddGrafiek(grafiek);
        }

        public List<Grafiek> GetVasteGrafieken()
        {
            InitNonExistingRepo();
            return _postRepository.AlleGrafieken().Where(x => x.Type == Domain.Enum.GrafiekType.VASTE).ToList();

        }

        public void UpdateGrafiek(int id)
        {
            InitNonExistingRepo();
            Grafiek grafiekToUpdate = _postRepository.GetAllGrafieken().Single(x => x.GrafiekId == id);
            BerekenGrafiekWaarde(grafiekToUpdate.Type, null);
        }

        public Grafiek GetGrafiek(int id)
        {
            InitNonExistingRepo();
            return _postRepository.ReadGrafiek(id);
        }

        public void UpdateGrafiek(List<int> entiteitIds, Grafiek grafiek)
        {
            InitNonExistingRepo(true);
            EntiteitManager entiteitManager = new EntiteitManager(_uowManager);

            Grafiek grafiekToUpdate = GetGrafiek(grafiek.GrafiekId);
            List<Entiteit> entiteiten = new List<Entiteit>();

            grafiekToUpdate.Entiteiten.Clear();
            foreach (var i in entiteitIds)
            {
                var e = _postRepository.GetAlleEntiteiten().Single(x => x.EntiteitId == i);
                entiteiten.Add(e);
                grafiekToUpdate.Entiteiten.Add(e);
            }

            grafiekToUpdate.Waardes = BerekenGrafiekWaardes(grafiekToUpdate.CijferOpties, entiteiten);
            grafiekToUpdate.Naam = grafiek.Naam;
            grafiekToUpdate.GrafiekSoort = grafiek.GrafiekSoort;

            //grafiekToUpdate.Entiteiten = entiteiten;
            //entiteiten.Clear();
            //grafiekToUpdate.Entiteiten.Add(entiteitManager.getEntiteit(4));
            _postRepository.UpdateGrafiek(grafiekToUpdate);
            _uowManager.Save();
        }

        public List<GrafiekWaarde> BerekenGrafiekWaardes(List<CijferOpties> opties, List<Entiteit> entiteiten)
        {
            InitNonExistingRepo();
            //EntiteitManager entiteitManager = new EntiteitManager(uowManager);
            List<GrafiekWaarde> grafiekWaardes = new List<GrafiekWaarde>();

            //Alle opties overlopen
            foreach (var o in opties)
            {
                //Als optie aantal posts is, voor elke entiteit het totaal aantal posts ophalen
                if(o.Optie.ToLower() == "aantalposts" || o.Optie.ToLower() == "populariteit")
                {
                    foreach (var e in entiteiten)
                    {
                        GrafiekWaarde waarde = new GrafiekWaarde()
                        {
                            Naam = "# Posts " + e.Naam,
                            Waarde = e.Posts.Count
                        };
                        grafiekWaardes.Add(waarde);
                    }
                }
                else if(o.Optie.ToLower() == "aantalretweets")
                {
                    foreach (var e in entiteiten)
                    {
                        GrafiekWaarde waarde = new GrafiekWaarde()
                        {
                            Naam = "# Retweets " + e.Naam,
                            Waarde = e.Posts.Where(x => x.Retweet == true).Count()
                        };
                        grafiekWaardes.Add(waarde);
                    }
                }
                else if (o.Optie.ToLower() == "aanwezigetrends")
                {
                    foreach (var e in entiteiten)
                    {
                        if(e.Trends == null)
                        {
                            e.Trends = new List<Trend>();
                        }
                        foreach (var t in e.Trends)
                        {
                            GrafiekWaarde waarde = new GrafiekWaarde()
                            {
                                Naam = "Trend " + t.Voorwaarde,
                                Waarde = 1 //1 van true, aanwezig
                            };
                            grafiekWaardes.Add(waarde);
                        }
                    }
                }
                else if(o.Optie.ToLower() == "postfrequentie")
                {
                    foreach (var e in entiteiten)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            GrafiekWaarde waarde = new GrafiekWaarde()
                            {
                                Naam = "Posts " + e.Naam,
                                Waarde = e.Posts.Where(x => x.Date.Date == DateTime.Today.AddDays(-i).Date).Count()
                            };
                            grafiekWaardes.Add(waarde);
                        }
                        GrafiekWaarde end = new GrafiekWaarde()
                        {
                            Naam = "endpostfrequentie"
                        };
                        grafiekWaardes.Add(end);
                    }
                }
            }
            return grafiekWaardes;
        }
    }
}
