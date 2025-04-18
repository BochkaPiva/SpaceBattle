namespace SpaceBattle.Lib;

public interface IUObject
{
    T GetProperty<T>(string key);
    void SetProperty<T>(string key, T value);
} 