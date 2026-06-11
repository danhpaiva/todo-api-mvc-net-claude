using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Tags("Health")]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly ILogger<HealthController> _logger;

        public HealthController(HealthCheckService healthCheckService, ILogger<HealthController> logger)
        {
            _healthCheckService = healthCheckService;
            _logger = logger;
        }

        /// <summary>Retorna o status de saúde da API e suas dependências.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var report = await _healthCheckService.CheckHealthAsync(cancellationToken: cancellationToken);

            if (report.Status != HealthStatus.Healthy)
                _logger.LogWarning("Health check degradado: {Status}.", report.Status);

            var result = new
            {
                status = report.Status.ToString(),
                duration = report.TotalDuration,
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    duration = e.Value.Duration
                })
            };

            return report.Status == HealthStatus.Healthy
                ? Ok(result)
                : StatusCode(StatusCodes.Status503ServiceUnavailable, result);
        }
    }
}
