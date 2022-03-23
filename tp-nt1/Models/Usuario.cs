using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace tp_nt1.Models
{
    public abstract class Usuario
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(50, ErrorMessage = "El campo {0} acepta un máximo de {1} caracteres")]
        [MinLength(3, ErrorMessage = "El campo {0} acepta un mínimo de {1} caracteres")]
        [RegularExpression(@"[a-zA-Z áéíóú]*", ErrorMessage = "El campo {0} solamente admite caracteres alfabéticos")]
        [Display(Name = "Nombre")]
        public String Nombre { get; set; }
        
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(50, ErrorMessage = "El campo {0} acepta un máximo de {1} caracteres")]
        [MinLength(3, ErrorMessage = "El campo {0} acepta un mínimo de {1} caracteres")]
        [RegularExpression(@"[a-zA-Z áéíóú]*", ErrorMessage = "El campo {0} solamente admite caracteres alfabéticos")]
        [Display(Name = "Apellido")]
        public String Apellido { get; set; }
        
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo {0} solamente admite formatos de email")]
        [MaxLength(100, ErrorMessage = "El campo {0} acepta un máximo de {1} caracteres")]
        [MinLength(3, ErrorMessage = "El campo {0} acepta un mínimo de {1} caracteres")]
        [Display(Name = "Email")]
        public String Email { get; set; }
        
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(50, ErrorMessage = "El campo {0} acepta un máximo de {1} caracteres")]
        [MinLength(6, ErrorMessage = "El campo {0} acepta un mínimo de {1} caracteres")]
        [RegularExpression(@"[0-9/+-]*", ErrorMessage = "El campo {0} solamente admite caracteres alfabéticos")]
        [Display(Name = "Teléfono")]
        public String Telefono { get; set; }
        
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(100, ErrorMessage = "El campo {0} acepta un máximo de {1} caracteres")]
        [MinLength(5, ErrorMessage = "El campo {0} acepta un mínimo de {1} caracteres")]
        [RegularExpression(@"[a-zA-Z áéíóú0-9]*", ErrorMessage = "El campo {0} solamente admite caracteres alfabéticos")]
        [Display(Name = "Dirección")]
        public String Direccion { get; set; }
       
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Fecha de alta")]
        public DateTime FechaAlta { get; set; }
        
        public byte[] Password { get; set; }

        public abstract Rol Rol { get; }
    }
}
