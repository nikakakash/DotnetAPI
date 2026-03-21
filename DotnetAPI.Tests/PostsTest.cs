using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Xunit;
namespace DotnetAPI.Tests;



public class PostsTest : BaseTest
{
    [Fact]
    public async Task GET_AllPosts_WithValidToken_Returns200()
    {
        await AuthenticateAsync();
        var response = await _client.GetAsync("/Post/Posts/0/0/None");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_Posts_ByUserId_Returns200()
    {
        await AuthenticateAsync();
        var response = await _client.GetAsync("/Post/Posts/0/1006/None");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_Posts_WithSearchText_Returns200()
    {
        await AuthenticateAsync();
        var response = await _client.GetAsync("/Post/Posts/0/0/Test");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSinglePost_WithValidToken_Returns200()
    {
        await AuthenticateAsync();
        var response = await _client.GetAsync("/Post/Posts/1/0/None");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_SinglePost_ByPostId_Returns200()
    {
        await AuthenticateAsync();
        string postId = await CreateTestPostAsync();

        var response = await _client.GetAsync($"/Post/Posts/{postId}/0/None");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_Posts_WithInvalidToken_Returns401()
    {
        var response = await _client.GetAsync("Post/Posts/0/0/None");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GET_Posts_ForCurrentUserOnly_Returns200()
    {
        //My posts endpoint returns posts that belong to
        //the authenticated user and returns only their posts
        await AuthenticateAsync(); 
        var response = await _client.GetAsync("Post/MyPosts");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Update_posts_success()
    {
        await AuthenticateAsync();
        await CleanupLeftoverTestPostsAsync();
        string postId = await CreateTestPostAsync();

        var updatedPost = new
        {
            postId = int.Parse(postId),
            userId = 0,
            postTitle = "Updated Test Post",
            postContent = "This is an updated test post via automation",
            postCreated = DateTime.UtcNow,
            postUpdated= DateTime.UtcNow

        };
        var response = await _client.PutAsJsonAsync("/Post/UpsertPost", updatedPost);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    
    }

    [Fact]
    public async Task Delete_post_success()
    {
        await AuthenticateAsync();
        string postId = await CreateTestPostAsync();

        var response = await _client.DeleteAsync($"/Post/Post/{int.Parse(postId)}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_Post_WithoutAuth_Returns401()
    {
        var response = await _client.DeleteAsync("/Post/Post/1");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }





    private async Task CleanupLeftoverTestPostsAsync()
    {
        var response = await _client.GetAsync("/Post/Posts/0/0/None");
        var content = await response.Content.ReadAsStringAsync();
        var posts = JsonDocument.Parse(content);

        foreach (var post in posts.RootElement.EnumerateArray())
        {
            if (post.TryGetProperty("postTitle", out var postTitle) &&
                postTitle.GetString() == "Test Post automation")
            {
                int id = post.GetProperty("postId").GetInt32();
                await _client.DeleteAsync($"/Post/Post/{id}");
            }
        }
    }


    private async Task<string> CreateTestPostAsync()
    {
        var testPost = new
        {
            postId = 0,
            userId = 0,
            postTitle = "Test Post automation",
            postContent = "This is a test post via automation",
            postCreated = DateTime.UtcNow,
            postUpdated = DateTime.UtcNow
        };
        var response = await _client.PutAsJsonAsync("/Post/UpsertPost", testPost);
        response.EnsureSuccessStatusCode();

        var post = await response.Content.ReadAsStringAsync();

        var json = JsonDocument.Parse(post);
        return json.RootElement.GetProperty("postId").GetInt32().ToString();
    }

}