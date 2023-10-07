using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.SqlClient;
using Albergo.Models;

namespace Albergo.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        private static string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["db"].ConnectionString;
        }

        [HttpPost]
        public ActionResult Index(Login model)
        {
            if (ModelState.IsValid && IsValidUser(model.Username, model.Password))
            {
                string role = GetUserRole(model.Username);
               

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, model.Username, DateTime.Now, DateTime.Now.AddMinutes(30), false, role); 
                string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(cookie);


                System.Diagnostics.Debug.WriteLine("Cookie: " + cookie.Value);
                Session["IsLoggedIn"] = true;


                return RedirectToAction("Dashboard");
            }
            ModelState.AddModelError("", "Username o Password non validi.");
            return View(model);
        }

        [Authorize]
        public ActionResult Dashboard()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult RegistraDipendente()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult RegistraDipendente(Dipendente model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("INSERT INTO Dipendenti (Username, Password, Ruolo) VALUES (@Username, @Password, 'User')", connection))
                    {
                        command.Parameters.AddWithValue("@Username", model.Username);
                        command.Parameters.AddWithValue("@Password", model.Password);
                        command.ExecuteNonQuery();
                    }
                }
                ViewBag.Message = "Dipendente registrato con successo!";
                return RedirectToAction("Dashboard");
            }
            return View(model);
        }

        private bool IsValidUser(string username, string password)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Dipendenti WHERE Username = @Username AND Password = @Password", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private string GetUserRole(string username)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT Ruolo FROM Dipendenti WHERE Username = @Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    return command.ExecuteScalar().ToString();
                }
            }
        }



        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }


    }
}

