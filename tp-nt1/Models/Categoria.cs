using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace tp_nt1.Models
{
    [Display(Name = "Categoría")]
    public class Categoria
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
        [MaxLength(200, ErrorMessage = "El campo {0} acepta un máximo de {1} caracteres")]
        [MinLength(3, ErrorMessage = "El campo {0} acepta un mínimo de {1} caracteres")]
        [RegularExpression(@"[a-zA-Z,ñÑ áéíóú]*", ErrorMessage = "El campo {0} solamente admite caracteres alfabéticos")]
        [Display(Name = "Descripción")]
        public String Descripcion { get; set; }

        [Display(Name = "Productos")]
        public List<Producto> Productos { get; set; }
    }
}
