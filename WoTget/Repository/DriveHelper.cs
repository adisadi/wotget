using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

namespace WoTget.Core.Repository
{
    internal static class DriveHelper
    {
        internal static IList<File> GetFiles(DriveService service, string search)
        {

            IList<File> Files = new List<File>();

            string pageToken = null;
            do
            {
                var request = service.Files.List();
                if (search != null)
                {
                    request.Q = search;
                }
                request.Fields = "nextPageToken, files(id, name , properties, description)";
                request.PageToken = pageToken;
                var result = request.Execute();
                foreach (var file in result.Files)
                {
                    Files.Add(file);
                }
                pageToken = result.NextPageToken;
            } while (pageToken != null);

            return Files;
        }

        internal static File CreateDirectory(DriveService service, string title, string parentId, Dictionary<string, string> properties)
        {
            // Create metaData for a new Directory
            File fileMetadata = new File()
            {
                Name = title,
                MimeType = "application/vnd.google-apps.folder",
                Parents = new List<string>
                            {
                                parentId
                            },
                Properties = properties
            };

            var request = service.Files.Create(fileMetadata);
            request.Fields = "id";
            request.Fields = "id";
            var file = request.Execute();

            return file;
        }

        internal static void DeleteFile(DriveService service, string fileId)
        {
            FilesResource.DeleteRequest DeleteRequest = service.Files.Delete(fileId);
            DeleteRequest.Execute();
        }

        internal static File UploadFile(DriveService service, System.IO.Stream fileStream, string name, string description, string parentId, Dictionary<string, string> properties)
        {
            File body = new File()
            {
                Name = name,
                MimeType = "application/unknown",
                Parents = new List<string>() { parentId },
                Description = description,
                Properties = properties
            };


            fileStream.Seek(0, System.IO.SeekOrigin.Begin);
            var request = service.Files.Create(body, fileStream, body.MimeType);
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

        internal static void DownloadFile(DriveService service, string fileId, string saveTo)
        {
            if (!String.IsNullOrEmpty(fileId))
            {
                var request = service.Files.Get(fileId);

                using (var fs = new System.IO.FileStream(saveTo, System.IO.FileMode.OpenOrCreate))
                {
                    request.Download(fs);
                }

            }
        }

        internal static System.IO.Stream DownloadFile(DriveService service, string fileId)
        {
            if (!String.IsNullOrEmpty(fileId))
            {
                var request = service.Files.Get(fileId);
                var fs = new System.IO.MemoryStream();

                request.Download(fs);
                return fs;
            }
            return null;
        }
    }
}