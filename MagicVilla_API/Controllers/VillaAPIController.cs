using AutoMapper;
using MagicVilla_API.Data;
using MagicVilla_API.Logging;
using MagicVilla_API.Models;
using MagicVilla_API.Models.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController:ControllerBase
    {


        //private readonly ILogger<VillaAPIController> _logger;

        private readonly ILogging _logger;

        private readonly IMapper _mapper;

        private readonly ApplicationDbContext _db;

        public VillaAPIController(ILogging logger, ApplicationDbContext db, IMapper mapper)
        {
            _logger = logger;
            _db = db;
            _mapper= mapper;
        }



        // IEnumerable<T> arabirimi, bir koleksiyonu temsil eden genel bir arayüzdür.Bu arabirim, koleksiyon üzerinde sıralı bir şekilde döngü işlemleri yapmak için gerekli olan yöntemleri tanımlar.GetVillas() metodunda, List<Villa> koleksiyonu IEnumerable<Villa> türüne dönüştürülerek geri döndürülüyor.

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
         {
            _logger.Log("Getting All Villas","");


            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(villaList));
         }




        [HttpGet("{id:int}",Name ="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVillas(int id)
        {
            if(id == 0) 
            {
                _logger.Log("Get Error with Id : " + id,"error");

                return BadRequest();
            }

            var villa = await _db.Villas.FirstOrDefaultAsync(u=>u.Id==id);
            if(villa==null)
            {
                _logger.Log("Villa is not found!","");

                return NotFound();
            }

            return Ok(_mapper.Map<VillaDTO>(villa));
        }



        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody]VillaCreateDTO createDTO)
        {
            //if(!ModelState.IsValid) 
            //{
            //   return BadRequest(ModelState);
            //}

            if(await _db.Villas.FirstOrDefaultAsync(u =>u.Name.ToLower()== createDTO.Name.ToLower())!=null)
            {
                ModelState.AddModelError("Custom Error", "Villa Already Exists!");
                return BadRequest(ModelState);
            }

            if(createDTO == null) 
            {
                return BadRequest(createDTO);
            }



            //if(villaDTO.Id > 0) 
            //{
            //   return StatusCode(StatusCodes.Status500InternalServerError);
            //}

            Villa model = _mapper.Map<Villa>(createDTO);


            // Mapleme işlemi sayesinde ortadan kalktı.

            //Villa model = new()
            //{
            //    Amenity = createDTO.Amenity,
            //    Details = createDTO.Details,
            //    ImageUrl = createDTO.ImageUrl,
            //    Name = createDTO.Name,
            //    Occupancy = createDTO.Occupancy,
            //    Rate = createDTO.Rate,
            //    Sqft = createDTO.Sqft
            //};

            await _db.Villas.AddAsync(model);
            await _db.SaveChangesAsync();

            return CreatedAtRoute("GetVilla", new {id = model.Id}, createDTO);
        }

        // CreatedAtRoute metodu, belirtilen yönlendirmeyi kullanarak ve yeni oluşturulan kaynağın URI'sini içeren bir 201 Created yanıtı döndürür. Bu yanıt, oluşturulan kaynağın erişimini sağlamak için bir yol (URI) içerir ve aynı zamanda oluşturulan kaynağın kendisini içeren içeriği taşır. Bu sayede istemciler, oluşturulan kaynağa kolayca erişebilirler.




        [HttpDelete("{id:int}",Name ="DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteVilla(int id) 
        {
           if(id==0) // Eğer id=0 ise hata mesajı döner.
            {
                return BadRequest();
            }

           var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id==id);
            if(villa == null)
            {
                return NotFound();
            }

            _db.Villas.Remove(villa);
            _db.SaveChangesAsync();
            return NoContent(); // Silindikten sonra bir geri dönüş almamıza gerek yok.
        }





        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            if(updateDTO == null || id != updateDTO.Id) // villaDTO verileri boş ise ve mevcut eşleşen id yok ise BadRequest döner.
            {
                return BadRequest();
            }

            //var villa = _db.Villas.FirstOrDefault(u=>u.Id==id);
            //villa.Name = villaDTO.Name;
            //villa.Sqft = villaDTO.Sqft;
            //villa.Occupancy = villaDTO.Occupancy;



            Villa model = _mapper.Map<Villa>(updateDTO);
            // Mapleme işlemi sayesinde ortadan kalktı.

            //Villa model = new()
            //{
            //    Amenity = updateDTO.Amenity,
            //    Details = updateDTO.Details,
            //    Id = updateDTO.Id,
            //    ImageUrl = updateDTO.ImageUrl,
            //    Name = updateDTO.Name,
            //    Occupancy = updateDTO.Occupancy,
            //    Rate = updateDTO.Rate,
            //    Sqft = updateDTO.Sqft,
            //};

            _db.Villas.Update(model);
            _db.SaveChanges();
            return NoContent();
        }






        [HttpPatch("{id:int}", Name = "UpdatedPartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatedPartialVilla(int id,JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if(patchDTO==null || id==0)
            {
                return BadRequest();
            }

            var villa =await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u=>u.Id==id);


            VillaUpdateDTO villaDTO =_mapper.Map<VillaUpdateDTO>(villa);
            // Mapleme işlemi sayesinde ortadan kalktı.


            //VillaUpdateDTO villaDTO = new()
            // {
            //     Amenity = villa.Amenity,
            //     Details = villa.Details,
            //     Id = villa.Id,
            //     ImageUrl = villa.ImageUrl,
            //     Name = villa.Name,
            //     Occupancy = villa.Occupancy,
            //     Rate = villa.Rate,
            //     Sqft = villa.Sqft,
            // };



            if (villa==null)
            {
                return BadRequest();
            }
            // REPLACE için kullandık. -->{ "op": "replace", "path": "/name", "value": "Chocolate Villa" } (https://jsonpatch.com/)

            // ModelState --> ASP.NET Core MVC framework'ünde, bir HTTP isteği sırasında gelen verilerin, modelin doğruluğunu kontrol etmek ve bu doğrulama sonuçlarını saklamak için kullanılan bir mekanizmadır. ModelState nesnesi, bir HTTP isteği sırasında model bağlaması (model binding) işlemi sırasında modeldeki her bir özellik için yapılan doğrulama sonuçlarını içerir.

            patchDTO.ApplyTo(villaDTO, ModelState);

            Villa model = _mapper.Map<Villa>(villaDTO);
            // Mapleme işlemi sayesinde ortadan kalktı.

            //Villa model = new Villa()
            //{
            //    Amenity = villaDTO.Amenity,
            //    Details = villaDTO.Details,
            //    Id = villaDTO.Id,
            //    ImageUrl = villaDTO.ImageUrl,
            //    Name = villaDTO.Name,
            //    Occupancy = villaDTO.Occupancy,
            //    Rate = villaDTO.Rate,
            //    Sqft = villaDTO.Sqft,
            //};

            _db.Villas.Update(model);
           await _db.SaveChangesAsync();   

            if (!ModelState.IsValid) // ModelState.IsValid ifadesi, gelen verilerin doğrulama sonuçlarını kontrol eder. Eğer ModelState geçerli değilse (yani bir veya daha fazla doğrulama hatası varsa), işlem başarısız olmuştur.
            {
                return BadRequest();
            }

            return NoContent() ;
        }





    }
}
