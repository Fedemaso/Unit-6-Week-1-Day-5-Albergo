﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Albergo.Models
{
    public class PrenotazioneViewModel
    {
        public int PrenotazioneID { get; set; }
        public string ClienteNome { get; set; } 
        public int NumeroCamera { get; set; } 
        public DateTime DataPrenotazione { get; set; }
        public int NumeroProgressivo { get; set; }
        public int Anno { get; set; }
        public DateTime PeriodoDal { get; set; }
        public DateTime PeriodoAl { get; set; }
        public decimal Caparra { get; set; }
        public decimal Tariffa { get; set; }
        public string Dettagli { get; set; }
        public string TipologiaPernottamento { get; set; }
        public List<ServizioAggiuntivo> ServiziAggiuntivi { get; set; } = new List<ServizioAggiuntivo>(); 


    }
}
