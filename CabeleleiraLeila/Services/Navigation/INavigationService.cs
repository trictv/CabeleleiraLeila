namespace CabeleleiraLeila.Services.Navigation;

public interface INavigationService
{
    Task NavigateToAsync<TViewModel>();

    Task NavigateToAsync<TViewModel, TParameter>(
        TParameter parameter);

    Task GoBackAsync();

    Task GoToRootAsync();
}