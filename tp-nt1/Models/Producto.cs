using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tp_nt1.Models
{
    public class Producto
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(50, ErrorMessage = "El campo {0} acepta un máximo de {1} caracteres")]
        [MinLength(3, ErrorMessage = "El campo {0} acepta un mínimo de {1} caracteres")]
        [RegularExpression(@"[a-zA-Z áéíóú0-9]*", ErrorMessage = "El campo {0} solamente admite caracteres alfabéticos")]
        [Display(Name = "Nombre")]
        public String Nombre { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [MaxLength(50, ErrorMessage = "El campo {0} acepta un máximo de {1} caracteres")]
        [MinLength(3, ErrorMessage = "El campo {0} acepta un mínimo de {1} caracteres")]
        [RegularExpression(@"[a-zA-Z áéíóú0-9]*", ErrorMessage = "El campo {0} solamente admite caracteres alfabéticos")]
        [Display(Name = "Descripción")]
        public String Descripcion { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0,1000000, ErrorMessage = "El campo {0} acepta un valores entre {1} y {2}")]
        [Display(Name = "Precio")]
        public decimal PrecioVigente { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Disponibilidad")]
        public bool EsActivo { get; set; }

        [ForeignKey(nameof(Categoria))]
        public Guid CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

    }
}
