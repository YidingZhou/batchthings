using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

// Batch namespace
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.Azure.Batch.Common;


namespace DemoClient
{
    class BatchServiceClient
    {
        static string prefix = "MVP_";
        public static void Submit()
        {
            Log("Start submission process.");
            state = null;

            BatchSharedKeyCredentials cred = new BatchSharedKeyCredentials(Settings.batchEndpoint, Settings.batchAccount, Settings.batchKey);
            using (BatchClient client = BatchClient.Open(cred)) // <-- connect to the cluster
            {
                #region job submission

                string jobname = prefix + Environment.GetEnvironmentVariable("USERNAME") + "_" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                PoolInformation pool = new PoolInformation();
                pool.PoolId = Settings.poolname;

                CloudJob job = client.JobOperations.CreateJob(jobname, pool); // <-- create a workitem that runs on pool "trdemo"

                Log("Submitting...");
                job.Commit();
                jobName = jobname;
                Log(string.Format("Job {0} created.", jobname));

                job = client.JobOperations.GetJob(jobname);

                Log("Analyzing input blobs...");
                string inputcontainersas = StorageHelper.GetContainerSAS(Settings.inputContainer);
                string outputcontainersas = StorageHelper.GetContainerSAS(Settings.outputContainer);
                foreach (string blob in StorageHelper.ListBlobs(Settings.inputContainer))
                {
                    string filename = System.IO.Path.GetFileName((new Uri(blob)).LocalPath);
                    string taskname = "task_" + System.IO.Path.GetFileNameWithoutExtension(filename);

                    // prepare the command line
                    string cli;
                    cli = ". robocopy.exe ${env:WATASK_TVM_ROOT_DIR}\\shared\\ . *.*;";
                    cli += "ffmpeg.exe -i {0} -vf 'movie=microsoft.png [watermark]; [in][watermark] overlay=10:main_h-overlay_h-10 [out]' {0}.output.avi;".Replace("{0}", filename);
                    cli += "azcopy.exe . {0} *.output.avi /destsas:'{1}' /y".Replace("{0}", Settings.outputContainer).Replace("{1}", outputcontainersas);

                    cli = string.Format("powershell -command \"{0}\"", cli);

                    // prepare task object
                    CloudTask task = new CloudTask(taskname, cli);
                    task.ResourceFiles = new List<ResourceFile>();
                    task.ResourceFiles.Add(new ResourceFile(blob + inputcontainersas, filename));

                    job.AddTask(task); // <-- add Task
                }


                #endregion job submission

                ThreadPool.QueueUserWorkItem((x) => { Monitor(); });

                client.Utilities.CreateTaskStateMonitor().WaitAll(client.JobOperations.ListTasks(jobname), TaskState.Completed, new TimeSpan(0, 60, 0));
                client.JobOperations.GetJob(jobname).Terminate();

            }
        }

        public static void Terminate()
        {
            Log("Start Terminating Workitem");

            BatchSharedKeyCredentials cred = new BatchSharedKeyCredentials(Settings.batchEndpoint, Settings.batchAccount, Settings.batchKey);
            using (BatchClient client = BatchClient.Open(cred)) // <-- connect to the cluster
            {
                try
                {
                    client.JobOperations.TerminateJob(jobName);
                }
                catch (Exception) { }
                Log(string.Format("Job {0} terminated.", jobName));
            }
        }

        public static void Delete()
        {
            Log("Start Deleting jobs");

            BatchSharedKeyCredentials cred = new BatchSharedKeyCredentials(Settings.batchEndpoint, Settings.batchAccount, Settings.batchKey);
            using (BatchClient client = BatchClient.Open(cred)) // <-- connect to the cluster
            {
                try
                {
                    foreach (var job in client.JobOperations.ListJobs(new ODATADetailLevel(filterClause: "startswith(id, '" + prefix + "')")))
                    {
                        Log("Delete " + job.Id);
                        client.JobOperations.DeleteJob(job.Id);
                    }
                }
                catch { }
                Log("All WI deleted.");
            }
        }

        public static void ReCreatePool()
        {
            Log("Recreate pool");

            BatchSharedKeyCredentials cred = new BatchSharedKeyCredentials(Settings.batchEndpoint, Settings.batchAccount, Settings.batchKey);
            using (BatchClient client = BatchClient.Open(cred)) // <-- connect to the cluster
            {
                {
                    bool found = false;
                    foreach (var p in client.PoolOperations.ListPools(new ODATADetailLevel(filterClause: "id eq '" + Settings.poolname + "'")))
                    {
                        found = true;
                        break;
                    }

                    if (found)
                    {
                        Log("Deleting current pool...");
                        client.PoolOperations.DeletePool(Settings.poolname);
                        Log("Delete command sent.");

                        while (found)
                        {
                            found = false;
                            Thread.Sleep(1000);
                            Log("Waiting pool to be deleted.");
                            foreach (var p in client.PoolOperations.ListPools(new ODATADetailLevel(filterClause: "id eq '" + Settings.poolname + "'")))
                            {
                                found = true;
                                break;
                            }
                        }
                        Log("Pool deleted.");
                    }

                    #region resource file
                    List<ResourceFile> resources = new List<ResourceFile>();
                    foreach (string blob in StorageHelper.ListBlobs(Settings.resourceContainer))
                    {
                        string filename = System.IO.Path.GetFileName((new Uri(blob)).LocalPath);
                        resources.Add(new ResourceFile(StorageHelper.GetBlobSASURL(blob), filename));
                    }
                    #endregion

                    CloudPool pool = client.PoolOperations.CreatePool(Settings.poolname, "4", "medium", 10);
                    pool.StartTask = new StartTask();
                    pool.StartTask.ResourceFiles = resources;
                    pool.StartTask.CommandLine = @"cmd /c copy *.* %WATASK_TVM_ROOT_DIR%\shared\";
                    pool.StartTask.WaitForSuccess = true;
                    Log("Creating the new pool...");
                    pool.Commit();
                    Log("Pool created.");
                }
            }
        }
        public static void Monitor()
        {
            if (!string.IsNullOrEmpty(jobName))
            {
                BatchSharedKeyCredentials cred = new BatchSharedKeyCredentials(Settings.batchEndpoint, Settings.batchAccount, Settings.batchKey);
                using (BatchClient client = BatchClient.Open(cred)) // <-- connect to the cluster
                {
                    {
                        do
                        {
                            int completed = 0;
                            int all = 0;

                            CloudJob job = client.JobOperations.GetJob(jobName);
                            if(state != job.State)
                                Log("job state is " + job.State);
                            state = job.State;

                            var tasks = client.JobOperations.ListTasks(jobName).ToList<CloudTask>();
                            completed = tasks.Where(t => t.State == TaskState.Completed).Count();
                            all = tasks.Count();

                            BatchServiceClient.completed = completed;
                            BatchServiceClient.all = all;
                            Thread.Sleep(1000);

                        } while (state != JobState.Completed);
                    }
                }
            }
        }

        #region status
        static string jobName;
        static int all = 1;
        static int completed = 0;
        static JobState? state = null;
        public static bool IsRunning()
        {
            return state == JobState.Active;
        }

        public static int progress
        {
            get
            {
                return 100 * completed / all;
            }
        }
        #endregion

        #region logs
        private static readonly BlockingCollection<string> logs = new BlockingCollection<string>();
        static void Log(string log) {
            logs.Add(DateTime.Now.ToString("[HH:mm:ss] ") + log);
        }
        public static string FetchLog()
        {
            if (logs.Count > 0)
                return logs.Take();
            else
                return null;
        }
        #endregion logs
    }
}
