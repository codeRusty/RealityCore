using Amazon.Lambda.Core;
using AWS.Cognito.Core;
using Reality.Cognito.Models;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Reality.ConfirmSignUp
{
    public class Function
    {

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(ConfirmSignUpRequest input, ILambdaContext context)
        {
            CognitoHelper helper = new CognitoHelper();
            await helper.VerifyAccessCode(input.username, input.tempCode);
            return input.ToString<ConfirmSignUpRequest>();

        }
    }
}
