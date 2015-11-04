using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;

namespace DemoClient
{
    static class StorageHelper
    {
        public static IEnumerable<string> ListContainers()
        {
            CloudStorageAccount account = new CloudStorageAccount(new StorageCredentials(Settings.storageAccount, Settings.storageKey), true);

            return account.CreateCloudBlobClient().ListContainers().Select<CloudBlobContainer, string>(x => x.Uri.ToString());
        }

        public static IEnumerable<string> ListBlobs(string containerURL)
        {
            string container = new Uri(containerURL).AbsolutePath.TrimStart('/');

            CloudStorageAccount account = new CloudStorageAccount(new StorageCredentials(Settings.storageAccount, Settings.storageKey), true);            
            CloudBlobContainer blobContainer = account.CreateCloudBlobClient().GetContainerReference(container);
                     
            IEnumerable<IListBlobItem> blobs = blobContainer.ListBlobs();

            return blobs.Select<IListBlobItem, string>(x => x.Uri.ToString());
        }

        public static string GetBlobSASURL(string url)
        {
            SharedAccessBlobPermissions perm = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write| SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Delete;

            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTime.Now.Subtract(new TimeSpan(0, 15, 0));
            sasConstraints.SharedAccessExpiryTime = DateTime.Now.AddDays(15);
            sasConstraints.Permissions = perm;

            CloudStorageAccount storageaccount = new CloudStorageAccount(new StorageCredentials(Settings.storageAccount, Settings.storageKey), true);
            CloudBlobClient client = storageaccount.CreateCloudBlobClient();

            ICloudBlob storageblob = client.GetBlobReferenceFromServer(new Uri(url));
            storageblob.FetchAttributes(); // if blob doesn't exist, this will throw StorageClientException with StorageErrorCode.ResourceNotFound

            string sas = storageblob.GetSharedAccessSignature(sasConstraints);
            return storageblob.Uri.AbsoluteUri + sas;
        }

        public static string GetContainerSAS(string url)
        {
            string container = new Uri(url).AbsolutePath.TrimStart('/');

            SharedAccessBlobPermissions perm = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Delete;

            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTime.Now.Subtract(new TimeSpan(0, 5, 0));
            sasConstraints.SharedAccessExpiryTime = DateTime.Now.AddDays(1);
            sasConstraints.Permissions = perm;

            CloudStorageAccount storageaccount = new CloudStorageAccount(new StorageCredentials(Settings.storageAccount, Settings.storageKey), true);
            CloudBlobContainer blobContainer = storageaccount.CreateCloudBlobClient().GetContainerReference(container);

            blobContainer.FetchAttributes();

            return blobContainer.GetSharedAccessSignature(sasConstraints);
        }
    }
}
