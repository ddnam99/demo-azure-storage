using AzStorage.MVC.Models;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AzStorage.CoreAPI.Models
{
    public static class AzBlob
    {
        public static CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("StorageConnectionString"));
        public static CloudBlobClient BlobClient = storageAccount.CreateCloudBlobClient();
        public static string BlobContainerName = "demo";

        private static CloudBlobContainer CreateBlobContainerIfNotExists()
        {
            var blobContainer = BlobClient.GetContainerReference(BlobContainerName);
            blobContainer.CreateIfNotExists();
            blobContainer.SetPermissions(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            return blobContainer;
        }

        public static async Task<(bool ok, string message)> UploadFileToStorage(Stream fileStream, string fileName)
        {
            var blobContainer = CreateBlobContainerIfNotExists();

            var destBlob = blobContainer.GetBlockBlobReference(fileName);

            if (destBlob.Exists()) return await Task.FromResult((false, "File is exist!"));

            await destBlob.UploadFromStreamAsync(fileStream);

            return await Task.FromResult((true, $"{storageAccount.BlobEndpoint.OriginalString}{BlobContainerName}/{fileName}"));
        }

        public static async Task<List<string>> GetAllFilesFromBlobContainer()
        {
            var blobContainer = CreateBlobContainerIfNotExists();

            var dirBlob = blobContainer.GetDirectoryReference(BlobContainerName);

            var resultSegment = await blobContainer.ListBlobsSegmentedAsync(string.Empty, true, BlobListingDetails.Metadata, 100, null, null, null);
            var files = new List<string>(resultSegment.Results.Select(i => (i as CloudBlob).Name));

            return files;
        }

        public static async Task<(Stream FileStream, string ContentType)> DownloadFile(string fileName)
        {
            var blobContainer = BlobClient.GetContainerReference(BlobContainerName);
            var destBlob = blobContainer.GetBlockBlobReference(fileName);

            var data = new MemoryStream();

            await destBlob.DownloadToStreamAsync(data);

            return (destBlob.OpenReadAsync().Result, destBlob.Properties.ContentType);
        }

        public static async Task<bool> DeleteFile(string fileName)
        {
            var blobContainer = BlobClient.GetContainerReference(BlobContainerName);
            var destBlob = blobContainer.GetBlockBlobReference(fileName);

            return await destBlob.DeleteIfExistsAsync(); ;
        }
    }
}
