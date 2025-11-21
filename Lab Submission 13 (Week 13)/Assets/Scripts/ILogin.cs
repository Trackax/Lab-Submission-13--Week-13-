using PlayFab.ClientModels;
using PlayFab;

public interface ILogin
{
    void Login(System.Action<LoginResult>)
}