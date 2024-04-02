using System;
using System.Data.SqlClient;
using Shortlist.Models;
using System.Collections.Generic;
using System.Collections;
using System.Data;

namespace Shortlist.Services
{
    public class Database
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        public Database()
        {
            try
            {
                builder.DataSource = "shortlist.database.windows.net";
                builder.UserID = "dylan";
                builder.Password = "Shortl1st";
                builder.InitialCatalog = "Shortlist";
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public User CreateUser(string name, string username, string password)
        {
            if (UserExists(username))
            {
                return null;
            }

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    // Construct parameterized query
                    string query = "INSERT INTO [User] (name, username, password) OUTPUT Inserted.unique_id VALUES (@Name, @Username, @Password)";

                    // Create SqlCommand with parameterized query
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@Password", password);

                        // Execute the command and get the inserted ID
                        int id = (int)command.ExecuteScalar();

                        Console.WriteLine("Created user: " + username);
                        return new User(id, name, username);
                    }
                }
                catch (SqlException e)
                {
                    // Handle SQL exceptions
                    Console.WriteLine("Error creating user: " + e.ToString());
                    return null;
                }
            }
        }

        public User Login(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string query = $"SELECT * FROM [User] WHERE (username = @username AND password = @password)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);
                        command.Parameters.AddWithValue("@password", password);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string name = reader.GetString(reader.GetOrdinal("name"));
                                int id = reader.GetInt32(reader.GetOrdinal("unique_id"));
                                return new User(id, name, username);
                            }
                            else
                            {
                                return new User(0, null, null);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
        }

        public string ConnectionString()
        {
            return builder.ConnectionString;
        }

        bool UserExists(string username)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string query = $"SELECT * FROM [User] WHERE (username = @username)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", username);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return true;
                }
            }
        }

        public int CreateShortlist(string name, string description, int creator, int primaryColour, int secondaryColour)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                int id = 0;
                try
                {
                    connection.Open();

                    string query = "INSERT INTO [Shortlist] (name, creator, description, createdDate, primary_colour, secondary_colour, modifiedDate) OUTPUT Inserted.unique_id VALUES (@Name, @Creator, @Description, @CreatedDate, @PrimaryColour, @SecondaryColour, @ModDate)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Creator", creator);
                        command.Parameters.AddWithValue("@Description", description);
                        command.Parameters.AddWithValue("@CreatedDate", DateTime.Now.ToString());
                        command.Parameters.AddWithValue("@PrimaryColour", primaryColour);
                        command.Parameters.AddWithValue("@SecondaryColour", secondaryColour);
                        command.Parameters.AddWithValue("@ModDate", DateTime.Now.ToString());

                        id = (int)command.ExecuteScalar();

                        Console.WriteLine("Created shortlist: " + name);
                    }

                    AddMemberToShortlist(id, creator);

                    return id;
                }
                catch (SqlException e)
                {
                    Console.WriteLine("Error creating shortlist: " + e.ToString());
                    return 0;
                }
            }
        }

        public void AddMemberToShortlist(int shortlist, int user)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string query = "INSERT INTO [ShortlistMember] ([ShortlistMember].[user], shortlist, createdDate) VALUES (@User, @Shortlist, @CreatedDate)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@User", user);
                        command.Parameters.AddWithValue("@Shortlist", shortlist);
                        command.Parameters.AddWithValue("@CreatedDate", DateTime.Now.ToString());

                        command.ExecuteNonQuery();
                    }
                    connection.Close();

                    connection.Open();
                    query = "UPDATE Shortlist SET members_count = members_count + 1 WHERE unique_id = @Shortlist";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Shortlist", shortlist);

                        command.ExecuteNonQuery();
                    }
                }

                catch (SqlException e)
                {
                    Console.WriteLine("Error creating ShortlistMember: " + e);
                }
            }
        }

        public User User(int id)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string query = $"SELECT * FROM [User] WHERE (unique_id = @id)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string name = reader.GetString(reader.GetOrdinal("name"));
                                string username = reader.GetString(reader.GetOrdinal("username"));

                                return new User(id, name, username);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
        }

        public Models.Shortlist Shortlist(int id)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string query = $"SELECT * FROM [Shortlist] WHERE (unique_id = @id)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string name = reader.GetString(reader.GetOrdinal("name"));
                                string desc = reader.GetString(reader.GetOrdinal("description"));
                                int memCount = reader.GetInt32(reader.GetOrdinal("members_count"));
                                int postCount = reader.GetInt32(reader.GetOrdinal("posts_count"));
                                int priCol = reader.GetInt32(reader.GetOrdinal("primary_colour"));
                                int secCol = reader.GetInt32(reader.GetOrdinal("secondary_colour"));
                                DateTime modDate = reader.GetDateTime(reader.GetOrdinal("modifiedDate"));
                                return new Models.Shortlist(id, name, desc, memCount, postCount, priCol, secCol, modDate);
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }
        }

        public List<Models.Shortlist> UsersShortlists(int userID)
        {
            List<Models.Shortlist> shortlists = new List<Models.Shortlist>();
            List<int> shortlistIDs = new List<int>();

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT * FROM [ShortlistMember] WHERE ([user] = @userid)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userid", userID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                shortlistIDs.Add(reader.GetInt32(reader.GetOrdinal("shortlist")));
                            }
                        }
                    }
                    connection.Close();
                }

                catch (Exception e)
                {
                    Console.WriteLine("Error fetching ShortlistMembers:\n" + e);
                }

                if (shortlistIDs.Count != 0)
                {
                    try
                    {
                        connection.Open();
                        string query = "SELECT * FROM [Shortlist] WHERE [unique_id] IN (" + string.Join(",", shortlistIDs) + ") ORDER BY modifiedDate DESC";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    shortlists.Add(new Models.Shortlist(
                                        reader.GetInt32(reader.GetOrdinal("unique_id")),
                                        reader.GetString(reader.GetOrdinal("name")),
                                        reader.GetString(reader.GetOrdinal("description")),
                                        reader.GetInt32(reader.GetOrdinal("members_count")),
                                        reader.GetInt32(reader.GetOrdinal("posts_count")),
                                        reader.GetInt32(reader.GetOrdinal("primary_colour")),
                                        reader.GetInt32(reader.GetOrdinal("secondary_colour")),
                                        reader.GetDateTime(reader.GetOrdinal("modifiedDate"))
                                        ));
                                }
                            }

                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error fetching shortlists:\n" + e);
                        return null;
                    }
                }
            }

            return shortlists;
        }

        public List<Post> FetchPosts(int shortlist)
        {
            List<Post> posts = new List<Post>();

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM [Post] WHERE Shortlist = @Shortlist ORDER BY votes DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Shortlist", shortlist);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(reader.GetOrdinal("unique_id"));
                                string title = reader.GetString(reader.GetOrdinal("title"));
                                string body = reader.GetString(reader.GetOrdinal("body"));
                                bool isLink = reader.GetBoolean(reader.GetOrdinal("isLink"));
                                int votes = reader.GetInt32(reader.GetOrdinal("votes"));
                                string thumbnail = reader.GetString(reader.GetOrdinal("thumbnail"));
                                DateTime date = reader.GetDateTime(reader.GetOrdinal("createdDate"));
                                User user = User(reader.GetInt32(reader.GetOrdinal("creator")));
                                int commentCount = reader.GetInt32(reader.GetOrdinal("comment_count"));
                                posts.Add(new Post(id, title, body, isLink, votes, thumbnail, date, user, commentCount));
                            }

                            return posts;
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error fetching shortlists:\n" + e);
                    return null;
                }
            }
        }

        public bool CreatePost(string title, string body, bool isLink, int shortlist, int creator, string thumbnail = "")
        {
            int link = 0;
            if (isLink)
            {
                link = 1;
            }

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO [Post] (title, body, isLink, thumbnail, shortlist, createdDate, creator) VALUES (@Title, @Body,@IsLink, @Thumbnail, @Shortlist, @CreatedDate, @Creator)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Title", title);
                        command.Parameters.AddWithValue("@Body", body);
                        command.Parameters.AddWithValue("@IsLink", link);
                        command.Parameters.AddWithValue("@Thumbnail", thumbnail);
                        command.Parameters.AddWithValue("@Shortlist", shortlist);
                        command.Parameters.AddWithValue("@CreatedDate", DateTime.Now.ToString());
                        command.Parameters.AddWithValue("@Creator", creator);

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    connection.Open();

                    query = "UPDATE [Shortlist] SET posts_count = posts_count + 1, modifiedDate = @Date WHERE unique_id = @Shortlist";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Shortlist", shortlist);
                        command.Parameters.AddWithValue("@Date", DateTime.Now.ToString());

                        command.ExecuteNonQuery();
                    }

                    return true;
                }

                catch (Exception e)
                {
                    Console.WriteLine("Error creating post: " + e);
                    return false;
                }
            }
        }

        public int Vote(int post, int user)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT * FROM [Vote] WHERE (post=@Post AND [user]=@User)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Post", post);
                        command.Parameters.AddWithValue("@User", user);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) return reader.GetInt32(reader.GetOrdinal("vote"));
                            else return 0;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error fetching vote: " + e);
                    return 0;
                }
            }
        }

        public bool DeleteVote(int userId, int postId, int currentVote)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string query = "DELETE FROM [Vote] WHERE (post = @Post AND [user] = @user)\n" +
                        "UPDATE [Post] SET votes = votes - @Vote WHERE (unique_id = @Post)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Post", postId);
                        command.Parameters.AddWithValue("@User", userId);
                        command.Parameters.AddWithValue("@Vote", currentVote);

                        command.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (SqlException e)
                {
                    Console.WriteLine("Error deleting vote: " + e.ToString());
                    return false;
                }
            }
        }

        public bool Vote(int user, int post, int vote)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string query = "INSERT INTO [Vote] (post, [user], vote) VALUES (@Post, @User, @Vote)\n" +
                        "UPDATE [Post] SET votes = votes + @Vote WHERE (unique_id = @Post)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Post", post);
                        command.Parameters.AddWithValue("@User", user);
                        command.Parameters.AddWithValue("@Vote", vote);

                        command.ExecuteNonQuery();
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error inserting vote: " + e.ToString());
                    return false;
                }
            }
        }

        public bool IsMember(int user, int shortlist)
        {
            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT * FROM [ShortlistMember] WHERE [user] = @User AND shortlist = @Shortlist";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@User", user);
                        command.Parameters.AddWithValue("@Shortlist", shortlist);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) return true;
                            else return false;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error fetching ShortlistMember: " + e);
                    return false;
                }
            }
        }

        public List<Comment> FetchComments(int postId)
        {
            List<Comment> comments = new List<Comment>();

            using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
            {
                connection.Open();
                string query = "SELECT unique_id, body, dateCreated, creator FROM [Comment] WHERE (post = @Post) ORDER BY dateCreated DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Post", postId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            comments.Add(new Comment(
                                reader.GetInt32(reader.GetOrdinal("unique_id")),
                                reader.GetString(reader.GetOrdinal("body")),
                                reader.GetDateTime(reader.GetOrdinal("dateCreated")),
                                User(reader.GetInt32(reader.GetOrdinal("creator"))),
                                postId
                            ));
                        }
                    }
                }

                return comments;
            }
        }

        public bool AddComment(int postId, string body, int userId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO [Comment] (body, creator, post, dateCreated) VALUES (@Body, @Creator, @Post, @Date)\n" +
                        "UPDATE [Post] SET comment_count = comment_count + 1 WHERE (unique_id = @Post)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Body", body);
                        command.Parameters.AddWithValue("@Creator", userId);
                        command.Parameters.AddWithValue("@Post", postId);
                        command.Parameters.AddWithValue("@Date", DateTime.Now.ToString());

                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception e) { Console.WriteLine("Error creating comment: " + e); return false; }
        }
    }
}
