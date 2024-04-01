using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Shortlist.Models;
using Shortlist.Services;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Web;
using System.Collections.Generic;
using System.Security.Policy;
using Microsoft.AspNetCore.Http;

namespace Shortlist.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Shortlist()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Account()
        {
            return View();
        }

        public IActionResult Post()
        {
            return View();
        }

        public class Request
        {
            public string name { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string description { get; set; }
            public int primaryColour { get; set; }
            public int secondaryColour { get; set; }
            public string title { get; set; }
            public string body { get; set; }
            public bool isLink { get; set; }
            public string thumbnail { get; set; }
            public int shortlist { get; set; }
            public int userId { get; set; }
            public int post { get; set; }
            public int vote { get; set; }
            public int currentVote { get; set; }
        }

        [HttpPost]
        public ActionResult CreateUser([FromBody] Request request)
        {
            string name = request.name;
            string username = request.username;
            string password = Utilities.Hash(request.password);

            User user = new Database().CreateUser(name, username, password);
            string serialisedUser = JsonConvert.SerializeObject(user);

            if (user != null)
            {
                Response.Cookies.Append("user", serialisedUser);
                return Ok(new { success = true });
            }
            else return StatusCode(500, new { success = false });
        }

        [HttpPost]
        public ActionResult Login([FromBody] Request request)
        {
            string email = request.username;
            string password = Utilities.Hash(request.password);

            User user = new Database().Login(email, password);

            if (user != null)
            {
                string serialisedUser = JsonConvert.SerializeObject(user);
                Response.Cookies.Append("user", serialisedUser);
                return Ok(new { success = true });
            }
            else return StatusCode(500, new { success = false });
        }

        [HttpPost]
        public ActionResult Logout()
        {
            Response.Cookies.Delete("user");
            return Ok(new { success = true });
        }

        [HttpPost]
        public ActionResult CreateShortlist([FromBody] Request request)
        {
            string name = request.name;
            string description = request.description;
            int primaryColour = request.primaryColour;
            int secondaryColour = request.secondaryColour;
            int userID = JsonConvert.DeserializeObject<User>(HttpContext.Request.Cookies["user"]).id;

            int shortlistID = new Database().CreateShortlist(name, description, userID, primaryColour, secondaryColour);

            if (shortlistID != 0)
            {
                return Ok(new { success = true, id = shortlistID });
            }
            else return StatusCode(500, new { succes = false });
        }

        [HttpPost]
        public ActionResult CreatePost([FromBody] Request request)
        {
            int userid = JsonConvert.DeserializeObject<User>(HttpContext.Request.Cookies["user"]).id;
            bool success;
            if (!request.isLink)
            {
                success = new Database().CreatePost(request.title, request.body, request.isLink, request.shortlist, userid);
            }
            else
            {
                if (!(request.body.Contains("https://") || request.body.Contains("http://")))
                {
                    request.body = "http://" + request.body;
                }
                string thumbnail = Utilities.ScrapeLinkImage(request.body);
                if (thumbnail == null) return StatusCode(500, new { success = false });
                success = new Database().CreatePost(request.title, request.body, request.isLink, request.shortlist, userid, thumbnail);
            }

            if (success) return Ok(new { success = true });
            else return StatusCode(500, new { success = false });
        }

        [HttpGet]
        public ActionResult<string> FetchLinkImage([FromBody] string link)
        {
            if (!(link.Contains("https://") || link.Contains("http://")))
            {
                link = "http://" + link;
            }
            string img = Utilities.ScrapeLinkImage(link);
            if (img == null) return StatusCode(500, new { success = false, issue = "badlink"});
            return Ok(new { success = true, img = img }); 
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public ActionResult DeleteVote([FromBody] Request request)
        {
            int postId = request.post;
            int userId = request.userId;
            int currentVote = request.currentVote;

            bool success = new Database().DeleteVote(userId, postId, currentVote);

            if (success) { return Ok(new { success = true }); }
            else { return StatusCode(500, new { success = false }); }
        }

        [HttpPost]
        public ActionResult Vote([FromBody] Request request)
        {
            int postId = request.post;
            int userId = request.userId;
            int vote = request.vote;

            bool success = new Database().Vote(userId, postId, vote);

            if (success) { return Ok(new { success = true }); }
            else { return StatusCode(500, new { success = false }); }
        }

        [HttpPost]
        public ActionResult JoinShortlist([FromBody] Request request)
        {
            new Database().AddMemberToShortlist(request.shortlist, request.userId);

            return Ok(new { success = true });
        }

        [HttpGet]
        public ActionResult<IEnumerable<Comment>> FetchComments(int postId)
        {
            List<Comment> comments = new Database().FetchComments(postId);

            return Ok(new { success = true, comments = JsonConvert.SerializeObject(comments) }); ;
        }

        [HttpPost]
        public ActionResult CreateComment([FromBody] Request request)
        {
            int userid = JsonConvert.DeserializeObject<User>(HttpContext.Request.Cookies["user"]).id;
            bool success;
            success = new Database().AddComment(request.post, request.body, userid);

            if (success) return Ok(new { success = true });
            else return StatusCode(500, new { success = false });
        }
    }
}   


