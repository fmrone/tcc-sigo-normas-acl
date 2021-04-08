using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Tcc.Sigo.Normas.Acl.Attributes;
using Tcc.Sigo.Normas.Acl.Models;
using Tcc.Sigo.Normas.Acl.Repository;

namespace Tcc.Sigo.Normas.Acl.Controllers
{
    [ApiController]
    [Route("normas-acl")]
    public class NormaController : ControllerBase
    {
        private readonly ILogger<NormaController> _logger;
        private readonly INormaRepository _normaRepository;

        public NormaController(ILogger<NormaController> logger,
            INormaRepository normaRepository)
        {
            _logger = logger;
            _normaRepository = normaRepository;
        }

        [HttpGet]
        [ApiKey]
        public async Task<IActionResult> Get()
        {
            try
            {
                var resultado = await _normaRepository.ListarNormasSegurancaQA();
                if (resultado == null)
                    return NoContent();

                return Ok(resultado.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [ApiKey]
        public async Task<IActionResult> Post([FromBody][Required] NormaMessageModel normaMessageModel)
        {
            try
            {
                var resultado = await _normaRepository.PersistirNorma(normaMessageModel);
                if (resultado)
                    return Ok();

                return Accepted(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                return BadRequest(ex.Message);
            }
        }
    }
}
