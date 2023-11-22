using AutoMapper;
using Azure;
using MagicVilla_API.Data;
using MagicVilla_API.Logging;
using MagicVilla_API.Models;
using MagicVilla_API.Models.Dto;
using MagicVilla_API.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_API.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {


        protected APIResponse _response;

        private readonly ILogging _logger;

        private readonly IMapper _mapper;

        private readonly IVillaRepository _dbVilla;

        public VillaAPIController(ILogging logger, IVillaRepository dbVilla, IMapper mapper)
        {
            _logger = logger;
            _dbVilla = dbVilla;
            _mapper = mapper;
            this._response = new();
        }



        // IEnumerable<T> arabirimi, bir koleksiyonu temsil eden genel bir arayüzdür.Bu arabirim, koleksiyon üzerinde sıralı bir şekilde döngü işlemleri yapmak için gerekli olan yöntemleri tanımlar.GetVillas() metodunda, List<Villa> koleksiyonu IEnumerable<Villa> türüne dönüştürülerek geri döndürülüyor.

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {

            try
            {

                _logger.Log("Getting All Villas", "");


                IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }




        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {

            try
            {
                if (id == 0)
                {
                    _logger.Log("Get Error with Id : " + id, "error");
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                var villa = await _dbVilla.GetAsync(u => u.Id == id);
                if (villa == null)
                {
                    _logger.Log("Villa is not found!", "");
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }


                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }

            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }



        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            try
            {

                if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("Custom Error", "Villa Already Exists!");
                    return BadRequest(ModelState);
                }

                if (createDTO == null)
                {
                    return BadRequest(createDTO);
                }




                Villa villa = _mapper.Map<Villa>(createDTO);




                await _dbVilla.CreateAsync(villa);

                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.Created;



                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;

            // CreatedAtRoute metodu, belirtilen yönlendirmeyi kullanarak ve yeni oluşturulan kaynağın URI'sini içeren bir 201 Created yanıtı döndürür. Bu yanıt, oluşturulan kaynağın erişimini sağlamak için bir yol (URI) içerir ve aynı zamanda oluşturulan kaynağın kendisini içeren içeriği taşır. Bu sayede istemciler, oluşturulan kaynağa kolayca erişebilirler.
        }



        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0) // Eğer id=0 ise hata mesajı döner.
                {
                    return BadRequest();
                }

                var villa = await _dbVilla.GetAsync(v => v.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }

                await _dbVilla.RemoveAsync(villa);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                _dbVilla.SaveAsync();
                return Ok(_response); // Silindikten sonra bir geri dönüş almamıza gerek yok.
            }

            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }





        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id) // villaDTO verileri boş ise ve mevcut eşleşen id yok ise BadRequest döner.
                {
                    return BadRequest();
                }



                Villa model = _mapper.Map<Villa>(updateDTO);


                await _dbVilla.UpdateAsync(model);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }

            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _response;
        }






        [HttpPatch("{id:int}", Name = "UpdatedPartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatedPartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }

            var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);


            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);
            // Mapleme işlemi sayesinde ortadan kalktı.






            if (villa == null)
            {
                return BadRequest();
            }
            // REPLACE için kullandık. -->{ "op": "replace", "path": "/name", "value": "Chocolate Villa" } (https://jsonpatch.com/)

            // ModelState --> ASP.NET Core MVC framework'ünde, bir HTTP isteği sırasında gelen verilerin, modelin doğruluğunu kontrol etmek ve bu doğrulama sonuçlarını saklamak için kullanılan bir mekanizmadır. ModelState nesnesi, bir HTTP isteği sırasında model bağlaması (model binding) işlemi sırasında modeldeki her bir özellik için yapılan doğrulama sonuçlarını içerir.

            patchDTO.ApplyTo(villaDTO, ModelState);

            Villa model = _mapper.Map<Villa>(villaDTO);


            _dbVilla.UpdateAsync(model);
            await _dbVilla.SaveAsync();

            if (!ModelState.IsValid) // ModelState.IsValid ifadesi, gelen verilerin doğrulama sonuçlarını kontrol eder. Eğer ModelState geçerli değilse (yani bir veya daha fazla doğrulama hatası varsa), işlem başarısız olmuştur.
            {
                return BadRequest();
            }

            return NoContent();
        }





    }
}
