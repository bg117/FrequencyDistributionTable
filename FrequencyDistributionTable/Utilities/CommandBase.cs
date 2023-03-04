using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FrequencyDistributionTable.Utilities;

public class CommandBase : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Predicate<object?> _predicate = (o) => true;

    public CommandBase(Action<object?> execute)
    {
        _execute = execute;
    }

    public CommandBase(Action<object?> execute, Predicate<object?> predicate)
    {
        _execute = execute;
        _predicate = predicate;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter)
    {
        return _predicate(parameter);
    }

    public void Execute(object? parameter)
    {
        _execute(parameter);
    }
}
