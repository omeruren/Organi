using MediatR;
using Organi.Server.Application.Common.Models;
using Organi.Server.Application.Features.Blog.Commands.CreateBlogComment;
using Organi.Server.Application.Features.Blog.Commands.CreateBlogPost;
using Organi.Server.Application.Features.Blog.Commands.DeleteBlogComment;
using Organi.Server.Application.Features.Blog.Commands.DeleteBlogPost;
using Organi.Server.Application.Features.Blog.Commands.UpdateBlogPost;
using Organi.Server.Application.Features.Blog.DTOs;
using Organi.Server.Application.Features.Blog.Queries.GetBlogPostById;
using Organi.Server.Application.Features.Blog.Queries.GetBlogPostBySlug;
using Organi.Server.Application.Features.Blog.Queries.GetBlogPosts;

namespace Organi.Server.WebAPI.Endpoints;

public static class BlogEndpoints
{
    public static void MapBlogEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/blog-posts").WithTags("Blog");

        group.MapGet("/", GetBlogPosts)
            .WithName("GetBlogPosts")
            .WithDescription("Retrieves a paginated list of published blog posts.")
            .Produces<PagedResponse<BlogPostSummaryResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        group.MapGet("/slug/{slug}", GetBlogPostBySlug)
            .WithName("GetBlogPostBySlug")
            .WithDescription("Retrieves a blog post by its URL-friendly slug.")
            .Produces<BlogPostResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}", GetBlogPostById)
            .WithName("GetBlogPostById")
            .WithDescription("Retrieves a blog post by its unique identifier.")
            .Produces<BlogPostResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateBlogPost)
            .WithName("CreateBlogPost")
            .WithDescription("Creates a blog post.")
            .RequireAuthorization("CanManageProducts")
            .Produces<BlogPostResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapPut("/{id:guid}", UpdateBlogPost)
            .WithName("UpdateBlogPost")
            .WithDescription("Updates an existing blog post.")
            .RequireAuthorization()
            .Produces<BlogPostResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeleteBlogPost)
            .WithName("DeleteBlogPost")
            .WithDescription("Soft-deletes a blog post.")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/comments", CreateBlogComment)
            .WithName("CreateBlogComment")
            .WithDescription("Adds a comment to a published blog post.")
            .RequireAuthorization()
            .Produces<BlogCommentResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/comments/{id:guid}", DeleteBlogComment)
            .WithName("DeleteBlogComment")
            .WithDescription("Deletes a comment.")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetBlogPosts(
        ISender sender,
        CancellationToken cancellationToken,
        string? search = null,
        bool? isPublished = null,
        int page = 1,
        int pageSize = 10)
    {
        var result = await sender.Send(new GetBlogPostsQuery(search, isPublished, page, pageSize), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetBlogPostBySlug(
        string slug,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetBlogPostBySlugQuery(slug), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetBlogPostById(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetBlogPostByIdQuery(id), cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateBlogPost(
        ISender sender,
        CreateBlogPostCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);
        return Results.Created($"/api/blog-posts/{result.Id}", result);
    }

    private static async Task<IResult> UpdateBlogPost(
        Guid id,
        ISender sender,
        UpdateBlogPostCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command with { Id = id }, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteBlogPost(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteBlogPostCommand(id), cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> CreateBlogComment(
        Guid id,
        ISender sender,
        CreateBlogCommentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command with { BlogPostId = id }, cancellationToken);
        return Results.Created($"/api/blog-posts/{id}", result);
    }

    private static async Task<IResult> DeleteBlogComment(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteBlogCommentCommand(id), cancellationToken);
        return Results.NoContent();
    }
}
