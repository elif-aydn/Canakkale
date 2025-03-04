using System;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Collections.Generic;
using canakkale.Models;

public class ImagesController : Controller
{
    private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

    // GET: Admin/ManageImages
    public ActionResult ManageImages()
    {
        var images = new List<Image>();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Images";
            SqlCommand command = new SqlCommand(query, connection);
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
                        Category = reader.IsDBNull(3) ? null : reader.GetString(3)
                    });
                }
            }
        }

        return View(images); // Veriler View'e gönderilir
    }


    // POST: Admin/ManageImages
    [HttpPost]
    public ActionResult ManageImages(string Url, string Description, string Category)
    {
        if (string.IsNullOrWhiteSpace(Url) || string.IsNullOrWhiteSpace(Category))
        {
            TempData["ErrorMessage"] = "URL ve Kategori alanları zorunludur.";
            return RedirectToAction("ManageImages");
        }

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO Images (Url, Description, Category) VALUES (@Url, @Description, @Category)";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Url", Url);
            command.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(Description) ? (object)DBNull.Value : Description);
            command.Parameters.AddWithValue("@Category", Category);

            connection.Open();
            command.ExecuteNonQuery();
        }

        TempData["Message"] = "Resim başarıyla kaydedildi!";
        return RedirectToAction("ManageImages");
    }

    public ActionResult Edit(int id)
    {
        Image image = null;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            string query = "SELECT * FROM Images WHERE Id = @Id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    image = new Image
                    {
                        Id = reader.GetInt32(0),
                        Url = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Category = reader.IsDBNull(3) ? null : reader.GetString(3)
                    };
                }
            }
        }

        if (image == null)
        {
            return HttpNotFound("Resim bulunamadı.");
        }

        return View(image);
    }
    [HttpPost]
    public ActionResult Edit(int id, string Url, string Description, string Category)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Images SET Url = @Url, Description = @Description, Category = @Category WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Url", Url);
                command.Parameters.AddWithValue("@Description", string.IsNullOrEmpty(Description) ? (object)DBNull.Value : Description);
                command.Parameters.AddWithValue("@Category", Category);

                connection.Open();
                command.ExecuteNonQuery();
            }

            TempData["Message"] = "Resim başarıyla güncellendi!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Hata: " + ex.Message;
        }

        return RedirectToAction("ManageImages");
    }
    [HttpPost]
    public ActionResult Delete(int id)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Images WHERE Id = @Id";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }

            TempData["Message"] = "Resim başarıyla silindi!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Hata: " + ex.Message;
        }

        return RedirectToAction("ManageImages");
    }


}
