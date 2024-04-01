using System;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;
using Shortlist.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Xml;
using HtmlAgilityPack;
using System.Linq;
using System.Net;

namespace Shortlist.Services
{
	public static class Utilities
	{
		public static string Hash(string str)
		{
			byte[] hashed;
			using (HashAlgorithm alg = SHA256.Create())
			{
				hashed = alg.ComputeHash(Encoding.UTF8.GetBytes(str));
			}

            StringBuilder sb = new StringBuilder();
			foreach (byte b in hashed)
			{
				sb.Append(b.ToString("X2"));
			}

			return sb.ToString();
        }

        public static string ScrapeLinkImage(string url)
        {
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);

                string img = "";

                HtmlNode ogImgTag = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
                if (ogImgTag != null)
                {
                    img = ogImgTag.GetAttributeValue("content", "");
                }

				if (img == "")
				{
					HtmlNodeCollection imgTags = doc.DocumentNode.SelectNodes("//img");
					if (imgTags != null && imgTags.Count > 0)
					{
						img = imgTags[0].GetAttributeValue("src", null);
					}
				}

				if (!img.Contains("http") && img != ""){
					string baseUrl = url.Split('/')[2];
					img = "http://" + baseUrl + img;
				}

                return img;
            }
            catch (Exception e){ Console.WriteLine("Error scraping image: " + e); return null; }
        }

		public static void date()
		{
			Console.WriteLine(DateTime.Now.ToString());
		}
	}
}

