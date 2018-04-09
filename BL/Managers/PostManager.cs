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

namespace BL
{
    public class PostManager : IPostManager
    {
        private IPostRepository postRepository;

        public PostManager()
        {
            postRepository = new PostRepository();
        }

        public void AddPost(Post post)
        {
            postRepository.AddPost(post);
        }

        public List<Post> getAllPosts()
        {
            return postRepository.getAllPosts();
        }

        public async Task SyncDataAsync()
        {
            //Sync willen we datum van vandaag en gisteren.
            DateTime vandaag = DateTime.Today;
            DateTime gisteren = DateTime.Today.AddDays(-1);
            //een lijst van requests die we gaan versturen naar textgain
            List<PostRequest> requests = new List<PostRequest>();

            //Enkele test entiteiten, puur voor debug, later vragen we deze op uit onze repository//
            List<Domain.Entiteit.Entiteit> TestEntiteiten = new List<Domain.Entiteit.Entiteit>();
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

            BenWeyts.Organisaties.Add(NVA);
            NVA.Leden.Add(BenWeyts);

            TestEntiteiten.Add(NVA);
            TestEntiteiten.Add(BenWeyts);

            //Voor elke entiteit een request maken, momenteel gebruikt het test data, later halen we al onze entiteiten op.
            foreach (var Entiteit in TestEntiteiten)
            {
                PostRequest postRequest = new PostRequest()
                {
                    name = Entiteit.Naam,
                    since = new DateTime(2018,04,01),
                    until = new DateTime(2018,04,09)
                };
                requests.Add(postRequest);
            }

            //elke request versturen naar textgain
            foreach (var request in requests)
            {
                //lijst van posts die we kunnen terugkrijgen als response van textgain
                List<TextGainResponse> posts = new List<TextGainResponse>();

                using (HttpClient http = new HttpClient())
                {
                    string uri = "http://kdg.textgain.com/query";
                    http.DefaultRequestHeaders.Add("X-API-Key", "aEN3K6VJPEoh3sMp9ZVA73kkr");
                    var myContent = JsonConvert.SerializeObject(request);
                    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                    var byteContent = new ByteArrayContent(buffer);
                    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var result = await http.PostAsync(uri, byteContent).Result.Content.ReadAsStringAsync();
                    posts = JsonConvert.DeserializeObject<List<TextGainResponse>>(result);
                    //ConvertAndSaveToDb(posts);
                }
            }
        }

        private void ConvertAndSaveToDb(List<TextGainResponse> response, Domain.Entiteit.Entiteit entiteit)
        {
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
                    double polariteit = double.Parse(post.sentiment.ElementAt(0));
                    double subjectiviteit = double.Parse(post.sentiment.ElementAt(1));
                    newPost.Sentiment.polariteit = polariteit;
                    newPost.Sentiment.subjectiviteit = subjectiviteit;
                }

                newPost.retweet = post.retweet;
                newPost.source = post.source;

                PostsToAdd.Add(newPost);
            }

            postRepository.AddPosts(PostsToAdd);
        }
    }
}
