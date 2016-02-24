using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;


namespace WoTget.Core
{
    internal static class Helper
    {
        internal static IList<File> GetFiles(DriveService service, string search)
        {

            IList<File> Files = new List<File>();

            //List all of the files and directories for the current user.  
            // Documentation: https://developers.google.com/drive/v2/reference/files/list
            FilesResource.ListRequest list = service.Files.List();

            if (search != null)
            {
                list.Q = search;
            }
            FileList filesFeed = list.Execute();

            //// Loop through until we arrive at an empty page
            while (filesFeed.Items != null)
            {
                // Adding each item  to the list.
                foreach (File item in filesFeed.Items)
                {
                    Files.Add(item);
                }

                // We will know we are on the last page when the next page token is
                // null.
                // If this is the case, break.
                if (filesFeed.NextPageToken == null)
                {
                    break;
                }

                // Prepare the next page of results
                list.PageToken = filesFeed.NextPageToken;

                // Execute and process the next page request
                filesFeed = list.Execute();
            }

            return Files;
        }

        internal static File CreateDirectory(DriveService service, string title, string parent, params Property[] properties)
        {

            File NewDirectory = null;

            // Create metaData for a new Directory
            File body = new File();
            body.Title = title;
            body.MimeType = "application/vnd.google-apps.folder";
            body.Parents = new List<ParentReference>() { new ParentReference() { Id = parent } };

            body.Properties = new List<Property>();
            foreach (var property in properties)
            {
                body.Properties.Add(property);
            }

            FilesResource.InsertRequest request = service.Files.Insert(body);
            NewDirectory = request.Execute();


            return NewDirectory;
        }

        internal static void DeleteFile(DriveService service, string fileId)
        {
            FilesResource.DeleteRequest DeleteRequest = service.Files.Delete(fileId);
            DeleteRequest.Execute();
        }

        internal static File UploadFile(DriveService service, System.IO.Stream fileStream, string title, string description, string parent, params Property[] properties)
        {
            File body = new File()
            {
                Title = title,
                MimeType = "application/unknown",
                Parents = new List<ParentReference>() { new ParentReference() { Id = parent } },
                Description = description,
                Properties = new List<Property>()
            };

            foreach (var property in properties)
            {
                body.Properties.Add(property);
            }

            fileStream.Seek(0, System.IO.SeekOrigin.Begin);
            FilesResource.InsertMediaUpload request = service.Files.Insert(body, fileStream, body.MimeType);
            request.Upload();
            return request.ResponseBody;
        }



        internal static DriveService Authenticate(string keyFile)
        {
            //Scopes for use with the Google Drive API
            string[] scopes = new string[] { DriveService.Scope.Drive,
                                 DriveService.Scope.DriveFile};

         
            GoogleCredential c;
            using (var stream =
               new System.IO.FileStream(keyFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                c = GoogleCredential.FromStream(stream);
                c = c.CreateScoped(scopes);

            }

            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = c,
                ApplicationName = "WotSync",
            });

            return service;

        }

        internal static Boolean DownloadFile(DriveService service, string downloadUrl, string saveTo)
        {
            if (!String.IsNullOrEmpty(downloadUrl))
            {
                var x = service.HttpClient.GetByteArrayAsync(downloadUrl);
                byte[] arrBytes = x.Result;
                System.IO.File.WriteAllBytes(saveTo, arrBytes);
                return true;
            }
            else
            {
                // The file doesn't have any content stored on Drive.
                return false;
            }
        }

        internal static System.IO.Stream DownloadFile(DriveService service, string downloadUrl)
        {
            if (!String.IsNullOrEmpty(downloadUrl))
            {
                var x = service.HttpClient.GetByteArrayAsync(downloadUrl);
                return new System.IO.MemoryStream(x.Result);
            }
            return null;
        }

        internal static File UpdateFile(DriveService service, string uploadFile, string parent, string fileId)
        {

            if (System.IO.File.Exists(uploadFile))
            {
                File body = new File();
                body.Title = System.IO.Path.GetFileName(uploadFile);
                body.MimeType = "application/unknown";
                body.Parents = new List<ParentReference>() { new ParentReference() { Id = parent } };

                // File's content.
                byte[] byteArray = System.IO.File.ReadAllBytes(uploadFile);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

                FilesResource.UpdateMediaUpload request = service.Files.Update(body, fileId, stream, body.MimeType);
                request.Upload();
                return request.ResponseBody;
            }
            else
            {
                Console.WriteLine("File does not exist: " + uploadFile);
                return null;
            }

        }

        internal static string GetPackageName(this File f)
        {
            return System.IO.Path.GetFileNameWithoutExtension(f.Title).ToLower();
        }

    }
}
