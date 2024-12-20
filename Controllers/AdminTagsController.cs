using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Controllers
{
    public class AdminTagsController : Controller
    {
        #region Controller 

        private readonly ITagRepository _tagRepository;
        public AdminTagsController(ITagRepository tagRepository)
        {
            this._tagRepository = tagRepository;
        }

        #endregion

        #region ActionMethods

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ActionName("Add")]
        public async Task<IActionResult> Add(AddTagRequest addTagRequest)
        {
            if (ModelState.IsValid == false) 
            {
                return View();
            }
            //Mapping AddTagRequest to Tag Domain Model
            Tag tag = new Tag
            {
                Name = addTagRequest.Name,
                DisplayName = addTagRequest.DisplayName
            };

            await _tagRepository.AddAsync(tag);

            return RedirectToAction("List");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ActionName("List")]
        public async Task<IActionResult> List()
        {
            //use DbContext to read the tags
            var tags = await _tagRepository.GetAllAsync();

            return View(tags);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var tag = await _tagRepository.GetAsync(id);
            
            if (tag != null)
            {   
                //Turning the database response back into a model object 
                var editTagRequest = new EditTagRequest
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    DisplayName = tag.DisplayName
                };
                return View(editTagRequest);
            }

            return View(null);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditTagRequest editTagRequest)
        {   
            //TUrning the view model back into a domain model
            var tag = new Tag
            {
                Id = editTagRequest.Id,
                Name = editTagRequest.Name,
                DisplayName = editTagRequest.DisplayName
            };

            var updatedTag = await _tagRepository.UpdateAsync(tag);

            if (updatedTag != null)
            {
                //Show success notification
            }
            else
            {
                //show error notification
            }
            
            return RedirectToAction("Edit", new { id = editTagRequest.Id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(EditTagRequest editTagRequest)
        {
            var deletedTag = await _tagRepository.DeleteAsync(editTagRequest.Id); 

            if (deletedTag != null)
            {
                //Show success notification
                return RedirectToAction("List");
            }

            //Show error notification
            return RedirectToAction("Edit", new { id = editTagRequest.Id });
        }

        #endregion

    }
}
