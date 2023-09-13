using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SupplyChain;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolCotEmailController : ControllerBase
    {
        private readonly SolCotEmailRepository _solCotEmailRepository;

        public SolCotEmailController(SolCotEmailRepository solCotEmailRepository)
        {
            _solCotEmailRepository = solCotEmailRepository;
        }


        [HttpGet()]
        public async Task<ActionResult<IEnumerable<SolCotEmail>>> Get()
        {
            return await _solCotEmailRepository.ObtenerTodos();
        }

        [HttpPost("BySugerenciasCompras")]
        public async Task<ActionResult<IEnumerable<SolCotEmail>>> GetProve(List<Compra> sugerencias)
        {
            try
            {
                
                return await _solCotEmailRepository.GetWithNameProveBySugerenacias(sugerencias);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("EnviarMail")]
        public async Task<ActionResult<IEnumerable<SolCotEmail>>> EnviarMail(List<SolCotEmail> mails)
        {
            try
            {
                foreach (SolCotEmail mail in mails)
                {
                    mail.USUARIO = HttpContext.User.Identity.Name;
                }

                await _solCotEmailRepository.EnviarMail(mails);
                return mails;
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
