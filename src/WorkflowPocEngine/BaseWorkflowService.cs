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

        public void LogTransition(StateMachine<WorkflowStatus, WorkflowAction>.Transition transition)
        {
            Console.WriteLine($"Workflow transitioned from {transition.Source} => {transition.Destination} via {transition.Trigger}.");
        }

    }
}
