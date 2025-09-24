using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PhotoService.Application.DTOs;
using PhotoService.Application.Interfaces;
using PhotoService.Domain.Entities;

namespace PhotoService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController(IPhotoService photoService, IRabbitMqService rabbitMqService, IMapper mapper) : ControllerBase
    {
        #region Photos API Endpoints

        // GET: api/photos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhotoDto>>> GetAllPhotos()
        {
            var photos = await photoService.GetAllPhotosAsync();
            var photosDto = mapper.Map<IEnumerable<PhotoDto>>(photos);
            return Ok(photosDto);
        }

        // GET: api/photos/{photoGuid}
        [HttpGet("{photoGuid:guid}")]
        public async Task<ActionResult<PhotoDto>> GetPhotoByGuid(Guid photoGuid)
        {
            var photo = await photoService.GetPhotoByGuidAsync(photoGuid);
            if (photo == null)
                return NotFound();
            var photoDto = mapper.Map<PhotoDto>(photo);
            return Ok(photoDto);
        }

        // POST: api/photos
        [HttpPost]
        public async Task<ActionResult<PhotoDto>> CreatePhoto([FromBody] PhotoCreateDto photoDto)
        {
            var createdPhoto = await photoService.AddPhotoAsync(photoDto);
            var createdPhotoDto = mapper.Map<PhotoDto>(createdPhoto);
            return CreatedAtAction(nameof(GetPhotoByGuid), new { photoGuid = createdPhotoDto.PhotoGuid }, createdPhotoDto);
        }

        // PUT: api/photos/{photoGuid}
        [HttpPut("{photoGuid:guid}")]
        public async Task<IActionResult> UpdatePhoto(Guid photoGuid, [FromBody] PhotoUpdateDto photoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (photoGuid != photoDto.PhotoGuid)
                return BadRequest("Photo GUID mismatch.");

            var result = await photoService.UpdatePhotoAsync(photoDto);
            if (!result)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/photos/{photoGuid}
        [HttpDelete("{photoGuid:guid}")]
        public async Task<IActionResult> DeletePhoto(Guid photoGuid)
        {
            var result = await photoService.DeletePhotoAsync(photoGuid);
            if (!result)
                return NotFound();

            return NoContent();
        }

        #endregion




        #region PhotoLike API Endpoints

        // POST: api/photos/{photoGuid}/like/{userGuid}
        [HttpPost("{photoGuid:guid}/like/{userGuid:guid}")]
        public async Task<ActionResult<PhotoLikeDto>> LikePhoto(Guid photoGuid, Guid userGuid)
        {
            var photoLike = await photoService.LikePhotoAsync(photoGuid, userGuid);
            if (photoLike == null)
                return NotFound();

            var photoLikeDto = mapper.Map<PhotoLikeDto>(photoLike);

            return Ok(photoLikeDto);
        }

        // DELETE: api/photos/{photoGuid}/like/{userGuid}
        [HttpDelete("{photoGuid:guid}/like/{userGuid:guid}")]
        public async Task<IActionResult> UnlikePhoto(Guid photoGuid, Guid userGuid)
        {
            var result = await photoService.UnlikePhotoAsync(photoGuid, userGuid);
            if (!result)
                return NotFound();
            return NoContent();
        }

        #endregion





        #region PhotoCategory API Endpoints

        // GET: api/photos/{photoGuid}/categories
        [HttpGet("{photoGuid:guid}/categories")]
        public async Task<ActionResult<IEnumerable<PhotoCategoryDto>>> GetCategoriesByPhotoGuid(Guid photoGuid)
        {
            var photoCategories = await photoService.GetPhotoCategoryByPhotoGuidAsync(photoGuid);
            if (photoCategories == null || !photoCategories.Any())
                return NotFound();

            var photoCategoriesDto = mapper.Map<IEnumerable<PhotoCategoryDto>>(photoCategories);
            return Ok(photoCategoriesDto);
        }

        // POST: api/photos/{photoGuid}/categories
        [HttpPost("{photoGuid:guid}/categories")]
        public async Task<IActionResult> AddCategoryToPhoto(Guid photoGuid, [FromBody] PhotoCategoryDto photoCategoryDto)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if (photoGuid != photoCategoryDto.PhotoGuid)
                return BadRequest("Photo ID mismatch.");

            // Validate Category Guid is valid or not via RabbitMQ to CategoryService before tagging it to Photo.
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            bool isValid = await rabbitMqService.ValidateCategoryAsync(photoCategoryDto.CategoryGuid, cts.Token);

            if (!isValid)
                return BadRequest("Invalid Category!");

            // If pass, proceed to next step.
            var result = await photoService.CreatePhotoCategoryAsync(photoCategoryDto);
            if (!result)
                return BadRequest("Could not add category to photo.");

            return NoContent();
        }

        // DELETE: api/photos/{photoGuid}/categories/{categoryGuid}
        [HttpDelete("{photoGuid:guid}/categories/{categoryGuid:guid}")]
        public async Task<IActionResult> RemoveCategoryFromPhoto(Guid photoGuid, Guid categoryGuid)
        {
            var result = await photoService.DeletePhotoCategoryAsync(photoGuid, categoryGuid);
            if (!result)
                return NotFound();
            return NoContent();
        }

        #endregion





    }
}
