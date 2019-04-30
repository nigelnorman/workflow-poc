using Stateless;
using System;
using WorkflowPocEngine.Actions;
using WorkflowPocEngine.Statuses;

namespace WorkflowPocEngine
{
    public class BaseWorkflowService
    {
        static BaseWorkflowService()
        {

        }

        // builds a state machine instanced at the current workflow state
        public static StateMachine<WorkflowStatus, WorkflowAction> BuildMachine(WorkflowStatus status)
        {
            
            var machine = new StateMachine<WorkflowStatus, WorkflowAction>(
                () => status,
                s => status = s
                );

            machine.OnTransitioned(t => LogTransition(t));

            machine.Configure(WorkflowStatus.ToDo)
                .Permit(WorkflowAction.Start, WorkflowStatus.Doing)
                .Permit(WorkflowAction.Pause, WorkflowStatus.Waiting)
                .Permit(WorkflowAction.Cancel, WorkflowStatus.Done);

            machine.Configure(WorkflowStatus.Doing)
                .Permit(WorkflowAction.End, WorkflowStatus.Done)
                .Permit(WorkflowAction.Pause, WorkflowStatus.Waiting)
                .Permit(WorkflowAction.Cancel, WorkflowStatus.Done);

            machine.Configure(WorkflowStatus.Waiting)
                .Permit(WorkflowAction.Start, WorkflowStatus.Doing)
                .Permit(WorkflowAction.End, WorkflowStatus.Done)
                .Permit(WorkflowAction.Cancel, WorkflowStatus.Done);

            machine.Configure(WorkflowStatus.Done)
                .Permit(WorkflowAction.Start, WorkflowStatus.Doing);

            return machine;
        }

        public static void ProcessCommenceWorkAction(WorkflowStatus status)
        {
            ProcessAction(status, WorkflowAction.Start);
        }

        public static void ProcessPauseWorkAction(WorkflowStatus status)
        {
            ProcessAction(status, WorkflowAction.Pause);
        }

        public static void ProcessEndWorkAction(WorkflowStatus status)
        {
            ProcessAction(status, WorkflowAction.End);
        }

        public static void ProcessCancellationAction(WorkflowStatus status)
        {
            ProcessAction(status, WorkflowAction.Cancel);
        }

        private static void ProcessAction(WorkflowStatus currentStatus, WorkflowAction action)
        {
            var machine = BuildMachine(currentStatus);
            machine.Fire(action);
        }

        private static void LogTransition(StateMachine<WorkflowStatus, WorkflowAction>.Transition transition)
        {
            Console.WriteLine($"Workflow transitioned from {transition.Source} => {transition.Destination} via {transition.Trigger}.");
        }

    }
}
