﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using RealEstate.Database;

namespace RealEstate.Security
{
    public class UserStore<TUser> : IUserStore<TUser>, 
        IUserPasswordStore<TUser>, IUserLoginStore<TUser> where TUser : IdentityUser
    {
        private readonly IdentityContext _context;

        public UserStore(IdentityContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            //auto for mongo
        }

        public Task CreateAsync(TUser user)
        {
            return Task.Run(() => _context.Users.Insert(user));
        }

        public Task UpdateAsync(TUser user)
        {
            return Task.Run(() => _context.Users.Save(user));
        }

        public Task DeleteAsync(TUser user)
        {
            var userToDelete = Query<TUser>.EQ(u => u.Id, user.Id);
            return Task.Run(() => _context.Users.Remove(userToDelete));
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            return Task.Run(() => _context.Users.FindOneByIdAs<TUser>(ObjectId.Parse(userId)));
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            var byName = Query<TUser>.EQ(u => u.UserName, userName);
            return Task.Run(() => _context.Users.FindOneAs<TUser>(byName));
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            if(user == null) throw new ArgumentException("user");

            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PasswordHash != null);
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if(user == null) throw new ArgumentException("user is null");

            if (!IsExistingLogin(user, login))
            {
                user.Logins.Add(login);
            }

            return Task.FromResult(true);
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null) throw new ArgumentException("user is null");

            user.Logins.RemoveAll(l => l.LoginProvider == login.LoginProvider && l.ProviderKey == login.ProviderKey);

            return Task.FromResult(0);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            if (user == null) throw new ArgumentException("user is null");

            return Task.FromResult(user.Logins.ToList() as IList<UserLoginInfo>);
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            TUser user =
                _context.Users.FindOneAs<TUser>(Query.And(Query.EQ("Logins.LoginProvider", login.LoginProvider),
                    Query.EQ("Logins.ProviderKey", login.ProviderKey)));

            return Task.FromResult(user);
        }


        private static bool IsExistingLogin(TUser user, UserLoginInfo login)
        {
            return user.Logins.Any(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);
        }
    }
}