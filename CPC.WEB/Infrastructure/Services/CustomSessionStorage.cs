using SI_Horarios_CTPCB.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace SI_Horarios_CTPCB.Infrastructure.Services
{
    public class CustomSessionStorage : ICustomSessionStorage
    {
        private readonly ProtectedLocalStorage _storage;

        public CustomSessionStorage(ProtectedLocalStorage storage)
        {
            _storage = storage;
        }

        public async Task<T?> GetObjectAsync<T>(string objKey)
        {
            var result = await _storage.GetAsync<T>(objKey);
            return result.Value;
        }

        public async Task SetObjectAsync(string objKey, object value)
        {
            await _storage.SetAsync(objKey, value);
        }

        public async Task DeleteObjectAsync(string objKey)
        {
            await _storage.DeleteAsync(objKey);
        }

    }
}
