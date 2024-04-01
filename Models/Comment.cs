using System;
namespace Shortlist.Models
{
	public class Comment
	{
		public int id;
		public string body;
		public DateTime date;
		public User user;
		public int postId;

		public Comment(int id, string body, DateTime date, User user, int postId)
		{
			{
				this.id = id;
				this.body = body;
				this.date = date;
				this.user = user;
				this.postId = postId;
			}
		}
	}
}

