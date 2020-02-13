using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuroAgil.Models;
using MuroAgil.Others;

namespace MuroAgil.Controllers
{
	[Authorize]
	public class UsuarioMuroController : Controller
    {
		private MuroAgilContext _dbContext;

		public UsuarioMuroController(MuroAgilContext dbContext) {
			_dbContext = dbContext;
		}

		[HttpGet]
		public IActionResult Index() {
			var muros = _dbContext.UsuarioMuro
				.Include(um => um.IdDuennoNavigation)
				.Include(um => um.IdMuroNavigation)
				.Where(um => um.IdDuenno == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value))
				.ToList();

            foreach (var usuarioMuro in muros) {
                if (usuarioMuro.Permiso != 1) {
                    usuarioMuro.IdDuennoNavigation = _dbContext.UsuarioMuro
                        .Include(um => um.IdDuennoNavigation)
                        .Include(um => um.IdMuroNavigation)
                        .Where(um => um.IdMuroNavigation.Id == usuarioMuro.IdMuro
                            && um.Permiso == 1)
                        .Last()
                        .IdDuennoNavigation;
                }
            }

			return View(muros);
		}

		[HttpGet]
		public IActionResult Permisos(int id) {
			var usuarioMuro = _dbContext.UsuarioMuro
				.SingleOrDefault(um => 
					um.IdDuenno == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value) &&
					um.IdMuro == id &&
					um.Permiso == 1);

			if (usuarioMuro == null) {
				throw new ExcepcionMuroAgil("No está autorizado para ver los permisos del muro seleccionado.");
			}

			var muro = _dbContext.Muro
				.Include(m => m.UsuarioMuro)
					.ThenInclude(um => um.IdDuennoNavigation)
				.SingleOrDefault(m => m.Id == id);
			return View(muro);
		}

		[HttpPost]
		public void Grabar(string nombre) {
			if (nombre == null || nombre.Trim().Equals("")) {
				Response.StatusCode = 500;
				return;
			}

			var muro = new Muro() {
				Nombre = nombre,
				FechaCreacion = DateTime.Now,
				FechaUltimaModificacion = DateTime.Now
			};

			_dbContext.Muro.Add(muro);
			_dbContext.SaveChanges();

			var usuarioMuro = new UsuarioMuro() {
				IdDuenno = Int32.Parse(((ClaimsIdentity) User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value),
				IdMuro = muro.Id,
				Permiso = 1
			};

			_dbContext.UsuarioMuro.Add(usuarioMuro);

			_dbContext.Etapa.Add(new Etapa() {
				Nombre = "HISTORIA",
				IdMuro = muro.Id,
				Posicion = 1
			});
			_dbContext.Etapa.Add(new Etapa() {
				Nombre = "PENDIENTE",
				IdMuro = muro.Id,
				Posicion = 2
			});
			_dbContext.Etapa.Add(new Etapa() {
				Nombre = "EN CONSTRUCCIÓN",
				IdMuro = muro.Id,
				Posicion = 3
			});
			_dbContext.Etapa.Add(new Etapa() {
				Nombre = "COMPLETADO",
				IdMuro = muro.Id,
				Posicion = 4
			});
			_dbContext.Etapa.Add(new Etapa() {
				Nombre = "PRUEBA",
				IdMuro = muro.Id,
				Posicion = 5
			});

			_dbContext.SaveChanges();
			Response.StatusCode = 200;
		}

		[HttpPost]
		public void Modificar(int id, string nombre) {
			if (nombre == null || nombre.Trim().Equals("")) {
				Response.StatusCode = 500;
				return;
			}

			var usuarioMuro = _dbContext.UsuarioMuro.SingleOrDefault(um =>
				um.IdDuenno == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value) &&
				um.IdMuro == id);

			if (usuarioMuro == null || usuarioMuro.Permiso != 1) {
				Response.StatusCode = 500;
				return;
			}

			var muro = _dbContext.Muro.SingleOrDefault(m => m.Id == id);
			if (muro == null) {
				Response.StatusCode = 500;
				return;
			}

			muro.Nombre = nombre;
			_dbContext.Muro.Update(muro);
			_dbContext.SaveChanges();
			Response.StatusCode = 200;
		}

		[HttpPost]
		public void Eliminar(int id) {
			var usuarioMuro = _dbContext.UsuarioMuro.SingleOrDefault(um =>
				um.IdDuenno == Int32.Parse(((ClaimsIdentity) User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value) &&
				um.IdMuro == id);

			if (usuarioMuro == null || usuarioMuro.Permiso != 1) {
				Response.StatusCode = 500;
				return;
			}

			var muro = _dbContext.Muro.SingleOrDefault(m => m.Id == id);

			if (muro == null) {
				Response.StatusCode = 500;
				return;
			}

			_dbContext.Muro.Remove(muro);		
			_dbContext.SaveChanges();
			Response.StatusCode = 200;
		}

		[HttpPost]
		public void OtorgarPermiso(int idMuro, string usuario, short permiso) {
			if (permiso < 2 || permiso > 3) {
				Response.StatusCode = 500;
				return;
			}

			if (usuario == null || usuario.Trim().Equals("")) {
				Response.StatusCode = 500;
				return;
			}

			var usuarioAux = _dbContext.Usuario.SingleOrDefault(u =>
				u.Correo == usuario);

			if (usuarioAux == null) {
				Response.StatusCode = 500;
				return;
			}

			if (usuarioAux.Id == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value)) {
				Response.StatusCode = 500;
				return;
			}

			var usuarioMuro = _dbContext.UsuarioMuro.SingleOrDefault(um =>
				um.IdDuenno == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value) &&
				um.IdMuro == idMuro);

			if (usuarioMuro == null || usuarioMuro.Permiso != 1) {
				Response.StatusCode = 500;
				return;
			}

			usuarioMuro = _dbContext.UsuarioMuro.SingleOrDefault(um =>
				um.IdDuenno == usuarioAux.Id &&
				um.IdMuro == idMuro);

			if (usuarioMuro != null) {
				Response.StatusCode = 500;
				return;
			}

			usuarioMuro = new UsuarioMuro {
				IdDuenno = usuarioAux.Id,
				IdMuro = idMuro,
				Permiso = permiso
			};

			_dbContext.UsuarioMuro.Add(usuarioMuro);
			_dbContext.SaveChanges();
			Response.StatusCode = 200;
		}

		[HttpPost]
		public void ModificarPermiso(int idUsuario, int idMuro, short permiso) {
			if (permiso < 2 || permiso > 3) {
				Response.StatusCode = 500;
				return;
			}

			var usuarioMuro = _dbContext.UsuarioMuro.SingleOrDefault(um =>
				um.IdDuenno == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value) &&
				um.IdMuro == idMuro);

			if (usuarioMuro == null || usuarioMuro.Permiso != 1) {
				Response.StatusCode = 500;
				return;
			}

			if (idUsuario == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value)) {
				Response.StatusCode = 500;
				return;
			}

			usuarioMuro = _dbContext.UsuarioMuro.SingleOrDefault(um =>
				um.IdDuenno == idUsuario &&
				um.IdMuro == idMuro);

			if (usuarioMuro == null) {
				Response.StatusCode = 500;
				return;
			}

			usuarioMuro.Permiso = permiso;
			_dbContext.UsuarioMuro.Update(usuarioMuro);
			_dbContext.SaveChanges();
			Response.StatusCode = 200;
		}

		[HttpPost]
		public void EliminarPermiso(int idUsuario, int idMuro) {
			if (idUsuario == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value)) {
				Response.StatusCode = 500;
				return;
			}

			var usuarioMuro = _dbContext.UsuarioMuro.SingleOrDefault(um =>
				um.IdDuenno == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value) &&
				um.IdMuro == idMuro);

			if (usuarioMuro == null || usuarioMuro.Permiso != 1) {
				Response.StatusCode = 500;
				return;
			}

			usuarioMuro = _dbContext.UsuarioMuro.SingleOrDefault(um =>
				um.IdDuenno == idUsuario &&
				um.IdMuro == idMuro);

			if (usuarioMuro == null) {
				Response.StatusCode = 500;
				return;
			}

			_dbContext.UsuarioMuro.Remove(usuarioMuro);
			_dbContext.SaveChanges();
			Response.StatusCode = 200;
		}

		[HttpPost]
		public void Renunciar(int idMuro) {
			var usuarioMuro = _dbContext.UsuarioMuro.SingleOrDefault(um =>
				um.IdDuenno == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value) &&
				um.IdMuro == idMuro);

			if (usuarioMuro == null || usuarioMuro.Permiso == 1) {
				Response.StatusCode = 500;
				return;
			}

			_dbContext.UsuarioMuro.Remove(usuarioMuro);
			_dbContext.SaveChanges();
			Response.StatusCode = 200;
		}
	}
}