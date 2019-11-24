using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using RedisCache.Helpers;
using RedisCache.Model;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;

namespace RedisCache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [Route("set")]
        public HttpStatusCode Set()
        {
            using (IRedisClient client = new RedisClient())
            {
                client.Set("firstname", "oktay");
                client.Set("secondname", "dagdelen");
                client.Set("age", "25");
            }
            return HttpStatusCode.OK;
        }

        [Route("get")]
        public string Get()
        {
            using (RedisClient client = new RedisClient())
            {
                var firstNameAsByte = client.Get("firstname");
                var firstName = EncodingHelper.GetStringFromByteArray(firstNameAsByte);

                StringBuilder infoAboutMe = new StringBuilder();
                infoAboutMe.AppendLine("Hello ! ");
                infoAboutMe.AppendLine($"My Name is {firstName}");
                // You can add more details

                return infoAboutMe.ToString();
            }

        }


        [Route("lists")]
        public string SetLists()
        {
            const string USERS_LIST_NAME = "urn:users";

            using (IRedisClient client = new RedisClient())
            {
                var users = client.Lists[USERS_LIST_NAME];

                users.Add("Oktay");
                users.Add("Mehmet");
                users.Add("Gizem");
                users.Add("Murat");
                users.Add("Aslı");

                var userNames = client.Lists[USERS_LIST_NAME];


                StringBuilder names = new StringBuilder();
                foreach (var username in userNames)
                {
                    names.AppendLine($"Welcome {username}");
                }


                return names.ToString();
            }
        }


        [Route("setcomplextype")]
        public string ComplexType()
        {
            using (IRedisClient client = new RedisClient())
            {
                IRedisTypedClient<Movie> movieClient = client.As<Movie>();

                Movie customer = new Movie()
                {
                    Id = movieClient.GetNextSequence(),
                    Title = "Spider Man",
                    PublicationYear = new DateTime(2002,6,14),
                    Actors = new List<Actor>
                             {
                               new Actor {Name = "Tobey Maguire", Age = 27},
                               new Actor {Name = "Kirsten Dunst", Age = 25},
                             }
                };

                var storedCustomer = movieClient.Store(customer);

                return $"Movie is added as Id = {storedCustomer.Id}";
            }

        }


        [Route("getcomplextype")]
        public string GetComplexType()
        {
            using (IRedisClient client = new RedisClient())
            {
                var movieClient = client.As<Movie>();

                const int SPIDERMAN_ID = 1;
                var movie = movieClient.GetById(SPIDERMAN_ID);

                StringBuilder movieInfo = new StringBuilder();
                movieInfo.AppendLine($"Movie Name = {movie.Title}");               
                movieInfo.AppendLine($"Movie Publication Year = {movie.PublicationYear}");
                movieInfo.AppendLine($"Movie Actors = ");

                foreach (var actor in movie.Actors)
                {
                    movieInfo.AppendLine($"{actor.Name}");
                }

                return movieInfo.ToString();
            }
        }

        [Route("transaction")]
        public string TransactionManagement()
        {
            const string ACCOUNT_A = "account-a";
            const string ACCOUNT_B = "account-b";

            using (IRedisClient client = new RedisClient())
            {
                var transaction = client.CreateTransaction();

                // when you create a transaction, redis write a query as "multi" and other queries will be queued since see the commit refers to exec
                transaction.QueueCommand(c => c.Set(ACCOUNT_A, 500));
                transaction.QueueCommand(c => c.Set(ACCOUNT_B, 500));

                transaction.QueueCommand(c => c.Increment(ACCOUNT_A, 100)); // last : 600
                transaction.QueueCommand(c => c.Decrement(ACCOUNT_B, 100)); // last : 400

                transaction.Commit(); // when you write commit, redis exec the queries between multi and exec



                var accountA = client.Get<string>(ACCOUNT_A);
                var accountB = client.Get<string>(ACCOUNT_B);

                return $"Account A : {accountA} and Account B : {accountB}";
            }

        }
    }

}