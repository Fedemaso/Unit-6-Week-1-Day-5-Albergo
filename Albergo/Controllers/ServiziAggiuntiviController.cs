using Albergo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace Albergo.Controllers
{
    public class ServiziAggiuntiviController : Controller
    {
        private static string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings["db"].ConnectionString;
        }

        private List<SelectListItem> GetClienti()
        {
            List<SelectListItem> clienti = new List<SelectListItem>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT ClienteID, Nome + ' ' + Cognome AS NomeCompleto FROM Clienti", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clienti.Add(new SelectListItem
                            {
                                Value = reader["ClienteID"].ToString(),
                                Text = reader["NomeCompleto"].ToString()
                            });
                        }
                    }
                }
            }
            return clienti;
        }

        [HttpGet]
        public JsonResult GetPrenotazioni(int id)
        {
            List<SelectListItem> prenotazioni = new List<SelectListItem>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT PrenotazioneID, NumeroProgressivo FROM Prenotazioni WHERE ClienteID = @ClienteID", connection))
                {
                    command.Parameters.AddWithValue("@ClienteID", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            prenotazioni.Add(new SelectListItem
                            {
                                Value = reader["PrenotazioneID"].ToString(),
                                Text = reader["NumeroProgressivo"].ToString()
                            });
                        }
                    }
                }
            }
            return Json(prenotazioni.Select(p => new { Value = p.Value, Text = p.Text }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AggiungiServizi(int id) 
        {
            ViewBag.Clienti = GetClienti();

            List<TipoServizio> serviziDisponibili = new List<TipoServizio>();
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                var cmdTipiServizio = new SqlCommand("SELECT TipoServizioID, Descrizione, Prezzo FROM TipiServizio", conn);
                using (var readerTipiServizio = cmdTipiServizio.ExecuteReader())
                {
                    while (readerTipiServizio.Read())
                    {
                        serviziDisponibili.Add(new TipoServizio
                        {
                            TipoServizioID = Convert.ToInt32(readerTipiServizio["TipoServizioID"]),
                            Descrizione = readerTipiServizio["Descrizione"].ToString(),
                            Prezzo = Convert.ToDecimal(readerTipiServizio["Prezzo"])
                        });
                    }
                }
                conn.Close();
            }

            var model = new ServizioAggiuntivoList
            {
                ServiziDisponibili = serviziDisponibili,
                QuantitaSelezionate = new Dictionary<int, int>()
            };

            ViewBag.PrenotazioneID = id; 

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AggiungiServizi(ServizioAggiuntivoList serviziAggiuntiviList, int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    foreach (var item in serviziAggiuntiviList.QuantitaSelezionate)
                    {
                        if (item.Value > 0)
                        {
                            string insertQuery = "INSERT INTO ServiziAggiuntivi (PrenotazioneID, TipoServizioID, Data, Quantita) VALUES (@PrenotazioneID, @TipoServizioID, @Data, @Quantita)";
                            using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@PrenotazioneID", id);
                                cmd.Parameters.AddWithValue("@TipoServizioID", item.Key);
                                cmd.Parameters.AddWithValue("@Data", DateTime.Now);
                                cmd.Parameters.AddWithValue("@Quantita", item.Value);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    conn.Close();
                }
            }
            catch (SqlException ex)
            {
                ViewBag.ErrorMessage = "Si è verificato un errore: " + ex.Message;
                return View(serviziAggiuntiviList);
            }

            return RedirectToAction("ListaPrenotazioni", "Prenotazioni");
        }

    }
}
