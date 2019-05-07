using Stateless;
using System;
using WorkflowPocEngine.Actions;
using WorkflowPocEngine.States;

namespace WorkflowPocEngine
{
    public class BaseWorkflowService
    {
        static BaseWorkflowService()
        {

        }

        // builds a state machine instanced at the current workflow state
        private static StateMachine<WorkflowState, WorkflowAction> BuildMachine(WorkflowState state)
        {
            
            var machine = new StateMachine<WorkflowState, WorkflowAction>(
                () => state,
                s => state = s
                );

            machine.OnTransitioned(t => LogTransition(t));

            machine.Configure(WorkflowState.ToDo)
                .Permit(WorkflowAction.Start, WorkflowState.Doing)
                .Permit(WorkflowAction.Pause, WorkflowState.Waiting)
                .Permit(WorkflowAction.Cancel, WorkflowState.Done);

            machine.Configure(WorkflowState.Doing)
                .Permit(WorkflowAction.End, WorkflowState.Done)
                .Permit(WorkflowAction.Pause, WorkflowState.Waiting)
                .Permit(WorkflowAction.Cancel, WorkflowState.Done);

            machine.Configure(WorkflowState.Waiting)
                .Permit(WorkflowAction.Start, WorkflowState.Doing)
                .Permit(WorkflowAction.End, WorkflowState.Done)
                .Permit(WorkflowAction.Cancel, WorkflowState.Done);

            machine.Configure(WorkflowState.Done)
                .Permit(WorkflowAction.Start, WorkflowState.Doing);

            return machine;
        }

        public static void ProcessCommenceWorkAction(WorkflowState state)
        {
            ProcessAction(state, WorkflowAction.Start);
        }

        public static void ProcessPauseWorkAction(WorkflowState state)
        {
            ProcessAction(state, WorkflowAction.Pause);
        }

        public static void ProcessEndWorkAction(WorkflowState state)
        {
            ProcessAction(state, WorkflowAction.End);
        }

        public static void ProcessCancellationAction(WorkflowState state)
        {
            ProcessAction(state, WorkflowAction.Cancel);
        }

        private static void ProcessAction(WorkflowState currentState, WorkflowAction action)
        {
            var machine = BuildMachine(currentState);
            machine.Fire(action);
        }

        private static void LogTransition(StateMachine<WorkflowState, WorkflowAction>.Transition transition)
        {
            Console.WriteLine($"Workflow transitioned from {transition.Source} => {transition.Destination} via {transition.Trigger}.");
        }

    }
}
