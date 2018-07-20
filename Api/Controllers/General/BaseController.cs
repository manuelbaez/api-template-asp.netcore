using Api.Config;
using Api.Definitions;
using BusinessLogic.DTO.General;
using BusinessLogic.Services.General;
using BusinessLogic.Util;
using DataBaseAccess.Contexts;
using FieldsSelector;
using Filtering;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Api.Controllers.General
{
    [EnableCors("EveryOne")]
    [Route("/api/[controller]")]
    public class BaseController<TDto> : Controller where TDto : class, IDto
    {
        protected readonly IService<TDto> _service;

        public BaseController(IService<TDto> service)
        {
            _service = service;

        }



        [HttpGet]
        public virtual IActionResult GetAll(ApiParameter parameters)
        {
            try
            {

                var propertiesToExpand = typeof(TDto).GetPropertiesName(parameters.Expand?.Split(","));
                var propertiesToSelect = typeof(TDto).GetPropertiesName(parameters.Fields?.Split(","));
                var data = _service.GetAllActive(propertiesToExpand);

                var propertiesToExclude = new List<string>() { "expand", "select", "pagesize", "pagenumber", "fields" };

                var filters = HttpContext.Request.Query
                    .Where(m => !propertiesToExclude.Contains(m.Key.ToLower()))
                    .ToDictionary(k => k.Key.ToLower(), v => (string)v.Value);
                data = data.FilterData(filters);

                IQueryable<TDto> sendData;
                if (parameters.PageSize != null)
                {
                    var paginated = WebPaginator.Paginate(data, parameters.PageNumber, (int)parameters.PageSize);
                    HttpContext.Response.Headers.Add("Pagination", paginated.paginationHeader);
                    sendData = paginated.data;

                }
                else
                {
                    var count = data.Count();
                    HttpContext.Response.Headers.Add("Pagination",
                        JsonConvert.SerializeObject(new { count }));
                    sendData = data;
                }

                IQueryable<object> selected;
                if (parameters.Fields != null)
                    selected = Selector.Select(sendData, propertiesToSelect);
                else
                    selected = sendData;

                var dataSend = selected.ToList();
                return Ok(dataSend);
            }
            catch (Exception ex)
            {

                var statusCode = ex.Data["StatusCode"];
                if (statusCode != null)
                    return StatusCode((int)ex.Data["StatusCode"], new
                    {
                        message = ex.Message
                    });
                return StatusCode((int)HttpStatusCode.InternalServerError, new
                {
                    message = Constants.ControllerLoadErrorMessage
                });
            }
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(int id, string expand = null, string fields = null)
        {
            try
            {
                var propertiesName = typeof(TDto).GetPropertiesName(expand?.Split(","));
                var propertiesToSelect = typeof(TDto).GetPropertiesName(fields?.Split(","));
                var data = _service.Get(id, propertiesName);
                if (!string.IsNullOrEmpty(fields))
                {
                    var selected = data.SelectObjectFields(propertiesToSelect);
                    return Ok(selected);
                }
                else
                {
                    return Ok(data);
                }
            }
            catch (Exception ex)
            {

                var statusCode = ex.Data["StatusCode"];
                if (statusCode != null)
                    return StatusCode((int)ex.Data["StatusCode"], new { message = ex.Message });
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = Constants.ControllerLoadErrorMessage });
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]TDto dto)
        {
            try
            {
                dynamic entity = _service.Add(dto, true);
                return StatusCode((int)HttpStatusCode.OK, new { entity.Id, message = "Guardado exitosamente" });
            }
            catch (Exception ex)
            {
                var statusCode = ex.Data["StatusCode"];
                if (statusCode != null)
                    return StatusCode((int)ex.Data["StatusCode"], new { message = ex.Message });
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = Constants.ControllerSaveErrorMessage });
            }
        }

        [HttpPatch]
        [Route("{id}")]
        public virtual IActionResult Patch(int id, [FromBody]object dto)
        {
            try
            {
                var objectToApplyTo = _service.Get(id);
                var jsonPatch = dto.ToJsonPatchDocument();
                jsonPatch.ApplyTo(objectToApplyTo);
                _service.Update(id, objectToApplyTo);
                _service.SaveChanges();
                return Ok(new { message = "Actualizado Exitosamente" });
            }
            catch (Exception ex)
            {
                var statusCode = ex.Data["StatusCode"];
                if (statusCode != null)
                    return StatusCode((int)ex.Data["StatusCode"], new { message = ex.Message });
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = Constants.ControllerSaveErrorMessage });

            }
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _service.Delete(id);
                return StatusCode((int)HttpStatusCode.OK, "Eliminado Correctamente");
            }
            catch (Exception ex)
            {

                var statusCode = ex.Data["StatusCode"];
                if (statusCode != null)
                    return StatusCode((int)ex.Data["StatusCode"], new { message = ex.Message });
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = Constants.ControllerSaveErrorMessage });
            }
        }

    }
}
