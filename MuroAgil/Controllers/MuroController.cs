using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MuroAgil.Models;
using MuroAgil.Others;

namespace MuroAgil.Controllers {
    [Authorize]
    public class MuroController : Controller {
        private MuroAgilContext _dbContext;

        public MuroController(MuroAgilContext dbContext) {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index(int id) {
            var usuarioMuro = _dbContext.UsuarioMuro.SingleOrDefault(um =>
                    um.IdDuenno == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value) &&
                    um.IdMuro == id);

            if (usuarioMuro == null) {
                throw new ExcepcionMuroAgil("No está autorizado para ver el detalle del muro seleccionado.");
            }

            Muro muro = _dbContext.Muro.Where(m => m.Id == id).FirstOrDefault();

            var listaEtapas = _dbContext.Etapa.Where(e => e.IdMuro == muro.Id).OrderBy(e => e.Posicion).ToList();
            foreach (var etapa in listaEtapas) {
                var listaTareas = _dbContext.Tarea.Where(t => t.IdEtapa == etapa.Id).OrderBy(t => t.Posicion).ToList();
                etapa.Tarea = listaTareas;
            }
            muro.Etapa = listaEtapas;

            usuarioMuro.IdMuroNavigation = muro;

            return View("Index", usuarioMuro);
        }

        [HttpGet]
        public async Task<ActionResult> ObtenerFechaModificacion(int idMuro) {
            var usuarioMuro = await _dbContext.UsuarioMuro
                .Include(um => um.IdMuroNavigation)
                .SingleOrDefaultAsync(um =>
                    um.IdDuenno == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value) &&
                    um.IdMuro == idMuro);

            if (usuarioMuro == null) {
                return StatusCode(401);
            }

            return StatusCode(200, Json(usuarioMuro.IdMuroNavigation.FechaUltimaModificacion.ToString("yyyyMMddHHmmss")));
        }

        [HttpGet]
        public async Task<ActionResult> ObtenerInfoActualizada(int idMuro) {
            UsuarioMuro usuarioMuro = await _dbContext.UsuarioMuro
                .SingleOrDefaultAsync(um =>
                    um.IdDuenno == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value) &&
                    um.IdMuro == idMuro);

            if (usuarioMuro == null) {
                return StatusCode(401);
            }

            Muro muro = await _dbContext.Muro
                    .SingleOrDefaultAsync(m =>
                        m.Id == idMuro);

            JSONMuro jsonMuro = new JSONMuro {
                Id = muro.Id,
                Nombre = WebUtility.HtmlEncode(muro.Nombre),
                FechaCreacion = muro.FechaCreacion.ToString("yyyyMMddHHmmss"),
                FechaUltimaModificacion = muro.FechaUltimaModificacion.ToString("yyyyMMddHHmmss"),
                Permiso = usuarioMuro.Permiso,
                Etapas = new List<JSONEtapa>()
            };

            List<Etapa> listaEtapas = await _dbContext.Etapa
                .Where(e => e.IdMuro == muro.Id)
                .OrderBy(e => e.Posicion)
                .ToListAsync();

            foreach (Etapa etapa in listaEtapas) {
                jsonMuro.Etapas.Add(new JSONEtapa {
                    Id = etapa.Id,
                    Nombre = WebUtility.HtmlEncode(etapa.Nombre),
                    Posicion = etapa.Posicion,
                    Tareas = new List<JSONTarea>()
                });

                List<Tarea> listaTareas = await _dbContext.Tarea
                    .Where(t => t.IdEtapa == etapa.Id)
                    .OrderBy(t => t.Posicion)
                    .ToListAsync();

                foreach (Tarea tarea in listaTareas) {
                    jsonMuro.Etapas.LastOrDefault().Tareas.Add(new JSONTarea {
                        Id = tarea.Id,
                        Titulo = WebUtility.HtmlEncode(tarea.Titulo),
                        Descripcion = WebUtility.HtmlEncode(tarea.Descripcion),
                        Posicion = tarea.Posicion,
                        Familia = tarea.Familia,
                        Red = tarea.Red,
                        Green = tarea.Green,
                        Blue = tarea.Blue
                    });
                }
            }

            return StatusCode(200, Json(jsonMuro));
        }

        private class JSONMuro {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string FechaCreacion { get; set; }
            public string FechaUltimaModificacion { get; set; }
            public short Permiso { get; set; }

            public ICollection<JSONEtapa> Etapas { get; set; }

        }

        private class JSONEtapa {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public short Posicion { get; set; }

            public ICollection<JSONTarea> Tareas { get; set; }
        }

        private class JSONTarea {
            public int Id { get; set; }
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public short Posicion { get; set; }
            public short Familia { get; set; }

            public short Red { get; set; }
            public short Green { get; set; }
            public short Blue { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> EditarMuro(int id = 0) {
            EditarMuroViewModel model = new EditarMuroViewModel();

            if (id != 0) {
                var usuarioMuro = await _dbContext.UsuarioMuro.SingleOrDefaultAsync(um =>
                    um.IdDuenno == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value) &&
                    um.IdMuro == id &&
                    um.Permiso == 1);

                if (usuarioMuro == null) {
                    throw new ExcepcionMuroAgil("No está autorizado para editar el muro seleccionado.");
                }

                Muro muro = await _dbContext.Muro
                    .SingleOrDefaultAsync(m => m.Id == id);

                if (muro == null) {
                    throw new ExcepcionMuroAgil("No existe el muro seleccionado.");
                }

                model.Id = muro.Id;
                model.Nombre = muro.Nombre;

                ICollection<Etapa> etapas = null;
                etapas = await _dbContext.Etapa
                    .Where(e => e.IdMuro == muro.Id)
                    .OrderBy(e => e.Posicion)
                    .ToListAsync();

                model.Etapas = new List<EditarMuroViewModel.Etapa>();
                if (etapas != null) {
                    foreach (var etapa in etapas) {
                        EditarMuroViewModel.Etapa viewModelEtapa = new EditarMuroViewModel.Etapa();

                        viewModelEtapa.Id = etapa.Id;
                        viewModelEtapa.Nombre = etapa.Nombre;
                        viewModelEtapa.Posicion = etapa.Posicion;

                        model.Etapas.Add(viewModelEtapa);
                    }
                }
            } else {
                model.Id = 0;
                model.Nombre = "";

                UsuarioMuro usuarioMuro = await _dbContext.UsuarioMuro
                    .Include(um => um.IdMuroNavigation)
                    .Where(um => um.IdDuenno == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value) && um.Permiso == 1)
                    .OrderByDescending(um => um.IdMuroNavigation.FechaCreacion)
                    .FirstOrDefaultAsync();

                ICollection<Etapa> etapas = null;
                if (usuarioMuro != null) {
                    etapas = await _dbContext.Etapa
                        .Where(e => e.IdMuro == usuarioMuro.IdMuro)
                        .OrderBy(e => e.Posicion)
                        .ToListAsync();
                } else {
                    etapas = new List<Etapa>();
                    etapas.Add(new Etapa() {
                        Nombre = "Pendiente",
                        Posicion = 1
                    });
                    etapas.Add(new Etapa() {
                        Nombre = "En Proceso",
                        Posicion = 2
                    });
                    etapas.Add(new Etapa() {
                        Nombre = "Terminado",
                        Posicion = 3
                    });
                }

                model.Etapas = new List<EditarMuroViewModel.Etapa>();
                if (etapas != null) {
                    foreach (var etapa in etapas) {
                        EditarMuroViewModel.Etapa viewModelEtapa = new EditarMuroViewModel.Etapa();

                        viewModelEtapa.Id = 0;
                        viewModelEtapa.Nombre = etapa.Nombre;
                        viewModelEtapa.Posicion = etapa.Posicion;

                        model.Etapas.Add(viewModelEtapa);
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditarMuro(EditarMuroViewModel model) {
            if (!ModelState.IsValid) {
                throw new ExcepcionMuroAgil(ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .First());
            }

            // Se crea el muro, o se modifica si ya existe.
            Muro muro = null;
            if (model.Id == 0) {
                muro = new Muro() {
                    Nombre = model.Nombre,
                    FechaCreacion = DateTime.Now,
                    FechaUltimaModificacion = DateTime.Now
                };

                await _dbContext.Muro.AddAsync(muro);
                await _dbContext.SaveChangesAsync();

                UsuarioMuro usuarioMuro = new UsuarioMuro() {
                    IdDuenno = int.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value),
                    IdMuro = muro.Id,
                    Permiso = 1
                };

                await _dbContext.UsuarioMuro.AddAsync(usuarioMuro);
            } else {
                UsuarioMuro usuarioMuro = await _dbContext.UsuarioMuro.SingleOrDefaultAsync(um =>
                    um.IdDuenno == int.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value) &&
                    um.IdMuro == model.Id &&
                    um.Permiso == 1);

                if (usuarioMuro == null) {
                    throw new ExcepcionMuroAgil("No está autorizado para modificar el muro seleccionado.");
                }

                muro = await _dbContext.Muro.SingleOrDefaultAsync(m => m.Id == model.Id);
                if (muro == null) {
                    throw new ExcepcionMuroAgil("No se encontró el muro que está intentando modificar.");
                }

                muro.Nombre = model.Nombre;
                muro.FechaUltimaModificacion = DateTime.Now;

                await _dbContext.SaveChangesAsync();
            }

            // Se modifican las etapas existentes, y se marcan las etapas a eliminar
            List<Etapa> etapasEliminadas = new List<Etapa>();
            List<Etapa> etapas = await _dbContext.Etapa
                .Where(e => e.IdMuro == muro.Id)
                .OrderBy(e => e.Posicion)
                .ToListAsync();

            foreach(Etapa etapa in etapas) {
                bool clienteLoElimino = true;
                foreach(EditarMuroViewModel.Etapa etapaCliente in model.Etapas) {
                    if (etapa.Id == etapaCliente.Id) {
                        etapa.Nombre = etapaCliente.Nombre;
                        etapa.Posicion = etapaCliente.Posicion;
                        clienteLoElimino = false;
                        break;
                    }
                }

                if (clienteLoElimino) {
                    etapasEliminadas.Add(etapa);
                } else {
                    //_dbContext.Etapa.Update(etapa);
                }
            }

            // Se agregan las etapas nuevas al muro
            foreach(EditarMuroViewModel.Etapa etapaCliente in model.Etapas) {
                if (etapaCliente.Id == 0) {
                    Etapa etapa = new Etapa() {
                        IdMuro = muro.Id,
                        Nombre = etapaCliente.Nombre,
                        Posicion = etapaCliente.Posicion
                    };

                    await _dbContext.Etapa.AddAsync(etapa);
                }
            }
            await _dbContext.SaveChangesAsync();

            if (model.AccionTareas != 3) {
                // Se determina la etapa de destino para las tareas que no se quieren eliminar
                Etapa etapaDestino = null;

                if (model.AccionTareas == 1) {
                    etapas = await _dbContext.Etapa
                        .Where(e => e.IdMuro == muro.Id)
                        .OrderBy(e => e.Posicion)
                        .ToListAsync();
                } else {
                    etapas = await _dbContext.Etapa
                        .Where(e => e.IdMuro == muro.Id)
                        .OrderByDescending(e => e.Posicion)
                        .ToListAsync();
                }

                foreach (Etapa etapa in etapas) {
                    bool seraEliminada = false;
                    foreach (Etapa etapaEliminada in etapasEliminadas) {
                        if (etapa.Id == etapaEliminada.Id) {
                            seraEliminada = true;
                            break;
                        }
                    }

                    if (!seraEliminada) {
                        etapaDestino = etapa;
                        break;
                    }
                }

                if (etapaDestino != null) {
                    Tarea ultTarea = await _dbContext.Tarea
                        .Where(t => t.IdEtapa == etapaDestino.Id)
                        .OrderByDescending(t => t.Posicion)
                        .FirstOrDefaultAsync();

                    List<Tarea> tareasMover = new List<Tarea>();
                    foreach (Etapa etapaEliminada in etapasEliminadas) {
                        tareasMover.AddRange(await _dbContext.Tarea
                            .Where(t => t.IdEtapa == etapaEliminada.Id)
                            .OrderBy(t => t.Posicion)
                            .ToListAsync());
                    }

                    short pos = 0;
                    if (ultTarea != null) {
                        pos = ultTarea.Posicion;
                    } 

                    foreach (Tarea tareaMover in tareasMover) {
                        pos++;
                        tareaMover.IdEtapa = etapaDestino.Id;
                        tareaMover.Posicion = pos;
                        //_dbContext.Tarea.Update(tareaMover);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }

            _dbContext.Etapa.RemoveRange(etapasEliminadas);
            await _dbContext.SaveChangesAsync();

            return Redirect("/UsuarioMuro/");
        }

        [HttpPost]
        public async Task<IActionResult> Grabar(ParametroGrabar parametro) {
            ListaRelacionesIds objRelaciones = new ListaRelacionesIds();

            bool error = false;
			var usuarioMuro = _dbContext.UsuarioMuro
				.SingleOrDefault(um =>
					um.IdDuenno == Int32.Parse(((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier).Value) &&
					um.IdMuro == parametro.IdMuro);

			if (usuarioMuro == null || usuarioMuro.Permiso > 2 || usuarioMuro.Permiso < 1) {
				error = true;
			} else {
                // Se eliminan las tareas que no son recibidas desde el cliente                
                var listaTareas = await _dbContext.Tarea
                    .Include(t => t.Etapa)
                    .Where(t => t.Etapa.IdMuro == parametro.IdMuro)
                    .ToListAsync();

                foreach (Tarea tareaExistente in listaTareas) {
                    bool existeTarea = false;

                    if (parametro.ListaTareas != null) {
                        foreach (ParametroTarea tarea in parametro.ListaTareas) {
                            if (tareaExistente.Id == tarea.IdTarea) {
                                existeTarea = true;
                                break;
                            }
                        }
                    }

                    if (!existeTarea) {
                        _dbContext.Tarea.Remove(tareaExistente);
                    }
                }

                if (parametro.ListaTareas != null) {
                    foreach (ParametroTarea tarea in parametro.ListaTareas) {
                        if (tarea.TituloTarea == null) {
                            tarea.TituloTarea = "";
                        }
                        if (tarea.DescripcionTarea == null) {
                            tarea.DescripcionTarea = "";
                        }

                        tarea.DescripcionTarea = tarea.DescripcionTarea;

                        var tareaMod = await _dbContext.Tarea
                            .Include(t => t.Etapa)
                            .SingleOrDefaultAsync(t =>
                                t.Id == tarea.IdTarea &&
                                t.Etapa.IdMuro == parametro.IdMuro);

                        if (tareaMod == null) {
                            // Se agregan las tareas que no existen en la base de datos
                            var etapaDestino = _dbContext.Etapa.SingleOrDefault(e => e.Id == tarea.IdEtapa);
                            if (etapaDestino == null || etapaDestino.IdMuro != parametro.IdMuro) {
                                error = true;
                            } else {
                                var nuevaTarea = new Tarea {
                                    Titulo = tarea.TituloTarea,
                                    Descripcion = tarea.DescripcionTarea,
                                    IdEtapa = tarea.IdEtapa,
                                    Posicion = tarea.Posicion,
                                    Familia = tarea.Familia,
                                    Red = tarea.Red,
                                    Green = tarea.Green,
                                    Blue = tarea.Blue
                                };
                                _dbContext.Tarea.Add(nuevaTarea);
                                objRelaciones.Add(new RelacionIds(tarea.IdTarea, nuevaTarea));
                            }
                        } else {
                            // Se modifican las tareas que ya existen en la base de datos
                            var etapaOrigen = _dbContext.Etapa.SingleOrDefault(e => e.Id == tareaMod.IdEtapa);
                            var etapaDestino = _dbContext.Etapa.SingleOrDefault(e => e.Id == tarea.IdEtapa);
                            if (etapaOrigen == null || etapaDestino == null || etapaOrigen.IdMuro != parametro.IdMuro || etapaDestino.IdMuro != parametro.IdMuro) {
                                error = true;
                            } else {
                                tareaMod.Titulo = tarea.TituloTarea;
                                tareaMod.Descripcion = tarea.DescripcionTarea;
                                tareaMod.IdEtapa = etapaDestino.Id;
                                tareaMod.Posicion = tarea.Posicion;
                                tareaMod.Familia = tarea.Familia;
                                tareaMod.Red = tarea.Red;
                                tareaMod.Green = tarea.Green;
                                tareaMod.Blue = tarea.Blue;

                                _dbContext.Tarea.Update(tareaMod);
                            }
                        }
                    }
                }
			}

			if (!error) {
				var muro = _dbContext.Muro.SingleOrDefault(m => m.Id == parametro.IdMuro);
				if (muro != null) {
					muro.FechaUltimaModificacion = DateTime.Now;
				}
				await _dbContext.SaveChangesAsync();
				Response.StatusCode = 200;
			} else {
				Response.StatusCode = 500;
			}

            return View(objRelaciones);
        }

		public class ParametroGrabar {
			public int IdMuro { get; set; }
			public ICollection<ParametroTarea> ListaTareas { get; set; }
		}

        public class ParametroTarea {
			public int IdEtapa { get; set; }
			public int IdTarea { get; set; }
            public string TituloTarea { get; set; }
			public string DescripcionTarea { get; set; }
			public short Posicion { get; set; }
            public short Familia { get; set; }
            public short Red { get; set; }
            public short Green { get; set; }
            public short Blue { get; set; }
        }
	}
}