using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Albergo.Models
{
    public class TipoServizio
    {
        public int TipoServizioID { get; set; }
        public string Descrizione { get; set; }
        public decimal Prezzo { get; set; }
    }
}