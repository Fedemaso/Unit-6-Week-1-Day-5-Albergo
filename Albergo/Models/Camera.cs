using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Albergo.Models
{
    public class Camera
    {
        public int CameraID { get; set; }
        public int Numero { get; set; }
        public string Descrizione { get; set; }
        public string Tipologia { get; set; } 
    }
}