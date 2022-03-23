﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tp_nt1.Models
{
    public class StockItem
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(0,100000)]
        [Display(Name = "Cantidad")]
        public int Cantidad { get; set; }

        [ForeignKey(nameof(Sucursal))]
        public Guid SucursalId { get; set; }
        public Sucursal Sucursal { get; set; }

        [ForeignKey(nameof(Producto))]
        public Guid ProductoId { get; set; }
        public Producto Producto { get; set; }
    }
}
