using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace IPTGram.Data
{
    /// <summary>
    /// Inicializa a base de dados.
    /// Esta classe usa os ficheiros na pasta "SeedData" do projecto
    /// para adicionar os conteúdos à base de dados.
    /// (Ver classe Startup, método Configure, para ver onde é usada)
    /// </summary>
    public class DbInitializer
    {
        private readonly IPTGramDb db;
        private readonly UserManager<User> userManager;
        private readonly ILogger<DbInitializer> logger;

        public DbInitializer(IPTGramDb db, UserManager<User> userManager, ILogger<DbInitializer> logger)
        {
            this.db = db;
            this.userManager = userManager;
            this.logger = logger;
        }

        public async Task Seed()
        {
            await SeedIdentity();
            await SeedData();
        }

        /// <summary>
        /// Inicializa posts, comentários, e likes.
        /// </summary>
        public async Task SeedData()
        {
            var text = File.ReadAllText("SeedData/data.json");

            var data = JsonConvert.DeserializeAnonymousType(text, new
            {
                posts = new List<Post>(),
                comments = new List<Comment>(),
                likes = new List<PostLike>()
            });

            await SeedPosts(data.posts);

            await SeedComments(data.comments);

            await SeedLikes(data.likes);

            await db.SaveChangesAsync();
        }

        private async Task SeedLikes(List<PostLike> likes)
        {
            if (await db.PostLikes.AnyAsync())
            {
                logger.LogInformation("Likes already has data.");
                return;
            }

            foreach (var like in likes)
            {
                if (await db.PostLikes.AnyAsync(p => p.PostId == like.PostId && p.UserId == like.UserId))
                {
                    logger.LogInformation("Skipping like for post {PostId} and user {UserId}", like.PostId, like.UserId);
                    continue;
                }

                db.PostLikes.Add(like);
            }

            await db.SaveChangesAsync();
        }

        private async Task SeedComments(List<Comment> comments)
        {
            if (await db.Comments.AnyAsync())
            {
                return;
            }

            foreach (var comment in comments)
            {
                if (await db.Comments.AnyAsync(p => p.Id == comment.Id))
                {
                    logger.LogInformation("Skipping comment for post {PostId} and user {UserId}", comment.PostId, comment.UserId);
                    continue;
                }

                comment.Id = 0;

                db.Comments.Add(comment);
            }

            await db.SaveChangesAsync();
        }

        private async Task SeedPosts(List<Post> posts)
        {
            if (await db.Posts.AnyAsync())
            {
                logger.LogInformation("Posts already has data.");
                return;
            }

            foreach (var post in posts)
            {
                if (await db.Posts.AnyAsync(p => p.Id == post.Id))
                {
                    continue;
                }

                post.Id = 0;

                db.Posts.Add(post);
            }

            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Inicializa os users.
        /// </summary>
        public async Task SeedIdentity()
        {
            if (await db.Users.AnyAsync())
            {
                logger.LogInformation("Users already has data.");
                return;
            }

            var text = File.ReadAllText("SeedData/users.json");

            var users = JsonConvert.DeserializeAnonymousType(text, new[]
            {
                new { UserName = "", Name = "", Password = "", Id = "", IsDefault = false }
            });

            foreach (var userData in users)
            {
                if (await userManager.FindByNameAsync(userData.UserName) != null)
                {
                    logger.LogInformation("User {UserName} already exists.", userData.UserName);
                    continue;
                }

                var user = new User
                {
                    UserName = userData.UserName,
                    Id = userData.Id,
                    Name = userData.Name,
                    IsDefault = userData.IsDefault
                };

                await userManager.CreateAsync(user, userData.Password);
            }
        }
    }
}
