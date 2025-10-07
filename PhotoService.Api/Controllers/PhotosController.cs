using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PhotoService.Application.DTOs;
using PhotoService.Application.Interfaces;

namespace PhotoService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController(IPhotoService photoService, IPhotoStorageService photoStorageService, IRabbitMqService rabbitMqService, IMapper mapper) : ControllerBase
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

        // POST: api/photos/upsert
        [HttpPost("upsert")]
        public async Task<ActionResult<PhotoDto>> UpsertPhoto([FromForm] PhotoWriteFormDto photoWriteFormDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string? fileName = null;
            if(photoWriteFormDto.File != null && photoWriteFormDto.File.Length > 0)
            {
                // File upload to GCS.
                using var fStream = photoWriteFormDto.File.OpenReadStream();
                var fileUrl = await photoStorageService.UploadFileAsync(fStream, photoWriteFormDto.File.FileName, photoWriteFormDto.File.ContentType);
                fileName = photoWriteFormDto.File.FileName;
            }

            PhotoDto? result;
            Guid photoGuid;


            if (photoWriteFormDto.PhotoGuid.HasValue)
            {
                // Update existing record info.
                var updated = await photoService.UpdatePhotoAsync(photoWriteFormDto);
                if (!updated) return NotFound();

                result = await photoService.GetPhotoByGuidAsync(photoWriteFormDto.PhotoGuid.Value);
                if (result == null)
                    return BadRequest();

                //return Ok(result);
                photoGuid = result.PhotoGuid;
            }
            else
            {
                // Create new record.
                if (fileName == null)
                    return BadRequest("File is required for creating a new photo.");

                var createDto = new PhotoCreateDto
                {
                    FileName = fileName,
                    Title = photoWriteFormDto.Title,
                    Description = photoWriteFormDto.Description,
                    Location = photoWriteFormDto.Location,
                    Country = photoWriteFormDto.Country,
                    DateTaken = photoWriteFormDto.DateTaken
                };

                var created = await photoService.AddPhotoAsync(createDto);
                result = mapper.Map<PhotoDto>(created);
                photoGuid = result.PhotoGuid;
                // return CreatedAtAction(nameof(GetPhotoByGuid), new { photoGuid = result.PhotoGuid }, result);
            }

            // Category section.
            if (photoWriteFormDto.CategoryGuids != null && photoWriteFormDto.CategoryGuids.Any())
            {
                // Remove existing link (Photo <=> Category).
                await photoService.DeletePhotoCategoryAsync(photoGuid);

                foreach (var catId in photoWriteFormDto.CategoryGuids)
                {
                    // RabbitMQ validation.
                    // Validate Category Guid is valid or not via RabbitMQ to CategoryService before tagging it to Photo.
                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    bool isValid = await rabbitMqService.ValidateCategoryAsync(catId, cts.Token);

                    if (!isValid)
                        return BadRequest("Invalid Category!");

                    // If pass, proceed to next step.
                    var added = await photoService.CreatePhotoCategoryAsync(new PhotoCategoryDto { PhotoGuid = photoGuid, CategoryGuid = catId });
                    if (!added)
                        return BadRequest("Could not add category to photo.");
                }
            }

            // Return result.
            return photoWriteFormDto.PhotoGuid.HasValue
                ? Ok(result)
                : CreatedAtAction(nameof(GetPhotoByGuid), new { photoGuid = result.PhotoGuid }, result);
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


        /* Combine below 3 into one as 'UpsertPhoto' EndPoint.
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
        public async Task<IActionResult> UpdatePhoto(Guid photoGuid, [FromBody] PhotoWriteFormDto photoDto)
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

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file provided!");

            using var fStream = file.OpenReadStream();
            var fileUrl = await photoStorageService.UploadFileAsync(fStream, file.FileName, file.ContentType);

            return Ok(new { Url = fileUrl });
        }
        */


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


        // Maintain it for RabbitMQ Event Broker. In case other services want to call PhotoService directly.
        // For my BFF design pattern, I will skip RabbitMQ.
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

        // DELETE: api/photos/categories/{photoGuid}
        [HttpDelete("{photoGuid:guid}/categories")]
        public async Task<IActionResult> RemoveCategoryFromPhoto(Guid photoGuid)
        {
            var result = await photoService.DeletePhotoCategoryAsync(photoGuid);
            if (!result)
                return NotFound();
            return NoContent();
        }

        #endregion





    }
}
