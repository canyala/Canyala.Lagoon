//-------------------------------------------------------------------------------
//
//  MIT License
//
//  Copyright (c) 2012-2022 Canyala Innovation
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
//
//-------------------------------------------------------------------------------

using System.Windows.Input;

namespace Canyala.Lagoon.Command;

/// <summary>
/// This implementation intentionally hides the argument sent by the Execute(object arg) interface member.
/// </summary>
public class DelegateCommand : ICommand
{
    /// <summary>
    /// Occurs when changes occur that affect whether the command should execute.
    /// </summary>
    public event EventHandler? CanExecuteChanged;

    readonly Func<object?, bool> canExecute;        
    readonly Action executeAction;
    bool canExecuteCache;

    public DelegateCommand(Action executeAction) : this(executeAction, x => true)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
    /// </summary>
    /// <param name="executeAction">The execute action.</param>
    /// <param name="canExecute">The can execute.</param>
    public DelegateCommand(Action executeAction, 
                           Func<object?, bool> canExecute)
    {
        this.executeAction = executeAction;
        this.canExecute = canExecute;
    }

    public void RaiseCanExecute()
    {
        CanExecuteChanged?.Invoke(this, new EventArgs());
    }

    #region ICommand Members
    /// <summary>
    /// Defines the method that determines whether the command 
    /// can execute in its current state.
    /// </summary>
    /// <param name="parameter">
    /// Data used by the command. 
    /// If the command does not require data to be passed,
    /// this object can be set to null.
    /// </param>
    /// <returns>
    /// true if this command can be executed; otherwise, false.
    /// </returns>
    public bool CanExecute(object? parameter)
    {
        bool tempCanExecute = canExecute(parameter);

        if (canExecuteCache != tempCanExecute)
        {
            canExecuteCache = tempCanExecute;
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        return canExecuteCache;
    }

    /// <summary>
    /// Defines the method to be called when the command is invoked.
    /// </summary>
    /// <param name="parameter">
    /// Data used by the command. 
    /// If the command does not require data to be passed, 
    /// this object can be set to null.
    /// </param>
    public void Execute(object? parameter)
    {
        executeAction();
    }
    #endregion
}

