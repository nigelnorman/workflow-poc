using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowPocEngine.States
{
    public enum WorkflowState
    {
        None = 0,
        ToDo = 1,
        Doing = 2,
        Done = 3,
        Waiting = 4,
    }
}
