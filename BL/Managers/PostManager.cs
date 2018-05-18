using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Newtonsoft.Json;
using Domain.TextGain;
using Domain.Post;
using Domain.Entiteit;
using System.Globalization;

namespace BL
{
    public class PostManager : IPostManager
    {
        private IPostRepository postRepository;
        private UnitOfWorkManager uowManager;

        public PostManager()
        {
            postRepository = new PostRepository();
        }

        public PostManager(UnitOfWorkManager uofMgr)
        {
            postRepository = new PostRepository();
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
                    postRepository = new PostRepository(uowManager.UnitOfWork);
                }
            }
            // Als we niet met UoW willen werken, dan maken we een repo aan als die nog niet bestaat.
            else
            {
                postRepository = (postRepository == null) ? new PostRepository() : postRepository;
            }
        }

        public void AddPost(Post post)
        {
            initNonExistingRepo();
            postRepository.AddPost(post);
        }

        public List<Post> getAllPosts()
        {
            initNonExistingRepo();
            return postRepository.getAllPosts();
        }

        public async Task SyncDataAsync()
        {
            initNonExistingRepo(true);
            EntiteitManager entiteitManager = new EntiteitManager(uowManager);
            //Sync willen we datum van vandaag en gisteren.
            DateTime vandaag = DateTime.Today.Date;
            DateTime gisteren = DateTime.Today.AddDays(-15).Date;

            //Enkele test entiteiten, puur voor debug, later vragen we deze op uit onze repository//
            List<Domain.Entiteit.Persoon> AllePersonen = entiteitManager.GetAllPeople(1);

            //Voor elke entiteit een request maken, momenteel gebruikt het test data, later halen we al onze entiteiten op.
            
            foreach (var Persoon in AllePersonen)
            {
                PostRequest postRequest = new PostRequest()
                {
                    name = Persoon.Naam,
                    since = new DateTime(2018, 04, 01),
                    until = new DateTime(2018, 04, 30)
                    //since = gisteren,
                    //until = vandaag
                };

                List<TextGainResponse> posts = new List<TextGainResponse>();

                using (HttpClient http = new HttpClient())
                {
                    string uri = "https://kdg.textgain.com/query";
                    http.DefaultRequestHeaders.Add("X-API-Key", "aEN3K6VJPEoh3sMp9ZVA73kkr");
                    var myContent = JsonConvert.SerializeObject(postRequest);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var result = await http.PostAsync(uri, byteContent).Result.Content.ReadAsStringAsync();
                    try
                    {
                        posts = JsonConvert.DeserializeObject<List<TextGainResponse>>(result);
                        if (posts.Count != 0)
                        {
                            ConvertAndSaveToDb(posts, Persoon.EntiteitId);
                        }
                    } catch (Newtonsoft.Json.JsonReaderException)
                    {

                    } 
                }
            }
        }

        private void ConvertAndSaveToDb(List<TextGainResponse> response, int entiteitId)
        {
            initNonExistingRepo(true);
            EntiteitManager entiteitManager = new EntiteitManager(uowManager);
            Entiteit entiteit = entiteitManager.getAlleEntiteiten().Single(x => x.EntiteitId == entiteitId);
            List<Post> PostsToAdd = new List<Post>();
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
                    retweet = post.retweet,
                    source = post.source,
                    Urls = new List<URL>(),
                    Mentions = new List<Mention>(),
                    postNummer = post.id
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
                    URL newURL = new URL()
                    {
                        Link = url
                    };
                    newPost.Urls.Add(newURL);
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
                if(post.sentiment.Count != 0)
                {
                    double polariteit = double.Parse(post.sentiment.ElementAt(0),CultureInfo.InvariantCulture);
                    double subjectiviteit = double.Parse(post.sentiment.ElementAt(1),CultureInfo.InvariantCulture);
                    newPost.Sentiment.polariteit = polariteit;
                    newPost.Sentiment.subjectiviteit = subjectiviteit;
                }

                newPost.retweet = post.retweet;
                newPost.source = post.source;

                entiteit.Posts.Add(newPost);
                PostsToAdd.Add(newPost);
            }

            //linkt de juist entiteit en voegt nieuwe posts toe.
            //postRepository.AddPosts(PostsToAdd);
            entiteitManager.updateEntiteit(entiteit);
            uowManager.Save();
        }

        public Dictionary<string, double> BerekenGrafiekWaarde(Domain.Enum.GrafiekType grafiekType, List<Entiteit> entiteiten)
        {
            initNonExistingRepo(true);
            IEntiteitManager entiteitManager = new EntiteitManager(uowManager);
            Dictionary<string, double> grafiekMap = new Dictionary<string, double>();

            switch (grafiekType)
            {
                case Domain.Enum.GrafiekType.CIJFERS:
                    Entiteit e1 = entiteitManager.getAlleEntiteiten().Single(x => x.EntiteitId == entiteiten.First().EntiteitId);
                    List<Post> postsEerste = e1.Posts;
                    int aantalPosts = postsEerste.Count;
                    int retweets = postsEerste.Where(x => x.retweet == true).Count();
                    //grafiek.Entiteiten.First().Trends;

                    grafiekMap.Add("aantalPosts", aantalPosts);
                    grafiekMap.Add("aantalRetweets", retweets);
                    break;
            }
            return grafiekMap;
        }

        public List<Post> getRecentePosts()
        {
            return getAllPosts().Skip(Math.Max(0, getAllPosts().Count() - 3)).ToList();
        }


        public List<Grafiek> getAllGrafieken()
        {
            return postRepository.GetAllGrafieken().ToList();

        public void maakVasteGrafieken()
        {
            initNonExistingRepo(true);
            DateTime since = new DateTime(2018, 04, 01);
            DateTime until = new DateTime(2018, 04, 30);
            EntiteitManager entiteitManager = new EntiteitManager(uowManager);
            AccountManager accountManager = new AccountManager(uowManager);
            Dictionary<int, double> dictionarySentiment = new Dictionary<int, double>();
            Dictionary<int, int> dictionaryPopulariteit = new Dictionary<int, int>();
            Dictionary<string, int> dictionaryWords = new Dictionary<string, int>();

            foreach (var p in entiteitManager.GetAllPeople(1))
            {
                double sentiment = 0;
                foreach (var post in p.Posts)
                {
                    sentiment += (post.Sentiment.polariteit * post.Sentiment.subjectiviteit) / p.Posts.Count();
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
            var frequency = postRepository.GetAllWords().GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()).OrderByDescending(x => x.Value);

            for (int i=0; i < 4; i++)
            {
                GrafiekWaarde waarde = new GrafiekWaarde()
                {
                    Naam = entiteitManager.getEntiteit(orderedSentiment.ElementAt(i).Key).Naam,
                    Waarde = orderedSentiment.ElementAt(i).Value
                };
                GrafiekWaarde waardePop = new GrafiekWaarde()
                {
                    Naam = entiteitManager.getEntiteit(orderedPopulariteit.ElementAt(i).Key).Naam,
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
            postRepository.AddGrafiek(grafiekSentiment);
            postRepository.AddGrafiek(grafiekPopulair);
            postRepository.AddGrafiek(grafiekPopulairWords);
            uowManager.Save();
        }

        public void addGrafiek(Grafiek grafiek)
        {
            initNonExistingRepo();
            postRepository.AddGrafiek(grafiek);
        }

        public List<Grafiek> GetVasteGrafieken()
        {
            initNonExistingRepo();
            return postRepository.AlleGrafieken().Where(x => x.Type == Domain.Enum.GrafiekType.VASTE).ToList();

        }
    }
}
