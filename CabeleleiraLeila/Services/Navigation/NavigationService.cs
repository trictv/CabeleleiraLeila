using CabeleleiraLeila.ViewModels.Base;

namespace CabeleleiraLeila.Services.Navigation;

public class NavigationService : INavigationService
{
    public Task NavigateToAsync<TViewModel>()
    {
        var route = GetRoute<TViewModel>();
        return ExecuteSafeNavigationAsync(() => Shell.Current.GoToAsync(route));
    }

    public Task GoBackAsync()
    {
        return ExecuteSafeNavigationAsync(async () =>
        {
            if (Shell.Current.Navigation.NavigationStack.Count > 1)
            {
                await Shell.Current.GoToAsync("..");
            }
        });
    }

    public Task NavigateToAsync<TViewModel, TParameter>(TParameter parameter)
    {
        var route = GetRoute<TViewModel>();
        var parameters = new Dictionary<string, object> { ["Parameter"] = parameter! };

        return ExecuteSafeNavigationAsync(() => Shell.Current.GoToAsync(route, parameters));
    }

    public Task GoToRootAsync()
    {
        return ExecuteSafeNavigationAsync(() => Shell.Current.GoToAsync("//inicio"));
    }

    private static string GetRoute<TViewModel>()
    {
        var viewModelName = typeof(TViewModel).Name;

        if (!viewModelName.EndsWith("ViewModel"))
        {
            throw new InvalidOperationException(
                $"O ViewModel '{viewModelName}' deve terminar com 'ViewModel'.");
        }

        return viewModelName.Replace(
            "ViewModel",
            "Page");
    }

    private async Task ExecuteSafeNavigationAsync(Func<Task> navigationAction)
    {

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await navigationAction();
        });
    }
}