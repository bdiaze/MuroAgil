using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MuroAgil.Models;
using MuroAgil.Others;
using MuroAgil.ViewModels;
using MuroAgil.ViewModels.Usuario;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MuroAgil.Controllers {
    [Authorize]
    public class UsuarioController : Controller {
		private readonly MuroAgilContext _dbContext;
        private readonly IConfiguration _configuration;

        public UsuarioController(MuroAgilContext dbContext, IConfiguration configuration) {
            _dbContext = dbContext;
            _configuration = configuration;
        }
		
		[AllowAnonymous, HttpGet]
		public async Task<IActionResult> Registrarse() {
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.GoogleReCaptchaClientKey = _configuration.GetValue<string>("GoogleReCaptcha:ClientKey");
            return View("Registrarse");
		}

		[AllowAnonymous, HttpPost]
		public async Task<IActionResult> Registrarse(RegistrarseViewModel model) {
			if (!ModelState.IsValid) {
                throw new ExcepcionMuroAgil(ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .First());
            }

            //Se elimina a los usuarios cuyas direcciones de correo no han sido confirmados.
            List<Usuario> usuariosSinConfirmar = _dbContext.Usuario
                .Where(u => 
                    u.TokenVerificador != null &&
                    u.TokenVerificador.Length > 0 &&
                    u.FechaCreacion.AddDays(1).CompareTo(DateTime.Now) <= 0)
                .ToList();
            
            _dbContext.Usuario.RemoveRange(usuariosSinConfirmar);
            await _dbContext.SaveChangesAsync();

			model.Codigo = model.Codigo.Trim();
			model.Nombre = model.Nombre.Trim();

			var targetUser = _dbContext.Usuario
				.SingleOrDefault(u => u.Correo.Equals(model.Codigo));

			if (targetUser != null) {
				throw new ExcepcionMuroAgil("Ya existe un usuario con dicho nombre de usuario.");
			}

            if (model.Contrasenna.Length < 8) {
                throw new ExcepcionMuroAgil("La contraseña seleccionada debe tener 8 o más caracteres.");
            }

            if (!model.Contrasenna.Equals(model.RepContrasenna)) {
				throw new ExcepcionMuroAgil("Las contraseñas ingresadas no coinciden.");
			}

            byte[] randomBytes = new byte[72];
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(randomBytes);
            }

            targetUser = new Usuario {
                Correo = model.Codigo,
                Nombre = model.Nombre,
                FechaCreacion = DateTime.Now,
                TokenVerificador = Convert.ToBase64String(randomBytes)
            };

            var hasher = new PasswordHasher<Usuario>();
            targetUser.HashContrasenna = hasher.HashPassword(targetUser, model.Contrasenna);
			
			await _dbContext.Usuario.AddAsync(targetUser);
			await _dbContext.SaveChangesAsync();

            EnviarCorreoVerificador(targetUser);

			return View("RegistroCompleto");
		}

        [AllowAnonymous, HttpGet]
        public async Task<IActionResult> RecuperarContrasenna() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View("RecuperarContrasenna");
        }

        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> RecuperarContrasenna(RecuperarContrasennaViewModel model) {
            if (!ModelState.IsValid) {
                throw new ExcepcionMuroAgil(ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .First());
            }

            //Se busca si está registrado el usuario.
            Usuario targetUser = _dbContext.Usuario
                .SingleOrDefault(u => u.Correo.Equals(model.Correo));

            if (targetUser == null) {
                throw new ExcepcionMuroAgil("La dirección de correo electrónico no se encuentra registrada en nuestro sistema.");
            }

            byte[] randomBytes = new byte[72];
            using (var rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(randomBytes);
            }

            targetUser.FechaRecupContr = DateTime.Now;
            targetUser.TokenRecupContr = Convert.ToBase64String(randomBytes);
            await _dbContext.SaveChangesAsync();

            EnviarCorreoRecuperacion(targetUser);

            return View("RecuperacionContrasennaSolicitada");
        }

        [AllowAnonymous, HttpGet]
        public async Task<IActionResult> ValidarCorreo(string Correo, string TokenVerificador) {
            Usuario usuario = _dbContext.Usuario
                .SingleOrDefault(u => 
                    u.Correo.Equals(Correo) && 
                    u.TokenVerificador.Equals(TokenVerificador));

            if (usuario == null) {
                throw new ExcepcionMuroAgil("El correo ingresado no requiere de la verificación. Su inicio de sesión ya quedó habilitado.");
            }

            usuario.TokenVerificador = null;
            await _dbContext.SaveChangesAsync();

            return View("CorreoValidadoExitosamente");
        }

        [AllowAnonymous, HttpGet]
        public async Task<IActionResult> GenerarNuevaContrasenna(string Correo, string TokenRecupContr) {
            //Se eliminan los token de recuperación para los usuarios fuera del tiempo límite.
            List<Usuario> usuariosSinRecuperar = _dbContext.Usuario
                .Where(u =>
                    u.TokenRecupContr != null &&
                    u.TokenRecupContr.Length > 0 &&
                    u.FechaRecupContr != null &&
                    u.FechaRecupContr.GetValueOrDefault().AddMinutes(10).CompareTo(DateTime.Now) <= 0)
                .ToList();

            foreach(Usuario usuarioSinRecuperar in usuariosSinRecuperar) {
                usuarioSinRecuperar.FechaRecupContr = null;
                usuarioSinRecuperar.TokenRecupContr = null;
            }

            await _dbContext.SaveChangesAsync();
            
            Usuario usuario = _dbContext.Usuario
                .SingleOrDefault(u => 
                    u.Correo.Equals(Correo) &&
                    u.TokenRecupContr.Equals(TokenRecupContr));

            if (usuario == null) {
                throw new ExcepcionMuroAgil("El correo ingresado no ha solicitado una recuperación de contraseña, o no terminó el proceso en el plazo de 10 minutos.");
            }

            GenerarNuevaContrasenaViewModel model = new GenerarNuevaContrasenaViewModel() {
                Correo = Correo,
                TokenRecupContr = TokenRecupContr
            };

            return View("GenerarNuevaContrasenna", model);
        }

        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> GenerarNuevaContrasenna(GenerarNuevaContrasenaViewModel model) {
            if (!ModelState.IsValid) {
                throw new ExcepcionMuroAgil(ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .First());
            }

            if (model.Contrasenna.Length < 8) {
                throw new ExcepcionMuroAgil("La contraseña seleccionada debe tener 8 o más caracteres.");
            }

            if (!model.Contrasenna.Equals(model.RepContrasenna)) {
                throw new ExcepcionMuroAgil("Las contraseñas ingresadas no coinciden.");
            }

            Usuario usuario = _dbContext.Usuario
                .SingleOrDefault(u =>
                    u.Correo.Equals(model.Correo) &&
                    u.TokenRecupContr.Equals(model.TokenRecupContr));

            if (usuario == null) {
                throw new ExcepcionMuroAgil("El correo ingresado no ha solicitado una recuperación de contraseña, o no terminó el proceso en el plazo de 10 minutos.");
            }

            usuario.FechaRecupContr = null;
            usuario.TokenRecupContr = null;
            
            var hasher = new PasswordHasher<Usuario>();
            usuario.HashContrasenna = hasher.HashPassword(usuario, model.Contrasenna);

            await _dbContext.SaveChangesAsync();
            return View("NuevaContrasennaGenerada");
        }

        [AllowAnonymous, HttpGet]
		public async Task<IActionResult> IniciarSesion() {
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ViewBag.GoogleReCaptchaClientKey = _configuration.GetValue<string>("GoogleReCaptcha:ClientKey");
            return View("IniciarSesion");
		}

		[AllowAnonymous, HttpPost]
		public async Task<IActionResult> IniciarSesion(IniciarSesionViewModel model) {
			if (!ModelState.IsValid) {
                throw new ExcepcionMuroAgil(ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .First());
            }

			var targetUser = _dbContext.Usuario.SingleOrDefault(u => u.Correo.Equals(model.Codigo));
			if (targetUser == null) {
				throw new ExcepcionMuroAgil("El correo electrónico y la contraseña ingresada no son correctos.");
			}

			var hasher = new PasswordHasher<Usuario>();
			var result = hasher.VerifyHashedPassword(targetUser, targetUser.HashContrasenna, model.Contrasenna);
			if (result != PasswordVerificationResult.Success) {
				throw new ExcepcionMuroAgil("El correo electrónico y la contraseña ingresada no son correctos.");
			}

            if (targetUser.TokenVerificador != null && targetUser.TokenVerificador.Length > 0) {
                return View("CorreoSinValidar", new CorreoSinValidarViewModel() { Correo = targetUser.Correo });
                //EnviarCorreoVerificador(targetUser);
                //throw new ExcepcionMuroAgil("Para iniciar sesión es necesario validar su correo electrónico, mediante el correo que le acabamos de mandar.");
            }

			await LogInUserAsync(targetUser);
			return RedirectToAction("Index", "UsuarioMuro");
		}

        [AllowAnonymous, HttpPost]
        public IActionResult EnviarCorreoVerificacion(CorreoSinValidarViewModel model) {
            if (!ModelState.IsValid) {
                throw new ExcepcionMuroAgil(ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .First());
            }

            var targetUser = _dbContext.Usuario.SingleOrDefault(u => u.Correo.Equals(model.Correo));
            if (targetUser == null || targetUser.TokenVerificador == null || targetUser.TokenVerificador.Length == 0) {
                throw new ExcepcionMuroAgil("Tu cuenta no requiere de la verificación del correo electrónico.");
            }

            EnviarCorreoVerificador(targetUser);

            return View("CorreoVerificacionEnviado");
        }

        private async Task LogInUserAsync(Usuario user) {
			var claims = new List<Claim>();
			claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
			claims.Add(new Claim(ClaimTypes.Name, user.Nombre));

			var claimsIndentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var claimsPrincipal = new ClaimsPrincipal(claimsIndentity);
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
		}

		[HttpGet]
		public async Task<IActionResult> CerrarSesion() {
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("IniciarSesion", "Usuario");
		}

		[HttpGet]
		public IActionResult EditarCuenta() {
			var usuario = _dbContext.Usuario.SingleOrDefault(u =>
				u.Id == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value));
			return View("EditarCuenta", usuario);
		}

        [HttpPost]
        public async void GrabarEdicionCuenta(string nombre, string contActual, string contNueva, string contNuevaConf) {
            Usuario usuario = _dbContext.Usuario.SingleOrDefault(u =>
                u.Id == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value));

            if (contActual != null && contNueva != null && contNuevaConf != null && contActual.Length != 0 && contNueva.Length != 0 && contNuevaConf.Length != 0) {
                if (!contNueva.Equals(contNuevaConf)) {
                    Response.StatusCode = 410;
                    return;
                } else {
                    if (contNueva.Length < 8) {
                        Response.StatusCode = 411;
                        return;
                    }
                }

                var hasher = new PasswordHasher<Usuario>();
                var result = hasher.VerifyHashedPassword(usuario, usuario.HashContrasenna, contActual);
                if (result != PasswordVerificationResult.Success) {
                    Response.StatusCode = 412;
                    return;
                }

                usuario.HashContrasenna = hasher.HashPassword(usuario, contNueva);
            } else {
                if ((contActual != null && contActual.Length != 0) || (contNueva != null && contNueva.Length != 0) || (contNuevaConf != null && contNuevaConf.Length != 0)) {
                    Response.StatusCode = 413;
                    return;
                }
            }

            usuario.Nombre = nombre.Trim();
            _dbContext.SaveChanges();
            
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await LogInUserAsync(usuario);
            
        }

        private static string Base64UrlEncode(byte[] inputBytes) {
            return Convert.ToBase64String(inputBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
        
        private async void EnviarCorreoVerificador(Usuario usuario) {
            string nombreAplicacion = _configuration.GetValue<string>("Correo:NombreAplicacion");
            string muroAgilEmail = _configuration.GetValue<string>("Correo:Direccion");
            string muroAgilNombre = _configuration.GetValue<string>("Correo:Nombre");
            string servAccountEmail = _configuration.GetValue<string>("Correo:ServiceAccount:client_email");
            string servAccountPrivKey = _configuration.GetValue<string>("Correo:ServiceAccount:private_key");
            string hostName = Request.Host.ToString();

            ServiceAccountCredential credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(servAccountEmail) {
                    User = muroAgilEmail,
                    Scopes = new[] { GmailService.Scope.GmailSend }
                }.FromPrivateKey(servAccountPrivKey)
            );

            bool gotAccessToken = await credential.RequestAccessTokenAsync(CancellationToken.None);
            if (gotAccessToken) {
                GmailService service = new GmailService(
                    new BaseClientService.Initializer() {
                        ApplicationName = nombreAplicacion,
                        HttpClientInitializer = credential
                    }
                );

                MailAddress fromAddress = new MailAddress(muroAgilEmail, muroAgilNombre, System.Text.Encoding.UTF8);
                MailAddress toAddress = new MailAddress(usuario.Correo, usuario.Nombre, System.Text.Encoding.UTF8);
                MailMessage message = new MailMessage(fromAddress, toAddress) {
                    Subject = "Verificación de Correo Electrónico - Muro Ágil",
                    Body = CuerpoCorreo.getCuerpoVerificacion(usuario.Correo, usuario.Nombre, usuario.TokenVerificador, hostName),
                    SubjectEncoding = Encoding.UTF8,
                    HeadersEncoding = Encoding.UTF8,
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = true
                };

                MimeMessage mimeMessage = MimeMessage.CreateFromMailMessage(message);
                MemoryStream stream = new MemoryStream();
                mimeMessage.WriteTo(stream);

                string rawMessage = Base64UrlEncode(stream.ToArray());
                service.Users.Messages.Send(new Message { 
                    Raw = rawMessage
                }, muroAgilEmail).Execute();
            }
        }

        private async void EnviarCorreoRecuperacion(Usuario usuario) {
            string nombreAplicacion = _configuration.GetValue<string>("Correo:NombreAplicacion");
            string muroAgilEmail = _configuration.GetValue<string>("Correo:Direccion");
            string muroAgilNombre = _configuration.GetValue<string>("Correo:Nombre");
            string servAccountEmail = _configuration.GetValue<string>("Correo:ServiceAccount:client_email");
            string servAccountPrivKey = _configuration.GetValue<string>("Correo:ServiceAccount:private_key");
            string hostName = Request.Host.ToString();

            ServiceAccountCredential credential = new ServiceAccountCredential(
                new ServiceAccountCredential.Initializer(servAccountEmail) {
                    User = muroAgilEmail,
                    Scopes = new[] { GmailService.Scope.GmailSend }
                }.FromPrivateKey(servAccountPrivKey)
            );

            bool gotAccessToken = await credential.RequestAccessTokenAsync(CancellationToken.None);
            if (gotAccessToken) {
                GmailService service = new GmailService(
                    new BaseClientService.Initializer() {
                        ApplicationName = nombreAplicacion,
                        HttpClientInitializer = credential
                    }
                );

                MailAddress fromAddress = new MailAddress(muroAgilEmail, muroAgilNombre, System.Text.Encoding.UTF8);
                MailAddress toAddress = new MailAddress(usuario.Correo, usuario.Nombre, System.Text.Encoding.UTF8);
                MailMessage message = new MailMessage(fromAddress, toAddress) {
                    Subject = "Recuperación de Contraseña - Muro Ágil",
                    Body = CuerpoCorreo.getCuerpoRecuperacion(usuario.Correo, usuario.Nombre, usuario.TokenRecupContr, hostName),
                    SubjectEncoding = Encoding.UTF8,
                    HeadersEncoding = Encoding.UTF8,
                    BodyEncoding = Encoding.UTF8,
                    IsBodyHtml = true
                };

                MimeMessage mimeMessage = MimeMessage.CreateFromMailMessage(message);
                MemoryStream stream = new MemoryStream();
                mimeMessage.WriteTo(stream);

                string rawMessage = Base64UrlEncode(stream.ToArray());
                service.Users.Messages.Send(new Message {
                    Raw = rawMessage
                }, muroAgilEmail).Execute();
            }
        }
    }
}