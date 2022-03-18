using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage___Blob_Operations
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Azure Blob Storage exercise\n");

            // Run the examples asynchronously, wait for the results before proceeding
            ProcessAsync().GetAwaiter().GetResult();


            Console.WriteLine("Press enter to exit the sample application.");
            Console.ReadLine();
        }

        private static async Task ProcessAsync()
        {
            // Copy the connection string from the portal in the variable below.
            string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=storageaccountdemo97;AccountKey=kAAziOPfhsCHIYD5ey8f7Dgaw22xpJ6Ndv74jpmd6HLVBH1Pk5F1/A+hU4BdIaiSH6uI/zQI2YcW+AStjYV9fQ==;EndpointSuffix=core.windows.net";

            // Create a client that can authenticate with a connection string
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);

            // EXAMPLE CODE STARTS BELOW HERE
            #region Create Blob Container
            //Create a unique name for the container
            string containerName = "wtblob" + Guid.NewGuid().ToString();
            // Create the container and return a container client object
            BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);

            //Uncomment these 2 lines if you want to use an existing Container
            //containerName = "blobcontainer";
            //containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            Console.WriteLine("A container named '" + containerName + "' has been created. ");
            #endregion

            #region Upload Blobs to the Conatiner
            string localPath = @"C:\Users\jyoti\source\repos\AzureLabs Az204\Storage - Blob Operations\data";
            string fileName = "wtfile" + Guid.NewGuid().ToString() + ".txt";
            //string fileName = "text1.txt";
            string localFilePath = Path.Combine(localPath, fileName);

            File.WriteAllText(localFilePath, "Hello, Text 1 from code");

            //Uncomment these 2 lines if you are trying to update/fetch the existing Container
            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

            // Open the file and upload its data
            FileStream uploadFileStream = File.OpenRead(localFilePath);
            await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();
            Console.WriteLine("\nThe file was uploaded. We'll verify by listing");
            #endregion

            #region List the blobs in a Container
            Console.WriteLine("Listing blobs...");
            foreach (BlobItem blobItem in containerClient.GetBlobs())
            {
                Console.WriteLine("\t" + blobItem.Name);
            }
            #endregion

            #region Download a blob to a Local File
            // Download the blob to a local file
            // Append the string "DOWNLOADED" before the .txt extension
            string downloadFilePath = localFilePath.Replace(".txt", "DOWNLOADED.txt");

            //Uncomment these 2 lines if you want to download one existing file from the blob
            //blobClient = containerClient.GetBlobClient("ErrorExamtopics.png");
            //downloadFilePath = "C:\\Users\\jyoti\\source\\repos\\AzureLabs Az204\\Storage - Blob Operations\\data\\ErrorExamtopicsDOWNLOADED.png";
            
            Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadFilePath);

            // Download the blob's contents and save it to a file
            BlobDownloadInfo download = await blobClient.DownloadAsync();

            using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
            {
                await download.Content.CopyToAsync(downloadFileStream);
                downloadFileStream.Close();
            }
            Console.WriteLine("\nLocate the local file to verify it was downloaded.");
            #endregion


            #region Delete Container
            // Delete the container and clean up local files created
            Console.WriteLine("\n\nDeleting blob container...");
            await containerClient.DeleteAsync();

            Console.WriteLine("Deleting the local source and downloaded files...");
            File.Delete(localFilePath);
            File.Delete(downloadFilePath);

            Console.WriteLine("Finished cleaning up.");
            #endregion

            Console.WriteLine("Press 'Enter' to continue.");
            Console.ReadLine();
            

        }
    }
}
