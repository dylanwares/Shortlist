using System;
namespace Shortlist.Models
{
    [Serializable]
    public class User
	{
		public int id;
		public string name;
		public string username;

		public User(int id, string name, string username)
		{
			this.id = id;
			this.name = name;
			this.username = username;
		}
	}
}

