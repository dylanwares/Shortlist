using System;
using Shortlist.Services;
using System.Web;

namespace Shortlist.Models
{
    [Serializable]
    public class Vote
    {
        public int vote;
        public User user;

        public Vote(int vote, User user)
        {
            this.vote = vote;
            this.user = user;
        }
    }
}

