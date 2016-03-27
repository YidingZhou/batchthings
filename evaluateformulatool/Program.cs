using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.Azure.Batch.Common;

namespace evaluateformulatool
{
    class Program
    {
        const string account = "";
        const string url = "";
        const string key = "";

        static void Main(string[] args)
        {
            BatchSharedKeyCredentials cred = new BatchSharedKeyCredentials(url, account, key);
            string now = DateTime.UtcNow.ToString("r");
            using (BatchClient client = BatchClient.Open(cred))
            {
                //client.PoolOperations.EnableAutoScale("demo", "2");
                string formula = string.Format(@"
$TargetDedicated={1};
lifespan=time()-time(""{0}"");
span=TimeInterval_Minute * 60;
startup=TimeInterval_Minute * 10;
ratio=50;
$TargetDedicated=(lifespan>startup?(max($RunningTasks.GetSample(span, ratio), $ActiveTasks.GetSample(span, ratio)) == 0 ? 0 : $TargetDedicated):{1});
", now, 4);
                try {
                    CloudPool p = client.PoolOperations.CreatePool("formulasample", "4", "small");
                    p.AutoScaleEnabled = true;
                    p.AutoScaleFormula = formula;
                    p.Commit();
                } catch(Exception ex)
                {
                    // Go through all exceptions and dump useful information
                    (ex as AggregateException).Handle((x) =>
                    {
                        if (x is BatchException)
                        {
                            BatchException be = x as BatchException;

                            if (null != be.RequestInformation && null != be.RequestInformation.AzureError)
                            {
                                // Write the server side error information
                                Console.Error.WriteLine(be.RequestInformation.AzureError.Code);
                                Console.Error.WriteLine(be.RequestInformation.AzureError.Message.Value);

                                if (null != be.RequestInformation.AzureError.Values)
                                {
                                    foreach (var v in be.RequestInformation.AzureError.Values)
                                    {
                                        Console.Error.WriteLine(v.Key + " : " + v.Value);
                                    }
                                }
                            }
                        }

                        return false;
                    });
                }
                //var result = client.PoolOperations.EvaluateAutoScale("demo", formula);
                //if(result.AutoScaleRun.Error != null)
                //{
                //    Console.WriteLine(result.AutoScaleRun.Error.Code + " : " + result.AutoScaleRun.Error.Message);
                //    foreach(var e in result.AutoScaleRun.Error.Values)
                //    {
                //        Console.WriteLine(" " + e.Name + " : " + e.Value);
                //    }
                //}
                //Console.WriteLine(result.AutoScaleRun.Results);
                //Console.ReadLine();
            }
        }
    }
}
