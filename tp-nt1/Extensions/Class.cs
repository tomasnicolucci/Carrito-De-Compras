using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace tp_nt1.Extensions
{
    public static class ValidacionesExtensions
    {
        public static Byte[] Encriptar(this String pass)
        {
            return new SHA256Managed().ComputeHash(Encoding.ASCII.GetBytes(pass));
        }
        public static void ValidarPassword(this String password)
        {
            if (String.IsNullOrWhiteSpace(password))
            {
                throw new Exception("La contraseña esta vacía.");
            }
            if (password.Length < 8)
            {
                throw new Exception("La contraseña debe de tener más de 8 caracteres.");
            }

            bool contieneCaracterEspecial = new Regex("[@,%,&,#,$,?,¿,<,>,/]").Match(password).Success;
            bool contieneUnNumero = new Regex("[0-9]").Match(password).Success;
            bool contieneUnaLetraMinus = new Regex("[a-z]").Match(password).Success;
            bool contieneUnaLetraMayus = new Regex("[A-Z]").Match(password).Success;

            if (!contieneCaracterEspecial || !contieneUnaLetraMayus || !contieneUnaLetraMinus || !contieneUnNumero)
            {
                throw new Exception("La contraseña debe tener al menos un caracter especial, una minúscula, una mayúscula, un número.");
            }

            

        }
    }
}
