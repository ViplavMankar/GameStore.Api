using System.IO.Compression;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using GameStore.Api.DTOs;
using Amazon.S3.Model;
using System.Diagnostics;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GameStore.Api.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly AwsSettings _settings;

        public S3Service(IOptions<AwsSettings> options)
        {
            _settings = options.Value;
            _s3Client = new AmazonS3Client(
                _settings.AccessKey,
                _settings.SecretKey,
                Amazon.RegionEndpoint.GetBySystemName(_settings.Region)
            );
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string key, string contentType)
        {
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = fileStream,
                Key = key,
                BucketName = _settings.BucketName,
                ContentType = contentType
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);

            return GetFileUrl(key);
        }

        public async Task<string> UploadZipAsync(Stream zipStream, string gameName)
        {
            string mainFileUrl = string.Empty;

            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);

            foreach (var entry in archive.Entries)
            {
                if (string.IsNullOrWhiteSpace(entry.Name))
                    continue; // skip folders

                using var entryStream = entry.Open();

                var key = $"{gameName}/{entry.FullName.Replace("\\", "/")}";
                var url = await UploadFileAsync(entryStream, key, GetContentType(entry.Name));

                // first HTML file becomes the main file
                if (string.IsNullOrEmpty(mainFileUrl) && entry.Name.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
                {
                    mainFileUrl = url;
                }
            }

            return string.IsNullOrEmpty(mainFileUrl) ? string.Empty : mainFileUrl;
        }

        public async Task DeleteFolderAsync(string folderName)
        {
            var listRequest = new ListObjectsV2Request
            {
                BucketName = _settings.BucketName,
                Prefix = folderName + "/"
            };

            ListObjectsV2Response listResponse;
            do
            {
                listResponse = await _s3Client.ListObjectsV2Async(listRequest);

                if (listResponse.S3Objects != null && listResponse.S3Objects.Any())
                {
                    var deleteObjects = listResponse.S3Objects.Select(o => new KeyVersion { Key = o.Key }).ToList();
                    if (deleteObjects.Count > 0)
                    {
                        var deleteRequest = new DeleteObjectsRequest
                        {
                            BucketName = _settings.BucketName,
                            Objects = deleteObjects
                        };
                        await _s3Client.DeleteObjectsAsync(deleteRequest);
                    }
                }

                listRequest.ContinuationToken = listResponse.NextContinuationToken;
            }
            while (listResponse.IsTruncated.GetValueOrDefault());
        }

        public async Task<string> ConvertDocxToPdf(Guid fileId, string inputKey, string outputKey)
        {
            var tempInputPath = Path.Combine(Path.GetTempPath(), $"{fileId}.docx");
            var tempOutputDir = Path.GetTempPath();
            var tempOutputPath = Path.Combine(tempOutputDir, $"{fileId}.pdf");

            try
            {
                // 🔹 1. Download DOCX from S3
                var response = await _s3Client.GetObjectAsync(_settings.BucketName, inputKey);

                using (var fs = System.IO.File.Create(tempInputPath))
                {
                    await response.ResponseStream.CopyToAsync(fs);
                }

                // 🔹 2. Run LibreOffice
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "libreoffice",
                        Arguments = $"--headless --convert-to pdf \"{tempInputPath}\" --outdir \"{tempOutputDir}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();

                // 🔹 3. Check if PDF exists
                if (!System.IO.File.Exists(tempOutputPath))
                {
                    var error = await process.StandardError.ReadToEndAsync();
                    throw new Exception($"Conversion failed: {error}");
                }

                // 🔹 4. Upload PDF to S3
                using var pdfStream = System.IO.File.OpenRead(tempOutputPath);

                await UploadFileAsync(pdfStream, outputKey, "application/pdf");

                return GetFileUrl(outputKey);
            }
            catch (Exception ex)
            {
                // 🔥 Log error (for real app, use a logging framework)
                Console.WriteLine($"Error in ConvertDocxToPdf: {ex.Message}");
                throw; // rethrow to let controller handle it
            }
            finally
            {
                // 🔥 Cleanup
                if (System.IO.File.Exists(tempInputPath))
                    System.IO.File.Delete(tempInputPath);

                if (System.IO.File.Exists(tempOutputPath))
                    System.IO.File.Delete(tempOutputPath);
            }
        }

        public byte[] ConvertSingleImageToPdf(byte[] image)
        {
            using var stream = new MemoryStream();

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(10);

                    page.Content()
                        .Image(image, ImageScaling.FitArea);
                });
            })
            .GeneratePdf(stream);

            return stream.ToArray();
        }

        private string GetFileUrl(string key) =>
            $"https://{_settings.BucketName}.s3.{_settings.Region}.amazonaws.com/{key}";

        private static string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".html" => "text/html",
                ".css" => "text/css",
                ".js" => "application/javascript",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                ".json" => "application/json",
                ".txt" => "text/plain",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };
        }
    }
}