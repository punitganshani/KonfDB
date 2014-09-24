#region License and Product Information

// 
//     This file 'StateWorkflow.cs' is part of KonfDB application - 
//     a project perceived and developed by Punit Ganshani.
// 
//     KonfDB is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     KonfDB is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with KonfDB.  If not, see <http://www.gnu.org/licenses/>.
// 
//     You can also view the documentation and progress of this project 'KonfDB'
//     on the project website, <http://www.konfdb.com> or on 
//     <http://www.ganshani.com/applications/konfdb>

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using KonfDB.Infrastructure.Exceptions;

namespace KonfDB.Infrastructure.Workflow
{
    public class StateWorkflow<TStates> where TStates : struct, IConvertible
    {
        private readonly Dictionary<TStates, Lazy<IStateAction<TStates>>> _stateDictionary;
        private readonly StateWorkflowConfig<TStates> _configuration;

        public Action<State<TStates>> OnStateChanged;
        public Action<TStates, Dictionary<string, object>, object[]> OnInvalidState;
        public Action<Dictionary<string, object>, object[]> OnWorkflowEnded;

        public StateWorkflow(StateWorkflowConfig<TStates> configuration)
        {
            _configuration = configuration;
            _stateDictionary = new Dictionary<TStates, Lazy<IStateAction<TStates>>>();
        }

        public void Add<T>(TStates state) where T : IStateAction<TStates>
        {
            _stateDictionary.Add(state,
                new Lazy<IStateAction<TStates>>(() => (IStateAction<TStates>) Activator.CreateInstance<T>()));
        }

        public void Add<T>(TStates state, Func<IStateAction<TStates>> createAction) where T : IStateAction<TStates>
        {
            _stateDictionary.Add(state, new Lazy<IStateAction<TStates>>(createAction));
        }

        public State<TStates> ExecuteState(TStates currentState, Dictionary<string, object> dataDictionary,
            params object[] data)
        {
            if (currentState.Equals(_configuration.EndState))
            {
                if (OnWorkflowEnded != null)
                    OnWorkflowEnded(dataDictionary, data);

                return new State<TStates> {Success = true, Previous = currentState, IsEndState = true};
            }
            if (!_stateDictionary.ContainsKey(currentState))
            {
                if (OnInvalidState != null)
                    OnInvalidState(currentState, dataDictionary, data);

                if (_configuration.ThrowErrorOnInvalidState)
                    throw new InvalidOperationException("State does not exist in StateDictionary: " + currentState);
            }
            Debug.WriteLine("Executing state:" + currentState);

            var state = new State<TStates>
            {
                Previous = currentState,
                DataDictinary = dataDictionary,
                Data = data,
                Path = currentState.ToString()
            };
            var stateAction = _stateDictionary[currentState];
            try
            {
                state.Current = stateAction.Value.Execute(currentState, dataDictionary, data);
                state.Success = true;
            }
            catch (Exception ex)
            {
                if (_configuration.ThrowExceptions)
                    throw new StateWorkflowException("Error executing state: " + state.Current, ex);

                state.Errors.Add(ex);
                state.Current = state.Previous;
                state.Success = false;
            }

            if (OnStateChanged != null)
                OnStateChanged(state);

            // Only if no errors, then do autoexecute
            if (_configuration.AutoExecuteOnStateChanged && state.Errors.Count == 0)
            {
                try
                {
                    var currentForNextState = state.Current;
                    var newState = ExecuteState(currentForNextState, dataDictionary, data);
                    state.Current = newState.Current;
                    state.Path += ">" + newState.Path;
                    state.Errors.AddRange(newState.Errors);
                    state.Success &= newState.Success;
                }
                catch (Exception ex)
                {
                    if (_configuration.ThrowExceptions)
                        throw new StateWorkflowException("Error executing state: " + state.Current, ex);

                    state.Errors.Add(ex);
                    state.Current = state.Previous;
                    state.Success = false;
                }
            }
            return state;
        }
    }
}