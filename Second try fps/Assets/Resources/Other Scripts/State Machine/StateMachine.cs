using System.Collections.Generic;
using System;

public class StateMachine 
{
    private IState _currentState;

    private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();
    private List<Transition> _currentTransitions = new List<Transition>();
    private List<Transition> _anyTransitions = new List<Transition>();

    private static List<Transition> EmptyTransitions = new List<Transition>(0);

    public StateMachine() { }

    private class Transition
    {
        public Func<bool> ConditionMet { get; }
        public IState To { get; }

        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            ConditionMet = condition;
        }
    }

    public void Tick()
    {
        var transition = CheckForTransition();

        if(transition != null)
        {
            SetNewState(transition.To);
        }

        _currentState?.Tick();
    }

    private Transition CheckForTransition()
    {
        foreach (var transition in _anyTransitions)
            if (transition.ConditionMet())
                return transition;

        foreach (var transition in _currentTransitions)
            if (transition.ConditionMet())
                return transition;

        return null;
    }

    private void SetNewState(IState state)
    {
        if (state == _currentState)
            return;

        _currentState?.OnExit();
        _currentState = state;

        _transitions.TryGetValue(_currentState.GetType(), out _currentTransitions);
        if (_currentTransitions == null)
            _currentTransitions = EmptyTransitions;

        _currentState.OnEnter();
    }

    public void AddTransition(IState from, IState to, Func<bool> predicate)
    {
        List<Transition> transitions;

        if (_transitions.TryGetValue(from.GetType(), out transitions) == false)
        {
            transitions = new List<Transition>();
            _transitions[from.GetType()] = transitions;
        }

        transitions.Add(new Transition(to, predicate));
    }

    public void AddAnyTransition(IState to, Func<bool> predicate)
    {
        _anyTransitions.Add(new Transition(to, predicate));
    }
}
