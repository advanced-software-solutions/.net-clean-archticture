using CleanBase;
using CleanBase.CleanAbstractions.CleanBusiness;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace CleanAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseController<T> : ControllerBase where T : class, IEntityRoot
    {
        private readonly IRootService<T> _service;
        public BaseController(IRootService<T> service)
        {
            _service = service;
        }
        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_service.Query());
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] T entity)
        {
            var result = await _service.InsertAsync(entity);
            return Ok(result);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> InsertList([FromBody] List<T> entity)
        {
            await _service.InsertAsync(entity);
            return Ok(entity);
        }
        [HttpPut]
        public IActionResult Put([FromBody] T entity)
        {
            var result = _service.Update(entity);
            return Ok(result.Entity);
        }
        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            _service.Delete(id);
            return Ok();
        }
    }
}
