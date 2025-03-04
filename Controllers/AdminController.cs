using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using canakkale.Models;

namespace canakkale.Controllers
{
    public class AdminController : Controller
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // İlçe Yönetim Sayfası
        public ActionResult ManageDistricts()
        {
            if (Session["UserRole"]?.ToString() != "Yönetici")
            {
                return RedirectToAction("Login", "Users");
            }

            var districts = GetDistricts();
            return View(districts);
        }

        // İlçe Verilerini Getir
        private List<District> GetDistricts()
        {
            var districts = new List<District>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Districts";
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

        // İlçe Ekleme (POST)
        [HttpPost]
        public ActionResult AddDistrict(string Name, int Population)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Districts (Name, Population) VALUES (@Name, @Population)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Population", Population);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                TempData["Message"] = "Yeni ilçe başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction("ManageDistricts");
        }

        // İlçe Silme (POST)
        [HttpPost]
        public ActionResult DeleteDistrict(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Districts WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                TempData["Message"] = "İlçe başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction("ManageDistricts");
        }

        // İlçe Güncelleme Sayfası
        public ActionResult EditDistrict(int id)
        {
            if (Session["UserRole"]?.ToString() != "Yönetici")
            {
                return RedirectToAction("Login", "Users");
            }

            var district = GetDistrictById(id);
            if (district == null)
            {
                return HttpNotFound();
            }

            return View("~/Views/Admin/EditDistrict.cshtml", district);

        }

        // İlçe Bilgilerini ID'ye Göre Getir
        private District GetDistrictById(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Districts WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new District
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Population = reader.GetInt32(2)
                        };
                    }
                }
            }
            return null;
        }


        // İlçe Güncelleme (POST)
        [HttpPost]
        public ActionResult EditDistrict(int id, string Name, int Population)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Districts SET Name = @Name, Population = @Population WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", Name);
                    command.Parameters.AddWithValue("@Population", Population);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                TempData["Message"] = "İlçe bilgileri başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction("ManageDistricts");
        }

        // EditCanakkale Action Method
        public ActionResult EditCanakkale()
        {
            if (Session["UserRole"]?.ToString() != "Yönetici")
            {
                return RedirectToAction("Login", "Users");
            }

            return View("~/Views/Admin/EditCanakkale.cshtml");
        }

        public ActionResult ManageImages()
        {
            if (Session["UserRole"]?.ToString() != "Yönetici")
            {
                return RedirectToAction("Login", "Users");
            }

            return View();
        }

        
        // Population Yönetim Sayfası
        public ActionResult ManagePopulation()
        {
            if (Session["UserRole"]?.ToString() != "Yönetici")
            {
                return RedirectToAction("Login", "Users");
            }

            var populationData = GetPopulationData();
            return View(populationData);
        }

        // Population Verilerini Getir
        private List<Populations> GetPopulationData()
        {
            var populationData = new List<Populations>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Yil, ErkekNufus, KadinNufus FROM Populations";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int malePopulation = reader.GetInt32(2);
                        int femalePopulation = reader.GetInt32(3);

                        populationData.Add(new Populations
                        {
                            Id = reader.GetInt32(0),
                            Year = reader.GetInt32(1),
                            MalePopulation = malePopulation,
                            FemalePopulation = femalePopulation,
                            TotalPopulation = malePopulation + femalePopulation // Toplam nüfus hesaplama
                        });
                    }
                }
            }

            return populationData;
        }

        // Nüfus Ekleme (POST)
        [HttpPost]
        public ActionResult AddPopulation(int Year, int MalePopulation, int FemalePopulation)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Populations (Yil, ErkekNufus, KadinNufus) VALUES (@Year, @MalePopulation, @FemalePopulation)";
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@Year", Year);
                    command.Parameters.AddWithValue("@MalePopulation", MalePopulation);
                    command.Parameters.AddWithValue("@FemalePopulation", FemalePopulation);

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        TempData["Message"] = "Yeni nüfus bilgisi başarıyla eklendi.";
                    }
                    else
                    {
                        TempData["Error"] = "Ekleme işlemi sırasında bir sorun oluştu.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction("ManagePopulation");
        }


        // Nüfus Silme (POST)
        [HttpPost]
        public ActionResult DeletePopulation(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Populations WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                TempData["Message"] = "Nüfus bilgisi başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction("ManagePopulation");
        }

        // Nüfus Güncelleme (POST)
        [HttpPost]
        public ActionResult EditPopulation(int Id, int Year, int MalePopulation, int FemalePopulation)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query;

                    // Id = 0 ise yeni kayıt ekle, değilse güncelle
                    if (Id == 0)
                    {
                        query = "INSERT INTO Populations (Yil, ErkekNufus, KadinNufus) VALUES (@Year, @MalePopulation, @FemalePopulation)";
                    }
                    else
                    {
                        query = "UPDATE Populations SET Yil = @Year, ErkekNufus = @MalePopulation, KadinNufus = @FemalePopulation WHERE Id = @Id";
                    }

                    SqlCommand command = new SqlCommand(query, connection);

                    // Parametreleri ekle
                    if (Id != 0)
                        command.Parameters.AddWithValue("@Id", Id);

                    command.Parameters.AddWithValue("@Year", Year);
                    command.Parameters.AddWithValue("@MalePopulation", MalePopulation);
                    command.Parameters.AddWithValue("@FemalePopulation", FemalePopulation);

                    connection.Open();
                    command.ExecuteNonQuery();

                    TempData["Message"] = Id == 0
                        ? "Yeni nüfus bilgisi başarıyla eklendi."
                        : "Nüfus bilgisi başarıyla güncellendi.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Bir hata oluştu: " + ex.Message;
            }

            return RedirectToAction("ManagePopulation");
        }


        // Turistik Yerler Yönetim Sayfası
        public ActionResult ManageTouristicPlaces()
        {
            if (Session["UserRole"]?.ToString() != "Yönetici")
                return RedirectToAction("Login", "Users");

            var places = GetTouristicPlaces();
            return View(places);
        }

        private List<TouristicPlace> GetTouristicPlaces()
        {
            var places = new List<TouristicPlace>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM TouristicPlaces";
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
                            Location = reader.GetString(3)
                        });
                    }
                }
            }
            return places;
        }

        // Turistik Yer Ekleme/Düzenleme İşlemleri (POST)
        [HttpPost]
        public ActionResult SaveTouristicPlace(int? id, string name, string description, string location)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query;

                if (id == null || id == 0) // Yeni ekleme
                {
                    query = "INSERT INTO TouristicPlaces (Name, Description, Location) VALUES (@Name, @Description, @Location)";
                }
                else // Güncelleme
                {
                    query = "UPDATE TouristicPlaces SET Name = @Name, Description = @Description, Location = @Location WHERE Id = @Id";
                }

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Description", description);
                command.Parameters.AddWithValue("@Location", location);

                if (id != null && id > 0)
                    command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToAction("ManageTouristicPlaces");
        }

        // Turistik Yer Silme
        [HttpPost]
        public ActionResult DeleteTouristicPlace(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM TouristicPlaces WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
            return RedirectToAction("ManageTouristicPlaces");
        }
    }
}

