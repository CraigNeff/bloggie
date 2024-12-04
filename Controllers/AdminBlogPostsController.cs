using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bloggie.Web.Controllers
{
    [Authorize(Roles = "Admin")]  //Sets authorization on all controller's action methods within this class. 

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

        [HttpGet]
        public async Task<IActionResult> List()
        {
            //Call the repository
            var blogPosts = await _blogPostRepository.GetAllAsync();

            return View(blogPosts);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            //Retrieve the result from the reposiroty 
            var blogPost = await _blogPostRepository.GetAsync(id);
            var tagsDomainModel = await _tagRepository.GetAllAsync();
            if (blogPost != null)
            {
                //map domain model into the view model (blogPost is the domain model) model is going to be the view model
                var model = new EditBlogPostRequest
                {
                    Id = blogPost.Id,
                    Heading = blogPost.Heading,
                    PageTitle = blogPost.PageTitle,
                    Content = blogPost.Content,
                    Author = blogPost.Author,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    UrlHandle = blogPost.UrlHandle,
                    ShortDescription = blogPost.ShortDescription,
                    PublishedDate = blogPost.PublishedDate,
                    Visible = blogPost.Visible,
                    Tags = tagsDomainModel.Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }),
                    SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray()

                };
                return View(model);
            }

            //pass data to view 
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
        {
            //map view model back to domain model 
            var blogPostDomainModel = new BlogPost
            {
                Id = editBlogPostRequest.Id,
                Heading = editBlogPostRequest.Heading,
                PageTitle = editBlogPostRequest.PageTitle,
                Content = editBlogPostRequest.Content,
                Author = editBlogPostRequest.Author,
                ShortDescription = editBlogPostRequest.ShortDescription,
                FeaturedImageUrl = editBlogPostRequest.FeaturedImageUrl,
                PublishedDate = editBlogPostRequest.PublishedDate,
                UrlHandle = editBlogPostRequest.UrlHandle,
                Visible = editBlogPostRequest.Visible
            };

            //Map tags into domain model 
            var selectedTags = new List<Tag>();
            foreach (var selectedTag in editBlogPostRequest.SelectedTags)
            {
                if (Guid.TryParse(selectedTag, out var tag))
                {
                    var foundTag = await _tagRepository.GetAsync(tag);

                    if (foundTag != null)
                    {
                        selectedTags.Add(foundTag);
                    }
                }
            }

            blogPostDomainModel.Tags = selectedTags;

            //Submit information to repository to update 
            var updatedBlog = await _blogPostRepository.UpdateAsync(blogPostDomainModel);

            if(updatedBlog != null)
            {
                //Show success notification
                return RedirectToAction("Edit");
            }
            //redirect to GET
            //Show error notification 
            return RedirectToAction("Edit");

        }

        [HttpPost]
        public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
        {
            // Talk to repository to delete this blog post and tags 
            var deletedBlogPost = await _blogPostRepository.DeleteAsync(editBlogPostRequest.Id);

            if (deletedBlogPost != null)
            {
                //Show success notification 
                return RedirectToAction("List");
            }

            //Show error notification 
            return RedirectToAction("Edit", new { id = editBlogPostRequest.Id });
        }
    }


}
