using System.Collections.ObjectModel;

using Prism.Mvvm;
using Prism.Regions;

using SampleWpf.Core.Contracts.Services;
using SampleWpf.Core.Models;

namespace SampleWpf.ViewModels;

public class ListDetailsViewModel : BindableBase, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;
    private SampleOrder _selected;

    public SampleOrder Selected
    {
        get { return _selected; }
        set { SetProperty(ref _selected, value); }
    }

    public ObservableCollection<SampleOrder> SampleItems { get; private set; } = new ObservableCollection<SampleOrder>();

    public ListDetailsViewModel(ISampleDataService sampleDataService)
    {
        _sampleDataService = sampleDataService;
    }

    public async void OnNavigatedTo(NavigationContext navigationContext)
    {
        SampleItems.Clear();

        var data = await _sampleDataService.GetListDetailsDataAsync();

        foreach (var item in data)
        {
            SampleItems.Add(item);
        }

        Selected = SampleItems.First();
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
        => true;
}
