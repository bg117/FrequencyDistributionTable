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
        Classes.ClassChanged += (a, b) => UpdateNonSetProps();
        Classes.CollectionChanged += (a, b) => UpdateNonSetProps();
    }

    private void UpdateNonSetProps()
    {
        RaisePropertyChangedEvent(nameof(TotalFrequency));
        RaisePropertyChangedEvent(nameof(Mean));
        RaisePropertyChangedEvent(nameof(Median));
        RaisePropertyChangedEvent(nameof(Mode));
        RaisePropertyChangedEvent(nameof(Range));
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

    public decimal Median
    {
        get
        {
            if (Classes.Count < 2)
                return 0;

            var halfOfTotalFreq = TotalFrequency / 2m; // half of total frequency
            var medianClass = Classes.FirstOrDefault(x => x.CumulativeFrequencyLess >= halfOfTotalFreq); // first class where cumulative frequency is greater than half of total frequency
            var belowMedianClassIndex = Classes.IndexOf(medianClass) - 1; // class below median class
            
            // if no class below median class, 0
            var belowLessCumulFreq = Classes.ElementAtOrDefault(belowMedianClassIndex)?.CumulativeFrequencyLess ?? 0; // cumulative frequency of class below median class
            var medianClassFreq = medianClass.Class.Frequency; // frequency of median class
            var medianClassLb = medianClass.Class.LowerBoundary; // lower bound of median class

            if (medianClassFreq == 0)
                return 0;

            // medianClassLb + ((halfOfTotalFreq - belowLessCumulFreq) / medianClassFreq) * class interval
            return medianClassLb + ((halfOfTotalFreq - belowLessCumulFreq) / medianClassFreq) * ClassInterval;
        }
    }

    public decimal Mode
    {
        get
        {
            if (Classes.Count < 2)
                return 0;

            var modalClass = Classes.OrderByDescending(x => x.Class.Frequency).First(); // class with highest frequency
            var belowModalClassIndex = Classes.IndexOf(modalClass) - 1;
            var aboveModalClassIndex = Classes.IndexOf(modalClass) + 1;

            // D1 (modal class frequency - frequency of class below modal class)
            var freqMinusBelow = modalClass.Class.Frequency -
                                 (Classes.ElementAtOrDefault(belowModalClassIndex)?.Class.Frequency ?? 0m); // if no class below modal class, self

            // D2 (modal class frequency - frequency of class above modal class)
            var freqMinusAbove = modalClass.Class.Frequency -
                                 (Classes.ElementAtOrDefault(aboveModalClassIndex)?.Class.Frequency ?? 0m); // if no class above modal class, self

            var modalClassLb = modalClass.Class.LowerBoundary;
            
            if (freqMinusBelow + freqMinusAbove == 0)
                return 0;
            
            return modalClassLb + (freqMinusBelow / (freqMinusBelow + freqMinusAbove)) * ClassInterval; // modal class lower bound + (D1 / (D1 + D2)) * class interval
        }
    }

    public decimal Range
    {
        get
        {
            if (Classes.Count < 2)
                return 0;

            var firstClass = Classes.First();
            var lastClass = Classes.Last();

            return lastClass.Class.UpperBoundary - firstClass.Class.LowerBoundary;
        }
    }

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
