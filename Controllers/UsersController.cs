using System;
using System.Data.SqlClient;
using System.Web.Mvc;

public class UsersController : Controller
{
    private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

    // GET: Users/Create (Formu görüntüler)
    public ActionResult Create()
    {
        return View();
    }

    // POST: Users/Create (Formdan gelen veriyi işler)
    [HttpPost]
    public ActionResult Create(string Ad, string Soyad, string Email, string Sifre, string Rol)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Users (Ad, Soyad, Email, Sifre, Rol) VALUES (@Ad, @Soyad, @Email, @Sifre, @Rol)";
                SqlCommand command = new SqlCommand(query, connection);

                // Parametreleri ekle
                command.Parameters.AddWithValue("@Ad", Ad);
                command.Parameters.AddWithValue("@Soyad", Soyad);
                command.Parameters.AddWithValue("@Email", Email);
                command.Parameters.AddWithValue("@Sifre", Sifre);
                command.Parameters.AddWithValue("@Rol", Rol);

                connection.Open();
                command.ExecuteNonQuery(); // Sorguyu çalıştır

                ViewBag.Message = "Kullanıcı başarıyla eklendi.";
            }
        }
        catch (Exception ex)
        {
            ViewBag.Message = "Kullanıcı eklenirken bir hata oluştu: " + ex.Message;
        }

        return View();
    }

    // GET: Users/Login (Giriş formunu görüntüler)
    public ActionResult Login()
    {
        return View();
    }

    // POST: Users/Login (Giriş işlemini yapar)
    [HttpPost]
    public ActionResult Login(string Email, string Sifre, string Rol)
    {
        // Boş alan kontrolü
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Sifre) || string.IsNullOrWhiteSpace(Rol))
        {
            ViewBag.Message = "E-posta, şifre ve rol alanları boş bırakılamaz.";
            return View();
        }

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // SQL sorgusu (Email, Sifre ve Rol kontrolü)
                string query = "SELECT Id, Ad FROM Users WHERE Email = @Email AND Sifre = @Sifre AND Rol = @Rol";
                SqlCommand command = new SqlCommand(query, connection);

                // Parametreleri ekle
                command.Parameters.AddWithValue("@Email", Email.Trim());
                command.Parameters.AddWithValue("@Sifre", Sifre.Trim());
                command.Parameters.AddWithValue("@Rol", Rol.Trim());

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Giriş başarılı
                        Session["UserId"] = reader["Id"];
                        Session["UserName"] = reader["Ad"];
                        Session["UserRole"] = Rol; // Rol bilgisini oturuma ekle

                        // Role göre yönlendirme
                        if (Rol.Equals("Yönetici", StringComparison.OrdinalIgnoreCase))
                        {
                            return RedirectToAction("EditCanakkale", "Admin");
                        }
                        else if (Rol.Equals("Kullanıcı", StringComparison.OrdinalIgnoreCase))
                        {
                            return RedirectToAction("Canakkale", "Home");
                        }
                        else
                        {
                            ViewBag.Message = "Geçersiz rol.";
                            return View();
                        }
                    }
                    else
                    {
                        // Hatalı giriş
                        ViewBag.Message = "Geçersiz e-posta, şifre veya rol.";
                        return View();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ViewBag.Message = "Giriş işlemi sırasında bir hata oluştu: " + ex.Message;
            return View();
        }
    }
}
