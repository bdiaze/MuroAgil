using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MuroAgil.Others {
    public static class CuerpoCorreo {
        public static string getCuerpoVerificacion(string correo, string nombre, string token, string dominio) {
            string nombreEncoded = WebUtility.HtmlEncode(nombre);
            string url = "https://" + dominio + "/Usuario/ValidarCorreo?Correo=" + Uri.EscapeDataString(correo) + "&TokenVerificador=" + Uri.EscapeDataString(token);

            string pathTemplate = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TemplatesCorreos", @"VerificarCorreo.html");
            string[] lineas = File.ReadAllLines(pathTemplate);

            StringBuilder builder = new StringBuilder();
            foreach (string linea in lineas) {
                builder.Append(linea);
            }

            string salida = builder.ToString().Replace("[NOMBRE]", nombreEncoded).Replace("[URL]", url);
            return salida;
        }

        public static string getCuerpoRecuperacion(string correo, string nombre, string token, string dominio) {
            string nombreEncoded = WebUtility.HtmlEncode(nombre);
            string url = "https://" + dominio + "/Usuario/GenerarNuevaContrasenna?Correo=" + Uri.EscapeDataString(correo) + "&TokenRecupContr=" + Uri.EscapeDataString(token);

            string pathTemplate = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"TemplatesCorreos", @"RecuperacionContrasenna.html");
            string[] lineas = File.ReadAllLines(pathTemplate);

            StringBuilder builder = new StringBuilder();
            foreach (string linea in lineas) {
                builder.Append(linea);
            }

            string salida = builder.ToString().Replace("[NOMBRE]", nombreEncoded).Replace("[URL]", url);
            return salida;
        }
    }
}
