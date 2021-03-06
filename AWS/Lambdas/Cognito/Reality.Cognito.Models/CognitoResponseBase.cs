﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reality.Cognito.Models
{
    public abstract class CognitoResponseBase
    {
        /// <summary>
        /// StatusMessage
        /// </summary>
        [JsonProperty("statusMessage")]
        public string StatusMessage { get; set; }

        /// <summary>
        /// StatusCode
        /// </summary>
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        /// <summary>
        /// Payload
        /// </summary>
        [JsonProperty("payload")]
        public object Payload { get; set; }
    }
}
