using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkflowPocApp.Models;
using WorkflowPocEngine;
using WorkflowPocEngine.Actions;
using WorkflowPocEngine.States;

namespace WorkflowPocApp
{
    class Program
    {
        const string workflow = "{\"productId\":1,\"versionNo\":1,\"workflowNodes\":[{\"statusName\":\"new\",\"state\":1,\"priority\":\"\",\"validActions\":\"\"},{\"statusName\":\"assigned\",\"state\":1,\"priority\":\"\",\"validActions\":\"\"},{\"statusName\":\"processing\",\"state\":2,\"priority\":\"\",\"validActions\":\"\"},{\"statusName\":\"waiting\",\"state\":4,\"priority\":\"\",\"validActions\":\"\"},{\"statusName\":\"compiling report\",\"state\":2,\"priority\":\"\",\"validActions\":\"\"},{\"statusName\":\"done\",\"state\":3,\"priority\":\"\",\"validActions\":\"\"}]}";

        static void Main(string[] args)
        {
            var model = new GenericExampleModel();
            InitModel(model);

            Console.WriteLine("initiating sequence:");
            Console.WriteLine($"current status: {model.StatusDisplay}");
            Console.ReadLine();

            while(model.StatusDisplay != "done")
            {
                ProcessModel(model);
                Console.ReadLine();
            }

            Console.WriteLine("sequence completed.");
            Console.ReadLine();
        }

        static void ProcessModel(GenericExampleModel model)
        {
            var nodes = DeserializeNodes();

            var currentNode = JsonConvert.DeserializeObject<JToken>(model.Status);

            var matchingCurrent = nodes.Find(n => JToken.DeepEquals(currentNode, n));

            var indexOfCurrent = nodes.IndexOf(matchingCurrent);

            var nextNode = nodes[indexOfCurrent + 1];

            if (JToken.DeepEquals(nextNode["state"], currentNode["state"]))
            {
                model.Status = nextNode.ToString();
                model.StatusDisplay = nextNode["statusName"].ToString();
            }
            else
            {
                var ready = EvaluateWorkflowIsReadyForTransition(model);
                if (ready)
                {
                    var success = PopNextWorkflow((WorkflowState)currentNode["state"].ToObject<int>());

                    if (success)
                    {
                        model.Status = nextNode.ToString();
                        model.StatusDisplay = nextNode["statusName"].ToString();
                    }
                }
            }

            Console.WriteLine($"current status: {model.StatusDisplay}");
        }

        static void InitModel(GenericExampleModel model)
        {
            model.Id = 1;
            model.Tasks = new List<GenericExampleTaskModel>();
            InitTasks(model.Tasks);

            // intitialize status for model
            var workflowNodes = DeserializeNodes();
            if (workflowNodes.Any())
            {
                var initialStatus = workflowNodes.First();
                model.StatusDisplay = initialStatus["statusName"].ToString();
                // storing the whole workflow node as the status
                model.Status = initialStatus.ToString();
            }
        }

        static void InitTasks(List<GenericExampleTaskModel> taskList)
        {
            taskList.Add(new GenericExampleTaskModel
            {
                Name = "Finalize Invoice",
                Completed = true,
                Variety = GenericExampleTaskVariety.Administrative
            });

            taskList.Add(new GenericExampleTaskModel
            {
                Name = "Send Request",
                Completed = true,
                Variety = GenericExampleTaskVariety.Communicative
            });

            taskList.Add(new GenericExampleTaskModel
            {
                Name = "Notify Customer",
                Completed = true,
                Variety = GenericExampleTaskVariety.Communicative
            });

            taskList.Add(new GenericExampleTaskModel
            {
                Name = "Validate Information",
                Completed = true,
                Variety = GenericExampleTaskVariety.Preliminary
            });

            taskList.Add(new GenericExampleTaskModel
            {
                Name = "Set ETA",
                Completed = true,
                Variety = GenericExampleTaskVariety.Preliminary
            });
        }

        static List<JToken> DeserializeNodes()
        {
            var nodes = new List<JToken>();

            var workflowObj = JsonConvert.DeserializeObject<JObject>(workflow);

            if (workflowObj["workflowNodes"] != null)
            {
                foreach (var node in workflowObj["workflowNodes"])
                {
                    nodes.Add(node);
                }
            }

            return nodes;
        }

        static bool PopNextWorkflow(WorkflowState state)
        {
            var success = false;
            switch (state)
            {
                case WorkflowState.ToDo:
                    BaseWorkflowService.ProcessCommenceWorkAction(state);
                    success = true;
                    break;
                case WorkflowState.Doing:
                    BaseWorkflowService.ProcessPauseWorkAction(state);
                    success = true;
                    break;
                case WorkflowState.Waiting:
                    BaseWorkflowService.ProcessEndWorkAction(state);
                    success = true;
                    break;
                case WorkflowState.Done:
                    break;
            }
            return success;
        }

        static bool EvaluateWorkflowIsReadyForTransition(GenericExampleModel model)
        {
            var readyForTransition = false;

            var status = JsonConvert.DeserializeObject<JToken>(model.Status);

            var state = (WorkflowState)status["state"].ToObject<int>();
            switch (state)
            {
                case WorkflowState.ToDo:
                    // all "preliminary" tasks must be completed before being considered "in progress"
                    readyForTransition = model.Tasks.Where(t => t.Variety == GenericExampleTaskVariety.Preliminary).All(t => t.Completed);
                    break;
                case WorkflowState.Doing:
                    // all "communicative" tasks must be completed
                    readyForTransition = model.Tasks.Where(t => t.Variety == GenericExampleTaskVariety.Communicative).All(t => t.Completed);
                    break;
                case WorkflowState.Waiting:
                    // all "administrative" tasks must be completed
                    readyForTransition = model.Tasks.Where(t => t.Variety == GenericExampleTaskVariety.Administrative).All(t => t.Completed);
                    break;
            }

            return readyForTransition;

        }
    }
}
