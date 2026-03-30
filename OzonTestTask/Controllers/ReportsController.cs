using Microsoft.AspNetCore.Mvc;
using OzonTestTask.DTOs;
using OzonTestTask.Services;

namespace OzonTestTask.Controllers {
    /// <summary>
    /// контроллер для работы с отчетами
    /// </summary>
    [Route("api/reports")]
    [ApiController]
    public class ReportsController: ControllerBase {
        private readonly IReportService _service;

        public ReportsController(IReportService service) {
            _service = service;
        }
        
        /// <summary>
        /// Роут на создание запроса на отчет
        /// </summary>
        /// <param name="data">Содержит идентификатор продукта, идентификатор оформления и временной промежуток для расчета</param>
        /// <returns>Если уже существует отчет на подобный запрос - Id отчета, если отчета нет, но есть подобный запрос - Id запроса и его статус
        /// Если нет ни отчета, ни запроса, то создает запрос и возвращает его Id и статус</returns>
        [HttpPost("create-request")]
        public async Task<ActionResult<ReportResponseDTO>> CreateRequest(ReportRequestDTO data) {
            return Ok(await _service.CreateReportRequest(data));
        }


        /// <summary>
        /// Проверяет статус запроса по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор запроса</param>
        /// <returns>Статус запроса, идентификатор отчета по нему и его статус</returns>
        [HttpGet("check-request/{id}")]
        public async Task<ActionResult<ReportResponseDTO>> CheckRequest(Guid id) {
            return Ok(await _service.CheckRequest(id));
        }


        /// <summary>
        /// Роут для получения отчета по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор отчета</param>
        /// <returns>Информацию по отчету</returns>
        [HttpGet("report/{id}")]
        public async Task<ActionResult<ReportDTO>> GetReport(Guid id) {
            return Ok(await _service.GetReport(id));
        }
    }
}
