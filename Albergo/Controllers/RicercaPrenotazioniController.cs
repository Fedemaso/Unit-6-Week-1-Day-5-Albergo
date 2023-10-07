using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Albergo.Models;

public class RicercaPrenotazioniController : Controller
{
    private string connectionString = GetConnectionString();

    private static string GetConnectionString()
    {
        return ConfigurationManager.ConnectionStrings["db"].ConnectionString;
    }

    [HttpPost]
    public ActionResult RicercaCliente(string codiceFiscale)
    {
        Cliente cliente = GetClienteByCodiceFiscale(codiceFiscale);

        if (cliente != null)
        {
            ViewBag.Cliente = cliente;
            bool haPrenotazioniAttive = HaPrenotazioniAttive(cliente.ClienteID);

            ViewBag.HaPrenotazioniAttive = haPrenotazioniAttive;
            if (haPrenotazioniAttive)
            {
                Prenotazione prenotazione = GetPrenotazioneAttiva(cliente.ClienteID);
                ViewBag.Prenotazione = prenotazione;
            }
        }
        else
        {
            ViewBag.ClienteNonTrovato = true;
        }

        return PartialView("_RisultatoRicerca", ViewBag);
    }




    private Cliente GetClienteByCodiceFiscale(string codiceFiscale)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM Clienti WHERE CodiceFiscale = @CodiceFiscale";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@CodiceFiscale", codiceFiscale);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Cliente cliente = new Cliente
                        {
                            ClienteID = Convert.ToInt32(reader["ClienteID"]),
                            CodiceFiscale = reader["CodiceFiscale"].ToString(),
                            Cognome = reader["Cognome"].ToString(),
                            Nome = reader["Nome"].ToString(),
                            Citta = reader["Citta"].ToString(),
                            Provincia = reader["Provincia"].ToString(),
                            Email = reader["Email"].ToString(),
                            Telefono = reader["Telefono"].ToString(),
                            Cellulare = reader["Cellulare"].ToString(),
                            NomeCompleto = reader["NomeCompleto"].ToString()
                        };
                        return cliente;
                    }
                }
            }
        }

        return null;
    }

    private bool HaPrenotazioniAttive(int clienteID)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT COUNT(*) FROM Prenotazioni WHERE ClienteID = @ClienteID";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@ClienteID", clienteID);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }
    }

    private Prenotazione GetPrenotazioneAttiva(int clienteID)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM Prenotazioni WHERE ClienteID = @ClienteID";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@ClienteID", clienteID);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
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
                        return prenotazione;
                    }
                }
            }
        }

        return null;
    }
}




