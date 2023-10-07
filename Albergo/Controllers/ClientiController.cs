using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Albergo.Models;

namespace Albergo.Controllers
{
    public class ClientiController : Controller
    {
        private static string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["db"].ConnectionString;
        }

        [HttpGet]
        public ActionResult AggiungiCliente()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AggiungiCliente(Cliente model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("INSERT INTO Clienti (CodiceFiscale, Cognome, Nome, Citta, Provincia, Email, Telefono, Cellulare) VALUES (@CodiceFiscale, @Cognome, @Nome, @Citta, @Provincia, @Email, @Telefono, @Cellulare)", connection))
                    {
                        command.Parameters.AddWithValue("@CodiceFiscale", model.CodiceFiscale);
                        command.Parameters.AddWithValue("@Cognome", model.Cognome);
                        command.Parameters.AddWithValue("@Nome", model.Nome);
                        command.Parameters.AddWithValue("@Citta", model.Citta);
                        command.Parameters.AddWithValue("@Provincia", model.Provincia);
                        command.Parameters.AddWithValue("@Email", model.Email);
                        command.Parameters.AddWithValue("@Telefono", model.Telefono);
                        command.Parameters.AddWithValue("@Cellulare", model.Cellulare);
                        command.ExecuteNonQuery();
                    }
                }
                ViewBag.Message = "Cliente aggiunto con successo!";
                return RedirectToAction("ListaClienti"); 
            }
            return View(model);
        }



        [HttpGet]
        [Authorize]
        public ActionResult ListaClienti()
        {
            List<Cliente> clienti = new List<Cliente>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM Clienti", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clienti.Add(new Cliente
                            {
                                ClienteID = (int)reader["ClienteID"],
                                CodiceFiscale = reader["CodiceFiscale"].ToString(),
                                Cognome = reader["Cognome"].ToString(),
                                Nome = reader["Nome"].ToString(),
                                Citta = reader["Citta"].ToString(),
                                Provincia = reader["Provincia"].ToString(),
                                Email = reader["Email"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                Cellulare = reader["Cellulare"].ToString()
                            });
                        }
                    }
                }
            }
            return View(clienti);
        }

        [HttpGet]
        [Authorize]
        public ActionResult ModificaCliente(int id)
        {
            Cliente cliente = null;
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM Clienti WHERE ClienteID = @ClienteID", connection))
                {
                    command.Parameters.AddWithValue("@ClienteID", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cliente = new Cliente
                            {
                                ClienteID = reader.GetInt32(0),
                                CodiceFiscale = reader.GetString(1),
                                Cognome = reader.GetString(2),
                                Nome = reader.GetString(3),
                                Citta = reader.GetString(4),
                                Provincia = reader.GetString(5),
                                Email = reader.GetString(6),
                                Telefono = reader.GetString(7),
                                Cellulare = reader.GetString(8)
                            };
                        }
                    }
                }
            }
            return View(cliente);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ModificaCliente(Cliente model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("UPDATE Clienti SET CodiceFiscale = @CodiceFiscale, Cognome = @Cognome, Nome = @Nome, Citta = @Citta, Provincia = @Provincia, Email = @Email, Telefono = @Telefono, Cellulare = @Cellulare WHERE ClienteID = @ClienteID", connection))
                    {
                        command.Parameters.AddWithValue("@ClienteID", model.ClienteID);
                        command.Parameters.AddWithValue("@CodiceFiscale", model.CodiceFiscale);
                        command.Parameters.AddWithValue("@Cognome", model.Cognome);
                        command.Parameters.AddWithValue("@Nome", model.Nome);
                        command.Parameters.AddWithValue("@Citta", model.Citta);
                        command.Parameters.AddWithValue("@Provincia", model.Provincia);
                        command.Parameters.AddWithValue("@Email", model.Email);
                        command.Parameters.AddWithValue("@Telefono", model.Telefono);
                        command.Parameters.AddWithValue("@Cellulare", model.Cellulare);
                        command.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("ListaClienti");
            }
            return View(model);
        }

        [Authorize]
        public ActionResult EliminaCliente(int id)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("DELETE FROM Clienti WHERE ClienteID = @ClienteID", connection))
                {
                    command.Parameters.AddWithValue("@ClienteID", id);
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToAction("ListaClienti");
        }
    }
}
















    