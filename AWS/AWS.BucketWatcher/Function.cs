using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.SQS;
using Amazon.SQS.Model;
using AWS.Lambda.CoreServices;
using AWS.Lambda.DI;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AWS.BucketWatcher
{
    public class Function
    {
        private readonly ILogService _loggingService;
        IAmazonS3 S3Client { get; set; }
        AmazonSQSClient sqsClient { get; set; }

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {
            S3Client = new AmazonS3Client();

            AmazonSQSConfig sqsConfig = new AmazonSQSConfig();
            sqsClient = new AmazonSQSClient();

            // Get dependency resolver
            DependencyResolver resolver = new DependencyResolver(ConfigureServices);
            // create instances of services using DI resolver
            _loggingService = resolver.ServiceProvider.GetRequiredService<ILogService>();
           

        }

        /// <summary>
        /// Constructs an instance with a preconfigured S3 client. This can be used for testing the outside of the Lambda environment.
        /// </summary>
        /// <param name="s3Client"></param>
        public Function(IAmazonS3 s3Client)
        {
            this.S3Client = s3Client;
        }

        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
        /// to respond to S3 notifications.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(S3Event evnt, ILambdaContext context)
        {
            var s3Event = evnt.Records?[0].S3;

            context.Logger.LogLine("Logged :" + s3Event.Object.Key);
            if (s3Event == null)
            {
                return null;
            }
           

            try
            {
                var response = await this.S3Client.GetObjectMetadataAsync(s3Event.Bucket.Name, s3Event.Object.Key);
                return response.Headers.ContentType;
            }
            catch (Exception e)
            {
                context.Logger.LogLine($"Error getting object {s3Event.Object.Key} from bucket {s3Event.Bucket.Name}. Make sure they exist and your bucket is in the same region as this function.");
                context.Logger.LogLine(e.Message);
                context.Logger.LogLine(e.StackTrace);
                throw;
            }
        }


        // Register services with DI system
        private void ConfigureServices(IServiceCollection services)
        {
            //TODO move to param store
            string regionName = "us-east-1";

            services.AddTransient<ILogService, LogService>();
            services.AddTransient<ILogService, LogService>();
        }
    }
}