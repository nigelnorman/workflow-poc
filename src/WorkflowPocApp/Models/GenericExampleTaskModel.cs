using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowPocApp.Models
{
    public class GenericExampleTaskModel
    { 
        public string Name { get; set; }

        public bool Completed { get; set; }

        public GenericExampleTaskVariety Variety { get; set; }
    }
}
