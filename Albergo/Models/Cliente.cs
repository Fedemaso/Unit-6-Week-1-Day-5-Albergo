using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Albergo.Models
{
    public class Cliente
    {
        public int ClienteID { get; set; }
        public string CodiceFiscale { get; set; }
        public string Cognome { get; set; }
        public string Nome { get; set; }
        public string Citta { get; set; }
        public string Provincia { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Cellulare { get; set; }
        public string NomeCompleto { get; set; }

    }
}