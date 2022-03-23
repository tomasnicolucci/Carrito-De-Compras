using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace tp_nt1.Models
{
    public class Cliente : Usuario
    {
        public override Rol Rol => Rol.Cliente;

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(50, ErrorMessage = "El campo {0} acepta un máximo de {1} caracteres")]
        [RegularExpression(@"[0-9/-]*", ErrorMessage = "El campo {0} solamente admite caracteres alfabéticos")]
        [Display(Name = "DNI")]
        public String DNI { get; set; }
        
        public List<Compra> Compras { get; set; }
       
        public List<Carrito> Carrito { get; set; }

    }
}

