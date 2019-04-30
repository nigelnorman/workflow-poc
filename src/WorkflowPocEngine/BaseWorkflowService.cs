using Stateless;
using System;
using WorkflowPocEngine.Actions;
using WorkflowPocEngine.Statuses;

namespace WorkflowPocEngine
{
    public class BaseWorkflowService<TStatus, TAction>
    {
        static BaseWorkflowService()
        {

        }

        // builds a state machine instanced at the current workflow state
        public StateMachine<WorkflowStatus, WorkflowAction> BuildMachine(WorkflowStatus status)
        {
            
            var machine = new StateMachine<WorkflowStatus, WorkflowAction>(
                () => status,
                s => status = s
                );

            machine.OnTransitioned(t => this.LogTransition(t));

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

        public void ProcessCommenceWorkAction(WorkflowStatus status)
        {
            this.ProcessAction(status, WorkflowAction.Start);
        }

        public void ProcessPauseWorkAction(WorkflowStatus status)
        {
            this.ProcessAction(status, WorkflowAction.Pause);
        }

        public void ProcessEndWorkAction(WorkflowStatus status)
        {
            this.ProcessAction(status, WorkflowAction.End);
        }

        public void ProcessCancellationAction(WorkflowStatus status)
        {
            this.ProcessAction(status, WorkflowAction.Cancel);
        }

        private void ProcessAction(WorkflowStatus currentStatus, WorkflowAction action)
        {
            var machine = this.BuildMachine(currentStatus);
            machine.Fire(action);
        }

        private void LogTransition(StateMachine<WorkflowStatus, WorkflowAction>.Transition transition)
        {
            Console.WriteLine($"Workflow transitioned from {transition.Source} => {transition.Destination} via {transition.Trigger}.");
        }

    }
}
