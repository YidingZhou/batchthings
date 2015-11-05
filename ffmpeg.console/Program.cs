using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Batch namespace
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.Azure.Batch.Common;

namespace ffmpeg.console
{
    class Program
    {
        const string endpoint = "https://yidingadf.westeurope.batch.azure.com";
        const string account = "yidingadf";
        const string key = "t8RiYvO4yjxHtJBSCGYKzW6MzHBiUgEU+Qaxr2wLS9Hud4Hk5pbF/3RBjSiNTFubjSEDURaNdx/abDenrS80kw==";

        const string poolname = "demo";

        const string inputContainer = "https://mvpdemo15.blob.core.windows.net/input";
        const string outputContainer = "https://mvpdemo15.blob.core.windows.net/output";
        const string resourceContainer = "https://mvpdemo15.blob.core.windows.net/resource";

        static void Main(string[] args)
        {
            BatchSharedKeyCredentials cred = new BatchSharedKeyCredentials(endpoint, account, key);
            using (BatchClient client = BatchClient.Open(cred)) // <-- connect to the cluster
            {
                List<ResourceFile> resources = new List<ResourceFile>();
                foreach (string blob in StorageHelper.ListBlobs(resourceContainer))
                {
                    string filename = System.IO.Path.GetFileName((new Uri(blob)).LocalPath);
                    resources.Add(new ResourceFile(StorageHelper.GetBlobSASURL(blob), filename));
                }

                CloudPool pool = client.PoolOperations.CreatePool(poolname, "4", "medium", 10);
                pool.StartTask = new StartTask();
                pool.StartTask.ResourceFiles = resources;
                pool.StartTask.CommandLine = @"cmd /c copy *.* %WATASK_TVM_ROOT_DIR%\shared\";
                pool.StartTask.WaitForSuccess = true;
                //pool.Commit(); // <-- Create demo pool

                // Submit Job
                string jobname = "MVP_" + Environment.GetEnvironmentVariable("USERNAME") + "_" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                PoolInformation poolinfo = new PoolInformation();
                poolinfo.PoolId = poolname;

                CloudJob job = client.JobOperations.CreateJob(jobname, poolinfo); // <-- create a job that runs on demo pool

                Console.WriteLine("Creating job..." + jobname);
                job.Commit();
                job = client.JobOperations.GetJob(jobname);

                string inputcontainersas = StorageHelper.GetContainerSAS(inputContainer);
                string outputcontainersas = StorageHelper.GetContainerSAS(outputContainer);
                List<CloudTask> tasks = new List<CloudTask>();
                Console.WriteLine("Analyzing blobs...");
                foreach (string blob in StorageHelper.ListBlobs(inputContainer)) // <-- Going through blobs
                {
                    string filename = System.IO.Path.GetFileName((new Uri(blob)).LocalPath);
                    string taskname = "task_" + System.IO.Path.GetFileNameWithoutExtension(filename);

                    // prepare the command line
                    string cli;
                    cli = ". robocopy.exe ${env:WATASK_TVM_ROOT_DIR}\\shared\\ . *.*;";
                    cli += "ffmpeg.exe -i {0} -vf 'movie=microsoft.png [watermark]; [in][watermark] overlay=10:main_h-overlay_h-10 [out]' {0}.output.avi;".Replace("{0}", filename);
                    cli += "azcopy.exe . {0} *.output.avi /destsas:'{1}' /y".Replace("{0}", outputContainer).Replace("{1}", outputcontainersas);

                    cli = string.Format("powershell -command \"{0}\"", cli);

                    // prepare task object
                    CloudTask task = new CloudTask(taskname, cli);
                    task.ResourceFiles = new List<ResourceFile>();
                    task.ResourceFiles.Add(new ResourceFile(blob + inputcontainersas, filename));

                    tasks.Add(task); // <-- prepare 1 task for 1 blob
                }

                Console.WriteLine("Submit tasks...");
                client.JobOperations.AddTask(jobname, tasks); // <-- Submit all 100 tasks with 1 API call

                Console.WriteLine("Waiting for tasks to finish...");
                client.Utilities.CreateTaskStateMonitor().WaitAll(client.JobOperations.ListTasks(jobname), TaskState.Completed, new TimeSpan(0, 60, 0));

                Console.WriteLine("Closing job...");
                client.JobOperations.TerminateJob(jobname);

                Console.WriteLine("All done. Press Enter to exit.");
                Console.ReadLine();
            }
        }
    }
}
