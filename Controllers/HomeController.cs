using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SearchOnGoogleShopping.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;



namespace SearchOnGoogleShopping.Controllers
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

        [HttpPost]
        public async Task<IActionResult> SearchProduct(string product)
        {
            if (string.IsNullOrEmpty(product))
            {
                return RedirectToAction("You must enter a value");
            }

            try
            {
                // Construct the search query
                string query = $"{product} disponible";
                // Search Google Shopping
                var googleShoppingUrls = await SearchGoogleShopping(query);

                var partialView = this.RenderPartialViewToString("_SearchResultsPartial", googleShoppingUrls);


                return PartialView("_SearchResultsPartial", googleShoppingUrls);
                //return Content(partialView);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SearchProduct");
                return RedirectToAction("Error");
            }

        }

        private async Task<List<ProductSearchResult>> SearchGoogleShopping(string query)
        {
            var googleShoppingUrl = BuildGoogleShoppingUrl(query);
            var content = await GetHttpResponseContent(googleShoppingUrl);
            var resultUrls = ExtractAndProcessResults(content);
            return resultUrls;
        }

        private string BuildGoogleShoppingUrl(string query)
        {
            //Google Shopping is not available
            //var googleShoppingUrl = $"https://www.google.com/shopping?q={Uri.EscapeDataString(query)}&tbs=mr:1&tbs=ppr_ofr&tbs=ppr_smp&tbs=ppr_inv:1";
            return $"https://www.google.es/search?q={Uri.EscapeDataString(query)}&tbs=mr:1&tbs=ppr_ofr&tbs=ppr_smp&tbs=ppr_inv:1";
        }

        private async Task<string> GetHttpResponseContent(string url)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        private List<ProductSearchResult> ExtractAndProcessResults(string content)
        {
            var resultUrls = new List<ProductSearchResult>();
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(content);

            var linkNodes = htmlDocument.DocumentNode.SelectNodes("//a[@href]");
            if (linkNodes != null)
            {
                foreach (var linkNode in linkNodes)
                {
                    var productInfo = ExtractProductInfo(linkNode);
                    if (productInfo != null)
                    {
                        resultUrls.Add(productInfo);
                    }
                }
            }

            return resultUrls;
        }

        private ProductSearchResult ExtractProductInfo(HtmlNode linkNode)
        {
            var href = linkNode.GetAttributeValue("href", "");
            if (!string.IsNullOrEmpty(href) && href.StartsWith("/url?q="))
            {
                var url = ExtractUrlFromGoogleRedirectUrl(href);
                if (!string.IsNullOrEmpty(url) && !IsExcludedUrl(url))
                {
                    string productId = null;
                    bool isFromLaCasa = href.Contains("https://www.lacasadelelectrodomestico");

                    if (isFromLaCasa)
                    {
                        productId = ExtractProductId(url);
                    }

                    return new ProductSearchResult(url, productId, isFromLaCasa);
                }
            }

            return null;
        }

        private string ExtractProductId(string url)
        {
            string pattern = @"IDArticulo~(\d+)";
            Match match = Regex.Match(url, pattern);
            return match.Success ? match.Groups[1].Value : null;
        }


        private string ExtractUrlFromGoogleRedirectUrl(string googleRedirectUrl)
        {
            // Google's URL format: /url?q=<url>&...
            var match = Regex.Match(googleRedirectUrl, @"^/url\?q=([^&]+)");
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return null;
        }

        private bool IsExcludedUrl(string url)
        {
            // Excluding PDF files and specific domains
            if (url.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var excludedDomains = new List<string>
            {
                "https://accounts.google.com",
                "https://maps.google.com",
                "https://support.google.com",
                "https://accounts.google.es",
                "https://maps.google.es",
                "https://support.google.es",
                "https://support",
                "https://www.facebook.com"
            };

            foreach (var excludedDomain in excludedDomains)
            {
                if (url.StartsWith(excludedDomain, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                try
                {
                    var engine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                    var viewResult = engine.FindView(ControllerContext, viewName, false);

                    var viewContext = new ViewContext(
                        ControllerContext,
                        viewResult.View,
                        ViewData,
                        TempData,
                        sw,
                        new HtmlHelperOptions()
                    );

                    viewResult.View.RenderAsync(viewContext).Wait();
                    return sw.GetStringBuilder().ToString();
                }
                catch (Exception ex)
                {
                    // Manejar la excepción aquí
                    // Por ejemplo, puedes imprimir el mensaje de error en la consola
                    Console.WriteLine("Error al renderizar la vista parcial: " + ex.Message);
                    return string.Empty; // Otra acción para manejar el error según tu necesidad
                }
            }
        }
    }
}