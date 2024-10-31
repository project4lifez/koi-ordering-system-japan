namespace KoiOrderingSystem.Models;

public class BlogViewModel
{
    public Blog Blog { get; set; }

    public Blog? MainFeaturedBlog { get; set; }
    public List<Blog> SideBlogs { get; set; } = new List<Blog>();
    public List<Blog> TrendingBlogs { get; set; } = new List<Blog>();
}