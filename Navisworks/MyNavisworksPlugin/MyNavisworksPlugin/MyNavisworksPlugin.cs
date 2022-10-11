using Autodesk.Navisworks.Api;
using bimU.io.Client.Core.Messages;
using bimU.io.Client.Core.Plugin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNavisworksPlugin
{
    public class MyNavisworksPlugin : IPlugin
    {
        static List<AuthoringToolClient> supportedClients = new List<AuthoringToolClient>(){ AuthoringToolClient.Navisworks2022 };

        public IEnumerable<AuthoringToolClient> getSupportedClients()
        {
            return supportedClients;
        }

        public string importIssues(object[] context, string configs)
        {
            var issues = new List<CustomIssue>();

            // TODO: Add more issues on your own with required issue fields

            return JsonConvert.SerializeObject(issues);
        }

        public string onIssueCreating(object[] context, string configs)
        {
            return createCustomIssueFields(context);
        }

        public string onIssueUpdating(object[] context, string configs)
        {
            return createCustomIssueFields(context);
        }

        private string createCustomIssueFields(object[] context) 
        {
            Document doc = (Document)context[0];
            CustomIssue issue = new CustomIssue()
            {
                // Populate custom fields only
                filename = doc.FileName,
                customField1 = doc.CurrentSelection.SelectedItems.Count,
                customField2 = doc.Models.Count
            };
            return issue.ToString();
        }
    }

    public class CustomIssue 
    {
        public string title { get; set; } // Required for bulk issue importer
        public string description { get; set; } // Required for bulk issue importer
        public string filename { get; set; }
        public int customField1 { get; set; }
        public int customField2 { get; set; }

        public override string ToString() 
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
