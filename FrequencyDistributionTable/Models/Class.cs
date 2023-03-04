using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FrequencyDistributionTable.Utilities;

namespace FrequencyDistributionTable.Models;

public class Class : ObservableObject
{
    private int _frequency;
    private decimal _lowerLimit;
    private decimal _upperLimit;

    public int Frequency
    {
        get => _frequency;
        set
        {
            SetProperty(ref _frequency, value);
            UpdateComputedProperties();   
        }
    }
    public decimal LowerLimit
    {
        get => _lowerLimit;
        set
        {
            SetProperty(ref _lowerLimit, value);
            UpdateComputedProperties();
        }
    }
    public decimal UpperLimit
    {
        get => _upperLimit;
        set
        {
            SetProperty(ref _upperLimit, value);
            UpdateComputedProperties();
        }
    }

    public decimal LowerBoundary => LowerLimit - 0.5m;
    public decimal UpperBoundary => UpperLimit + 0.5m;
    public decimal Midpoint => (LowerLimit + UpperLimit) / 2;
    public decimal FrequencyTimesMidpoint => Frequency * Midpoint;

    private void UpdateComputedProperties()
    {
        RaisePropertyChangedEvent(nameof(LowerBoundary));
        RaisePropertyChangedEvent(nameof(UpperBoundary));
        RaisePropertyChangedEvent(nameof(Midpoint));
        RaisePropertyChangedEvent(nameof(FrequencyTimesMidpoint));
    }
}

public class ClassModel : ObservableObject
{
    private Class _class;
    private int _cumulativeFrequencyGreater;
    private int _cumulativeFrequencyLess;

    public Class Class
    {
        get => _class;
        set => SetProperty(ref _class, value);
    }

    public int CumulativeFrequencyGreater
    {
        get => _cumulativeFrequencyGreater;
        set => SetProperty(ref _cumulativeFrequencyGreater, value);
    }

    public int CumulativeFrequencyLess
    {
        get => _cumulativeFrequencyLess;
        set => SetProperty(ref _cumulativeFrequencyLess, value);
    }
}
