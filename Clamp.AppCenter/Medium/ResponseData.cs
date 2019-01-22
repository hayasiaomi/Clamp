namespace Clamp.AppCenter.Medium
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Stores request trace information about the response.
    /// </summary>
    public class ResponseData
    {
        /// <summary>
        /// Gets or sets the content type of the response.
        /// </summary>
        /// <value>A <see cref="string"/> containing the content type.</value>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the headers of the response.
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey,TValue}"/> instance containing the headers.</value>
        public IDictionary<string, string> Headers { get; set; }

#pragma warning disable CS0246 // δ���ҵ����ͻ������ռ�����HttpStatusCode��(�Ƿ�ȱ�� using ָ����������?)
        /// <summary>
        /// Gets or sets the <see cref="HttpStatusCode"/> of the response.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
#pragma warning restore CS0246 // δ���ҵ����ͻ������ռ�����HttpStatusCode��(�Ƿ�ȱ�� using ָ����������?)

        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the response.
        /// </summary>
        /// <value>A <see cref="Type"/> instance.</value>
        public Type Type { get; set; }

#pragma warning disable CS0246 // δ���ҵ����ͻ������ռ�����Response��(�Ƿ�ȱ�� using ָ����������?)
        /// <summary>
        /// Implicitly casts a <see cref="Response"/> instance into a <see cref="ResponseData"/> instance.
        /// </summary>
        /// <param name="response">A <see cref="Response"/> instance.</param>
        /// <returns>A <see cref="ResponseData"/> instance.</returns>
        public static implicit operator ResponseData(Response response)
#pragma warning restore CS0246 // δ���ҵ����ͻ������ռ�����Response��(�Ƿ�ȱ�� using ָ����������?)
        {
            return new ResponseData
            {
                ContentType = response.ContentType,
                Headers = response.Headers,
                StatusCode = response.StatusCode,
                Type = response.GetType()
            };
        }
    }
}