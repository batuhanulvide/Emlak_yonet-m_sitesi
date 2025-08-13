using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RealEstate.UI.DTOs.CategoryDtos;
using RealEstate.UI.Models;
using System.Text;

namespace RealEstate.UI.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        public DefaultController(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("http://localhost:5000"); // API'nin çalıştığı adres
                var responseMessage = await client.GetAsync("api/Categories"); // Tam endpoint yolu

                if (responseMessage.IsSuccessStatusCode)
                {
                    var jsonData = await responseMessage.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<List<ResultCategoryDto>>(jsonData);
                    return View(values);
                }

                // Hata durumunda loglama
                Console.WriteLine($"API Error: {responseMessage.StatusCode}");
                return View(new List<ResultCategoryDto>()); // Boş liste dön
            }
            catch (Exception ex)
            {
                // Hata yakalama
                Console.WriteLine($"Exception: {ex.Message}");
                return View(new List<ResultCategoryDto>()); // Boş liste dön
            }
        }

        [HttpPost]
        public IActionResult PartialSearch(string searchKeyValue, int propertyCategoryId, string city)
        {
            TempData["searchKeyValue"] = searchKeyValue;
            TempData["propertyCategoryId"] = propertyCategoryId;
            TempData["city"] = city;
            return RedirectToAction("PropertyListWithSearch", "Property");
        }
    }
}