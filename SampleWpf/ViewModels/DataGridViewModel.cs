using System.Collections.ObjectModel;

using Prism.Mvvm;
using Prism.Regions;

using SampleWpf.Core.Contracts.Services;
using SampleWpf.Core.Models;

namespace SampleWpf.ViewModels;

public class DataGridViewModel : BindableBase, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;

    public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

    public DataGridViewModel(ISampleDataService sampleDataService)
    {
        _sampleDataService = sampleDataService;
    }

    public async void OnNavigatedTo(NavigationContext navigationContext)
    {
        Source.Clear();

        // Replace this with your actual data
        var data = await _sampleDataService.GetGridDataAsync();

        foreach (var item in data)
        {
            Source.Add(item);
        }
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
        => true;
}
