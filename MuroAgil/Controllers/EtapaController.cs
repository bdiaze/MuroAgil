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
	public class EtapaController : Controller
    {
		private MuroAgilContext _dbContext;

		public EtapaController(MuroAgilContext dbContext) {
			_dbContext = dbContext;
		}
    }
}