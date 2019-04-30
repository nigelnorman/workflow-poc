using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using WorkflowPocEngine;
using WorkflowPocEngine.Actions;
using WorkflowPocEngine.Statuses;

namespace WorkflowPocApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var workflow = "{\"productId\":1,\"versionNo\":1,\"workflowNodes\":[{\"statusName\":\"new\",\"state\":1,\"priority\":\"\",\"validActions\":\"\"},{\"statusName\":\"assigned\",\"state\":1,\"priority\":\"\",\"validActions\":\"\"},{\"statusName\":\"processing\",\"state\":2,\"priority\":\"\",\"validActions\":\"\"},{\"statusName\":\"waiting\",\"state\":4,\"priority\":\"\",\"validActions\":\"\"},{\"statusName\":\"compiling report\",\"state\":2,\"priority\":\"\",\"validActions\":\"\"},{\"statusName\":\"done\",\"state\":3,\"priority\":\"\",\"validActions\":\"\"}]}";

            var workflowObj = JsonConvert.DeserializeObject<JObject>(workflow);

            if (workflowObj["workflowNodes"] != null)
            {
                foreach (var node in workflowObj["workflowNodes"])
                {
                    Console.WriteLine(node["statusName"]);

                    if (node["state"] != null)
                    {
                        var state = (WorkflowStatus)node["state"].ToObject<int>();
                        switch (state)
                        {
                            case WorkflowStatus.ToDo:
                                BaseWorkflowService.ProcessCommenceWorkAction(state);
                                break;
                            case WorkflowStatus.Doing:
                                BaseWorkflowService.ProcessPauseWorkAction(state);
                                break;
                            case WorkflowStatus.Waiting:
                                BaseWorkflowService.ProcessEndWorkAction(state);
                                break;
                            case WorkflowStatus.Done:
                                break;
                        }
                    }
                }

                Console.WriteLine("sequence completed.");
                Console.ReadLine();
            }
        }
    }
}
