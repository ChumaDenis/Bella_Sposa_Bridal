using System.Xml.Linq;
using BellaSposaBridal.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bella_Sposa_Bridal_api.Controllers;

[ApiController]
public class SitemapController : ControllerBase
{
    private static readonly XNamespace Ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

    private readonly IDressService _dressService;
    private readonly ICollectionService _collectionService;
    private readonly IConfiguration _configuration;

    public SitemapController(
        IDressService dressService,
        ICollectionService collectionService,
        IConfiguration configuration)
    {
        _dressService = dressService;
        _collectionService = collectionService;
        _configuration = configuration;
    }

    [HttpGet("api/sitemap.xml")]
    [ResponseCache(Duration = 3600)]
    public async Task<IActionResult> Get()
    {
        // Public site origin; override via PUBLIC_SITE_URL env variable.
        var baseUrl = (_configuration["PUBLIC_SITE_URL"] ?? "https://bellasposa.co.uk").TrimEnd('/');

        var urls = new List<XElement>
        {
            UrlElement($"{baseUrl}/", null, "weekly", "1.0"),
            UrlElement($"{baseUrl}/catalog", null, "weekly", "0.9"),
            UrlElement($"{baseUrl}/collections", null, "weekly", "0.9"),
            UrlElement($"{baseUrl}/appointment", null, "monthly", "0.8"),
            UrlElement($"{baseUrl}/contact", null, "monthly", "0.7"),
        };

        var collections = await _collectionService.GetAllAsync(includeDeleted: false);
        urls.AddRange(collections
            .Where(c => c.IsActive)
            .Select(c => UrlElement($"{baseUrl}/collections/{c.Slug}", c.UpdatedAt, "weekly", "0.8")));

        var dresses = await _dressService.GetAllAsync(includeDeleted: false);
        urls.AddRange(dresses
            .Where(d => d.IsActive)
            .Select(d => UrlElement($"{baseUrl}/catalog/{d.Slug}", d.CreatedAt, "monthly", "0.6")));

        var doc = new XDocument(new XElement(Ns + "urlset", urls));
        var xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" + doc;

        return Content(xml, "application/xml; charset=utf-8");
    }

    private static XElement UrlElement(string loc, DateTime? lastMod, string changeFreq, string priority)
    {
        var el = new XElement(Ns + "url", new XElement(Ns + "loc", loc));
        if (lastMod.HasValue)
        {
            el.Add(new XElement(Ns + "lastmod", lastMod.Value.ToString("yyyy-MM-dd")));
        }
        el.Add(new XElement(Ns + "changefreq", changeFreq));
        el.Add(new XElement(Ns + "priority", priority));
        return el;
    }
}
