using System;
using Shortlist.Services;
using System.Web;

namespace Shortlist.Models
{
    [Serializable]
    public class Post
    {
        public int id;
        public string title;
        public string body;
        public bool link;
        public int votes;
        public string thumbnail;
        public DateTime dateCreated;
        public int vote;
        public User creator;
        public int commentCount;

        public Post(int id, string title, string body, bool link, int votes, string thumbnail, DateTime dateCreated, User creator, int commentCount)
        {
            this.id = id;
            this.title = title;
            this.body = body;
            this.link = link;
            this.votes = votes;
            this.thumbnail = thumbnail;
            this.dateCreated = dateCreated;
            this.creator = creator;
            this.commentCount = commentCount;
        }
    }
}

