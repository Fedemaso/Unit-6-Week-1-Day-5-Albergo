using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Albergo.Models
{
    public class ServizioAggiuntivo
    {
        public int ServizioAggiuntivoID { get; set; }
        public int PrenotazioneID { get; set; }
        public int TipoServizioID { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public int Quantita { get; set; }
        public TipoServizio TipoServizio { get; set; }

        public string Descrizione { get; set; }
        public decimal Costo { get; set; }
        public int Id { get; set; }

    }


    public class ServizioAggiuntivoList
    {
        public List<TipoServizio> ServiziDisponibili { get; set; }
        public Dictionary<int, int> QuantitaSelezionate { get; set; } 
    }
}