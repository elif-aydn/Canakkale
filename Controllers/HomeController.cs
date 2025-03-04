using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using canakkale.Models;

namespace canakkale.Controllers
{

    public class HomeController : Controller
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // Ana sayfa (Slider için resimleri getiriyoruz)
        public ActionResult Index()
        {
            var images = GetImagesByCategory("Slider");
            return View(images);
        }

        // Çanakkale Sayfası
        public ActionResult Canakkale()
        {
            // Kullanıcı oturum bilgilerini kontrol et
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Users");
            }

            // Kullanıcı bilgilerini oturumdan al
            ViewBag.UserName = Session["UserName"];

            // Veritabanından yıllık nüfus bilgilerini al
            var populationData = GetLastThreeYearsPopulationData();

            return View(populationData);
        }

        // Nüfus Verilerini Göster
        public ActionResult Population()
        {
            try
            {
                // Veritabanından son 3 yılın nüfus verilerini çek
                var populationData = GetLastThreeYearsPopulationData();

                // Görünümü verilerle birlikte döndür
                return View(populationData);
            }
            catch (Exception ex)
            {
                // Hata mesajı ile kullanıcıya bilgi ver
                TempData["Error"] = "Nüfus verileri alınırken bir hata oluştu: " + ex.Message;
                return View(new List<Populations>());
            }
        }

        // Son 3 Yılın Nüfus Verilerini Getir
        private List<Populations> GetLastThreeYearsPopulationData()
        {
            var populationData = new List<Populations>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT TOP 3 
                               Yil, 
                               SUM(ErkekNufus) AS ErkekNufus, 
                               SUM(KadinNufus) AS KadinNufus,
                               SUM(ErkekNufus + KadinNufus) AS ToplamNufus
                        FROM Populations
                        GROUP BY Yil
                        ORDER BY Yil DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            populationData.Add(new Populations
                            {
                                Year = reader.GetInt32(0),
                                MalePopulation = reader.GetInt32(1),
                                FemalePopulation = reader.GetInt32(2),
                                TotalPopulation = reader.GetInt32(3)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Veritabanı işlemi sırasında bir hata oluştu: " + ex.Message;
            }

            return populationData;
        }

        // Kategoriye Göre Resimleri Getiren Ortak Metod
        private List<Image> GetImagesByCategory(string category)
        {
            var images = new List<Image>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT * FROM Images WHERE Category = @Category";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Category", category);

                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            images.Add(new Image
                            {
                                Id = reader.GetInt32(0),
                                Url = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Category = category
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Görseller alınırken bir hata oluştu: " + ex.Message;
            }

            return images;
        }


        // Çanakkale'nin Turistik Yerleri Sayfası
        public ActionResult TouristicPlaces()
        {
            // Veritabanından turistik yerleri al
            var touristicPlaces = GetTouristicPlacesFromDatabase();

            // Veriyi View'e gönder
            return View(touristicPlaces);
        }

        // Veritabanından turistik yerleri çeken metod
        private List<TouristicPlace> GetTouristicPlacesFromDatabase()
        {
            var places = new List<TouristicPlace>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Name, Description, Location FROM TouristicPlaces";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        places.Add(new TouristicPlace
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Location = reader.IsDBNull(3) ? null : reader.GetString(3)
                        });
                    }
                }
            }

            return places;
        }

        public ActionResult Districts()
        {
            // Veritabanından ilçeleri al
            var districts = GetDistrictsFromDatabase();

            // Veriyi View'e gönder
            return View(districts);
        }

        // Veritabanından ilçeleri çeken metod
        private List<District> GetDistrictsFromDatabase()
        {
            var districts = new List<District>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Name, Population FROM Districts";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        districts.Add(new District
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Population = reader.GetInt32(2)
                        });
                    }
                }
            }

            return districts;
        }
    }
}


