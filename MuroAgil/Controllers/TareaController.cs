using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MuroAgil.Models;

namespace MuroAgil.Controllers
{
	[Authorize]
	public class TareaController : Controller
    {
		private MuroAgilContext _dbContext;

		public TareaController(MuroAgilContext dbContext) {
			_dbContext = dbContext;
		}
    }
}