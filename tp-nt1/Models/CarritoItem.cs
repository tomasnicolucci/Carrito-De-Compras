using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tp_nt1.Models
{
    public class CarritoItem
    {
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "Precio por unidad")]
        public Decimal ValorUnitario { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, 1000, ErrorMessage = "El campo {0} acepta un valores entre {1} y {2}")] //relacionado al stock actual * preguntar *
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [Display(Name = "Subtotal")]
        public Decimal Subtotal { get; set; }

        [ForeignKey(nameof(Carrito))]
        public Guid CarritoId { get; set; }
        public Carrito Carrito { get; set; }

        [ForeignKey(nameof(Producto))]
        public Guid ProductoId { get; set; }
        public Producto Producto { get; set; }
    }
}
