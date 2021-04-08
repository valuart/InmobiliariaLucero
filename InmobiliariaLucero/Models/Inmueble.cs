using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaLucero.Models
{
    public class Inmueble
    {
        [Key]
        [DisplayName("Codigo")]
        public int IdInmueble { get; set; }

        public int IdPropietario { get; set; }

        public Propietario Propietario { get; set; }
       
        public string Direccion { get; set; }
       
        public string Tipo { get; set; }

        public decimal Precio { get; set; }

        public bool Estado { get; set; }
    }
}
