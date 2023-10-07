using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Albergo.Models;

namespace Albergo.Controllers
{
    public class PrenotazioniController : Controller
    {



        private readonly string connectionString = GetConnectionString();

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

        private List<SelectListItem> GetCamere()
        {
            List<SelectListItem> camere = new List<SelectListItem>();
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT CameraID, Numero FROM Camere", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            camere.Add(new SelectListItem
                            {
                                Value = reader["CameraID"].ToString(),
                                Text = reader["Numero"].ToString()
                            });
                        }
                    }
                }
            }
            return camere;
        }

        [HttpGet]
        [Authorize]
        public ActionResult CreaPrenotazione()
        {
            ViewBag.TipologiePernottamento = new SelectList(new List<string>
            {
                "Mezza pensione",
                "Pensione completa",
                "Pernottamento con prima colazione"
            });

            ViewBag.Clienti = GetClienti();
            ViewBag.Camere = GetCamere();
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreaPrenotazione(Prenotazione model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("INSERT INTO Prenotazioni (ClienteID, CameraID, DataPrenotazione, NumeroProgressivo, Anno, PeriodoDal, PeriodoAl, Caparra, Tariffa, Dettagli, TipologiaPernottamento) VALUES (@ClienteID, @CameraID, @DataPrenotazione, @NumeroProgressivo, @Anno, @PeriodoDal, @PeriodoAl, @Caparra, @Tariffa, @Dettagli, @TipologiaPernottamento)", connection))
                    {
                        command.Parameters.AddWithValue("@ClienteID", model.ClienteID);
                        command.Parameters.AddWithValue("@CameraID", model.CameraID);
                        command.Parameters.AddWithValue("@DataPrenotazione", model.DataPrenotazione);
                        command.Parameters.AddWithValue("@NumeroProgressivo", model.NumeroProgressivo);
                        command.Parameters.AddWithValue("@Anno", model.Anno);
                        command.Parameters.AddWithValue("@PeriodoDal", model.PeriodoDal);
                        command.Parameters.AddWithValue("@PeriodoAl", model.PeriodoAl);
                        command.Parameters.AddWithValue("@Caparra", model.Caparra);
                        command.Parameters.AddWithValue("@Tariffa", model.Tariffa);
                        command.Parameters.AddWithValue("@Dettagli", model.Dettagli);
                        command.Parameters.AddWithValue("@TipologiaPernottamento", model.TipologiaPernottamento);
                        command.ExecuteNonQuery();
                    }
                }
                ViewBag.Message = "Prenotazione creata con successo!";
                return RedirectToAction("ListaPrenotazioni");
            }

            ViewBag.TipologiePernottamento = new SelectList(new List<string>
                {
                    "Mezza pensione",
                    "Pensione completa",
                    "Pernottamento con prima colazione"
                });

            ViewBag.Clienti = GetClienti();
            ViewBag.Camere = GetCamere();
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult ListaPrenotazioni()
        {
            List<PrenotazioneViewModel> prenotazioni = new List<PrenotazioneViewModel>();

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = @"SELECT p.PrenotazioneID, c.Nome + ' ' + c.Cognome AS ClienteNome, ca.Numero AS NumeroCamera, 
                                p.DataPrenotazione, p.NumeroProgressivo, p.Anno, p.PeriodoDal, p.PeriodoAl, p.Caparra, p.Tariffa, p.Dettagli, p.TipologiaPernottamento 
                                FROM Prenotazioni p 
                                INNER JOIN Clienti c ON p.ClienteID = c.ClienteID 
                                INNER JOIN Camere ca ON p.CameraID = ca.CameraID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var prenotazione = new PrenotazioneViewModel
                            {
                                PrenotazioneID = reader.GetInt32(0),
                                ClienteNome = reader.GetString(1),
                                NumeroCamera = reader.GetInt32(2),
                                DataPrenotazione = reader.GetDateTime(3),
                                NumeroProgressivo = reader.GetInt32(4),
                                Anno = reader.GetInt32(5),
                                PeriodoDal = reader.GetDateTime(6),
                                PeriodoAl = reader.GetDateTime(7),
                                Caparra = reader.GetDecimal(8),
                                Tariffa = reader.GetDecimal(9),
                                Dettagli = reader.GetString(10),
                                TipologiaPernottamento = reader.GetString(11)
                            };

                            prenotazione.ServiziAggiuntivi = GetServiziAggiuntivi(prenotazione.PrenotazioneID);
                            prenotazioni.Add(prenotazione);
                        }
                    }
                }
            }

            return View(prenotazioni);
        }



        private List<ServizioAggiuntivo> GetServiziAggiuntivi(int prenotazioneId)
        {
            List<ServizioAggiuntivo> servizi = new List<ServizioAggiuntivo>();

            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                string query = @"SELECT * FROM ServiziAggiuntivi WHERE PrenotazioneID = @PrenotazioneID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PrenotazioneID", prenotazioneId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            servizi.Add(new ServizioAggiuntivo
                            {
                                ServizioAggiuntivoID = reader.GetInt32(reader.GetOrdinal("ServizioAggiuntivoID")),
                                PrenotazioneID = reader.GetInt32(reader.GetOrdinal("PrenotazioneID")),
                                TipoServizioID = reader.GetInt32(reader.GetOrdinal("TipoServizioID")),
                                Data = reader.GetDateTime(reader.GetOrdinal("Data")),
                                Quantita = reader.GetInt32(reader.GetOrdinal("Quantita"))
                            });
                        }
                    }
                }
            }

            return servizi;
        }





        [HttpGet]
        [Authorize]
        public ActionResult ModificaPrenotazione(int id)
        {
            Prenotazione prenotazione = null;
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM Prenotazioni WHERE PrenotazioneID = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            prenotazione = new Prenotazione
                            {
                                PrenotazioneID = reader.GetInt32(0),
                                ClienteID = reader.GetInt32(1),
                                CameraID = reader.GetInt32(2),
                                DataPrenotazione = reader.GetDateTime(3),
                                NumeroProgressivo = reader.GetInt32(4),
                                Anno = reader.GetInt32(5),
                                PeriodoDal = reader.GetDateTime(6),
                                PeriodoAl = reader.GetDateTime(7),
                                Caparra = reader.GetDecimal(8),
                                Tariffa = reader.GetDecimal(9),
                                Dettagli = reader.GetString(10),
                                TipologiaPernottamento = reader.GetString(11)
                            };
                        }
                    }
                }
            }

            ViewBag.TipologiePernottamento = new SelectList(new List<string>
            {
                "Mezza pensione",
                "Pensione completa",
                "Pernottamento con prima colazione"
            });

            ViewBag.Clienti = GetClienti();
            ViewBag.Camere = GetCamere();
            return View(prenotazione);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ModificaPrenotazione(Prenotazione model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(GetConnectionString()))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("UPDATE Prenotazioni SET ClienteID = @ClienteID, CameraID = @CameraID, DataPrenotazione = @DataPrenotazione, NumeroProgressivo = @NumeroProgressivo, Anno = @Anno, PeriodoDal = @PeriodoDal, PeriodoAl = @PeriodoAl, Caparra = @Caparra, Tariffa = @Tariffa, Dettagli = @Dettagli, TipologiaPernottamento = @TipologiaPernottamento WHERE PrenotazioneID = @PrenotazioneID", connection))
                    {
                        command.Parameters.AddWithValue("@PrenotazioneID", model.PrenotazioneID);
                        command.Parameters.AddWithValue("@ClienteID", model.ClienteID);
                        command.Parameters.AddWithValue("@CameraID", model.CameraID);
                        command.Parameters.AddWithValue("@DataPrenotazione", model.DataPrenotazione);
                        command.Parameters.AddWithValue("@NumeroProgressivo", model.NumeroProgressivo);
                        command.Parameters.AddWithValue("@Anno", model.Anno);
                        command.Parameters.AddWithValue("@PeriodoDal", model.PeriodoDal);
                        command.Parameters.AddWithValue("@PeriodoAl", model.PeriodoAl);
                        command.Parameters.AddWithValue("@Caparra", model.Caparra);
                        command.Parameters.AddWithValue("@Tariffa", model.Tariffa);
                        command.Parameters.AddWithValue("@Dettagli", model.Dettagli);
                        command.Parameters.AddWithValue("@TipologiaPernottamento", model.TipologiaPernottamento);
                        command.ExecuteNonQuery();
                    }
                }
                return RedirectToAction("ListaPrenotazioni");
            }

            ViewBag.TipologiePernottamento = new SelectList(new List<string>
                {
                    "Mezza pensione",
                    "Pensione completa",
                    "Pernottamento con prima colazione"
                });
            ViewBag.Clienti = GetClienti();
            ViewBag.Camere = GetCamere();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EliminaPrenotazione(int id)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("DELETE FROM Prenotazioni WHERE PrenotazioneID = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToAction("ListaPrenotazioni");
        }


        [HttpGet]
        public ActionResult RicercaPrenotazioni()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RicercaPrenotazione(string codiceFiscale)
        {
            try
            {
                using (var db = new SqlConnection(GetConnectionString()))
                {
                    db.Open();

                    string query = "SELECT TOP 1 P.* FROM Prenotazioni P INNER JOIN Clienti C ON P.ClienteID = C.ClienteID WHERE C.CodiceFiscale = @CodiceFiscale";
                    using (var cmd = new SqlCommand(query, db))
                    {
                        cmd.Parameters.AddWithValue("@CodiceFiscale", codiceFiscale);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var prenotazione = new
                                {
                                    clienteNome = reader["Nome"].ToString() + " " + reader["Cognome"].ToString(),
                                    dataPrenotazione = Convert.ToDateTime(reader["DataPrenotazione"]).ToString("yyyy-MM-dd"),
                                    numeroProgressivo = reader["NumeroProgressivo"].ToString(),
                                    anno = reader["Anno"].ToString(),
                                    periodoDal = Convert.ToDateTime(reader["PeriodoDal"]).ToString("yyyy-MM-dd"),
                                    periodoAl = Convert.ToDateTime(reader["PeriodoAl"]).ToString("yyyy-MM-dd"),
                                    caparra = reader["Caparra"].ToString(),
                                    tariffa = reader["Tariffa"].ToString(),
                                    tipologiaPernottamento = reader["TipologiaPernottamento"].ToString(),
                                    dettagli = reader["Dettagli"].ToString()
                                };

                                return Json(prenotazione, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }

                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult QuantitaPensioneCompleta()
        {
            int count = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT COUNT(*) FROM Prenotazioni WHERE TipologiaPernottamento = 'pensione completa'";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }

                connection.Close();
            }

            return Json(count, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult VisualizzaPrenotazioniPensioneCompleta()
        {
            return View();
        }



        [HttpGet]
        public ActionResult GetPrenotazioniPensioneCompleta()
        {
            List<Prenotazione> prenotazioni = new List<Prenotazione>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Prenotazioni WHERE TipologiaPernottamento = 'pensione completa'";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Prenotazione prenotazione = new Prenotazione
                            {
                                PrenotazioneID = Convert.ToInt32(reader["PrenotazioneID"]),
                                ClienteID = Convert.ToInt32(reader["ClienteID"]),
                                CameraID = Convert.ToInt32(reader["CameraID"]),
                                DataPrenotazione = Convert.ToDateTime(reader["DataPrenotazione"]),
                                NumeroProgressivo = Convert.ToInt32(reader["NumeroProgressivo"]),
                                Anno = Convert.ToInt32(reader["Anno"]),
                                PeriodoDal = Convert.ToDateTime(reader["PeriodoDal"]),
                                PeriodoAl = Convert.ToDateTime(reader["PeriodoAl"]),
                                Caparra = Convert.ToDecimal(reader["Caparra"]),
                                Tariffa = Convert.ToDecimal(reader["Tariffa"]),
                                TipologiaPernottamento = reader["TipologiaPernottamento"].ToString(),
                                Dettagli = reader["Dettagli"].ToString()
                            };
                            prenotazioni.Add(prenotazione);
                        }
                    }
                }
                connection.Close();
            }
            return Json(prenotazioni, JsonRequestBehavior.AllowGet);
        }




        //public ActionResult Checkout(int prenotazioneId)
        //{
        //    Prenotazione prenotazione;
        //    List<ServizioAggiuntivo> serviziAggiuntivi = new List<ServizioAggiuntivo>();

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        string prenotazioneQuery = "SELECT * FROM Prenotazioni WHERE PrenotazioneID = @PrenotazioneID";
        //        using (SqlCommand prenotazioneCmd = new SqlCommand(prenotazioneQuery, connection))
        //        {
        //            prenotazioneCmd.Parameters.AddWithValue("@PrenotazioneID", prenotazioneId);
        //            using (SqlDataReader prenotazioneReader = prenotazioneCmd.ExecuteReader())
        //            {
        //                if (prenotazioneReader.Read())
        //                {
        //                    prenotazione = new Prenotazione
        //                    {
        //                        PrenotazioneID = Convert.ToInt32(prenotazioneReader["PrenotazioneID"]),
        //                        NumeroStanza = prenotazioneReader["NumeroStanza"].ToString(),
        //                        PeriodoDal = Convert.ToDateTime(prenotazioneReader["PeriodoDal"]),
        //                        PeriodoAl = Convert.ToDateTime(prenotazioneReader["PeriodoAl"]),
        //                        Tariffa = Convert.ToDecimal(prenotazioneReader["Tariffa"]),
        //                        Caparra = Convert.ToDecimal(prenotazioneReader["Caparra"]),
        //                    };
        //                }
        //                else
        //                {
        //                    return HttpNotFound();
        //                }
        //            }
        //        }

        //        string serviziQuery = "SELECT * FROM ServiziAggiuntivi WHERE PrenotazioneID = @PrenotazioneID";
        //        using (SqlCommand serviziCmd = new SqlCommand(serviziQuery, connection))
        //        {
        //            serviziCmd.Parameters.AddWithValue("@PrenotazioneID", prenotazioneId);
        //            using (SqlDataReader serviziReader = serviziCmd.ExecuteReader())
        //            {
        //                while (serviziReader.Read())
        //                {
        //                    var servizioAggiuntivo = new ServizioAggiuntivo
        //                    {
        //                        Descrizione = serviziReader["Descrizione"].ToString(),
        //                        Costo = Convert.ToDecimal(serviziReader["Costo"]),
        //                    };
        //                    serviziAggiuntivi.Add(servizioAggiuntivo);
        //                }
        //            }
        //        }
        //    }

        //    decimal importoTotale = prenotazione.Tariffa - prenotazione.Caparra + serviziAggiuntivi.Sum(s => s.Costo);

        //    var checkoutModel = new Checkout
        //    {
        //        PrenotazioneID = prenotazione.PrenotazioneID,
        //        NumeroStanza = prenotazione.NumeroStanza,
        //        PeriodoDal = prenotazione.PeriodoDal,
        //        PeriodoAl = prenotazione.PeriodoAl,
        //        Tariffa = prenotazione.Tariffa,
        //        Caparra = prenotazione.Caparra,
        //        ServiziAggiuntivi = serviziAggiuntivi,
        //        ImportoTotale = importoTotale,
        //    };

        //    return View(checkoutModel);
        //}





        //private Prenotazione GetPrenotazioneById(int id)
        //{
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        string query = "SELECT * FROM Prenotazioni WHERE PrenotazioneID = @PrenotazioneID";
        //        using (SqlCommand cmd = new SqlCommand(query, connection))
        //        {
        //            cmd.Parameters.AddWithValue("@PrenotazioneID", id);
        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                if (reader.Read())
        //                {
        //                    Prenotazione prenotazione = new Prenotazione
        //                    {
        //                        PrenotazioneID = Convert.ToInt32(reader["PrenotazioneID"]),
        //                        ClienteID = Convert.ToInt32(reader["ClienteID"]),
        //                        CameraID = Convert.ToInt32(reader["CameraID"]),
        //                        DataPrenotazione = Convert.ToDateTime(reader["DataPrenotazione"]),
        //                        NumeroProgressivo = Convert.ToInt32(reader["NumeroProgressivo"]),
        //                        Anno = Convert.ToInt32(reader["Anno"]),
        //                        PeriodoDal = Convert.ToDateTime(reader["PeriodoDal"]),
        //                        PeriodoAl = Convert.ToDateTime(reader["PeriodoAl"]),
        //                        Caparra = Convert.ToDecimal(reader["Caparra"]),
        //                        Tariffa = Convert.ToDecimal(reader["Tariffa"]),
        //                        TipologiaPernottamento = reader["TipologiaPernottamento"].ToString(),
        //                        Dettagli = reader["Dettagli"].ToString()
        //                    };


        //                    return prenotazione;
        //                }
        //            }
        //        }
        //    }

        //    return null;
        //}

        //private List<ServizioAggiuntivo> GetServiziAggiuntiviByPrenotazioneId(int prenotazioneId)
        //{
        //    List<ServizioAggiuntivo> serviziAggiuntivi = new List<ServizioAggiuntivo>();

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        string query = "SELECT * FROM ServiziAggiuntivi WHERE PrenotazioneID = @PrenotazioneID";
        //        using (SqlCommand cmd = new SqlCommand(query, connection))
        //        {
        //            cmd.Parameters.AddWithValue("@PrenotazioneID", prenotazioneId);

        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    ServizioAggiuntivo servizioAggiuntivo = new ServizioAggiuntivo
        //                    {
        //                        Id = Convert.ToInt32(reader["Id"]),
        //                        PrenotazioneID = Convert.ToInt32(reader["PrenotazioneID"]),
        //                        Descrizione = reader["Descrizione"].ToString(),
        //                        Costo = Convert.ToDecimal(reader["Costo"])
        //                    };
        //                    serviziAggiuntivi.Add(servizioAggiuntivo);
        //                }
        //            }
        //        }
        //    }

        //    return serviziAggiuntivi;
        //}





    }

}





