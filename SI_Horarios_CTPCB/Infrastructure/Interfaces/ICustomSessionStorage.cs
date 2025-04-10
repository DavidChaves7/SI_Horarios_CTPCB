namespace SI_Horarios_CTPCB.Infrastructure.Interfaces
{
    public interface ICustomSessionStorage
    {
        Task<T?> GetObjectAsync<T>(string objKey);

        Task SetObjectAsync(string objKey, object value);

        Task DeleteObjectAsync(string objKey);
    }
}
