namespace SFA.DAS.FAT.Domain.Interfaces;
public interface ISessionService
{
    void Set(string key, string value);
    void Set<T>(string key, T model);
    string Get(string key);
    T Get<T>(string key);
    void Delete(string key);
    void Clear();
    bool Contains(string key);
}
