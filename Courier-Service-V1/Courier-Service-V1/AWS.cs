using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.IO;

public static class AWS
{
    private static readonly string _accessKey = "AKIAU6GDYMTHTIZML6UG";
    private static readonly string _secretKey = "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";
    private static readonly string _bucketName = "courierbuckets3";
    private static readonly RegionEndpoint _region = RegionEndpoint.USEast1;

    public static void UploadFile(string filePath,string key)
    {
        using (var s3Client = new AmazonS3Client(_accessKey, _secretKey, _region))
        {
            using (var transferUtility = new TransferUtility(s3Client))
            {
                var uploadRequest = new TransferUtilityUploadRequest
                {
                    FilePath = filePath,
                    BucketName = _bucketName,
                    Key = key,
                    CannedACL = S3CannedACL.PublicRead
                };
                transferUtility.Upload(uploadRequest);
            }
        }
    }

    public static async void DeleteFile(string key)
    {
        using (var s3Client = new AmazonS3Client(_accessKey, _secretKey, _region))
        {
            using(var transferUtility = new TransferUtility(s3Client))
            {
                await transferUtility.S3Client.DeleteObjectAsync(_bucketName, key);
            }
        }
    }
}
