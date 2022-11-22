using Api.Configs;
using Api.Exceptions;
using Api.Models.Attach;
using Api.Models.Post;
using Api.Models.User;
using Api.Models.Comment;
using Api.Models.Like;
using Api.Models.Subscribe;
using AutoMapper;
using DataAccessLayer;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Api.Services
{
    public class PostService
    {
        private readonly IMapper _mapper;
        private readonly DataAccessLayer.DataContext _context;
        public PostService(IMapper mapper, IOptions<AuthConfig> config, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task CreatePost(CreatePostRequest request)
        {
            var model = _mapper.Map<CreatePostModel>(request);
            model.Contents.ForEach(x =>
            {
                x.AuthorId = model.AuthorId;
                x.FilePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "attaches",
                    x.TempId.ToString());

                var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), x.TempId.ToString()));
                if (tempFi.Exists)
                {
                    var destFi = new FileInfo(x.FilePath);
                    if (destFi.Directory != null && !destFi.Directory.Exists)
                        destFi.Directory.Create();

                    File.Move(tempFi.FullName, x.FilePath, true);
                }
            });
            var dbModel = _mapper.Map<Post>(model);
            var user = await _context.Users.Include(x => x.Avatar).Include(x => x.Posts).FirstOrDefaultAsync(x => x.Id == dbModel.AuthorId);
            if (user == null || user == default)
                throw new UserNotFoundException();
            dbModel.Author=user;

            await _context.Posts.AddAsync(dbModel);//Надо для dbModel автора найти
            await _context.SaveChangesAsync();
        }

        public async Task<List<PostModel>> GetPosts(int skip, int take) //получить общую ленту (со всеми не скрытыми постами)
        {
            var posts = await _context.Posts
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Include(x => x.PostContents).AsNoTracking().OrderByDescending(x => x.Created).Skip(skip).Take(take)
                .Where(x => x.VisibleToSubscribersOnly==false)//единственная измененная строка
                .Select(x => _mapper.Map<PostModel>(x))
                .ToListAsync();

            return posts;
        }
        public async Task<List<PostModel>> GetSubscriptionsPosts(int skip, int take,Guid userId) //Получить ленту - с постами юзеров, на которых подписан
        {
            var subs = await _context.Subscribes
                .Where(x => x.UserId == userId)
                .Select(x => _mapper.Map<SubscribeModel>(x))
                .ToListAsync();
            List<Guid> subsId = new List<Guid>();
            foreach (var sub in subs)
            {
                subsId.Add(sub.SubId); 
            }

            var posts = await _context.Posts
                    .Include(x => x.Author)
                    .ThenInclude(x => x.Avatar)                   
                    .Include(x => x.PostContents)
                    .AsNoTracking()
                    .OrderByDescending(x => x.Created).Skip(skip).Take(take)

                    .Where(x => subsId.Contains(x.AuthorId))
                    .Select(x => _mapper.Map<PostModel>(x))
                    .ToListAsync();

            return posts;
        }
        //Отложено, как ересь
        //public async Task<List<PostModel>>GetSubscribersPosts (Guid userId)
        //{
        //    var subs = await _context.Subscribes
        //        .Include(u => u.Id)
        //        .Where(u => u.SubId == userId)               
        //        .ToListAsync()
        //        ;



        //    var posts = await _context.Posts
        //        .Include(u => u.Id)//Подключили посты
        //        .Include(x => x.Author)
        //        .Include(s => s.VisibleToSubscribersOnly)
        //        .Where(s => s.VisibleToSubscribersOnly == true)
        //        .ToListAsync();
        //    ;
        //    var list = new List<Subs>();
        //    foreach (var item in subs)
        //    {
        //        var model = _mapper.Map<Api.Models.Subscribe.SubscribeModel>(item);//По идее можно объединить...
        //        var model2 = _mapper.Map<Api.Models.Subscribe.Subs>(model);
        //        list.Add(model2);
        //    }
        //    var result= new List<Post>();
        //    foreach (var item in list)
        //    {
        //        var res = posts.Where(x => x.AuthorId == item);
        //    }
        //    var result = posts.Where(x=>x.AuthorId==.)


        //    //var subscribers = await _context.Subscribes
        //    //    .Where(u=>u.UserId==userId)
        //    //    .Include(x=>x.UserId)
        //    //    .ThenInclude
        //    return posts;
        //}
        public async Task<PostModel> GetPostById(Guid id)
        {
            var post = await _context.Posts
                  .Include(x => x.Author).ThenInclude(x => x.Avatar)
                  .Include(x => x.PostContents).AsNoTracking()
                  .Where(x => x.Id == id)
                  .Select(x => _mapper.Map<PostModel>(x))
                  .FirstOrDefaultAsync();
            if (post == null)
                throw new PostNotFoundException();

            return post;
        }


        public async Task<AttachModel> GetPostContent(Guid postContentId)
        {
            var res = await _context.PostContents.FirstOrDefaultAsync(x => x.Id == postContentId);

            return _mapper.Map<AttachModel>(res);
        }
        public async Task CreateComment(CreateCommentRequest request)
        {
            var model = _mapper.Map<CreateCommentModel>(request);
            model.CommentText = request.CommentText;
            model.Id = Guid.NewGuid();
            //model.AuthorId=request.AuthorId;
            

            var dbModel = _mapper.Map<Comment>(model);
            await _context.Comments.AddAsync(dbModel);
            await _context.SaveChangesAsync();
        }
        public async Task <CommentModel> GetComment(Guid postId)
        {
            var comment = await _context.Comments
                .Include(x => x.Author).ThenInclude(x => x.Avatar)
                .Where(x => x.PostId == postId)
                .Select(x => _mapper.Map<CommentModel>(x))
                .FirstOrDefaultAsync();
                ;
            if (comment == null)
                throw new PostNotFoundException();
            return comment;
        }
        public async Task<List<CommentModel>> GetComments(Guid postId) // Не совсем понимаю, куда можно было бы добавить await
        {
            var comments = new List<CommentModel>(); //Костыльный вариант, стоит улучшить
            foreach (var comment in _context.Comments)
            {
                if (comment.PostId == postId)
                    comments.Add(_mapper.Map<CommentModel>(comment));
            }

            if (comments == null)
                throw new PostNotFoundException();
            return comments;
        }
        public async Task AddLikeToPost (Guid postId,Guid userId)
        {
            var post = await _context.Posts
                  .Include(x => x.Author).ThenInclude(x => x.Avatar)
                  .Include(x => x.PostContents).AsNoTracking()
                  .Where(x => x.Id == postId)
                  .Select(x => _mapper.Map<PostModel>(x))
                  .FirstOrDefaultAsync();
            if (post == null)
                throw new PostNotFoundException();

            var model = new LikePostModel();
            model.PostId = postId;
            model.UserId = userId;
            var dbModel = _mapper.Map<LikePost>(model);            
            await _context.LikesPost.AddAsync(dbModel);//Добавление в таблицу LikesPost
            await _context.SaveChangesAsync();
        }
        public async Task<List<LikePostModelRequest>> GetUsersWhoLikesAtPost(Guid postId)// надо подумать над выводом - здесь тупо юзеров выводить надо
        {
            var like = await _context.LikesPost.AsNoTracking()
                .Include(x => x.PostId)
                .Where(x => x.PostId == postId)
                .Select(x => _mapper.Map<LikePostModelRequest>(x))
                .ToListAsync()
                ;
            return like;
        }
        public async Task DelUserLikeAtPost(Guid postId,Guid UserId)
        {
            var like = await _context.LikesPost
                //.Include(x => x.PostId)
                .Where(x => x.PostId == postId)
                .Where(x => x.UserId == UserId)
                .FirstOrDefaultAsync()
                ;
            if (like == null)
                throw new LikesNotFoundException();
            var likeToRemove = await _context.LikesPost.SingleOrDefaultAsync(x=>x.Id==like.Id);
            var model = _mapper.Map<LikePost>(likeToRemove);
            if(likeToRemove != null)
            {
                _context.LikesPost.Remove(model);
                _context.SaveChanges();
            }
        }
        public async Task<int> GetCountLikesAtPost (Guid postId)//Можно подправить, чтоб был более приятный вывод, а не тупо число
        {
            var count =  _context.LikesPost.Count(t=>t.PostId==postId);
            return count;
        }       
    }
}
