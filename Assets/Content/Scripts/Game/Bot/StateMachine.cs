using System;
using System.Collections.Generic;
using System.Linq;

namespace Content.Scripts.Game.Bot
{
    public abstract class TypeState<T> where T : Enum
    {
        private T stateType;
        private bool isFinish;
        private StateMachine<T> stateMachine;

        public bool IsFinish => isFinish;

        public T StateType => stateType;

        public TypeState(T type)
        {
            stateType = type;
        }
        
        public virtual void Init(StateMachine<T> stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public virtual void Start(){}
        public virtual void Run(){}        
        public virtual void End(){}

        public void FinishState(bool state = true) => isFinish = state;

        public virtual void Gizmo()
        {
        }
    }
    
    public class StateMachine<T> where T : Enum
    {
        private List<TypeState<T>> states;
        private TypeState<T> currentState;

        public event Action<T> OnStateFinished;

         public T ActiveState => currentState.StateType;

        protected void StatesInit(T startState, params TypeState<T>[] statesArray)
        {
            states = statesArray.ToList();
            states.ForEach(x => x.Init(this));
            StateSet(startState);
        }

        protected void StateSet(T stateType)
        {
            if (currentState != null)
            {
                currentState.End();
            }

            currentState = states.Find(x => x.StateType.Equals(stateType));
            if (currentState != null)
            {
                currentState.FinishState(false);
                currentState.Start();
            }
        }

        protected void StateRun()
        {
            if (currentState != null)
            {
                if (!currentState.IsFinish)
                {
                    currentState.Run();
                }
                else
                {
                    OnStateFinished?.Invoke(currentState.StateType);
                }
            }
        }

        public virtual void Gizmo()
        {
            currentState?.Gizmo();
        }
    }
}