using System;
using System.Collections.Generic;

namespace RedisCache.Model
{
    public class Movie
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime PublicationYear { get; set; }
        public List<Actor> Actors { get; set; }
    }
}
