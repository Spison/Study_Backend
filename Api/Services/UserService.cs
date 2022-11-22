using Api.Configs;
using Api.Exceptions;
using Api.Models.Attach;
using Api.Models.User;
using Api.Models.Subscribe;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using DataAccessLayer;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;


namespace Api.Services
{
    public class UserService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;


        public UserService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<bool> CheckUserExist(string email)
        {

            return await _context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());

        }
        public async Task AddAvatarToUser(Guid userId, MetadataModel meta, string filePath)
        {
            var user = await _context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                var avatar = new Avatar
                {
                    Author = user,
                    MimeType = meta.MimeType,
                    FilePath = filePath,
                    Name = meta.Name,
                    Size = meta.Size
                };
                user.Avatar = avatar;

                await _context.SaveChangesAsync();
            }

        }
        public async Task<AttachModel> GetUserAvatar(Guid userId)
        {
            var user = await GetUserById(userId);
            var atach = _mapper.Map<AttachModel>(user.Avatar);
            return atach;
        }
        public async Task Delete(Guid id)
        {
            var dbUser = await GetUserById(id);
            if (dbUser != null)
            {
                _context.Users.Remove(dbUser);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Guid> CreateUser(CreateUserModel model)
        {
            var dbUser = _mapper.Map<DataAccessLayer.Entities.User>(model);
            var t = await _context.Users.AddAsync(dbUser);
            await _context.SaveChangesAsync();
            return t.Entity.Id;
        }
        public async Task<IEnumerable<UserAvatarModel>> GetUsers() =>
            await _context.Users.AsNoTracking()
            .Include(x => x.Avatar)
            .Include(x => x.Posts)
            .Select(x => _mapper.Map<UserAvatarModel>(x))
            .ToListAsync();


        public async Task<UserAvatarModel> GetUser(Guid id) =>
            _mapper.Map<User, UserAvatarModel>(await GetUserById(id));



        private async Task<User> GetUserById(Guid id)
        {
            var user = await _context.Users.Include(x => x.Avatar).Include(x => x.Posts).FirstOrDefaultAsync(x => x.Id == id);
            if (user == null || user == default)
                throw new UserNotFoundException();
            return user;
        }
        public async Task AddSubscribe (SubscribeModel model) // Подписаться на что-то
        {
            var dbSub = _mapper.Map<DataAccessLayer.Entities.Subscribe>(model);
            var t = await _context.Subscribes.AddAsync(dbSub);
            await _context.SaveChangesAsync();
        }
        public async Task DelSubscribe (SubscribeModel model)
        {
            var dbSub = _mapper.Map<DataAccessLayer.Entities.Subscribe>(model);
            if(dbSub != null)
            {
                _context.Subscribes.Remove(dbSub);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<Subs>> GetSubscribers (Guid mainId)
        {           
            var res = await _context.Subscribes
                .Where(x=>x.Id!=default)
                .Where(x=>x.SubId == mainId)
                .ToListAsync();
            if (res== null)
                throw new SubscribersNotFoundException();            
            var list = new List<Subs>();
            foreach (var item in res)
            {
                var model = _mapper.Map<Api.Models.Subscribe.SubscribeModel>(item);//По идее можно объединить...
                var model2 = _mapper.Map<Api.Models.Subscribe.Subs>(model);
                list.Add(model2);
            }
            return list;
        }
    }
}