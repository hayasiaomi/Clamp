using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ShanDian.SDK.Updater
{

    class UpgradeWebClient : WebClient
    {

        public Uri ResponseUri;

        /// <inheritdoc />
        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse webResponse = base.GetWebResponse(request, result);
            ResponseUri = webResponse.ResponseUri;
            return webResponse;
        }
    }
}
