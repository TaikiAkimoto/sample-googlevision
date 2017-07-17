using System;
using System.IO;
using System.Text;
using Google.Apis.Urlshortener.v1;
using Google.Apis.Vision.v1;
using Google.Apis.Vision.v1.Data;

namespace SampleVisonApi
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().VisionDocumentApi();
        }

        private readonly string APIKEY = "hoge";

        /// <summary>
        /// Visions the document API.
        /// </summary>
		public void VisionDocumentApi()
		{
			var vision = new VisionService(
				new Google.Apis.Services.BaseClientService.Initializer
				{
					ApiKey = APIKEY
				});

			byte[] imgArray = File.ReadAllBytes("img/BL.jpg");
			string imgContent = Convert.ToBase64String(imgArray);

			var responses = vision.Images.Annotate(
				new BatchAnnotateImagesRequest()
				{
					Requests = new[] {
						new AnnotateImageRequest()
						{
							Features = new [] { new Feature() { Type = "DOCUMENT_TEXT_DETECTION" } },
							Image = new Image() { Content = imgContent }
						}
					}
				}
			).Execute();

			var result = responses.Responses;
			if (result == null) return;

			foreach (var response in result)
			{
                var pages = response.FullTextAnnotation.Pages;
                foreach (var page in pages)
                {
                    var blocks = page.Blocks;
                    foreach (var block in blocks)
                    {
                        var paragraphs = block.Paragraphs;
                        foreach (var paragraph in paragraphs)
                        {
                            Console.WriteLine("-- Paragraph --");
                            var words = paragraph.Words;
                            foreach (var word in words)
                            {
                                var text = new StringBuilder();
                                var symbols = word.Symbols;
                                foreach (var symbole in symbols)
                                {
                                    text.Append(symbole.Text);
                                }
                                Console.WriteLine(text);
                            }
                        }
                    }
                }
			}
		}

        /// <summary>
        /// VisionAPI.
        /// </summary>
        public void VisionTextApi()
        {
            var vision = new VisionService(
                new Google.Apis.Services.BaseClientService.Initializer
                {
                    ApiKey = APIKEY
                });

            byte[] imgArray = File.ReadAllBytes("img/BL.jpg");
            string imgContent = Convert.ToBase64String(imgArray);

            var responses = vision.Images.Annotate(
                new BatchAnnotateImagesRequest()
                {
                    Requests = new[] {
                        new AnnotateImageRequest()
                        {
                            Features = new [] { new Feature() { Type = "TEXT_DETECTION" } },
                            Image = new Image() { Content = imgContent }
                        }
                    }
                }
            ).Execute();

            var result = responses.Responses;
            if (result == null) return;

            foreach (var response in result)
            {
                Console.WriteLine(response.FullTextAnnotation.Text);
            }
        }

        /// <summary>
        /// URLShortener API.
        /// </summary>
        public void UrlShortenerApi()
		{
			var shortener = new UrlshortenerService(
			    new Google.Apis.Services.BaseClientService.Initializer
			    {
                    ApiKey = APIKEY
			    });

			var shorten = shortener.Url.Insert(new Google.Apis.Urlshortener.v1.Data.Url
			{
				LongUrl = "https://www.nuget.org/packages/Google.Apis.Urlshortener.v1/"
			}).Execute();

			Console.WriteLine("短縮したURL {0}", shorten.Id);
			var url = shortener.Url.Get(shorten.Id).Execute();
			Console.WriteLine("復元した元のURL {0}", url.LongUrl);
        }
    }
}
