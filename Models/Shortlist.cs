using System;
namespace Shortlist.Models
{
	public class Shortlist
	{
		public int id;
		public string title;
		public string description;
		public int membersCount;
		public int postsCount;
		public string primaryColour;
		public string secondaryColour;

		public Shortlist(int id, string title, string description, int membersCount, int postsCount, int primaryColour, int secondaryColour, DateTime modifiedDate)
		{
			this.id = id;
			this.title = title;
			this.description = description;
			this.membersCount = membersCount;
			this.postsCount = postsCount;

			switch (primaryColour)
			{
				case 0:
					this.primaryColour = "255, 152, 67";
					break;
                case 1:
                    this.primaryColour = "66, 50, 168";
                    break;
                case 2:
                    this.primaryColour = "64, 168, 50";
                    break;
                case 3:
                    this.primaryColour = "207, 42, 33";
                    break;
                case 4:
                    this.primaryColour = "134, 167, 252";
                    break;
				default:
					break;
            }

            switch (secondaryColour)
            {
                case 0:
                    this.secondaryColour = "255, 152, 67";
                    break;
                case 1:  
                    this.secondaryColour = "66, 50, 168";
                    break;
                case 2:
                    this.secondaryColour = "64, 168, 50";
                    break;
                case 3:
                    this.secondaryColour = "207, 42, 33";
                    break;
                case 4:
                    this.secondaryColour = "134, 167, 252";
                    break;
                default:
                    break;
            }
        }
	}
}

