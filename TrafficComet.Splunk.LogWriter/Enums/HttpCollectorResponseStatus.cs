namespace TrafficComet.Splunk.LogWriter.Enums
{
    public enum HttpCollectorResponseStatus
    {
        /// <summary>
        /// OK (200)
        /// </summary>
        Success = 0,

        /// <summary>
        /// Forbidden (403)
        /// </summary>
        TokenDisabled = 1,

        /// <summary>
        /// Unauthorized (401)
        /// </summary>
        TokenIsRequired = 2,

        /// <summary>
        /// Unauthorized (401)
        /// </summary>
        InvalidAuthorization = 3,

        /// <summary>
        /// Forbidden (403)
        /// </summary>
        InvalidToken = 4,

        /// <summary>
        /// Bad Request (400)
        /// </summary>
        NoData = 5,

        /// <summary>
        /// Bad Request (400)
        /// </summary>
        InvalidDataFormat = 6,

        /// <summary>
        /// Bad Request (400)
        /// </summary>
        IncorrectIndex = 7,

        /// <summary>
        /// Internal Error (500)
        /// </summary>
        InternalServerError = 8,

        /// <summary>
        /// Service Unavailable (503)
        /// </summary>
        ServerIsBusy = 9,

        /// <summary>
        /// Bad Request (400)
        /// </summary>
        DataChannelIsMissing = 10,

        /// <summary>
        /// Bad Request (400)
        /// </summary>
        InvalidDataChannel = 11,

        /// <summary>
        /// Bad Request (400)
        /// </summary>
        EventFieldIsRequired = 12,

        /// <summary>
        /// Bad Request (400)
        /// </summary>
        EventFieldCannotBeBlank = 13,

        /// <summary>
        /// Bad Request (400)
        /// </summary>
        ACKIsDisabled = 14,

        /// <summary>
        /// Bad Request (400)
        /// </summary>
        ErrorInHandlingIndexedFields = 15,

        /// <summary>
        /// Bad Request (400)
        /// </summary>
        QueryStringAuthorizationIsNotEnabled = 16,
        /// <summary>
        /// OK (200)
        /// </summary>
        IsHealthy = 17
    }
}
