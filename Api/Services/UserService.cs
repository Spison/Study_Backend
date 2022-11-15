using Api.Models.Attach;
using Api.Models.User;
using Api.Configs;
using Common;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DataAccessLayer;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;


namespace Api.Services
{
    public class UserService 
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private Func<UserModel, string?>? _linkGenerator;
        public void SetLinkGenerator(Func<UserModel, string?> linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public UserService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<bool> CheckUserExist(string email)
        {

            return await _context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());

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
            await _context.Users.AddAsync(dbUser);
            var t = await _context.Users.AddAsync(dbUser);
            await _context.SaveChangesAsync();
            return t.Entity.Id;
        }

        public async Task<UserAvatarModel> GetUser(Guid id)
        {
            var user = await GetUserById(id);

            return new UserAvatarModel(_mapper.Map<UserModel>(user), user.Avatar == null ? null : _linkGenerator);

        }
        public async Task<IEnumerable<UserAvatarModel>> GetUsers()
        {
            var users = await _context.Users.AsNoTracking().ProjectTo<UserModel>(_mapper.ConfigurationProvider).ToListAsync();
            return users.Select(x => new UserAvatarModel(x, _linkGenerator));
        }
        private async Task<User> GetUserById(Guid id)
        {
            var user = await _context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                throw new Exception("user not found");
       
            return user;
        }
        

        
        public async Task<UserSession> GetSessionById(Guid id)
        {
            var session = await _context.UserSessions.FirstOrDefaultAsync(x => x.Id == id);
            if (session == null)
            {
                throw new Exception("session is not found");
            }
            return session;
        }
        private async Task<UserSession> GetSessionByRefreshToken(Guid id)
        {
            var session = await _context.UserSessions.Include(x => x.User).FirstOrDefaultAsync(x => x.RefreshToken == id);
            if (session == null)
            {
                throw new Exception("session is not found");
            }
            return session;
        }
        

        public void Dispose()
        {
            _context.Dispose();
        }
        //Переместить в другой сервис
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

    }
}