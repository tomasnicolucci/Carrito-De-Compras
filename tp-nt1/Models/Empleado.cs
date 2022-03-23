using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace tp_nt1.Models
{
    public class Empleado : Usuario
    {
        public override Rol Rol => Rol.Empleado;
    }
}
