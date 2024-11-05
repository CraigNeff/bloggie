using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bloggie.Web.Controllers
{
    public class AdminBlogPostsController : Controller
    {
        private readonly ITagRepository _tagRepository;
        private readonly IBlogPostRepository _blogPostRepository;

        public AdminBlogPostsController(ITagRepository tagRepository, IBlogPostRepository blogPostRepository)
        {
            this._tagRepository = tagRepository;
            this._blogPostRepository = blogPostRepository;
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            //Get stags from repository
            var tags = await _tagRepository.GetAllAsync();

            var model = new AddBlogPostRequest
            {
                Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
            }; 

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddBlogPostRequest addBlockPostRequest)
        {
            //Map view model to domain model 
            var blogPost = new BlogPost
            {
                Heading = addBlockPostRequest.Heading,
                PageTitle = addBlockPostRequest.PageTitle,
                Content = addBlockPostRequest.Content,
                ShortDescription = addBlockPostRequest.ShortDescription,
                FeaturedImageUrl = addBlockPostRequest.FeaturedImageUrl,
                UrlHandle = addBlockPostRequest.UrlHandle,
                PublishedDate = addBlockPostRequest.PublishedDate,
                Author = addBlockPostRequest.Author,
                Visible = addBlockPostRequest.Visible,
          
            };

            //Map tags from selected tags 
            var selectedTags = new List<Tag>();

            foreach(var selectedTagId in addBlockPostRequest.SelectedTags)
            {
                var selectedTagIdAsGuid = Guid.Parse(selectedTagId);
                var existingTag = await _tagRepository.GetAsync(selectedTagIdAsGuid);

                if (existingTag != null)
                {
                    selectedTags.Add(existingTag);
                }
            }
            //Mapping tags back to domain model 
            blogPost.Tags = selectedTags;

            await _blogPostRepository.AddAsync(blogPost);

            return RedirectToAction("Add");
        }
    }
}
