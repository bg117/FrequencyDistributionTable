using FrequencyDistributionTable.Models;
using FrequencyDistributionTable.Utilities;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FrequencyDistributionTable.ViewModels;

public class MainViewModel : ObservableObject
{
    private int _classInterval = 10;
    private decimal _lowestValue;
    private bool _canChangeLowestValue = true;
    private bool _canChangeClassInterval = true;

    public MainViewModel()
    {
        Classes.ClassChanged += Classes_ClassChanged;
    }

    private void Classes_ClassChanged(object? sender, ClassChangedEventArgs e)
    {
        UpdateNonSetProps();
    }

    private void UpdateNonSetProps()
    {
        RaisePropertyChangedEvent(nameof(TotalFrequency));
        RaisePropertyChangedEvent(nameof(Mean));
    }

    public int ClassInterval
    {
        get => _classInterval;
        set => SetProperty(ref _classInterval, value);
    }

    public decimal LowestValue
    {
        get => _lowestValue;
        set => SetProperty(ref _lowestValue, value);
    }

    public bool CanChangeLowestValue
    {
        get => _canChangeLowestValue;
        set => SetProperty(ref _canChangeLowestValue, value);
    }

    public bool CanChangeClassInterval
    {
        get => _canChangeClassInterval;
        set => SetProperty(ref _canChangeClassInterval, value);
    }

    public int TotalFrequency => Classes.Select(x => x.Class.Frequency).Sum();

    public decimal Mean =>
        // (sum of (class midpoint * frequency)) / total frequency
        TotalFrequency != 0 ? Classes.Select(x => x.Class.Midpoint * x.Class.Frequency).Sum() / TotalFrequency : 0;

    public ICommand AddClassCommand => new CommandBase(x => AddClass());
    public ICommand RemoveClassCommand => new CommandBase(x => RemoveClass(x as int? ?? -1));
    public ICommand ClearClassesCommand => new CommandBase(x => ClearClasses());

    // union of the two above and Classes
    public ClassModelObservableCollection Classes { get; } = new ();

    private void AddClass()
    {
        CanChangeClassInterval = false;
        CanChangeLowestValue = false;

        var tmp = Classes.LastOrDefault()?.Class.UpperLimit;
        decimal lower;
        if (tmp != null)
            lower = tmp.Value + 1; // if not null, next lower limit is 1 plus the previous upper limit
        else
            lower = _lowestValue; // else, set as lowest value

        Classes.Add(new ClassModel
        {
            Class = new Class
            {
                UpperLimit = lower + _classInterval - 1, // upper limit is lower + interval - 1
                LowerLimit = lower
            },
            CumulativeFrequencyGreater = Classes.LastOrDefault()?.CumulativeFrequencyGreater + Classes.LastOrDefault()?.Class.Frequency ?? 0,
            CumulativeFrequencyLess = Classes.LastOrDefault()?.CumulativeFrequencyLess - Classes.LastOrDefault()?.Class.Frequency ?? TotalFrequency
        });
    }

    private void RemoveClass(int index)
    {
        if (index < 0)
            return;

        Classes.RemoveAt(index);

        if (Classes.Count == 0)
        {
            CanChangeClassInterval = true;
            CanChangeLowestValue = true;
        }
    }

    private void ClearClasses()
    {
        Classes.Clear();
        CanChangeClassInterval = true;
        CanChangeLowestValue = true;
        UpdateNonSetProps();
    }
}
